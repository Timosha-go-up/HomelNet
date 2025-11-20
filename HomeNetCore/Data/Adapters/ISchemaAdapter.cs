using HomeNetCore.Data.Schemes;
using WpfHomeNet.Data.Schemes;

namespace HomeNetCore.Data.Adapters
{
    public interface ISchemaAdapter
    {
        string GetTableName(string rawName);
        List<string> GetColumnDefinitions(TableSchema schema);
    }



}
