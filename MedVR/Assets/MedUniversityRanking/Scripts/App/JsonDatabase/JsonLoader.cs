using System.IO;
using UnityEngine;

public static class JsonLoader
{
    public static string GetPersistentPath(string fileName) =>
      Path.Combine(Application.persistentDataPath, fileName);

    public static string GetStreamingPath(string fileName) =>
        Path.Combine(Application.streamingAssetsPath, fileName);

    public static string LoadJsonFile(string fileName, out string usedPath)
    {
        string persistentPath = GetPersistentPath(fileName);
        usedPath = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        string streamingPath = GetStreamingPath(fileName);
        UnityWebRequest www = UnityWebRequest.Get(streamingPath);
        www.downloadHandler = new DownloadHandlerBuffer();
        var request = www.SendWebRequest();
        while (!request.isDone) { }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Erro ao carregar JSON em Android: " + www.error);
            return null;
        }

        usedPath = streamingPath;
        Debug.Log($"Load on: {usedPath}");
        return www.downloadHandler.text;
#else

        if (File.Exists(persistentPath))
        {
            usedPath = persistentPath;
            Debug.Log($"Load on: {usedPath}");
            return File.ReadAllText(persistentPath);
        }
        else
        {
            string streamingPath = GetStreamingPath(fileName);
            usedPath = streamingPath;
            Debug.Log($"Load on: {usedPath}");
            return File.ReadAllText(streamingPath);
        }

#endif
    }

    public static T LoadJson<T>(string fileName)
    {
        string pathUsed;
        string json = LoadJsonFile(fileName, out pathUsed);

        if (string.IsNullOrEmpty(json))
            return default;

        return JsonUtility.FromJson<T>(json);
    }

    public static void SaveJson<T>(string fileName, T data, bool prettyPrint = true)
    {
        string persistentPath = GetPersistentPath(fileName);
        string json = JsonUtility.ToJson(data, prettyPrint);
        string path = "";
        if (File.Exists(persistentPath))
        {
            path = GetPersistentPath(fileName); ;
        }
        else
        {
            path = GetStreamingPath(fileName);
        }

        File.WriteAllText(path, json);
        Debug.Log($"Arquivo salvo em: {path}");
    }
}
