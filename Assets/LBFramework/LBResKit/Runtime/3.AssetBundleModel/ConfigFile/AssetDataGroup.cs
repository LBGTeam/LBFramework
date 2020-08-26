using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBFramework.ResKit
{
    //资源数据组
    public class AssetDataGroup
    {
        //依赖关系类
        [SerializeField]
        public class ABUnit
        {
            public string abName;    //ab资源的名字
            public string[] abDepends;    //依赖的资源
            
            /// 构造函数
            /// <param name="name">资源的名字</param>
            /// <param name="depends">资源的依赖</param>
            public ABUnit(string name, string[] depends)
            {
                abName = name;
                if (depends == null || depends.Length == 0) { }
                else
                { abDepends = depends; }
            }
            //展示依赖关系资源数据
            public override string ToString()
            {
                var result = string.Format("ABName:" + abName);
                if (abDepends == null)
                    return result;
                foreach (var abDepend in abDepends)
                    result += string.Format(" #:{0}", abDepend);
                return result;
            }
        }

        //序列化数据
        [Serializable]
        public class SerializeData
        {
            private string mKey;        //序列化数据关键字
            private ABUnit[] mABUnitArray;    //AB依赖资源组
            //资源名字对应资源的字典
            private AssetData[] mAssetDataArray;

            //公开的获取和创建序列化资源的关键字
            public string key
            {
                get { return mKey; }
                set { mKey = value; }
            }
            
            //公开的获取和设置序列化资源AB依赖资源的数组
            public ABUnit[] abUnitArray
            {
                get { return mABUnitArray; }
                set { mABUnitArray = value; }
            }
            //公开的获取和设置序列化资源中的资源组
            public AssetData[] assetDataArray
            {
                get { return mAssetDataArray; }
                set { mAssetDataArray = value; }
            }
            
        }
        
        private string mKey;        //数据组关键字
        private List<ABUnit> mABUnitArray;    //AB依赖资源组
        //资源名字对应资源的字典
        private Dictionary<string, AssetData> mAssetDataDic;
        //资源数据标识对应资源的字典
        private Dictionary<string, AssetData> mUUIDAssetDataDic;
        
        //公开的设置和获取资源组的关键字
        public string key { get { return mKey; } }
        
        //构造函数，创建的时候传入资源组的关键字
        public AssetDataGroup(string key) { mKey = key; }
        //构造函数传入序列化资源
        public AssetDataGroup(SerializeData data)
        {
            mKey = data.key;
            SetSerializeData(data);
        }
        
        //资源组进行重置
        public void Reset()
        {
            if (mABUnitArray != null)
                mABUnitArray.Clear();
            if (mAssetDataDic != null)
                mAssetDataDic.Clear();
        }
        
        /// 获取资源的AB名字
        /// <param name="assetName"></param>
        /// <param name="index"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool GetAssetBundleName(string assetName, int index, out string result)
        {
            //首先清空输出的变量
            result = null;
            //如果AB资源组为空直接返回获取失败
            if (mABUnitArray == null)
                return false;
            //如果索引大于AB资源总数，则直接返回获取失败
            if (index >= mABUnitArray.Count)
                return false;
            //查找资源字典中是否有资源
            if (mAssetDataDic.ContainsKey(assetName))
            {
                //有资源的时候直接把资源的ab名字赋值到输出的变量
                result = mABUnitArray[index].abName;
                return true;
            }
            return false;
        }
        
        //获取AB依赖关系
        public ABUnit GetABUnit(string assetName)
        {
            //关键字资源器对象池申请一个资源
            var resSearchRule = ResSearchKeys.Allocate(assetName);
            //通过资源器获取对应的资源
            AssetData data = GetAssetData(resSearchRule);
            //回收关键字资源器
            resSearchRule.Recycle2Cache();

            //如果资源为空返回空
            if (data == null)
                return null;
            //如果AB依赖组为空也直接返回空
            if (mABUnitArray == null)
                return null;
            //通过资源的AB索引获取资源依赖组中的依赖
            return mABUnitArray[data.AssetBundleIndex];
        }
        //获取AB的依赖
        public bool GetAssetBundleDepends(string abName, out string[] result)
        {
            result = null;
            //通过AB名字获取AB的依赖
            ABUnit unit = GetABUnit(abName);
            //如果为空直接返回获取失败
            if (unit == null)
                return false;
            //获取依赖中的依赖内容
            result = unit.abDepends;
            return true;
        }
        /// <summary>
        /// 添加AB资源
        /// </summary>
        /// <param name="name">资源名字</param>
        /// <param name="depends">资源的依赖</param>
        public int AddAssetBundleName(string name, string[] depends)
        {
            //判断名字是否为空，为空直接返回
            if (string.IsNullOrEmpty(name))
                return -1;
            //判断保存依赖的组是不是空，如果为空创建一个进行初始化
            if (mABUnitArray == null)
                mABUnitArray = new List<ABUnit>();
            //对象池创建一个查找关键字资源的对象，将名字传进去
            var resSearchRule = ResSearchKeys.Allocate(name);
            //通过关键字对象进行查找资源
            AssetData config = GetAssetData(resSearchRule);
            //回收刚才申请的对象
            resSearchRule.Recycle2Cache();
            //如果资源不是空说明添加过了，直接返回资源的索引
            if (config != null)
                return config.AssetBundleIndex;
            //将资源添加进依赖资源组中
            mABUnitArray.Add(new ABUnit(name, depends));
            //设置好资源的索引
            int index = mABUnitArray.Count - 1;
            //将资源添加进去
            AddAssetData(new AssetData(name, ResLoadType.AssetBundle, index,null));
            //返回索引就是添加的位置
            return index;
        }
        //获取序列化资源
        public SerializeData GetSerializeData()
        {
            //创建一个新的序列化资源
            var sd = new SerializeData();
            //将关键字传入资源
            sd.key = mKey;
            //把AB依赖资源组传入
            sd.abUnitArray = mABUnitArray.ToArray();
            if (mAssetDataDic != null)
            {
                //创建一个新的资源组
                AssetData[] acArray = new AssetData[mAssetDataDic.Count];

                //记录资源遍历索引顺序
                int index = 0;
                //遍历资源字典
                foreach (var item in mAssetDataDic)
                {
                    //将字典中的值储存到资源数据组中
                    acArray[index++] = item.Value;
                }
                //将临时组赋值到新的序列化组中
                sd.assetDataArray = acArray;
            }
            //返回创建的序列资源组
            return sd;
        }

        //设置序列化资源
        private void SetSerializeData(SerializeData data)
        {
            //如果数据为空直接返回
            if (data == null) return;
            //初始化依赖资源组
            mABUnitArray = new List<ABUnit>(data.abUnitArray);
            if (data.assetDataArray != null)
            {
                //初始化资源字典
                mAssetDataDic= new Dictionary<string, AssetData>();
                //遍历序列化资源中的资源组
                foreach (var config in data.assetDataArray)
                {
                    //依次吧资源加晋资源数据集合中
                    AddAssetData(config);
                }
            }
        }
        
        //添加资源到资源数据储存字典
        public bool AddAssetData(AssetData data)
        {
            //如果还未初始化资源字典就初始化一下
            if (mAssetDataDic == null)
                mAssetDataDic = new Dictionary<string, AssetData>();
            //如果资源标识对应资源的字典没有初始化，则同样初始化一下
            if (mUUIDAssetDataDic == null)
                mUUIDAssetDataDic = new Dictionary<string, AssetData>();

            //获取资源的名字作为关键字(全部变为小写)
            string key = data.AssetName.ToLower();

            //查找资源字典里是否有资源
            if (mAssetDataDic.ContainsKey(key))
            {
                //有的话直接获取资源，先创建关键字资源器
                var resSearchRule = ResSearchKeys.Allocate(data.AssetName);
                //通过关键字资源器获取资源
                var old = GetAssetData(resSearchRule);
                //对象池会有关键字资源器
                resSearchRule.Recycle2Cache();
            }
            else
            {
                //如果没有直接将资源添加进资源字典中，资源名字(全部小写)作为key
                mAssetDataDic.Add(key, data);
            }
            //查找资源标识里面是否有资源
            if (mUUIDAssetDataDic.ContainsKey(data.UUID))
            {
                //有的话直接获取资源，先创建关键字资源器
                var resSearchRule = ResSearchKeys.Allocate(data.AssetName,data.OwnerBundleName);
                //通过关键字资源器获取资源
                AssetData old = GetAssetData(resSearchRule);
                //对象池会有关键字资源器
                resSearchRule.Recycle2Cache();

            }
            else
            {
                //如果没有直接将资源添加进资源标识字典中，资源名字(全部小写)作为key
                mUUIDAssetDataDic.Add(data.UUID,data);
            }
            return true;
        }
        
        //通过关键字资源器获取资源
        public AssetData GetAssetData(ResSearchKeys resSearchRule)
        {
            //临时变量储存资源
            AssetData result = null;

            //如果ab不为空并且资源标识字典不为空，字典中获取资源
            if (resSearchRule.OwnerBundle != null && mUUIDAssetDataDic != null)
                return mUUIDAssetDataDic.
                    TryGetValue(resSearchRule.OwnerBundle + resSearchRule.AssetName, out result)
                    ? result : null;
            //如果ab为空，则从普通资源字典中获取资源
            if (resSearchRule.OwnerBundle == null && mAssetDataDic != null)
                return mAssetDataDic.TryGetValue(resSearchRule.AssetName, out result) 
                    ? result : null;
            return result;
        }
        
        //对外公开的获取资源数据
        public IEnumerable<AssetData> AssetDatas
        {
            get { return mAssetDataDic.Values; }
        }

        //对外公开的获取AB资源数据
        public IEnumerable<ABUnit> AssetBundleDatas
        {
            get
            {
                return mABUnitArray;
            }
        }
    }
}