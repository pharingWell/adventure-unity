using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Leguar.TotalJSON;
using Unity.VisualScripting;

namespace IFSKSTR.SaveSystem
{
    /*
    What does a field of my script need to be in order to be serialized?

    Be public, or have [SerializeField] attribute
    Not be static
    Not be const
    Not be readonly
    The fieldtype needs to be of a type that we can serialize.
    Which fieldtypes can we serialize?

    Custom non abstract classes with [Serializable] attribute.
    Custom structs with [Serializable] attribute. (new in Unity4.5)
    References to objects that derive from UntiyEngine.Object
    Primitive data types (int,float,double,bool,string,etc)
    Array of a fieldtype we can serialize
    List<T> of a fieldtype we can serialize
     */
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

        public override string ToString()
        {
            return TypeConduitPair + ", " + TypeConduitPair;
        }
    }

    public struct TypeValuePair : IJsonSerializable<TypeValuePair>
    {
        [SerializeField] public Type Type;
        [SerializeField] public object Value;
        private const string TypeKey = "type";
        private const string ValueKey = "value";

        public TypeValuePair(Type t, object v)
        {
            Type = t;
            Value = v;
        }

        public override string ToString()
        {
            return "(t: " + Type + " o: " + Value + ")";
        }
        

        public JSON JsonSerialize()
        {
            int typeCode = (int)Type.GetTypeCode(Type);
            JValue jValue;
            if (new[] { 5, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.Contains(typeCode)){ //is a number
                string s = Value.ToString();
                jValue = new JNumber(s);
            }
            else if (new[] {4, 16, 18 }.Contains(typeCode)){
                jValue = new JString((string)Value);
            }else if (typeCode is 0 or 2)
            {
                jValue = new JNull();
            }else if (typeCode is 3)
            {
                jValue = new JBoolean((bool)Value);
            }
            else
            {
                jValue = JSON.Serialize(Value);
            }
            return new JSON(new Dictionary<string, JValue>()
            {
                {TypeKey, new JNumber((int)Type.GetTypeCode(Type))},
                {ValueKey, jValue}
            });
        }

        private static Type GetType(TypeCode code)
        {         
            return Type.GetType("System." + Enum.GetName(typeof(TypeCode), code));
        }

        public TypeValuePair JsonDeserialize(JSON json)
        {
            Type type = GetType((TypeCode)json.GetInt(TypeKey));
            object value = json.GetJSON(ValueKey).Deserialize<object>();
            return new TypeValuePair(type, value);
        }
    }

    [Serializable]
    public struct ObjectSaveData: IJsonSerializable<ObjectSaveData>
    {
        [SerializeField] public int id;
        [SerializeField] public int hash;
        [SerializeField] public List<TypeValuePair> TypeValuePairs;
        private const string IDKey = "id"; 
        private const string HashKey = "hash"; 
        private const string ValuesKey = "values";     
        public ObjectSaveData(int id, List<TypeValuePair> typeValuePairs)
        {
            this.id = id;
            TypeValuePairs = typeValuePairs;
            hash = typeValuePairs.GetHashCode();
        }

        public override string ToString()
        {
            string result = "";
            foreach (var typeValue in TypeValuePairs)
            {
                result += "(" + typeValue + "), ";
            }
            return "(ID: " + id + ", Hash: " + hash + ", Values: " + result + ")";
        }

        public JSON JsonSerialize()
        {
            return new JSON(new Dictionary<string, JValue>{
                    {IDKey, new JNumber(id)},
                    {HashKey, new JNumber(hash)},
                    {ValuesKey, new JArray(TypeValuePairs.ConvertAll(x => x.JsonSerialize()).ToArray()) }
                }
            );
        }

        public ObjectSaveData JsonDeserialize(JSON json)
        {
            int localID = json.GetInt(IDKey);
            int localHash = json.GetInt(HashKey);
            List<JSON> jsonList = json.GetJArray(ValuesKey).AsJSONArray().ToList();
            List<TypeValuePair> valueList = jsonList.ConvertAll<TypeValuePair>(x => new TypeValuePair().JsonDeserialize(x));
            if (localHash != valueList.GetHashCode())
            {
                Debug.Log(localHash  + " != " + valueList.GetHashCode());
                return new ObjectSaveData();
            }

            return new ObjectSaveData(localID, valueList);
        }
    }

}