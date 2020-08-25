using UnityEngine;

namespace LBFramework
{
    public interface IJsonSerializeUtility
    {
        //序列化
        string SerializeJson<T>(T obj) where T : class;
        //反序列化
        T DeserializeJson<T>(string json) where T : class;
    }
}