using System;
using System.Collections.Generic;
using System.Linq;
using GBD.SaveSystem;
using UnityEngine; //https://github.com/GabrielBigardi/Generic-Save-System/blob/main/DOCUMENTATION.md

public class SaveManagerSingleton
{
    private static SaveManager _instance;

    public static SaveManager GetInstance()
    {
        if (_instance == null) {_instance = new SaveManager();}
        return _instance;
    }
}

public struct TypeConduitValue
{
    public Type Type;
    public Conduit<object> Conduit;
    public object Value;
    public bool IsValueValid;
}

public struct TypeObjectPair
{
    public Type Type;
    public object Object;

    public TypeObjectPair(Type t, object o)
    {
        Type = t;
        Object = o;
    }
}

public struct ObjectSaveData
{
    public int ID;
    public List<TypeObjectPair> TypeObjectPairs;

    public ObjectSaveData(int id, List<TypeObjectPair> typeObjectPairs)
    {
        ID = id;
        TypeObjectPairs = typeObjectPairs;
    }
}

public class SaveManager
{
    private const string FileName = "SaveGame";
    private Dictionary<int, List<TypeConduitValue>> _registeredObjects;
    public static void Register(int id,  List<TypeConduitValue> typeObjectList)
    {
        
        // TODO: map saved values to conduit values (one to one?)
        
        SaveManager self = SaveManagerSingleton.GetInstance();
        if (!self._registeredObjects.ContainsKey(id))
        {
            self._registeredObjects.Add(id, typeObjectList); 
        }
        else
        {
            foreach (TypeConduitValue typeConduitValue in self._registeredObjects[id])
            {
                if (typeConduitValue.IsValueValid)
                    typeConduitValue.Conduit.Set(typeConduitValue.Value);
            }
        }
    }

    public static void Save()
    {
        
        SaveManager self = SaveManagerSingleton.GetInstance();
        List<int> keys = self._registeredObjects.Keys.ToList();
        List<ObjectSaveData> saveData = keys.ConvertAll(id =>
        {
            return new ObjectSaveData(id,
                self._registeredObjects[id].ConvertAll(
                    x => new TypeObjectPair(x.Type, x.Conduit.Get())
                )
            );

        });
        bool saveSuccess = SaveSystem.SaveGame(FileName, saveData, GameSecrets.SaveKey);
        if (!saveSuccess)
        {
            Debug.Log("Error while saving");
            return;
        }
    }

    public static void Load()
    {
        List<ObjectSaveData> saveData;
        bool loadSuccess = SaveSystem.LoadGame(FileName, out saveData, GameSecrets.SaveKey);
        if (!loadSuccess)
        {
            Debug.Log("Error while loading");
            return;
        }
        
        SaveManager self = SaveManagerSingleton.GetInstance();
        foreach (ObjectSaveData objectSaveData in saveData)
        {
            //self._savedData[objectSaveData.ID].AddRange(objectSaveData.TypeObjectPairs);
        }
    }
    
    
}