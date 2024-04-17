using System;
using System.Collections.Generic;
using System.Linq;
using GBD.SaveSystem;
using Debug = UnityEngine.Debug; //https://github.com/GabrielBigardi/Generic-Save-System/blob/main/DOCUMENTATION.md

public class SaveManagerSingleton
{
    private static SaveManager _instance;

    public static SaveManager GetInstance()
    {
        if (_instance == null) {_instance = new SaveManager();}
        return _instance;
    }
}

public struct TypeConduitPair
{
    public Type Type;
    public Conduit<object> Conduit;
    public TypeConduitPair(Type t, Conduit<object> value)
    {
        Type = t;
        Conduit = value;
    }

    public TypeConduitPair(Type t, Action<object> set, Func<object> get)
    {
        Type = t;
        Conduit = new Conduit<object>(set, get);
    }
}

public struct TypeValuePair
{
    public Type Type;
    public object Value;

    public TypeValuePair(Type t, object v)
    {
        Type = t;
        Value = v;
    }
}

public struct ConduitValuePair
{
    public TypeConduitPair TypeConduitPair { get; set; }
    public TypeValuePair ValuePair { get; set; }
    public bool IsValueValid;
    public bool IsConduitValid;

    public ConduitValuePair(TypeConduitPair typeConduitPair)
    {
        ValuePair = new TypeValuePair();
        IsValueValid = false;
        
        TypeConduitPair = typeConduitPair;
        IsConduitValid = true;
    }
    public ConduitValuePair(TypeValuePair typeValuePair)
    {
        ValuePair = typeValuePair;
        IsValueValid = true;
        
        TypeConduitPair = new TypeConduitPair();
        IsConduitValid = false;
    }

    public void AddValuePair(TypeValuePair typeValuePair)
    {
        ValuePair = typeValuePair;
        IsValueValid = true;
    }
    public void AddConduitPair(TypeConduitPair typeConduitPair)
    {
        TypeConduitPair = typeConduitPair;
        IsConduitValid = true;
    }

    public void InvalidateConduit()
    {
        IsConduitValid = false;
        TypeConduitPair = new TypeConduitPair();
    }
    public void InvalidateValue()
    {
        IsValueValid = false;
        ValuePair = new TypeValuePair();
    }
    
}

public struct ObjectSaveData
{
    public int ID;
    public int Hash;
    public readonly List<TypeValuePair> TypeValuePairs;
    
    public ObjectSaveData(int id, List<TypeValuePair> typeValuePairs)
    {
        ID = id;
        TypeValuePairs = typeValuePairs;
        Hash = typeValuePairs.GetHashCode();
    }
}

public class SaveManager
{
    private const string FileName = "SaveGame";
    private Dictionary<int, List<ConduitValuePair>> _gameStateObjects;
    private static SaveManager _self;
    private static void _setup()
    {
        if (_self is null)
        {
            _self = new SaveManager
            {
                _gameStateObjects = new Dictionary<int, List<ConduitValuePair>>()
            };
        }
    }
    public static void Register(int id,  List<TypeConduitPair> typeConduitPairs)
    {
        
        _setup();
        if (_self._gameStateObjects[id].Count != typeConduitPairs.Count) //checks for variable updates/reregister updates
        {
            _self._gameStateObjects.Remove(id);
        }
        
        if (!_self._gameStateObjects.ContainsKey(id)) 
        {
            _self._gameStateObjects.Add(id, typeConduitPairs.ConvertAll(x => new ConduitValuePair(x))); 
        }
        else
        {
            for (int i = 0; i < typeConduitPairs.Count; i++)
            {
                TypeConduitPair typeConduitPair = typeConduitPairs[i];
                ConduitValuePair conduitValuePair = _self._gameStateObjects[id][i];
                if (conduitValuePair.IsValueValid)
                {
                    bool successful = typeConduitPair.Conduit.SetVariable(conduitValuePair.ValuePair);
                    if (!successful) conduitValuePair.AddValuePair(typeConduitPair.Conduit.GetVariable());
                }
                conduitValuePair.AddConduitPair(typeConduitPair);
                _self._gameStateObjects[id][i] = conduitValuePair;
            }
        }
    }

    public static void Save()
    {

        _setup();
        List<int> keys = _self._gameStateObjects.Keys.ToList();
        List<ObjectSaveData> saveData = keys.ConvertAll(id =>
        {
            return new ObjectSaveData(id,
                _self._gameStateObjects[id].ConvertAll(
                    x => new TypeValuePair(x.TypeConduitPair.Type, x.TypeConduitPair.Conduit.GetVariable()) //hash assigned here
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
        bool loadSuccess = SaveSystem.LoadGame(FileName, out List<ObjectSaveData> saveData, GameSecrets.SaveKey);
        if (!loadSuccess)
        {
            Debug.Log("Error while loading");
            return;
        }

        _setup();
        foreach (ObjectSaveData objectSaveData in saveData)
        {
            if (objectSaveData.Hash == objectSaveData.TypeValuePairs.GetHashCode()) //is loaded data valid
            {
                if(!_self._gameStateObjects.TryAdd(objectSaveData.ID,
                    objectSaveData.TypeValuePairs.ConvertAll(x => new ConduitValuePair(x))
                    )
                ){ 
                    /* if the object already exists, replace its value pair instead
                       this is unsafe, because it might try to assign a value to the wrong conduit
                       but we are helping to prevent corrupted data by checking the hash
                       if the order changes (which is what would need to happen for the above),
                       the hash should too, making the data invalid and preventing corruption */
                    
                    var conduits = _self._gameStateObjects[objectSaveData.ID];
                    var data = objectSaveData.TypeValuePairs;
                    for(int index = 0; index < data.Count; index++)
                    {
                        conduits[index].TypeConduitPair.Conduit.SetVariable(data[index]);
                        conduits[index].AddValuePair(data[index]);
                    }
                    _self._gameStateObjects.Add(objectSaveData.ID, conduits);
                }
            }
            else
            {
                Debug.Log("Warning while loading: Failed to load with ID " + objectSaveData.ID);
            }
            //_self._savedData[objectSaveData.ID].AddRange(objectSaveData.TypeValuePairs);
        }
    }
    
    
}