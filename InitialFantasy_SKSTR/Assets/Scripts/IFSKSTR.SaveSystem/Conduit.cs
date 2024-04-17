
using System;
using UnityEngine;


namespace IFSKSTR.SaveSystem
{
    public class Conduit<T>
    {
        private readonly Action<T> _set;
        private readonly Func<T> _get;



        public Conduit(Action<T> set, Func<T> get)
        {
            _set = set;
            _get = get;
        }



        public bool SetVariable(TypeValuePair typeValuePair)
        {
            if (typeof(T) != typeValuePair.type)
            {
                Debug.Log("ERROR: invalid type passed to Conduit.SetVariable, returned false");
                return false;
            }

            _set((T)Convert.ChangeType(typeValuePair.Value, typeValuePair.type));
            return true;
        }

        public TypeValuePair GetVariable()
        {
            T val = _get();
            return new TypeValuePair(val.GetType(), val);
        }
    }

}

