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
}