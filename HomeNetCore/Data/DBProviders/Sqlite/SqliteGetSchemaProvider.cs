using HomeNetCore.Data.Generators.SqlQueriesGenerator;
using HomeNetCore.Data.Schemes;
using HomeNetCore.Data.Schemes.GetSchemaTableBd;
using HomeNetCore.Helpers.Exceptions;
using Microsoft.Data.Sqlite;

using System.Data;
using HomeNetCore.Helpers;


namespace HomeNetCore.Data.SqliteClasses
{
    public class SqliteGetSchemaProvider : GetSchemaProvider
    {
        private readonly ISchemaSqlInitializer _generator;
        private readonly SqliteConnection _requiredConnection;
        private readonly ILogger _logger;

        public SqliteGetSchemaProvider(
            ISchemaSqlInitializer generator,
            SqliteConnection connection,
            ILogger logger)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _requiredConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TableSchema> GetActualTableSchemaAsync(string? tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Имя таблицы не может быть пустым", nameof(tableName));

            var columns = new List<ColumnSchema>();

            try
            {
                _logger.LogInformation("Получение схемы для таблицы {TableName} (SQLite)", tableName);

                using var command = _requiredConnection.CreateCommand();
                command.CommandText = _generator.GenerateGetTableStructureSql(tableName);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    columns.Add(new ColumnSchema
                    {
                        Name = reader.GetString(1),
                        Type = _generator.MapDatabaseType(reader.GetString(2)),
                        IsNullable = !reader.GetBoolean(3),
                        IsPrimaryKey = reader.GetInt32(5) > 0
                    });
                }
            }
            catch (Exception ex)
            {
                throw new SchemaProviderException(
                    $"Ошибка при получении схемы для таблицы {tableName} в SQLite: {ex.Message}", ex);
            }

            _logger.LogDebug($"Получено ColumnCount {columns.Count} столбцов для TableName{tableName}");

            return new TableSchema
            {
                TableName = tableName,
                Columns = columns
            };
        }
    }

}
