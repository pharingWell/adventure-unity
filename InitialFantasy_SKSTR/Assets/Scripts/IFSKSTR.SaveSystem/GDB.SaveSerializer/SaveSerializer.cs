using System;
using System.Collections.Generic;
using System.IO;
using Leguar.TotalJSON;
using IFSKSTR.SaveSystem.GDB.SaveSerializer;
using Unity.VisualScripting;
using UnityEngine;

namespace IFSKSTR.SaveSystem.GDB.SaveSerializer
{
    public static class SaveSerializer
    {
        public static event Action GameDataSaved;
        public static event Action GameDataLoaded;

        public const string DEFAULT_SECRET_KEY = "b14ca5898a4e4133bbce2ea2315a1916";

        public static bool SaveGame<T>(string saveName, T serializableObject, string secretKey = DEFAULT_SECRET_KEY)
        {
            string json = JSON.Serialize(serializableObject).CreateString();
            string encryptedJson = AesOperation.EncryptString(secretKey, json);
            string path = $"{Application.persistentDataPath}/{saveName}.json";
            File.WriteAllText(path, encryptedJson);

            if (File.Exists(path))
            {
                Debug.Log("Saved successfully!");
                GameDataSaved?.Invoke();
                return true;
            }
            else
            {
                Debug.LogError("An error has occurred while trying to save the game.");
                return false;
            }
        }

        public static bool LoadGame<T>(string loadName, string secretKey = DEFAULT_SECRET_KEY) => LoadGame<T>(loadName, out _, secretKey);

        public static bool LoadGame<T>(string loadName, out T serializableObject, string secretKey = DEFAULT_SECRET_KEY)
        {
            serializableObject = default;
            string path = $"{Application.persistentDataPath}/{loadName}.json";

            if (!File.Exists(path))
            {
                Debug.LogWarning($"File not found at path: {path}, nothing will be loaded.");
                return false;
            }

            string json = File.ReadAllText(path);
            string decryptedJson = AesOperation.DecryptString(secretKey, json);
            //Debug.Log(decryptedJson);
            serializableObject = JSON.ParseString(decryptedJson).Deserialize<T>();
            Debug.Log($"Loaded {loadName} successfully!");
            return true;
        }

        public static void Invoke()
        {
            
            GameDataLoaded?.Invoke();
        }
    }
    
}