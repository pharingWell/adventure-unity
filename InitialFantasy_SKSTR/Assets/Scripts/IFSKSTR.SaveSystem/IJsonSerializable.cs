using System;
using Leguar.TotalJSON;

namespace IFSKSTR.SaveSystem
{
    public interface IJsonSerializable<T>
    {
        public JSON JsonSerialize();

        public T JsonDeserialize(JSON json);
    }
}

