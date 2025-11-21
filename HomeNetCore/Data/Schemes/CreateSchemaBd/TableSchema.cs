using HomeNetCore.Data.Adapters;

namespace HomeNetCore.Data.Schemes
{
    // TableSchema с улучшенной логикой
    public class TableSchema
    {
        public string? TableName { get; set; }
        public List<ColumnSchema> Columns { get; set; } = new();
        public string? Fields { get; private set; }
        public string? Parameters { get; private set; }
        public string? SetClause { get; private set; }
      
        public string? IdColumnName
        {
            get
            {
                // Возвращаем имя первой найденной колонки с флагом первичного ключа
                return Columns.FirstOrDefault(c => c.IsPrimaryKey)?.Name;
            }
        }
        
        public void Initialize()
        {
            Fields = string.Join(", ", Columns.Select(c => c.Name));
            Parameters = string.Join(", ", Columns.Select(c => $"@{c.Name}"));
            SetClause = string.Join(", ", Columns.Select(c =>
                $"{c.Name} = @{c.Name}"));
        }
    }
}
