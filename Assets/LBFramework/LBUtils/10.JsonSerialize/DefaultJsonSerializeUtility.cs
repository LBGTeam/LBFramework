using LBFramework.LBUtils;
using UnityEngine;

namespace LBFramework
{
    public class DefaultJsonSerializeUtility : Singleton<DefaultJsonSerializeUtility>
    {
        private DefaultJsonSerializeUtility(){}
        
        //序列化
        public string SerializeJson<T>(T obj) where T : class
        {
            return JsonUtility.ToJson(obj, true);
        }
        //反序列化
        public T DeserializeJson<T>(string json) where T : class
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}