using System.Collections;
using LBFramework.LBUtils;
using LBFramework.Log;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class AssetBundleRes : Res
    {
        //标记是否被卸载
        private bool mUnloadFlag = true;
        //依赖资源列表
        private string[] mDependResList;
        //创建一个ab资源申请
        private AssetBundleCreateRequest mAssetBundleCreateRequest;
        
        //申请一个ab资源
        public static AssetBundleRes Allocate(string name)
        {
            //对象池申请一个资源
            var res = SafeObjectPool<AssetBundleRes>.Instance.Allocate();
            res.AssetName = name;
            res.AssetType = typeof(AssetBundle);
            //初始化AB名字
            res.InitAssetBundleName();
            //返回资源对象
            return res;
        }
        //初始化AB名字
        private void InitAssetBundleName()
        {
            mDependResList = AssetBundleSettings.AssetBundleConfigFile.GetAllDependenciesByUrl(AssetName);
        }
        
        //对外公开的获取资源的AB
        public AssetBundle AssetBundle
        {
            get { return (AssetBundle) mAsset; }
            private set { mAsset = value; }
        }

        //同步加载资源
        public override bool LoadSync()
        {
            //如果不是处于等待状态，表示加载失败
            if (!CheckLoadAble())
                return false;
            //改变资源处于加载状态
            State = ResState.Loading;
#if UNITY_EDITOR
            if (AssetBundleSettings.SimulateAssetBundleInEditor) { }
            else
#endif
            {
                //根据资源名字获取地址
                var url = AssetBundleSettings.AssetBundleName2Url(mAssetName);
                //从文件中加载对应的bundle
                var bundle = AssetBundle.LoadFromFile(url);
                //将资源标记为没有被卸载
                mUnloadFlag = true;
                //如果bundle资源为空直接返回加载失败
                if (bundle == null)
                {
                    LBLogWrapper.LogError("Failed Load AssetBundle:" + mAssetName);
                    OnResLoadFaild();
                    return false;
                }
                //将加载出来的资源记录到对象的变量中
                AssetBundle = bundle;
            }
            //将资源标记为准备状态，戴白哦加载完毕
            State = ResState.Ready;
            return true;
        }
        //异步加载资源
        public override void LoadAsync()
        {
            //如果资源不是处于等待状态直接返回
            if (!CheckLoadAble())
                return;
            //更改资源状态为加载中
            State = ResState.Loading;
            //添加异步加载的任务
            ResMgr.Instance.PushIEnumeratorTask(this);
        }

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            //如果引用数量小于0直接返回加载失败并响应完成事件
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }
#if UNITY_EDITOR
            if (AssetBundleSettings.SimulateAssetBundleInEditor)
            { yield return null; }
            else
#endif
            {
                //通过资源名字获取资源地址
                var url = AssetBundleSettings.AssetBundleName2Url(mAssetName);
                //异步加载对应的资源
                var abcR = AssetBundle.LoadFromFileAsync(url);
                //对象获取资源器赋值
                mAssetBundleCreateRequest = abcR;
                //返回资源创建和获取器
                yield return abcR;
                //结束后资源申请器清空
                mAssetBundleCreateRequest = null;
                
                //如果没有加载完成，响应加载失败事件并响应完成事件
                if (!abcR.isDone)
                {
                    LBLogWrapper.LogError("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
                    OnResLoadFaild();
                    finishCallback();
                    yield break;
                }
                //将加载出来的资源赋值在该对象中
                AssetBundle = abcR.assetBundle;
            }
            //将资源设置为准备状态代表加载完成
            State = ResState.Ready;
            //响应加载完成事件
            finishCallback();
        }
        //获取资源依赖的资源列表
        public override string[] GetDependResList()
        {
            return mDependResList;
        }
        
        //标记资源是否加载Image
        public override bool UnloadImage(bool flag)
        {
            if (AssetBundle != null)
                mUnloadFlag = flag;
            
            return true;
        }
        //释放资源，对象池释放res资源
        public override void Recycle2Cache()
        {
            SafeObjectPool<AssetBundleRes>.Instance.Recycle(this);
        }
        //资源回收响应的事件
        public override void OnRecycled()
        {
            base.OnRecycled();
            mUnloadFlag = true;
            mDependResList = null;
        }
        //计算资源加载的进度
        protected override float CalculateProgress()
        {
            if (mAssetBundleCreateRequest == null)
                return 0;
            return mAssetBundleCreateRequest.progress;
        }
        //资源释放响应的事件
        protected override void OnReleaseRes()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(mUnloadFlag);
                AssetBundle = null;
            }
        }
        //用于打印资源的信息  
        public override string ToString()
        {
            return string.Format("Type:AssetBundle\t {0}", base.ToString());
        }
    }
}