using System;
using System.Text;
using CodeGenerator.Models;

namespace CodeGenerator;

public  static class CodeGenerator
{
    public static string GenerateClass(this string tableName, List<ColumnInfo> columns,string namespaceName){
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"public class {tableName}");
        sb.AppendLine("{");
        foreach (var column in columns)
        {
            string csharpType = GetCSharpType(column.DataType, column.IsNullable);
            sb.AppendLine($"    public {csharpType} {column.Name} {{ get; private set; }}");
        }

        sb.AppendLine();
        sb.AppendLine($"    public {tableName}(");
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            string csharpType = GetCSharpType(column.DataType, column.IsNullable);
            string delimiter = i < columns.Count - 1 ? "," : "";
            sb.AppendLine($"        {csharpType} {ToCamelCase(column.Name)}{delimiter}");
        }
        sb.AppendLine("    )");
        sb.AppendLine("    {");
        foreach (var column in columns)
        {
            sb.AppendLine($"        {column.Name} = {ToCamelCase(column.Name)};");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetCSharpType(this string sqlType, bool isNullable)
    {
        string csharpType = sqlType switch
        {
            "int" => "int",
            "bigint" => "long",
            "smallint" => "short",
            "tinyint" => "byte",
            "bit" => "bool",
            "decimal" => "decimal",
            "numeric" => "decimal",
            "float" => "double",
            "real" => "float",
            "datetime" => "DateTime",
            "date" => "DateTime",
            "varchar" => "string",
            "nvarchar" => "string",
            "char" => "string",
            "text" => "string",
            _ => "string"
        };

        if (isNullable && csharpType != "string")
            csharpType += "?";

        return csharpType;
    }

    private static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
