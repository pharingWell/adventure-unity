using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Debug = UnityEngine.Debug; //https://github.com/GabrielBigardi/Generic-Save-System/blob/main/DOCUMENTATION.md
using IFSKSTR.SaveSystem.GDB.SaveSerializer;
using Leguar.TotalJSON;
using UnityEngine;
using UnityEngine.Scripting;
using Object = System.Object;

namespace IFSKSTR.SaveSystem
{
    public class SaveSystem
    {
        private const string FileName = "SaveGame";
        private static Dictionary<string, List<ConduitValuePair>> _gameStateObjects = new ();
        private static Dictionary<string, int> _registeredGameObjects = new ();

        public static void Register(GameObject registeredGameObject, List<TypeConduitPair> typeConduitPairs,
            ISavable self)
        {
            Register(registeredGameObject, typeConduitPairs, new LoadSaveCallback(self.OnLoad, self.OnSave));
        }
        
        public static void Register(GameObject registeredGameObject, List<TypeConduitPair> typeConduitPairs, LoadSaveCallback loadSave = null)
        {
            if (!_registeredGameObjects.TryAdd(registeredGameObject.name, 0))
            {
                _registeredGameObjects[registeredGameObject.name] += 1;
            }
            string id = registeredGameObject.name + "_" + _registeredGameObjects[registeredGameObject.name];
            Debug.Log("Registered " + id);
            if (!_gameStateObjects.ContainsKey(id))
            {
                _gameStateObjects.Add(id, typeConduitPairs.ConvertAll(x => new ConduitValuePair(x)));
                loadSave?.Saved();
            }
            else
            {
                
               //Debug.Log("Object with id \"" +id+"\" loaded from memory");
               LoadDataOnRegister(id, typeConduitPairs);
               loadSave?.Loaded();
            }
        }

        private static void LoadDataOnRegister(string id, List<TypeConduitPair> typeConduitPairs)
        {
            if (_gameStateObjects[id].Count !=
                typeConduitPairs.Count) //checks for variable updates/reregister updates
            {
                _gameStateObjects.Remove(id);
                _gameStateObjects.Add(id, typeConduitPairs.ConvertAll(x => new ConduitValuePair(x)));
                return;
            }
            
            for (int index = 0; index < typeConduitPairs.Count; index++)
            {
                TypeConduitPair typeConduitPair = typeConduitPairs[index];
                ConduitValuePair conduitValuePair = _gameStateObjects[id][index];
                if (conduitValuePair.IsValueValid)
                {
                    typeConduitPair.Conduit.SetVariable(conduitValuePair.ValuePair);
                    //if (!successful) conduitValuePair.AddValuePair(typeConduitPair.Conduit.GetVariable());
                }

                conduitValuePair.AddConduitPair(typeConduitPair);
                _gameStateObjects[id][index] = conduitValuePair;
            }
        }

        public static void Save()
        {
            List<string> keys = _gameStateObjects.Keys.ToList();
            List<ObjectSaveData> saveData = keys.ConvertAll(id =>
            {
                return new ObjectSaveData(id,
                    _gameStateObjects[id].ConvertAll(
                        x =>  x.ConduitPair.Conduit.GetVariable() //hash assigned here
                    )
                );
            });
            ListWrapper<ObjectSaveData> wrapper = new ListWrapper<ObjectSaveData>(saveData.ToList());
            wrapper.JsonSerialize();
            bool saveSuccess = SaveSerializer.SaveGame(FileName, wrapper, GameSecrets.SaveKey);
            if (!saveSuccess)
            {
                Debug.LogError("Error while saving");
                return;
            }
        }

        public static void Load()
        {
            bool loadSuccess = SaveSerializer.LoadGame(FileName, out ListWrapper<ObjectSaveData> wrapper, GameSecrets.SaveKey);
            if (!loadSuccess)
            {
                Debug.LogError("Error while loading");
                return;
            }
            _registeredGameObjects = new Dictionary<string, int>();
            _gameStateObjects = new Dictionary<string, List<ConduitValuePair>>();
            wrapper.JsonDeserialize();
            foreach (ObjectSaveData objectSaveData in wrapper.values)
            {
                _gameStateObjects.Add(
                    objectSaveData.id,
                    objectSaveData.typeValuePairs.ConvertAll(x => new ConduitValuePair(x))
                );
            }
            SaveSerializer.Invoke();
        }
    }

    public class AuthorAttribute : Attribute
    {
        public AuthorAttribute()
        {
        }
    }
}