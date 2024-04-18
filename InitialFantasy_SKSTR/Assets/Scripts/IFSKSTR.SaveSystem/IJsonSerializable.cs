using System;
using Leguar.TotalJSON;

namespace IFSKSTR.SaveSystem
{
    public interface IJsonSerializable
    {
        public void JsonSerialize();

        public void JsonDeserialize();
    }
}

