using System;
using System.Collections;
using LBFramework.LBUtils;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class Res : SimpleRC,IRes,IPoolable
    {
        
        //资源的名字
        protected string mAssetName;
        //资源的状态
        private ResState mResState = ResState.Waiting;
        //资源
        protected UnityEngine.Object mAsset;
        //资源加载完毕响应的事件
        private event Action<bool, IRes> mOnResLoadDoneEvent;
        //资源所有者的bundle的名字
        public virtual string OwnerBundleName { get; set; }
        //资源的类型
        public Type AssetType { get; set; }
        //资源是否已经回收
        public bool IsRecycled { get; set; }
        
        //有参构造函数
        protected Res(string assetName)
        { 
            IsRecycled = false;        //标记资源未被回收
            mAssetName = assetName;    //记录资源名字
        }
        //无参构造函数
        public Res()
        {
            IsRecycled = false;        //标记资源未被回收
        }
        
        //对外公开的获取资源名字的方法
        public string AssetName
        {
            get { return mAssetName; }
            protected set { mAssetName = value; }    //只允许自己或者继承者修改资源名字
        }

        //对外公开的获取资源的接口
        public UnityEngine.Object Asset
        {
            get { return mAsset; }
        }
        
        //对外公开的获取和设置资源加载的状态
        public ResState State
        {
            get { return mResState; }
            set
            {
                mResState = value;
                if (mResState == ResState.Ready)
                {
                    //当设置资源已经准备的状态时触发资源加载完毕事件
                    NotifyResLoadDoneEvent(true);
                }
            }
        }
        
        //资源加载的进度（弃用）
        public float Progress
        {
            get
            {
                switch (mResState)
                {
                    case ResState.Loading:
                        //如果正在加载则计算加载进度(已弃用，返回0)
                        return CalculateProgress();
                    case ResState.Ready:
                        return 1;
                }
                return 0;
            }
        }
        
        //注册资源加载的事件
        public void RegisteOnResLoadDoneEvent(Action<bool, IRes> listener)
        {
            if (listener == null)
                return;
            if (mResState == ResState.Ready)
            {
                //如果资源已经加载，则直接响应需要注册的事件
                listener(true, this);
                return;
            }
            mOnResLoadDoneEvent += listener;
        }
        
        //移除资源加载完成事件
        public void UnRegisteOnResLoadDoneEvent(Action<bool, IRes> listener)
        {
            if (listener == null)
                return;
            if (mOnResLoadDoneEvent == null)
                return;
            mOnResLoadDoneEvent -= listener;
        }
        
        //资源加载失败的时候执行的事件
        protected void OnResLoadFaild()
        {
            mResState = ResState.Waiting;    //恢复资源等待加载状态
            NotifyResLoadDoneEvent(false);        //执行资源加载完毕事件并传递失败参数
        }
        
        //检测资源是否可以加载
        protected bool CheckLoadAble()
        {
            //返回资源是否处于等待加载状态
            return mResState == ResState.Waiting;
        }
        
        //保留依赖的资源
        protected void HoldDependRes()
        {
            var depends = GetDependResList();    //获取依赖的资源
            if (depends == null || depends.Length == 0)
                return;
            for (var i = depends.Length - 1; i >= 0; --i)
            {
                //得到资源
                var resSearchRule = ResSearchKeys.Allocate(depends[i],null,typeof(AssetBundle));
                //生成资源
                var res = ResMgr.Instance.GetRes(resSearchRule, false);
                resSearchRule.Recycle2Cache();
                if (res != null)
                    res.Retain();        //资源增加计数
            }
        }
        
        //释放依赖的资源
        protected void UnHoldDependRes()
        {
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0)
                return;
            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchKeys.Allocate(depends[i]);
                var res = ResMgr.Instance.GetRes(resSearchRule, false);
                resSearchRule.Recycle2Cache();
                if (res != null)
                    res.Release();
            }
        }
        
        //触发资源加载完毕事件
        private void NotifyResLoadDoneEvent(bool result)
        {
            //判断资源是否为空来确定是否需要触发
            if (mOnResLoadDoneEvent != null)
            {
                //触发资源加载事件
                mOnResLoadDoneEvent(result, this);
                //触发完成后事件清空，防止事件多次触发
                mOnResLoadDoneEvent = null;
            }
        }
        
        //计算资源加载进度
        protected virtual float CalculateProgress()
        {
            return 0;
        }
        
        //判断依赖的资源是否加载完毕
        public bool IsDependResLoadFinish()
        {
            var depends = GetDependResList();
            if (depends == null || depends.Length == 0)
                return true;
            for (var i = depends.Length - 1; i >= 0; --i)
            {
                var resSearchRule = ResSearchKeys.Allocate(depends[i]);
                var res = ResMgr.Instance.GetRes(resSearchRule, false);
                resSearchRule.Recycle2Cache();
                if (res == null || res.State != ResState.Ready)
                    return false;
            }
            return true;
        }
        
        //释放资源
        public bool ReleaseRes()
        {
            if (mResState == ResState.Loading)
                return false;
            if (mResState != ResState.Ready)
                return true;
            OnReleaseRes();    //释放资源
            mResState = ResState.Waiting;        //恢复资源未加载等待状态
            mOnResLoadDoneEvent = null;            //清空加载完毕事件
            return true;
        }
        //释放资源
        protected virtual void OnReleaseRes()
        {
            //如果Image 直接释放了，这里会直接变成NULL
            if (mAsset != null)
            {
                if (mAsset is GameObject)
                {
                }
                else
                {
                    Resources.UnloadAsset(mAsset);
                }
                mAsset = null;
            }
        }

        #region 子类实现

        //同步加载资源
        public virtual bool LoadSync() { return false; }
        //异步加载资源
        public void LoadAsync() { }
        //获取依赖资源集合
        public virtual string[] GetDependResList() { return null; }
        //是否加载图片
        public virtual bool UnloadImage(bool flag) { return false; }
        
        //资源为0的时候释放资源
        protected override void OnZeroRef()
        {
            //资源如果正在加载则不需要释放
            if (mResState == ResState.Loading)
                return;
            ReleaseRes();
        }
        //释放到缓存资源
        public virtual void Recycle2Cache() { }
        //响应资源回收事件
        public virtual void OnRecycled()
        {
            mAssetName = null;
            mOnResLoadDoneEvent = null;
        }
        //异步加载资源，finishCallback为加载完毕返回事件
        public virtual IEnumerator DoLoadAsync(Action finishCallback)
        {
            finishCallback();
            yield break;
        }
        //打印资源的数据
        public override string ToString()
        {
            return string.Format("Name:{0}\t State:{1}\t RefCount:{2}", AssetName, State, RefCount);
        }
        
        #endregion
    }
}