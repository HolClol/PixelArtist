using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum SaveKey
{
    GameData,
    CurrencyData,
    SettingData,
    TutorialData
}
public interface ISaveData
{
    void Save();
    void Load();
}
/*public class ITC_UserData
{
    public int code;
    public List<UserData_Infor> data;
    public string message;
}*/
[System.Serializable]
public class UserData_Infor
{
    public string _id;
    public int account_id;
    public string create_at;
    public string type;
    public string value; // This is a JSON string we need to parse again
}
public static class SaveDataRegistry
{
    public static List<Type> GetAllSaveDataTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                typeof(ISaveData).IsAssignableFrom(type) && // implements ISaveData
                !type.IsInterface &&                        // skip ISaveData itself
                !type.IsAbstract)                           // skip abstract base classes
            .ToList();
    }

    public static int Count => GetAllSaveDataTypes().Count;
}
public class StackDataSave
{
    private Stack<(SaveKey, string)> saveStack = new();
    private bool isProcessing = false;
    private readonly object lockObj = new();

    public void SaveData(SaveKey key, string data = "")
    {
        lock (lockObj)
        {
            saveStack.Push((key, data));
            if (!isProcessing)
            {
                isProcessing = true;
                _ = ProcessOneAsync();
            }
        }
    }

    private async Task ProcessOneAsync()
    {
        (SaveKey key, string data) itemToSolve;
        lock (lockObj)
        {
            if (saveStack.Count == 0)
            {
                isProcessing = false;
                return;
            }

            itemToSolve = saveStack.Pop();
            saveStack.Clear();
        }
        await DataManager.SaveDataToLocalAsync(itemToSolve.key, itemToSolve.data);
        //Debug.Log(itemToSolve.key.ToString() + " - " + itemToSolve.data.ToString());
        lock (lockObj)
        {
            if (saveStack.Count > 0)
            {
                _ = ProcessOneAsync();
            }
            else
            {
                isProcessing = false;
            }
        }
    }
}
public abstract class DataBase<T> : ISaveData where T : DataBase<T>, new()
{
    private static T instance;
    private static readonly object locker = new object();
    protected StackDataSave stackDataSave = new StackDataSave();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new T();
                        instance.Load(); // Optional: auto-load on first access
                    }
                }
            }
            return instance;
        }
    }

    protected abstract SaveKey SaveKey { get; }

    public virtual void Save()
    {
        string data = JsonUtility.ToJson(this, true);
        stackDataSave.SaveData(SaveKey, data);
    }

    public virtual void Load()
    {
        string loadedData = DataManager.LoadDataFromLocal(SaveKey);
        if (!string.IsNullOrEmpty(loadedData))
        {
            instance = JsonUtility.FromJson<T>(loadedData);
        }
        else
        {
            instance = new T(); // fallback to default
        }
    }
}
public class DataManager
{
    public static List<SaveKey> keyList = new List<SaveKey>{
            SaveKey.CurrencyData
        };

    public static bool CheckDataLocal(SaveKey key)
    {
        string path = Path.Combine(Application.persistentDataPath, key + ".json");
        string data = "";
        if (File.Exists(path))
        {
            data = File.ReadAllText(path);
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void SaveDataToLocal(SaveKey key, string data = "")
    {
        string path = Path.Combine(Application.persistentDataPath, key + ".json");
        File.WriteAllText(path, data);
    }
    public static async Task SaveDataToLocalAsync(SaveKey key, string data = "")
    {
        string path = Path.Combine(Application.persistentDataPath, key + ".json");
        await File.WriteAllTextAsync(path, data);
    }
    public static string LoadDataFromLocal(SaveKey key)
    {
        string path = Path.Combine(Application.persistentDataPath, key + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return json;
        }
        else
        {
            Debug.Log("No save file found!");
            return null;
        }
    }
    public static async Task<string> LoadDataFromLocalAsync(SaveKey key)
    {
        string path = Path.Combine(Application.persistentDataPath, key + ".json");
        string data = "";
        if (File.Exists(path))
        {
            data = await File.ReadAllTextAsync(path);
        }
        else
        {
            data = "";
        }
        return data;
    }

}
