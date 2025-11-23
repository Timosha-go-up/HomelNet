using HomeNetCore.Data.Schemes;
using HomeNetCore.Data.Schemes.CheckTableBd;
using System;



namespace WpfHomeNet.Data.Schemes.CheckTableBd
{
    
    public class SchemaComparer
    {
        /// <summary>
        /// Сравнивает ожидаемую и фактическую схемы таблицы, возвращая различия.
        /// </summary>
        /// <param name="expected">Ожидаемая схема таблицы (эталон)</param>
        /// <param name="actual">Фактическая схема таблицы (из БД)</param>
        /// <returns>Объект с перечнем отсутствующих, лишних и несовпадающих колонок</returns>
        public SchemaDiff Compare(TableSchema expected, TableSchema actual)
        {
            var diff = new SchemaDiff { TableName = expected.TableName };
           // Находим колонки, которые есть в эталонной схеме, но отсутствуют в реальной
            diff.MissingColumns = FindMissingColumns(expected, actual);
            
              // Находим колонки, которые есть в реальной схеме, но отсутствуют в эталонной
            diff.ExtraColumns = FindExtraColumns(expected, actual);
            
           // Находим колонки с совпадающими именами, но разными свойствами
            diff.MismatchedColumns = FindMismatchedColumns(expected, actual);  

            return diff;
        }

        /// <summary>
        /// Находит колонки, присутствующие в expected, но отсутствующие в actual.
        /// </summary>
        private List<ColumnSchema> FindMissingColumns(TableSchema expected, TableSchema actual)
        {
            return expected.Columns.Where
                (// Проверяем, что в actual нет колонки с таким же именем (без учёта регистра) 
                  expectedCol => !actual.Columns.Any  
                  (
                      actualCol => StringEqualsIgnoreCase(actualCol.Name,expectedCol.Name)   
                  )                                    
                )
                .ToList();                                        
        }

        /// <summary>
        /// Находит колонки, присутствующие в actual, но отсутствующие в expected.
        /// </summary>
        private List<ColumnSchema> FindExtraColumns(TableSchema expected, TableSchema actual)
        {
            return actual.Columns.Where
                (// Проверяем, что в expected нет колонки с таким же именем (без учёта регистра)   
                  actualCol =>!expected.Columns.Any                 
                  (
                    expectedCol => StringEqualsIgnoreCase(expectedCol.Name, actualCol.Name)
                  )
                )                                                                                   
                .ToList();
        }

        /// <summary>
        /// Находит колонки с одинаковыми именами, но различающимися свойствами.
        /// </summary>
        private List<ColumnMismatch> FindMismatchedColumns(TableSchema expected, TableSchema actual)
        {
            var mismatches = new List<ColumnMismatch>();

            foreach (var expectedCol in expected.Columns)
            {
                // Ищем колонку в actual с таким же именем (без учёта регистра)
                var actualCol = actual.Columns
                    .FirstOrDefault(col => StringEqualsIgnoreCase(col.Name, expectedCol.Name));


                // Если колонка найдена, но её свойства не совпадают — добавляем в несоответствия
                if (actualCol != null && !AreColumnsEqual(expectedCol, actualCol))
                {
                    mismatches.Add(new ColumnMismatch
                    {
                        ColumnName = expectedCol.Name,
                        Expected = expectedCol,
                        Actual = actualCol
                    });
                }
            }

            return mismatches;
        }

        /// <summary>
        /// Проверяет, полностью ли совпадают свойства двух колонок.
        /// </summary>



        private bool AreColumnsEqual(ColumnSchema expected, ColumnSchema actual)
        {
            // Базовые проверки, которые всегда доступны
            if (expected.Name != actual.Name) return false;
            if (expected.Type != actual.Type) return false;
            if (expected.IsNullable != actual.IsNullable) return false;

            // Проверка PRIMARY KEY, так как это критично для структуры
            if (expected.IsPrimaryKey != actual.IsPrimaryKey) return false;

            // Если есть значение по умолчанию - проверяем его
            if (expected.DefaultValue != null || actual.DefaultValue != null)
            {
                return AreDefaultValuesEqual(expected.DefaultValue, actual.DefaultValue);
            }

            return true;
        }

        private bool AreDefaultValuesEqual(object expectedValue, object actualValue)
        {
            if (expectedValue == null && actualValue == null) return true;
            if (expectedValue == null || actualValue == null) return false;

            return expectedValue.Equals(actualValue);
        }

        /// <summary>
        /// Сравнивает две строки без учёта регистра (использует OrdinalIgnoreCase для точности).
        /// </summary>
        private bool StringEqualsIgnoreCase(string? a, string? b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }


       
       
    }












}





