using System.Reflection;
using System.Text.Json.Serialization;

namespace Infisical.Sdk.Util
{


  public static class ObjectToDictionaryConverter
  {
    /// <summary>
    /// Converts any object to a Dictionary&lt;string, string&gt;.
    /// Respects JsonPropertyName attributes for key names.
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <param name="includeNullValues">Whether to include properties with null values</param>
    /// <returns>Dictionary with string keys and string values</returns>
    public static Dictionary<string, string> ToDictionary(object obj, bool includeNullValues = false)
    {
      if (obj == null)
        return new Dictionary<string, string>();

      var result = new Dictionary<string, string>();
      var type = obj.GetType();
      var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

      foreach (var property in properties)
      {
        var value = property.GetValue(obj);

        // Skip null values if not including them
        if (!includeNullValues && value == null)
          continue;

        // Get the key name - prefer JsonPropertyName attribute, fallback to property name
        var keyName = GetPropertyKeyName(property);

        // Convert value to string
        var stringValue = ConvertValueToString(value);

        result[keyName] = stringValue;
      }

      return result;
    }

    /// <summary>
    /// Gets the key name for a property, checking for JsonPropertyName attribute first
    /// </summary>
    private static string GetPropertyKeyName(System.Reflection.PropertyInfo property)
    {
      var jsonPropertyNameAttr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
      return jsonPropertyNameAttr?.Name ?? property.Name;
    }

    /// <summary>
    /// Converts various value types to string representation
    /// </summary>
    private static string ConvertValueToString(object? value)
    {
      return value switch
      {
        null => string.Empty,
        string str => str,
        bool boolean => boolean.ToString().ToLowerInvariant(),
        Array array => string.Join(",", array.Cast<object>().Select(x => x?.ToString() ?? "")),
        IEnumerable<object> enumerable => string.Join(",", enumerable.Select(x => x?.ToString() ?? "")),
        _ => value.ToString() ?? string.Empty
      };
    }
  }
}