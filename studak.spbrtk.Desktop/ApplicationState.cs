using System.Collections.Generic;

namespace studak.spbrtk.Desktop;

public static class ApplicationState
{
    private static Dictionary<string, object> _values =
        new Dictionary<string, object>();
    
    public static void SetValue(string key, object value)
    {
        if (_values.ContainsKey(key))
        {
            _values.Remove(key);
        }
        _values.Add(key, value);
    }
    
    public static void ClearValue(string key)
    {
        if (_values.ContainsKey(key))
        {
            _values.Remove(key);
        }
    }
    
    public static T GetValue<T>(string key)
    {
        if (_values.ContainsKey(key))
        {
            return (T)_values[key];
        }
        return default(T);
    }
}