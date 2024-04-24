
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;


namespace IFSKSTR.SaveSystem
{
    public class Conduit
    {
        public delegate object Get();
        public delegate void Set(object value);
        private readonly Get _get;
        private readonly Set _set;
        
        public Conduit(in Get  get, in Set set)
        {
            _get = get;
            _set = set;
        }
        
        public void SetVariable(TypeValuePair typeValuePair)
        {
            _set(Convert.ChangeType(typeValuePair.Value, typeValuePair.Type));
        }

        public TypeValuePair GetVariable()
        {
            object val = _get();
            return new TypeValuePair(val.GetType(), val); //generates hash
        }
        
        public override string ToString()
        {
            object val = _get();
            return "(" + val.GetType()  + ": " + val + ")";
        }
    }

}

