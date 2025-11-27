using HomeNetCore.Data.Enums;
using HomeNetCore.Data.Schemes;

namespace HomeNetCore.Data.Interfaces
{
    public interface ISchemaProvider
    {       
        /// <summary>
        /// получаем актуальную схему бд
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        Task<TableSchema> GetActualTableSchemaAsync(string? tableName);

        ColumnType MapType(string? dbType);
    }




}
