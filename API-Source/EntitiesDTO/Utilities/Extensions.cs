using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EntitiesDTO.Utilities
{
    public static class Extensions
    {
        //public static T DataTablesToItem<T>(this DataTable dt)
        //{
        //    T item = default(T);
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        item = GetItem<T>(row);
        //        break;
        //    }
        //    return item;
        //}
        //public static List<T> DataTablesToList<T>(this DataTable dt)
        //{
        //    List<T> data = new List<T>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        T item = GetItem<T>(row);
        //        data.Add(item);
        //    }
        //    return data;
        //}
        //public static T GetItem<T>(DataRow dr)
        //{
        //    Type temp = typeof(T);
        //    T obj = Activator.CreateInstance<T>();

        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        foreach (PropertyInfo pro in temp.GetProperties())
        //        {
        //            if (pro.Name == column.ColumnName && pro.PropertyType == column.DataType)
        //            {
        //                pro.SetValue(obj, dr[column.ColumnName], null);
        //                break;
        //            }
        //            else if (pro.Name == column.ColumnName && pro.PropertyType == typeof(Guid))
        //            {
        //                pro.SetValue(obj, Guid.Parse(dr[column.ColumnName].ToString()), null);
        //                break;
        //            }
        //            else if (pro.Name == column.ColumnName && pro.PropertyType == typeof(decimal))
        //            {
        //                pro.SetValue(obj, decimal.Parse(dr[column.ColumnName].ToString()), null);
        //                break;
        //            }
        //            else if (pro.Name == column.ColumnName && pro.PropertyType == typeof(int))
        //            {
        //                pro.SetValue(obj, int.Parse(dr[column.ColumnName].ToString()), null);
        //                break;
        //            }
        //            else if (pro.Name == column.ColumnName)
        //            {
        //                pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
        //                break;
        //            }  
        //        }
        //    }
        //    return obj;
        //}

        //public static T DataTablesToItem<T>(this DataTable dt)
        //{
        //    T item = default(T);
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        item = GetItem<T>(row);
        //        break;
        //    }
        //    return item;
        //}
        //public static List<T> DataTablesToList<T>(this DataTable dt)
        //{
        //    List<T> data = new List<T>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        T item = GetItem<T>(row);
        //        data.Add(item);
        //    }
        //    return data;
        //}
        //public static T GetItem<T>(DataRow dr)
        //{
        //    Type temp = typeof(T);
        //    T obj = Activator.CreateInstance<T>();
        //    SqlI18N sqlI18 = new SqlI18N("India Standard Time", "dd-MM-yyyy hh:mm:ss tt");

        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        foreach (PropertyInfo pro in temp.GetProperties())
        //        {
        //            if (pro.Name == column.ColumnName)
        //            {
        //                pro.SetValueEx(obj, dr[column.ColumnName], sqlI18);
        //                break;
        //            }
        //            else
        //                continue;
        //        }
        //    }
        //    return obj;
        //}

        //internal static void SetValueEx(this PropertyInfo property, object ml, object value, SqlI18N sqlI18N)
        //{
        //    var dataType = value.GetType();
        //    value = value.ToStringOrEmpty();
        //    Type srcType;
        //    var dataTypeName = dataType.Name.ToLowerCase();
        //    var propType = property.PropertyType.Name.ToLowerCase();
        //    if (propType == "datei18")
        //        dataTypeName = "datei18";
        //    if (dataTypeName == "datetimeoffset" || dataTypeName == "datetime" || dataTypeName == "datei18")
        //        srcType = dataType;
        //    else
        //        srcType = value.GetType();
        //    if (srcType != property.PropertyType)
        //    {
        //        object newValue;
        //        switch (dataTypeName)
        //        {
        //            case "int64":
        //                newValue = value.ToInt64();
        //                break;
        //            case "bool":
        //            case "boolean":
        //                newValue = value.ToBoolean();
        //                break;
        //            case "datei18":
        //                if (value.ToStringOrEmpty() == string.Empty)
        //                    newValue = "";
        //                else if (value is null)
        //                    newValue = "";
        //                else
        //                    newValue = DateConverter.ToDateI18(value.ToString(), sqlI18N);
        //                break;
        //            case "datetime":
        //                if (value.ToStringOrEmpty() == string.Empty)
        //                    newValue = "";
        //                else
        //                    newValue = value.ToString().ToDateTime(sqlI18N);
        //                break;
        //            case "datetimeoffset":
        //                if (value.ToStringOrEmpty() == string.Empty)
        //                    newValue = "";
        //                else
        //                    newValue = DateConverter.ToDateTimeUTC(value.ToString(), sqlI18N).ToString();
        //                break;
        //            case "string":
        //                if(propType == "guid") newValue = new Guid(value.ToString());
        //                else newValue = value.ToStringOrEmpty();
        //                break;
        //            case "decimal":
        //                newValue = value.ToDecimal();
        //                break;
        //            case "int":
        //                newValue = value.ToInt();
        //                break;
        //            case "int32":
        //                newValue = value.ToInt();
        //                break;
        //            case "byte":
        //                newValue = value;
        //                break;
        //            case "guid":
        //                newValue = new Guid(value.ToString());
        //                break;
        //            default:
        //                {
        //                    newValue = Convert.ChangeType(value, property.PropertyType);
        //                }
        //                break;
        //        }
        //        var t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        //        var safeValue = (newValue == null || newValue == "") ? null : Convert.ChangeType(newValue, t);
        //        property.SetValue(ml, safeValue, null);
        //    }
        //    else { property.SetValue(ml, value, null); }
        //}

        public static DataTable ToItemDataTable<T>(this T item)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            // Get all properties of the object
            PropertyInfo[] properties = typeof(T).GetProperties();
            // Add columns based on properties
            foreach (PropertyInfo prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            // Add a single row
            DataRow row = dataTable.NewRow();
            foreach (PropertyInfo prop in properties)
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
            return dataTable;
        }
        public static T DataTablesToItem<T>(this DataTable dt)
        {
            T item = default(T);
            foreach (DataRow row in dt.Rows)
            {
                item = GetItem<T>(row);
                break;
            }
            return item;
        }
        public static List<T> DataTablesToList<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            SqlI18N sqlI18 = new SqlI18N("India Standard Time", "dd-MM-yyyy hh:mm:ss tt");

            foreach (DataColumn column in dr.Table.Columns)
            {
                var columnName = column.ColumnName;
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        pro.SetValueEx(obj, dr[column.ColumnName], sqlI18);
                        break;
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
        internal static void SetValueEx(this PropertyInfo property, object ml, object value, SqlI18N sqlI18N)
        {
            var dataType = value.GetType();
            value = value.ToStringOrEmpty();
            Type srcType;
            var dataTypeName = dataType.Name.ToLowerCase();
            var propType = property.PropertyType.Name.ToLowerCase();
            if (propType == "datei18")
                dataTypeName = "datei18";
            if (dataTypeName == "datetimeoffset" || dataTypeName == "datetime" || dataTypeName == "datei18")
                srcType = dataType;
            else
                srcType = value.GetType();
            if (srcType != property.PropertyType)
            {
                object newValue;
                switch (dataTypeName)
                {
                    case "int64":
                        newValue = value.ToInt64();
                        break;
                    case "bool":
                    case "boolean":
                        newValue = value.ToBoolean();
                        break;
                    case "datei18":
                        if (value.ToStringOrEmpty() == string.Empty)
                            newValue = "";
                        else if (value is null)
                            newValue = "";
                        else
                            newValue = DateConverter.ToDateI18(value.ToString(), sqlI18N);
                        break;
                    case "datetime":
                        if (value.ToStringOrEmpty() == string.Empty)
                            newValue = "";
                        else
                            newValue = value.ToString().ToDateTime(sqlI18N);
                        break;
                    case "datetimeoffset":
                        if (value.ToStringOrEmpty() == string.Empty)
                            newValue = "";
                        else
                            newValue = DateConverter.ToDateTimeUTC(value.ToString(), sqlI18N).ToString();
                        break;
                    case "string":
                        if (propType == "guid") newValue = new Guid(value.ToString());
                        else newValue = value.ToStringOrEmpty();
                        break;
                    case "decimal":
                        newValue = value.ToDecimal();
                        break;
                    case "int":
                        newValue = value.ToInt();
                        break;
                    case "int32":
                        newValue = value.ToInt();
                        break;
                    case "byte":
                        newValue = value;
                        break;
                    case "guid":
                        newValue = new Guid(value.ToString());
                        break;
                    default:
                        {
                            newValue = Convert.ChangeType(value, property.PropertyType);
                        }
                        break;
                }

                var t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var safeValue = (newValue == null || newValue == "") ? null : Convert.ChangeType(newValue, t);
                property.SetValue(ml, safeValue, null);
            }
            else { property.SetValue(ml, value, null); }
        }

        public static string ToConditionFormat (this string source) {
            return Regex.Replace(source, @"\s", string.Empty).ToUpper();
        }
    }
}
