using HomeNetCore.Data.Generators.SqlQueriesGenerator;
using HomeNetCore.Data.Schemes;
using HomeNetCore.Helpers;
using WpfHomeNet.Data.DBProviders.SqliteClasses;

namespace HomeNetCore.Data.DBProviders.Sqlite
{
    public class SqliteUserSqlGen : IUserSqlGenerator
    {

        private readonly TableSchema _originalTable;
        private readonly TableSchema _formattedTable;
        private readonly SqliteSchemaAdapter _adapter;
        private readonly ILogger _logger;
        private string tableName;
        private string fields;
        private string parameters;
        private string setClause;

        public SqliteUserSqlGen(
            TableSchema tableSchema,
            SqliteSchemaAdapter adapter,
            ILogger logger)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _originalTable = tableSchema ?? throw new ArgumentNullException(nameof(tableSchema));
            _formattedTable = adapter.ConvertToSnakeCaseSchema(tableSchema);

            if (string.IsNullOrEmpty(tableSchema.TableName))
            {
                _logger.LogError("Имя таблицы не может быть пустым или null");
                throw new ArgumentException();
            }
            try
            {
                if (string.IsNullOrEmpty(tableSchema.TableName))
                {
                    _logger.LogError("Имя таблицы не может быть пустым или null");
                    throw new Exception();
                }               
                tableName = _formattedTable.TableName ?? throw new Exception();
                fields = _formattedTable.Fields ?? throw new Exception();
                parameters = _formattedTable.Parameters ?? throw new Exception();
                setClause = _formattedTable.SetClause ?? throw new Exception();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при инициализации: {ex.Message}");
                throw;
            }        
        }

        public string GenerateInsert() 
        {
            if (string.IsNullOrEmpty(fields) || string.IsNullOrEmpty(parameters))
            {
                throw new InvalidOperationException("Некорректные поля или параметры для вставки");
            }
            return $"INSERT INTO {tableName} ({fields}) VALUES ({parameters})";
        }

        public string GenerateUpdate()
        {
            string idColumn = _formattedTable.IdColumnName
                ?? throw new InvalidOperationException("ID-колонка не найдена в таблице");

            if (string.IsNullOrEmpty(setClause))
            {
                throw new InvalidOperationException("Некорректный SET clause для обновления");
            }

            return $"UPDATE {tableName} SET {setClause} WHERE {idColumn} = @{idColumn}";
        }

        public string GenerateDelete()
        {
            string idColumn = _formattedTable.IdColumnName
                ?? throw new InvalidOperationException("ID-колонка не найдена в таблице");
            return $"DELETE FROM {tableName} WHERE {idColumn} = @{idColumn}";
        }

        public string GenerateSelectById()
        {
            string idColumn = _formattedTable.IdColumnName
                ?? throw new InvalidOperationException("ID-колонка не найдена в таблице");
            return $"SELECT {fields} FROM {tableName} WHERE {idColumn} = @{idColumn}";
        }

        public string GenerateSelectByEmail()
        {
            string emailColumn = _formattedTable.Columns
                .FirstOrDefault(c => c.Name == "email")?.Name
                ?? throw new InvalidOperationException("Колонка email не найдена в таблице");

            if (!_formattedTable.Columns.Any(c => c.Name == emailColumn))
            {
                throw new InvalidOperationException($"Колонка {emailColumn} не существует в таблице");
            }

            return $"SELECT {fields} FROM {tableName} WHERE {emailColumn} = @{emailColumn}";
        }

        public string GenerateSelectAll()
        {
            if (string.IsNullOrEmpty(fields))
            {
                throw new InvalidOperationException("Некорректные поля для выборки");
            }
            return $"SELECT {fields} FROM {tableName}";
        }       
    }
}
