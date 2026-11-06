using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class Extensions
    {
        /// <summary>
        /// Mapping object to objec
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myobj"></param>
        /// <returns></returns>
        //public static T Mapper<T>(this Object myobj)
        //{
        //    Type objectType = myobj.GetType();
        //    Type target = typeof(T);
        //    List<PropertyInfo> targetProps = new List<PropertyInfo>(target.GetProperties());
        //    List<PropertyInfo> srcProps = new List<PropertyInfo>(objectType.GetProperties());
        //    var x = Activator.CreateInstance(target, false);
        //    PropertyInfo propertyInfo;
        //    object value;
        //    foreach (var targetProp in targetProps)
        //    {
        //        if (srcProps.Exists(p => p.Name == targetProp.Name) && srcProps.Exists(p => p.PropertyType == targetProp.PropertyType))
        //        {
        //            propertyInfo = typeof(T).GetProperty(targetProp.Name);
        //            value = myobj.GetType().GetProperty(targetProp.Name).GetValue(myobj, null);

        //            propertyInfo.SetValue(x, value, null);
        //        }
        //    }
        //    return (T)x;
        //}
        ///// <summary>
        ///// List data to Datatable conversion
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public static DataTable ToDataTable<T>(this IList<T> data)
        //{
        //    PropertyDescriptorCollection properties =
        //        TypeDescriptor.GetProperties(typeof(T));
        //    DataTable table = new DataTable();
        //    foreach (PropertyDescriptor prop in properties)
        //        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //    foreach (T item in data)
        //    {
        //        DataRow row = table.NewRow();
        //        foreach (PropertyDescriptor prop in properties)
        //            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
        //        table.Rows.Add(row);
        //    }
        //    return table;
        //}
    }
}
