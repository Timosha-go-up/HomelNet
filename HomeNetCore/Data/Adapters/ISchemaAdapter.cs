using HomeNetCore.Data.Schemes;
using WpfHomeNet.Data.Schemes;

namespace HomeNetCore.Data.Adapters
{
    public interface ISchemaAdapter
    {
        string GetTableName(string? rawName);
        public string GetColumnName(string? rawName, bool toSnakeCase = true, bool toCamelCase = false);
        List<string> GetColumnDefinitions(TableSchema schema);
    }



}
