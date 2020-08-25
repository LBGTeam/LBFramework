using System;
using LBFramework.LBUtils;
using LBFramework.Log;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

namespace LBFramework.ResKit
{
    public class AssetRes : Res
    {
        //无参构造函数
        public AssetRes() { }
        //有参构造函数
        public AssetRes(string assetName) : base(assetName) { }

        //ab资源组
        protected string[] mAssetBundleArray;
        //ab资源请求
        protected AssetBundleRequest mAssetBundleRequest;
        //所拥有的ab名字
        protected string mOwnerBundleName;
        
        //设置或者获取ab名字
        public override string OwnerBundleName
        {
            get { return mOwnerBundleName; }
            set { mOwnerBundleName = value; }
        }
        /// 申请一个资源
        /// <param name="name">资源名字</param>
        /// <param name="onwerBundleName">ab名字</param>
        /// <param name="assetTypde">资源类型</param>
        public static AssetRes Allocate(string name, string onwerBundleName, Type assetTypde)
        {
            //从资源的对象池中申请一个
            var res = SafeObjectPool<AssetRes>.Instance.Allocate();
            //如果资源不为空开始为资源赋值
            if (res != null)
            {
                res.AssetName = name;
                res.mOwnerBundleName = onwerBundleName;
                res.AssetType = assetTypde;
                //资源初始化ab的名字
                res.InitAssetBundleName();
            }
            //返回该资源
            return res;
        }
        //获取资源的ab名字
        protected string AssetBundleName
        {
            get { return mAssetBundleArray == null ? null : mAssetBundleArray[0]; }
        }
        
        protected void InitAssetBundleName()
        {
            //置空ab资源组
            mAssetBundleArray = null;
            //申请一个用于查找的关键字资源
            var resSearchKeys = ResSearchKeys.Allocate(mAssetName,mOwnerBundleName,AssetType);
            //获取资源的配置数据
            var config = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(resSearchKeys);
            //回收刚才申请的关键字资源
            resSearchKeys.Recycle2Cache();
            //如果获取的为空直接返回
            if (config == null)
            {
                LBLogWrapper.LogError("Not Find AssetData For Asset:" + mAssetName);
                return;
            }
            //ab名字获取
            var assetBundleName = config.OwnerBundleName;
            //如果为空直接返回
            if (string.IsNullOrEmpty(assetBundleName))
            {
                LBLogWrapper.LogError("Not Find AssetBundle In Config:" + config.AssetBundleIndex + mOwnerBundleName);
                return;
            }
            //申请1个空间
            mAssetBundleArray = new string[1];
            //将ab名字当进去
            mAssetBundleArray[0] = assetBundleName;
        }

        //同步进行加载
        public override bool LoadSync()
        {
            //如果资源不是处于等待状态，直接返回失败
            if (!CheckLoadAble())
                return false;
            //如果ab名字为空也直接返回失败
            if (string.IsNullOrEmpty(AssetBundleName))
                return false;
            Object obj = null;
#if UNITY_EDITOR
            //如果在编辑器模式下使用ab资源
            if (AssetBundleSettings.SimulateAssetBundleInEditor 
                && !string.Equals(mAssetName, "assetbundlemanifest"))
            {
                //申请一个用于查找的关键字对象并进行不初始化
                var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName,null,typeof(AssetBundle));
                //通过申请的关键字对象获取对应的资源
                var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);
                //回收刚才申请的对象
                resSearchKeys.Recycle2Cache();
                //获取资源ab名字为abR.AssetName且资源名字为mAssetName的资源所在的路径
                var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
                //如果路径长度为0则获取失败
                if (assetPaths.Length == 0)
                {
                    LBLogWrapper.LogError("Failed Load Asset:" + mAssetName);
                    //响应资源加载失败的事件
                    OnResLoadFaild();
                    return false;
                }
				//保留所有的依赖资源
                HoldDependRes();
                //更改资源处于加载状态
                State = ResState.Loading;
                //如果存在类型就加载成对应的类型，如果不存在直接加载成object
                if (AssetType != null)
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPaths[0],AssetType);
                else
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPaths[0]);
            }
            else
#endif
            //不模拟使用ab资源的清空
            {
                //申请一个用于查找关键字资源类
                var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName, null, typeof(AssetBundle));
                //通过查找对象获取对应资源
                var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);
                //回收查找对象
                resSearchKeys.Recycle2Cache();
                //如果为空或者不存在ab直接返回失败
                if (abR == null || !abR.AssetBundle)
                {
                    LBLogWrapper.LogError("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
                    return false;
                }
                //保留所有的依赖资源
                HoldDependRes();
                //更改资源处于加载状态
                State = ResState.Loading;
                //如果存在类型就加载成对应的类型，如果不存在直接加载成object
                if (AssetType != null)
                    obj = abR.AssetBundle.LoadAsset(mAssetName,AssetType);
                else
                    obj = abR.AssetBundle.LoadAsset(mAssetName);
            }
            //取消所有的资源依赖
            UnHoldDependRes();
            //如果资源为空表示加载失败
            if (obj == null)
            {
                LBLogWrapper.LogError("Failed Load Asset:" + mAssetName + ":" + AssetType + ":" + AssetBundleName);
                //响应架子啊失败事件
                OnResLoadFaild();
                return false;
            }
            //保存加载出来的资源
            mAsset = obj;
            //更改加载状态处于准备状态
            State = ResState.Ready;
            return true;
        }
        //异步加载资源
        public override void LoadAsync()
        {
            //如果不是处于等待状态直接返回
            if (!CheckLoadAble())
                return;
            //如果不存在ab名字直接返回
            if (string.IsNullOrEmpty(AssetBundleName))
                return;
            //更改资源处于加载状态
            State = ResState.Loading;
            //添加异步加载的任务
            ResMgr.Instance.PushIEnumeratorTask(this);
        }
        //执行异步加载并传递完成事件
        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            //如果资源计数为0，表示加载失败
            if (RefCount <= 0)
            {
                //响应失败事件
                OnResLoadFaild();
                //响应返回
                finishCallback();
                yield break;
            }
            //申请一个查找资源关键字的对象
            var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName,null,typeof(AssetBundle));
            //通过申请的查找对象获取资源
            var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);
            //查找资源对象进行回收
            resSearchKeys.Recycle2Cache();
#if UNITY_EDITOR
            if (AssetBundleSettings.SimulateAssetBundleInEditor 
            && !string.Equals(mAssetName, "assetbundlemanifest"))
            {
                var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
                if (assetPaths.Length == 0)
                {
                    LBLogWrapper.LogError("Failed Load Asset:" + mAssetName);
                    OnResLoadFaild();
                    finishCallback();
                    yield break;
                }
                //确保加载过程中依赖资源不被释放:目前只有AssetRes需要处理该情况
                HoldDependRes();
                //更改资源处于加载状态
                State = ResState.Loading;
                // 模拟等一帧
                yield return new WaitForEndOfFrame();
                //取消所有的资源依赖
                UnHoldDependRes();
                //如果存在类型就加载成对应的类型，如果不存在直接加载成object
                if (AssetType != null)
                    mAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPaths[0],AssetType);
                else
                    mAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPaths[0]);

            }
            else
#endif
                //不模拟使用ab资源的清空
            {
                //如果资源为空或者ab名字为空直接返回失败
                if (abR == null || abR.AssetBundle == null)
                {
                    LBLogWrapper.LogError("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
                    //响应失败事件
                    OnResLoadFaild();
                    //响应完成事件
                    finishCallback();
                    yield break;
                }
                //保留所有的依赖资源
                HoldDependRes();
                //改变资源状态为加载状态
                State = ResState.Loading;
                AssetBundleRequest abQ = null;
                //如果存在类型就加载成对应的类型，如果不存在直接加载成object
                if (AssetType != null)
                {
                    abQ = abR.AssetBundle.LoadAssetAsync(mAssetName,AssetType);
                    mAssetBundleRequest = abQ;
                    yield return abQ;
                }
                else
                {
                    abQ = abR.AssetBundle.LoadAssetAsync(mAssetName);
                    mAssetBundleRequest = abQ;
                    yield return abQ;
                }
                mAssetBundleRequest = null;
                //取消所有的依赖
                UnHoldDependRes();
                //如果没有完成则返回失败并响应完成事件
                if (!abQ.isDone)
                {
                    LBLogWrapper.LogError("Failed Load Asset:" + mAssetName);
                    OnResLoadFaild();
                    finishCallback();
                    yield break;
                }
                mAsset = abQ.asset;
            }
            //改变加载状态为准备状态
            State = ResState.Ready;
            //响应完成的事件
            finishCallback();
        }
        //获取依赖资源列表
        public override string[] GetDependResList()
        {
            return mAssetBundleArray;
        }
        //释放依赖资源
        public override void OnRecycled()
        {
            mAssetBundleArray = null;
        }
        //对象池进行回收
        public override void Recycle2Cache()
        {
            SafeObjectPool<AssetRes>.Instance.Recycle(this);
        }
        //计算异步加载的进度
        protected override float CalculateProgress()
        {
            if (mAssetBundleRequest == null)
            {
                return 0;
            }
            return mAssetBundleRequest.progress;
        }
        
        //用于打印资源信息
        public override string ToString()
        {
            return string.Format("Type:Asset\t {0}\t FromAssetBundle:{1}", base.ToString(), AssetBundleName);
        }
    }
}