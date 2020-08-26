using System.Collections;
using LBFramework.LBUtils;
using LBFramework.Log;
using UnityEngine;

namespace LBFramework.ResKit
{
    //网络Image资源工具
    public static class NetImageResUtil
    {
        //转换为网络图片的名字
        public static string ToNetImageResName(this string selfHttpUrl)
        {
            return string.Format("NetImage:{0}", selfHttpUrl);
        }
    }
    public class NetImageRes : Res
    {
        //资源存放的地址
        private string mUrl;
        //资源的hashcode
        private string mHashCode;
        //图片资源
        private object mRawAsset;
        //资源的申请器
        private WWW mWWW;
        
        //申请一个网络图片资源
        public static NetImageRes Allocate(string name)
        {
            //对象池申请一个对象
            NetImageRes res = SafeObjectPool<NetImageRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
                //设置资源地址
                res.SetUrl(name.Substring(9));
            }
            return res;
        }
        //设置加载的进度
        public void SetDownloadProgress(int totalSize, int download) { }
        //获取加载的地址
        public string LocalResPath
        {
            get { return string.Format("{0}{1}", FilePath.PersistentDataPath4Photo, mHashCode); }
        }
        //对外公开的获取资源
        public virtual object RawAsset
        {
            get { return mRawAsset; }
        }
        //判断是否需要下载
        public bool NeedDownload
        {
            get { return RefCount > 0; }
        }
        //获取资源的url
        public string Url
        {
            get { return mUrl; }
        }
        //查看文件的大小
        public int FileSize
        {
            get { return 1; }
        }
        //设置资源地址
        public void SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            mUrl = url;
            mHashCode = string.Format("Photo_{0}", mUrl.GetHashCode());
        }
        //标记是否不加载图片
        public override bool UnloadImage(bool flag)
        {
            return false;
        }
        //网络图片不存在同步加载
        public override bool LoadSync()
        {
            return false;
        }
        //异步加载图片
        public override void LoadAsync()
        {
            //判断是否可以加载
            if (!CheckLoadAble())
                return;
            if (string.IsNullOrEmpty(mAssetName))
                return;
            DoLoadWork();
        }
        //开始添加异步加载工作
        private void DoLoadWork()
        {
            //设置加载状态为加载中
            State = ResState.Loading;
            //
            OnDownLoadResult(true);

            //检测本地文件是否存在
            /*
            if (!File.Exists(LocalResPath))
            {
                ResDownloader.S.AddDownloadTask(this);
            }
            else
            {
                OnDownLoadResult(true);
            }
            */
        }
        //对象池回收该资源
        public override void Recycle2Cache()
        {
            SafeObjectPool<NetImageRes>.Instance.Recycle(this);
        }
        //删除旧的资源文件
        public void DeleteOldResFile()
        {
            //throw new NotImplementedException();
        }
        //回收对象
        public override void OnRecycled() { }
        //释放资源
        protected override void OnReleaseRes()
        {
            if (mAsset != null)
            {
                GameObject.Destroy(mAsset);
                mAsset = null;
            }
            mRawAsset = null;
        }
        //添加加载任务
        public void OnDownLoadResult(bool result)
        {
            if (!result)
            {
                OnResLoadFaild();
                return;
            }
            if (RefCount <= 0)
            {
                State = ResState.Waiting;
                return;
            }
            //添加任务进度
            ResMgr.Instance.PushIEnumeratorTask(this);
        }
        
        //开始执行异步加载资源
        //完全的WWW方式,Unity 帮助管理纹理缓存，并且效率貌似更高
        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }
            WWW www = new WWW(mUrl);
            mWWW = www;
            yield return www;
            mWWW = null;
            if (www.error != null)
            {
                LBLogWrapper.Error(string.Format("Res:{0}, WWW Errors:{1}", mUrl, www.error));
                OnResLoadFaild();
                finishCallback();
                yield break;
            }
            if (!www.isDone)
            {
                LBLogWrapper.Error("NetImageRes WWW Not Done! Url:" + mUrl);
                OnResLoadFaild();
                finishCallback();
                www.Dispose();
                www = null;
                yield break;
            }
            if (RefCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                www.Dispose();
                www = null;
                yield break;
            }
            //这里是同步的操作
            mAsset = www.texture;
            www.Dispose();
            www = null;
            State = ResState.Ready;
            finishCallback();
        }
        //计算资源加载进度
        protected override float CalculateProgress()
        {
            if (mWWW == null)
                return 0;
            return mWWW.progress;
        }
    }
}