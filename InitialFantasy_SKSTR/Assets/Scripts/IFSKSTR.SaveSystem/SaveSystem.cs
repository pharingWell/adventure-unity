using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug; //https://github.com/GabrielBigardi/Generic-Save-System/blob/main/DOCUMENTATION.md
using IFSKSTR.SaveSystem.GDB.SaveSerializer;
using Leguar.TotalJSON;

namespace IFSKSTR.SaveSystem
{
    public class SaveSystem
    {
        private const string FileName = "SaveGame";
        private Dictionary<int, List<ConduitValuePair>> _gameStateObjects;
        private static SaveSystem _self;

        private static void _setup()
        {
            if (_self is null)
            {
                _self = new SaveSystem
                {
                    _gameStateObjects = new Dictionary<int, List<ConduitValuePair>>()
                };
            }
        }

        public static void Register(int id, List<TypeConduitPair> typeConduitPairs)
        {
            _setup();
            Debug.Log("Registered " + id);
            _self._gameStateObjects.Add(id, typeConduitPairs.ConvertAll(x => new ConduitValuePair(x)));
            if (!_self._gameStateObjects.ContainsKey(id))
            {
                _self._gameStateObjects.Add(id, typeConduitPairs.ConvertAll(x => new ConduitValuePair(x)));
            }
            else
            {
                if (_self._gameStateObjects[id].Count !=
                    typeConduitPairs.Count) //checks for variable updates/reregister updates
                {
                    _self._gameStateObjects.Remove(id);
                    _self._gameStateObjects.Add(id, typeConduitPairs.ConvertAll(x => new ConduitValuePair(x)));
                    return;
                }

                for (int i = 0; i < typeConduitPairs.Count; i++)
                {
                    TypeConduitPair typeConduitPair = typeConduitPairs[i];
                    ConduitValuePair conduitValuePair = _self._gameStateObjects[id][i];
                    if (conduitValuePair.IsValueValid)
                    {
                        bool successful = typeConduitPair.Conduit.SetVariable(conduitValuePair.ValuePair);
                        if (!successful) conduitValuePair.AddValuePair(
                            new TypeValuePair(typeConduitPair.Type, typeConduitPair.Conduit.GetVariable()
                        ));
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
                        x =>  x.TypeConduitPair.Conduit.GetVariable() //hash assigned here
                    )
                );
            });
            for (int i = 0; i < saveData.Count; i++)
            {
                saveData[i].JsonSerialize(); //we can't use foreach because we need the function called on this value
            }
            ListWrapper<ObjectSaveData> wrapper = new ListWrapper<ObjectSaveData>(saveData);
            bool saveSuccess = SaveSerializer.SaveGame(FileName, wrapper, GameSecrets.SaveKey);
            if (!saveSuccess)
            {
                Debug.Log("Error while saving");
                return;
            }
        }

        public static void Load()
        {
            bool loadSuccess = SaveSerializer.LoadGame(FileName, out ListWrapper<ObjectSaveData> wrapper, GameSecrets.SaveKey);
            if (!loadSuccess)
            {
                Debug.Log("Error while loading");
                return;
            }
            

            _setup();
            foreach (ObjectSaveData objectSaveData in wrapper.value)
            {
                objectSaveData.JsonDeserialize();
                if (objectSaveData.hash == objectSaveData.typeValuePairs.GetHashCode()) //is loaded data valid
                {
                    if (!_self._gameStateObjects.TryAdd(objectSaveData.id,
                            objectSaveData.typeValuePairs.ToList().ConvertAll(x => new ConduitValuePair(x))
                        )
                       )
                    {
                        /* if the object already exists, replace its value pair instead
                           this is unsafe, because it might try to assign a value to the wrong conduit
                           but we are helping to prevent corrupted data by checking the hash
                           if the order changes (which is what would need to happen for the above),
                           the hash should too, making the data invalid and preventing corruption */

                        var conduits = _self._gameStateObjects[objectSaveData.id];
                        var data = objectSaveData.typeValuePairs;
                        for (int index = 0; index < data.Count; index++)
                        {
                            conduits[index].TypeConduitPair.Conduit.SetVariable(data[index]);
                            conduits[index].AddValuePair(data[index]);
                        }

                        _self._gameStateObjects.Add(objectSaveData.id, conduits);
                    }
                }
                else
                {
                    Debug.Log("Warning while loading: Failed to load with ID " + objectSaveData.id);
                }
            }
        }
    }
}