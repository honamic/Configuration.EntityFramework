using System.Collections;
using System.Globalization;

namespace Honamic.Configuration.EntityFramework.Internal;

internal static class ObjectSerializer
{
    public static Dictionary<string, string> GetPropertiesValues(this object obj, Dictionary<string, string> dic = null, string parent = "")
    {
        var type = obj?.GetType();

        try
        {
            if (dic == null)
            {
                dic = new Dictionary<string, string>();
            }

            if (obj == null)
            {
                dic.Add(parent, null);
            }
            else if (type.IsValueType || type == typeof(string))
            {
                dic.Add(parent, Convert.ToString(obj, CultureInfo.InvariantCulture));
            }
            else if (obj is ICollection)
            {
                var collection = obj as ICollection;
                var index = 0;
                foreach (var item in collection)
                {
                    var itemType = item.GetType();

                    if (itemType.IsClass && itemType != typeof(string))
                    {
                        item.GetPropertiesValues(dic, parent + index + ":");

                    }
                    else
                    {
                        item.GetPropertiesValues(dic, parent + ":" + index);
                    }

                    index++;
                }
            }
            else
            {
                foreach (var property in type.GetProperties())
                {

                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        property.GetValue(obj).GetPropertiesValues(dic, parent + property.Name + ":");
                    }
                    else
                    {
                        property.GetValue(obj).GetPropertiesValues(dic, parent + property.Name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"serialize failed {parent}{type?.Name}", ex);
        }

        return dic;
    }

}
