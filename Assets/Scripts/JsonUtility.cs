using System;
using Newtonsoft.Json;

public static class JsonUtility 
{
    public static T Deserialize<T>(string value, Action<T> OnSuccess = null, Action OnError = null)
    {
        var result = JsonConvert.DeserializeObject<T>(value);
        if (result == null) OnError?.Invoke();
        else OnSuccess?.Invoke(result);
        return result;
    }
}
