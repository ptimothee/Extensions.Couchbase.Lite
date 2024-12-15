using Couchbase.Lite;
using System.Reflection;
using System.Collections;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Codemancer.Extensions.Couchbase.Lite;

public static class ObjectExtensions
{
    public static MutableDocument ToMutableDocument<T>(this T? obj) where T : class
    {
        return ToMutableDocument(obj, null);
    }

    public static MutableDocument ToMutableDocument<T>(this T? obj, string? id = null) where T : class
    {
        var data = obj.DeconstructAsDictionary();
        if (id is null)
        {
            return new MutableDocument(data);
        }

        return new MutableDocument(id, data);
    }

    private static bool IsSimple(this Type type) => (type.IsValueType || type == typeof(string));

    private static Dictionary<string, object?> DeconstructAsDictionary(this object? obj)
    {
        if (obj is null)
        {
            return [];
        }

        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(pi => !Attribute.IsDefined(pi, typeof(JsonIgnoreAttribute)))
                            .ToList();

        if(properties.Count == 0)
        {
            return [];
        }

        var dictionary = new Dictionary<string, object?>();
        foreach (var propertyInfo in properties)
        {
            var propertyType = propertyInfo.PropertyType;
            string propertyName = propertyInfo.GetPropertyName();
            var propertyValue = propertyInfo.GetPropertyValue(obj);

            if(propertyValue is null)
            {
                continue;
            }
          
            if (!propertyType.IsSimple() && propertyType.IsClass) // Complex type
            {
                if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                {
                    if (propertyType == typeof(byte[]))
                    {
                        dictionary[propertyName] = new Blob(string.Empty, (byte[])propertyValue);
                        continue;
                    }

                    if (propertyType.IsArray && propertyType.GetElementType()!.IsSimple()
                         || (propertyValue is IList && propertyValue.GetType().GetTypeInfo().GenericTypeArguments[0].IsSimple()))
                    {

                        dictionary[propertyName] = propertyValue;
                        continue;
                    }

                    var items = propertyValue as IEnumerable;

                    var dictionaries = new List<Dictionary<string, object?>>();
                    foreach (var item in items!)
                    {
                        dictionaries.Add(item.DeconstructAsDictionary());
                    }

                    dictionary[propertyName] = dictionaries.ToArray();
                    continue;                
                }

                if (propertyType.IsAssignableTo(typeof(Stream)))
                {
                    dictionary[propertyName] = new Blob(string.Empty, (Stream)propertyValue);
                    continue;
                }

                if (propertyType == typeof(Blob))
                {
                    dictionary[propertyName] = propertyValue;
                    continue;
                }

                dictionary[propertyName] = propertyValue.DeconstructAsDictionary();
                continue;
            }    
              
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                var dateTimeVal = (DateTime)propertyValue;
                if (dateTimeVal != default)
                {
                    dictionary[propertyName] = new DateTimeOffset(dateTimeVal);
                }
                continue;
            }

            if (propertyType.IsEnum)
            {
                dictionary[propertyName] = propertyValue?.ToString();
                continue;
            }

            dictionary[propertyName] = propertyValue;       
        }

        return dictionary;
    }

    private static object? GetPropertyValue(this PropertyInfo propertyInfo, object obj)
    {
        var propertyValue = propertyInfo.GetValue(obj);
        if (propertyValue == null)
        {
            return null;
        }

        if (propertyInfo.PropertyType.IsEnum)
        {
            string enumName = propertyValue.ToString()!;
            var enumMemberAttribute = propertyInfo.PropertyType.GetMember(enumName)
                                                        .FirstOrDefault()?
                                                        .GetCustomAttribute<EnumMemberAttribute>();
            if (enumMemberAttribute != null)
            {
                propertyValue = enumMemberAttribute.Value;
            }
        }

        return propertyValue;
    }

    private static string GetPropertyName(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.CustomAttributes?.Count() > 0 &&
            propertyInfo.GetCustomAttribute(typeof(JsonPropertyNameAttribute)) is JsonPropertyNameAttribute jsonProperty)
        {
            return jsonProperty.Name;
        }

        return propertyInfo.Name;        
    }
}
