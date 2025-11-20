using HomeNetCore.Data.Builders;
using HomeNetCore.Data.Generators.SqlQueriesGenerator;
using HomeNetCore.Data.Schemes;
using System.Text;
using WpfHomeNet.Data.DBProviders.SqliteClasses;

namespace WpfHomeNet.Data.SqliteClasses
{

    public class SqliteSchemaSqlInit : ISchemaSqlInitializer
    {
        private readonly SqliteSchemaAdapter _adapter;

        public SqliteSchemaSqlInit()
        {
            _adapter = new SqliteSchemaAdapter();
        }

        public string GenerateCreateTableSql(TableSchema schema)
        {
            // Получаем имя таблицы в формате snake_case
            string tableName = _adapter.GetTableName(schema.TableName);

            // Получаем определения всех колонок
            List<string> columnDefinitions = _adapter.GetColumnDefinitions(schema);

            // Формируем итоговый SQL-запрос
            return $"CREATE TABLE IF NOT EXISTS \"{tableName}\" (" +
                   string.Join(", ", columnDefinitions) +
                   ")";
        }

        public string GenerateTableExistsSql(string? tableName)
        {
            
            string escapedName = _adapter.GetTableName(tableName);
            return $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'"; 
        }

        public string GenerateGetTableStructureSql(string? tableName)
        {
            // Используем экранированное имя таблицы
            string escapedName = _adapter.GetTableName(tableName);

            return $"PRAGMA table_info(\"{escapedName}\")";
        }




        public ColumnType MapDatabaseType(string dbType)
        {
            var type = dbType.ToLower();
            switch (type)
            {
                case "integer":
                    return ColumnType.Integer;
                case "text":
                    return ColumnType.Varchar;
                case "real":
                   
                default:
                    return ColumnType.Unknown;
            }
        }
    }



}
