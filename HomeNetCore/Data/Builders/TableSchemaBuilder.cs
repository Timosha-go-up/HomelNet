using HomeNetCore.Data.Schemes;
using WpfHomeNet.Data.Builders;

namespace HomeNetCore.Data.Builders
{
    public class TableSchemaBuilder
    {
        private readonly string _tableName;
        private readonly List<ColumnSchemaBuilder> _columnBuilders = new();

        public TableSchemaBuilder(string tableName) => _tableName = tableName;

        // Возвращаем builder для дальнейшей настройки
        public ColumnSchemaBuilder AddColumn(string name)
        {
            var builder = new ColumnSchemaBuilder(name);
            _columnBuilders.Add(builder);
            return builder;
        }

        // Финализируем все builders при генерации схемы
        public TableSchema Generate() => new TableSchema
        {
            TableName = _tableName,
            Columns = _columnBuilders
                .Select(builder => builder.Build())  // Build() вызывается здесь!
                .ToList()
        };
    }



}
