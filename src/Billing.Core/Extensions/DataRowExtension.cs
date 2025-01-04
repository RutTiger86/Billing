using System.Data;

namespace Billing.Core.Extensions
{
    public static class DataRowExtension
    {
        public static T ToClass<T>(this DataRow row) where T : new()
        {
            var obj = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                if (row.Table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                {
                    var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    if (propertyType.IsEnum)
                    {
                        prop.SetValue(obj, Enum.ToObject(propertyType, row[prop.Name]));
                    }
                    else
                    {
                        prop.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyType));
                    }
                }
            }
            return obj;
        }
    }
}
