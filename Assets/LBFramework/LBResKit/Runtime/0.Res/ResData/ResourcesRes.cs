using System.Collections;
using LBFramework.LBUtils;
using LBFramework.Log;
using UnityEngine;

namespace LBFramework.ResKit
{
    //本地资源类型，url或者文件地址
    public enum InternalResNamePrefixType
    {
        Url, // resources://
        Folder, // Resources/
    }
    public class ResourcesRes : Res
    {
        //资源申请器
        private ResourceRequest mResourceRequest;
        //资源存放路径
        private string mPath;
        
        //申请一个资源
        public static ResourcesRes Allocate(string name, InternalResNamePrefixType prefixType)
        {
            //对象池申请一个对象
            var res = SafeObjectPool<ResourcesRes>.Instance.Allocate();
            //将资源名字赋值进变量
            if (res != null)
                res.AssetName = name;
            //将文件地址放进变量
            if (prefixType == InternalResNamePrefixType.Url)
                res.mPath = name.Substring("resources://".Length);
            else
                res.mPath = name.Substring("Resources/".Length);
            //返回资源
            return res;
        }
        //同步加载资源
        public override bool LoadSync()
        {
            //判断是否可以加载资源
            if (!CheckLoadAble())
                return false;
            //如果需要加载的资源为空
            if (string.IsNullOrEmpty(mAssetName))
                return false;
            //改变加载状态为加载中
            State = ResState.Loading;
            //判断是否保存的有类型加载成对应的类型，如果没有加载为object
            if (AssetType != null)
                mAsset = Resources.Load(mPath,AssetType);
            else
                mAsset = Resources.Load(mPath);
            //如果资源为空直接响应加载失败
            if (mAsset == null)
            {
                LBLogWrapper.LogError("Failed to Load Asset From Resources:" + mPath);
                OnResLoadFaild();
                return false;
            }
            //改变资源为准备状态
            State = ResState.Ready;
            return true;
        }
        //异步加载资源
        public override void LoadAsync()
        {
            //判断是否可以加载资源
            if (!CheckLoadAble())
                return;
            //判断需要架子啊的名字为空
            if (string.IsNullOrEmpty(mAssetName))
                return;

            State = ResState.Loading;
            //将加载任务添加进去
            ResMgr.Instance.PushIEnumeratorTask(this);
        }
        //开始执行异步加载
        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            //如果资源计数为0
            if (RefCount <= 0)
            {
                //响应加载失败事件
                OnResLoadFaild();
                //响应完成事件
                finishCallback();
                yield break;
            }
            //创建一个临时的资源申请器
            ResourceRequest resourceRequest = null;
            //根据类型加载对应资源的类型实例
            if (AssetType != null)
                resourceRequest = Resources.LoadAsync(mPath, AssetType);
            else
                resourceRequest = Resources.LoadAsync(mPath);
            //成员变量保存资源申请器
            mResourceRequest = resourceRequest;
            //等待资源加载完成
            yield return resourceRequest;
            //加载完成置空资源管理器
            mResourceRequest = null;
            //如果架子啊资源没有完成则表示加载失败
            if (!resourceRequest.isDone)
            {
                LBLogWrapper.LogError("Failed to Load Resources:" + mAssetName);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }
            //成员变量保存加载器加载出来的资源
            mAsset = resourceRequest.asset;
            //更改资源状态为准备状态
            State = ResState.Ready;
            //响应资源加载完成事件
            finishCallback();
        }
        //资源进行对象池的回收
        public override void Recycle2Cache()
        {
            SafeObjectPool<ResourcesRes>.Instance.Recycle(this);
        }
        //计算资源加载的进度，通过保存的资源加载器获取加载进度
        protected override float CalculateProgress()
        {
            if (mResourceRequest == null)
                return 0;
            
            return mResourceRequest.progress;
        }
        //用于展示资源的信息
        public override string ToString()
        {
            return string.Format("Type:Resources {1}", AssetName, base.ToString());
        }
    }
}