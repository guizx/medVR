using System.Collections.Generic;
using UnityEngine;

public static class JsonUtilityWrapper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }

    public static List<T> FromJson<T>(string json)
    {
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    public static string ToJson<T>(List<T> list, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T> { Items = list };
        string json = JsonUtility.ToJson(wrapper, prettyPrint);
        int start = json.IndexOf(":") + 1;
        int end = json.LastIndexOf("}");
        return json.Substring(start, end - start);
    }
}
