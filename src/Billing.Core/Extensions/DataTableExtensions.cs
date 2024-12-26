using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Extensions
{
    public static class DataTableExtensions
    {        
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
