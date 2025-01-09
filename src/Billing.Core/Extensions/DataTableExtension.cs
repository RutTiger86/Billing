using System.Data;

namespace Billing.Core.Extensions
{
    public static class DataTableExtension
    {        
        /// <summary>
        /// DataTable에 Class DataRow로 변환 입력
        /// MemoryDataService를 위해 작성 
        /// </summary>
        public static void AddRow<T>(this DataTable table, T obj)
        {
            var row = table.NewRow();

            foreach (var prop in typeof(T).GetProperties())
            {
                if (table.Columns.Contains(prop.Name))
                {
                    row[prop.Name] = prop.GetValue(obj) ?? DBNull.Value;
                }
            }

            table.Rows.Add(row);
        }
    }
}
