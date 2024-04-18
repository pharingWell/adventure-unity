using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Leguar.TotalJSON;
using Unity.VisualScripting;
using UnityEngine.Serialization;

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
    References to objects that derive from UnityEngine.Object
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

    [Serializable]
    public struct TypeValuePair : IJsonSerializable
    {
        [SerializeField] private int type;
        [NonSerialized] public Type Type;
        [SerializeField] private string value;
        [NonSerialized] public object Value;
        private const string TypeKey = "type";
        private const string ValueKey = "value";

        public TypeValuePair(Type t, object v)
        {
            Type = t;
            Value = v;
            value = null;
            type = -1;
            SerializeValue();
        }
        public override string ToString()
        {
            return "(type: " + Type + "(" + type + ") value: " + Value + "(" + value + "))";
        }
        

        public void SerializeValue()
        {
            TypeCode typeCode = Type.GetTypeCode(Type);
            JValue jValue;
            if (new[] {
                    TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32,
                    TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                }.Contains(typeCode)) //is a number
            { 
                string s = Value.ToString();
                jValue = new JNumber(s);
            }
            else if (new[] {TypeCode.Char, TypeCode.String, TypeCode.DateTime}.Contains(typeCode)){
                jValue = new JString((string)Value);
            }else if (typeCode is TypeCode.Empty or TypeCode.DBNull)
            {
                jValue = new JNull();
            }else if (typeCode is TypeCode.Boolean)
            {
                jValue = new JBoolean((bool)Value);
            }
            else //is object
            {
                jValue = JSON.Serialize(Value);
            }

            value = jValue.CreateString();
        }

        private static Type GetType(int code)
        {
            TypeCode tc = (TypeCode)code;
            return Type.GetType("System." + Enum.GetName(typeof(TypeCode), tc));
        }

        private static int GetTypeAsInt(Type type)
        {
            return (int)Type.GetTypeCode(type);
        }
        
        public void JsonSerialize()
        {
            SerializeValue();
            type = GetTypeAsInt(Type);
        }
        public void JsonDeserialize()
        {
            Type = GetType(type);
            var val = JSON.ParseString(value).GetString(ValueKey);
            Value = Convert.ChangeType(val, Type);
        }
    }

    [Serializable]
    public struct ObjectSaveData : IJsonSerializable //: ISerializationCallbackReceiver
    {
        public int id;
        public int hash;
        public List<TypeValuePair> typeValuePairs;

        public ObjectSaveData(int instanceID, List<TypeValuePair> valuePairs)
        {
            id = instanceID;
            typeValuePairs = valuePairs;
            hash = typeValuePairs.GetHashCode();
        }

        public override string ToString()
        {
            string result = "";
            foreach (var typeValue in typeValuePairs)
            {
                result += "(" + typeValue + "), ";
            }

            return "(ID: " + id + ", Hash: " + hash + ", Values: " + result + ")";
        }

        public void JsonSerialize()
        {
            for (int i = 0; i < typeValuePairs.Count; i++)
            {
                typeValuePairs[i].JsonSerialize();
            }

        }

        public void JsonDeserialize()
        {
            for (int i = 0; i < typeValuePairs.Count; i++)
            {
                typeValuePairs[i].JsonDeserialize();
            }
        }
        /*public void OnBeforeSerialize()
        {
            int j = TypeValuePairs.Count;
            valuesJson = new string[j];
            for (int i = 0; i < TypeValuePairs.Count; i++)
            {
                valuesJson[i] = JsonUtility.ToJson(TypeValuePairs[i]);
            }
        }

        public void OnAfterDeserialize()
        {
            TypeValuePairs = valuesJson.ToList().ConvertAll(JsonUtility.FromJson<TypeValuePair>);
        }
     }*/
    }
    
    [Serializable]
    public struct ListWrapper<T>
    {
        public List<T> value;
        public ListWrapper(List<T> value)
        {
            this.value = value;
        }
    }
    
}