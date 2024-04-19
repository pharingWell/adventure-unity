
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;


namespace IFSKSTR.SaveSystem
{
    public class Conduit<T> where T: IComparable 
    {
        
        private readonly Action<IComparable> _set;
        private readonly Func<IComparable> _get;
        
        public Conduit(Action<IComparable> set, Func<IComparable> get)
        {
            _set = set;
            _get = get;
        }
        
        public void SetVariable(TypeValuePair typeValuePair)
        {
            _set((IComparable)Convert.ChangeType(typeValuePair.Value, typeValuePair.Type));
        }

        public TypeValuePair GetVariable()
        {
            IComparable val = _get();
            return new TypeValuePair(val.GetType(), val); //generates hash
        }

        public override string ToString()
        {
            return "(" + typeof(T) + ": " + _get() + ")";
        }
    }

}

