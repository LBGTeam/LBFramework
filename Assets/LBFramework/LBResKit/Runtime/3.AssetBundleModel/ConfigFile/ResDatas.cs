using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LBFramework.Log;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class ResDatas : IResDatas
    {
        [Serializable]
        public class SerializeData    //序列化资源类
        {
            //定义一个序列化资源组中的资源对象
            private AssetDataGroup.SerializeData[] mAssetDataGroup;    
            //对外公开的设置和获取资源类
            public AssetDataGroup.SerializeData[] AssetDataGroup
            {
                get { return mAssetDataGroup; }
                set { mAssetDataGroup = value; }
            }
        }
        
        //列表保存所有资源数据组
        protected readonly List<AssetDataGroup> mAllAssetDataGroup 
            = new List<AssetDataGroup>();
        //资源数据表
        private AssetDataTable mAssetDataTable = null;
        
        //资源数据的文件名字
        public virtual string FileName
        {
            get { return "asset_bindle_config.bin"; }
        }
        //公开的获取所有的资源数据组
        public IList<AssetDataGroup> AllAssetDataGroups
        {
            get { return mAllAssetDataGroup; }
        }
        //重置所有的资源数据
        public void Reset()
        {
            //遍历所有的资源数据组，并进行重置
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
                mAllAssetDataGroup[i].Reset();
            //清空保存的资源数据组
            mAllAssetDataGroup.Clear();
            //如果存在资源表就释放
            if (mAssetDataTable != null)
                mAssetDataTable.Dispose();
            //置空资源数据表
            mAssetDataTable = null;
        }
        

        //添加AssetBundled的名字
        public int AddAssetBundleName(string name, string[] depends, out AssetDataGroup group)
        {
            //首先置空需要输出的资源组
            group = null;
            //如果名字为空直接返回-1表示失败
            if (string.IsNullOrEmpty(name))
                return -1;
            //获取资源名字标识，不一定是资源名字，存在il8资源
            var key = GetKeyFromABName(name);
            if (key == null)
                return -1;
            //获取资源所在的资源组
            group = GetAssetDataGroup(key);
            //如果没有存在
            if (group == null)
            {
                //创建一个新的资源组
                group = new AssetDataGroup(key);
                //将子元素添加到所有资源列表中
                mAllAssetDataGroup.Add(group);
            }
            //资源组添加AB资源名字和依赖
            return group.AddAssetBundleName(name, depends);
        }
        //通过url获取所有资源的依赖
        public string[] GetAllDependenciesByUrl(string url)
        {
            //通过url获取ab的名字
            var abName = AssetBundleSettings.AssetBundleUrl2Name(url);
            //遍历所有的资源数据组
            for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string[] depends;
                //遍历当前资源组获取所有的资源依赖的名字
                if (!mAllAssetDataGroup[i].GetAssetBundleDepends(abName, out depends))
                {
                    continue;
                }
                //返回依赖的名字集合
                return depends;
            }

            return null;
        }
        
        //通过关键字资源对象获取对应的资源
        public AssetData  GetAssetData(ResSearchKeys resSearchKeys)
        {
            if (mAssetDataTable == null)
            {
                //创建资源表
                mAssetDataTable = new AssetDataTable();
                
                //遍历所有的资源组
                for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
                {
                    //遍历当前资源组的资源
                    foreach (var assetData in mAllAssetDataGroup[i].AssetDatas)
                    {
                        //将资源添加到表里
                        mAssetDataTable.Add(assetData);
                    }
                }
            }
            //资源表通过资源关键字对象获取对应的资源
            return mAssetDataTable.GetAssetDataByResSearchKeys(resSearchKeys);
        }
        
        //获得资源所在的组
        private AssetDataGroup GetAssetDataGroup(string key)
        {
            //遍历资源组储存的列表
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                //判断是否是自己查找的资源，如果是的直接返回
                if (mAllAssetDataGroup[i].key.Equals(key))
                {
                    return mAllAssetDataGroup[i];
                }
            }
            //找不到就返回空
            return null;
        }
        
        //从文件中加载数据
        public virtual void LoadFromFile(string path)
        {
            //反序列化资源对象
            var data = SerializeHelper.DeserializeBinary(FileMgr.Instance.OpenReadStream(path));

            //如果为空直接返回
            if (data == null)
                return;
            //将资源转化为序列化资源
            var sd = data as SerializeData;

            if (sd == null)
                return;
            //设置序列化资源
            SetSerizlizeData(sd);
        }
        //异步从文件中获取资源
        public virtual IEnumerator LoadFromFileAsync(string path)
        {
            using (var www = new WWW(path))
            {
                yield return www;

                if (www.error != null)
                {
                    LBLogWrapper.LogError("Failed Deserialize AssetDataTable:" + path + " Error:" + www.error);
                    yield break;
                }
                var stream = new MemoryStream(www.bytes);
                var data = SerializeHelper.DeserializeBinary(stream);

                if (data == null)
                {
                    LBLogWrapper.LogError("Failed Deserialize AssetDataTable:" + path);
                    yield break;
                }
                var sd = data as SerializeData;
                if (sd == null)
                {
                    LBLogWrapper.LogError("Failed Load AssetDataTable:" + path);
                    yield break;
                }
                LBLogWrapper.Info("Load AssetConfig From File:" + path);
                SetSerizlizeData(sd);
            }
        }
        //将需要序列化的资源保存到地址中
        public virtual void Save(string outPath)
        {
            //创建一个新的序列化数据对象
            SerializeData sd = new SerializeData
            {
                AssetDataGroup = new AssetDataGroup.SerializeData[mAllAssetDataGroup.Count]
            };
            //遍历所有的资源组添加到序列化数据中
            for (var i = 0; i < mAllAssetDataGroup.Count; ++i)
            {
                sd.AssetDataGroup[i] = mAllAssetDataGroup[i].GetSerializeData();
            }
            //将序列化资源系列化到对应的地址中
            if (SerializeHelper.SerializeBinary(outPath, sd))
            {
                LBLogWrapper.LogInfo("Success Save AssetDataTable:" + outPath);
            }
            else
            {
                LBLogWrapper.LogInfo("Failed Save AssetDataTable:" + outPath);
            }
        }
        
        //设置序列化资源
        protected void SetSerizlizeData(SerializeData data)
        {
            //如果为空直接返回
            if (data == null || data.AssetDataGroup == null)
                return;
            //遍历序列化出来的资源依次加入资源组中
            for (int i = data.AssetDataGroup.Length - 1; i >= 0; --i)
            {
                mAllAssetDataGroup.Add(BuildAssetDataGroup(data.AssetDataGroup[i]));
            }
            //如果资源表为空
            if (mAssetDataTable == null)
            {
                //创建一个新的资源数据表
                mAssetDataTable = new AssetDataTable();
                //遍历所有的资源组
                foreach (var serializeData in data.AssetDataGroup)
                {
                    //遍历每个资源组中的所有资源
                    foreach (var assetData in serializeData.assetDataArray)
                    {
                        //将资源添加到资源表中
                        mAssetDataTable.Add(assetData);
                    }
                }
            }
        }
        //建立资源数据组
        private AssetDataGroup BuildAssetDataGroup(AssetDataGroup.SerializeData data)
        {
            //返回一个新生成的资源数据组
            return new AssetDataGroup(data);
        }
        
        //从AB资源中获取关键的key
        private static string GetKeyFromABName(string name)
        {
            //查找名字中带“/”的位置索引
            int pIndex = name.IndexOf('/');
            //如果小于0，证明没有，就是直接是资源名字，进行返回
            if (pIndex < 0)    
                return name;
            //获取资源字符串的前缀
            string key = name.Substring(0, pIndex);
            //判断字符串中是否包含il8res
            if (name.Contains("i18res"))
            {
                //获取il8资源的开始位置
                int i18Start = name.IndexOf("i18res") + 7;
                //获取资源的名字，因为刚才已经获取到名字开始位置
                name = name.Substring(i18Start);
                //从名字中获取“/”字符的位置
                pIndex = name.IndexOf('/');
                //如果没有说明格式不对，返回空
                if (pIndex < 0)
                {
                    //Log.W("Not Valid AB Path:" + name);
                    return null;
                }
                //从名字中截取到关键的语句
                string language = string.Format("[{0}]", name.Substring(0, pIndex));
                //连接关键字和刚才获取名字中的语句
                key = string.Format("{0}-i18res-{1}", key, language);
            }
            //输出新的名字
            return key;
        }
    }
}