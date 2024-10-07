using CodeGenerator.Models;
using Microsoft.Data.SqlClient;

namespace CodeGenerator.Helpers;

public class DatabaseReader(string connectionString)
{
    public List<ColumnInfo> GetTableColumns(string schemaName, string tableName)
    {
        var columns = new List<ColumnInfo>();
        string query = @"
        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_SCHEMA = @SchemaName AND TABLE_NAME = @TableName";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SchemaName", schemaName);
            command.Parameters.AddWithValue("@TableName", tableName);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(new ColumnInfo(
                    reader["COLUMN_NAME"].ToString() ?? string.Empty,
                    reader["DATA_TYPE"].ToString() ?? string.Empty,
                    reader["IS_NULLABLE"].ToString() == "YES"
                ));
            }
        }
        return columns;
    }

    public List<ColumnInfo> GetColumnsFromQuery(string query)
    {
        var columns = new List<ColumnInfo>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            // Ler o esquema do DataReader para obter informações das colunas
            var schemaTable = reader.GetSchemaTable();

            if (schemaTable != null)
            {
                foreach (System.Data.DataRow row in schemaTable.Rows)
                {
                    var columnName = row["ColumnName"].ToString() ?? string.Empty;
                    var dataType = row["DataType"] as Type;
                    bool isNullable = (bool)row["AllowDBNull"];

                    // Mapeando tipo do .NET para SQL
                    string sqlType = GetSqlTypeFromClrType(dataType);

                    columns.Add(new ColumnInfo(columnName, sqlType, isNullable));
                }
            }
        }

        return columns;
    }

    private string GetSqlTypeFromClrType(Type clrType)
    {
        if (clrType == typeof(int))
            return "int";
        if (clrType == typeof(long))
            return "bigint";
        if (clrType == typeof(short))
            return "smallint";
        if (clrType == typeof(byte))
            return "tinyint";
        if (clrType == typeof(bool))
            return "bit";
        if (clrType == typeof(decimal))
            return "decimal";
        if (clrType == typeof(double))
            return "float";
        if (clrType == typeof(float))
            return "real";
        if (clrType == typeof(DateTime))
            return "datetime";
        if (clrType == typeof(string))
            return "nvarchar";
        return "nvarchar"; // padrão se o tipo não for identificado
    }
}