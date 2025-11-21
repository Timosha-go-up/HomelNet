using HomeNetCore.Data.Adapters;
using HomeNetCore.Data.Builders;
using HomeNetCore.Data.Schemes;
using System.Text;

namespace WpfHomeNet.Data.DBProviders.SqliteClasses
{
    public class SqliteSchemaAdapter : ISchemaAdapter
    {
        public string GetTableName(string? rawName)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                throw new ArgumentException("Имя таблицы не может быть пустым");
            }

            return ToSnakeCase(rawName);
        }

        // Метод для преобразования имени колонки
        public string GetColumnName(string? rawName)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                throw new ArgumentException("Имя колонки не может быть пустым");
            }

            // Добавляем дополнительную проверку на специальные символы
            if (rawName.Any(char.IsWhiteSpace))
            {
                throw new ArgumentException("Имя колонки не должно содержать пробелы");
            }

            return ToSnakeCase(rawName);
        }

        /// <summary>
        /// Преобразует схему в формат snake_case для использования в SQL-запросах
        /// </summary>
        /// <param name="originalSchema">Исходная схема таблицы</param>
        /// <returns>Отформатированная схема с именами в snake_case</returns>
        public TableSchema ConvertToSnakeCaseSchema(TableSchema originalSchema)
        {
            if (originalSchema == null)
            {
                throw new ArgumentNullException(nameof(originalSchema), "Исходная схема не может быть null");
            }

            if (string.IsNullOrEmpty(originalSchema.TableName))
            {
                throw new ArgumentException("Имя таблицы не может быть пустым или null", nameof(originalSchema.TableName));
            }

            var snakeCaseSchema = new TableSchema
            {
                TableName = GetTableName(originalSchema.TableName),
                Columns = originalSchema.Columns?.Select(col =>
                {
                    if (col == null)
                    {
                        throw new InvalidOperationException("Колонка в схеме не может быть null");
                    }
                    return new ColumnSchema
                    {
                        Name = GetColumnName(col.Name) ?? throw new InvalidOperationException("Не удалось преобразовать имя колонки"),
                        Type = col.Type,
                        Length = col.Length,
                        IsNullable = col.IsNullable,
                        IsPrimaryKey = col.IsPrimaryKey,
                        IsUnique = col.IsUnique,
                        IsAutoIncrement = col.IsAutoIncrement,
                        CreatedAt = col.CreatedAt,
                        IsCreatedAt = col.IsCreatedAt,
                        Comment = col.Comment,
                        DefaultValue = col.DefaultValue
                    };
                }).ToList() ?? new List<ColumnSchema>()
            };

            // Добавляем проверку перед инициализацией
            if (!snakeCaseSchema.Columns.Any())
            {
                throw new InvalidOperationException("Схема не содержит колонок после преобразования");
            }

            snakeCaseSchema.Initialize();

            // Проверяем результат инициализации
            if (string.IsNullOrEmpty(snakeCaseSchema.Fields))
            {
                throw new InvalidOperationException("Не удалось сгенерировать список полей после инициализации");
            }

            return snakeCaseSchema;
        }




        public List<string> GetColumnDefinitions(TableSchema schema)
        {
            // Валидация всех колонок
            foreach (var col in schema.Columns)
            {
                ValidateColumn(col);
            }

            return schema.Columns.Select(col =>
            {
                var name = $"\"{GetColumnName(col.Name)}\"";  // Используем новый метод

                string sqlType = col.Type switch
                {
                    ColumnType.Varchar => "TEXT",
                    ColumnType.Integer => "INTEGER",
                    ColumnType.DateTime => "TIMESTAMP",
                    ColumnType.Boolean => "INTEGER",
                    _ => throw new NotSupportedException($"Тип {col.Type} не поддерживается")
                };

                var constraints = new List<string>();

                if (col.IsCreatedAt)
                {
                    constraints.Add("DEFAULT CURRENT_TIMESTAMP");
                }

                if (!col.IsNullable)
                {
                    constraints.Add("NOT NULL");
                }

                if (col.IsPrimaryKey)
                {
                    constraints.Add("PRIMARY KEY");
                }

                if (col.IsUnique)
                {
                    constraints.Add("UNIQUE");
                }

                var parts = new List<string> { name, sqlType };

                if (constraints.Any())
                    parts.Add(string.Join(" ", constraints));

                return string.Join(" ", parts);
            }).ToList();
        }

        private void ValidateColumn(ColumnSchema col)
        {
            if (col.Type == ColumnType.Unspecified)
                throw new InvalidOperationException(
                    $"Колонка '{col.Name}' " +
                    $"не имеет заданного типа. Вызовите WithDateTime() " +
                    $"или другой метод установки типа.");
        }

        private string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var builder = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                        builder.Append('_');
                    builder.Append(char.ToLower(c));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }

}
