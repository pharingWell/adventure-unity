using System;
using UnityEngine;
using System.Collections.Generic;

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


[Serializable]
public struct TypeValuePair : ISerializationCallbackReceiver
{
    public Type type;
    public string serializableRepresentation;
    private object _value;
    public object Value { get => _value; set => _value = Value; }

    public TypeValuePair(Type t, object v)
    {
        type = t;
        _value = v;
        serializableRepresentation = null;
    }

    public override string ToString()
    {
        return "(t: " + type + " o: " + _value + ")";
    }
    
    public void OnBeforeSerialize()
    {
        serializableRepresentation = JsonUtility.ToJson(Convert.ChangeType(_value, type));
        Debug.Log("Serialized as"+ serializableRepresentation);
    }

    public void OnAfterDeserialize()
    {
        _value = JsonUtility.FromJson(serializableRepresentation, type);
    }
}

[Serializable]
public struct ObjectSaveData
{
    public int id;
    public int hash;
    public List<TypeValuePair> typeValuePairs;
    
    public ObjectSaveData(int id, List<TypeValuePair> typeValuePairs)
    {
        this.id = id;
        this.typeValuePairs = typeValuePairs;
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

    public void OnBeforeSerialize()
    {
        throw new NotImplementedException();
    }

    public void OnAfterDeserialize()
    {
        throw new NotImplementedException();
    }
}


}