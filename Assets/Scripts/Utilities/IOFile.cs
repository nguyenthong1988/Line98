using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class IOFile
{
    protected static readonly string RESOURCE_PATH = @"Assets/Resources/";
    protected static readonly string CACHE_PATH = Application.persistentDataPath;//@"Assets/Resources/";

    public static string GetString(string fullPath, bool createIfNotExist = false)
    {
        string result = string.Empty;
        if (File.Exists(fullPath))
        {
            result = File.ReadAllText(fullPath);
        }
        else
        {
            if (createIfNotExist)
                File.WriteAllText(fullPath, result);
        }
        return result;
    }

    public static string GetCacheString(string fileName, bool createIfNotExist = false)
    {
        string fullPath = CachePath(fileName);
        return GetString(fullPath, createIfNotExist);
    }

    public static string GetResouceString(string fileName, bool createIfNotExist = false)
    {
        string fullPath = ResourcePath(fileName);
        return GetString(fullPath, createIfNotExist);
    }

    public static T ReadJson<T>(string fullPath)
    {
        string jsonString = GetString(fullPath);
        if (!string.IsNullOrEmpty(jsonString))
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        return default(T);
    }

    public static T ReadResourceJson<T>(string fileName, string subFolder = "")
    {
        string fullPath = ResourcePath(fileName, subFolder);
        return ReadJson<T>(fullPath);
    }

    public static T ReadCacheJson<T>(string fileName, string subFolder = "")
    {
        string fullPath = CachePath(fileName, subFolder);
        return ReadJson<T>(fullPath);
    }

    public static bool WriteJson<T>(T data, string fullPath)
    {
        bool success = false;

        string jsonString = JsonConvert.SerializeObject(data);
        if (!string.IsNullOrEmpty(jsonString))
        {
            if (!File.Exists(fullPath)) File.Create(fullPath).Dispose();
            File.WriteAllText(fullPath, jsonString);

            success = true;
        }

        return success;
    }

    public static bool WriteCacheJson<T>(T data, string fileName)
    {
        string fullPath = CachePath(fileName);
        Debug.Log(string.Format("WriteCacheJson[{0}: {1}", typeof(T), fullPath));
        return WriteJson<T>(data, fullPath);
    }

    public static string CachePath(string fileName, string subFolder = "")
    {
        if (string.IsNullOrEmpty(subFolder))
            return Path.Combine(CACHE_PATH, fileName);

        return Path.Combine(CACHE_PATH, subFolder, fileName);
    }

    public static string ResourcePath(string fileName, string subFolder = "")
    {
        if (string.IsNullOrEmpty(subFolder))
            return Path.Combine(RESOURCE_PATH, fileName);

        return Path.Combine(RESOURCE_PATH, subFolder, fileName);
    }
}