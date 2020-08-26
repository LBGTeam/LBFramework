using UnityEngine;
using System.IO;
using System.Xml.Serialization;

namespace LBFramework.ResKit
{
    public static class SerializeHelper
    {
        //序列化资源转化为Json
        public static string ToJson<T>(this T obj) where T : class
        {
            return DefaultJsonSerializeUtility.Instance.SerializeJson(obj);
        }
        //反序列化获取资源内容
        public static T FromJson<T>(this string json) where T : class
        {
            return DefaultJsonSerializeUtility.Instance.DeserializeJson<T>(json);
        }

        /// 将需要序列化的数据保存为json文件
        /// <param name="obj">需要序列化数据</param>
        /// <param name="path">文件地址</param>
        /// <typeparam name="T">序列化的类型</typeparam>
        public static string SaveJson<T>(this T obj, string path) where T : class
        {
            //保存数据转换的json
            var jsonContent = obj.ToJson();
            //将json数据写进文件
            File.WriteAllText(path, jsonContent);
            //返回json数据
            return jsonContent;
        }
        //从文件地址中加载json文件
        public static T LoadJson<T>(this string path) where T : class
        {
            //判断地址中是否正确包含项目资源根目录
            if (path.Contains(Application.streamingAssetsPath))
            {
                using (var streamReader = new StreamReader(path))
                {
                    return FromJson<T>(streamReader.ReadToEnd());
                }
            }
            return File.ReadAllText(path).FromJson<T>();
        }
        
        /// 序列化成二进制
        /// <param name="path">资源地址</param>
        /// <param name="obj">资源</param>
        public static bool SerializeBinary(string path, object obj)
        {
            //如果地址为空直接返回失败
            if (string.IsNullOrEmpty(path))
                return false;
            if (obj == null)
                return false;
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(fs, obj);
                return true;
            }
        }
        //反序列化二进制文件
        public static object DeserializeBinary(Stream stream)
        {
            if (stream == null)
                return null;
            using (stream)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                var data = bf.Deserialize(stream);

                // TODO:这里没风险嘛?
                return data;
            }
        }
        //二进制反序列化地址中的文件
        public static object DeserializeBinary(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
                return null;
            using (FileStream fs = fileInfo.OpenRead())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                object data = bf.Deserialize(fs);

                if (data != null)
                {
                    return data;
                }
            }
            return null;
        }
        //序列化为XML文件
        public static bool SerializeXML(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (obj == null)
                return false;

            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                var xmlserializer = new XmlSerializer(obj.GetType());
                xmlserializer.Serialize(fs, obj);
                return true;
            }
        }
        //反序列化XML为对象
        public static object DeserializeXML<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            FileInfo fileInfo = new FileInfo(path);

            using (FileStream fs = fileInfo.OpenRead())
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                object data = xmlserializer.Deserialize(fs);

                if (data != null)
                {
                    return data;
                }
            }
            return null;
        }
    }
}