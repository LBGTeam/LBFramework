using System;
using LBFramework.LBUtils;

namespace LBFramework.ResKit
{
    //资源的状态
    public enum ResState        
    {
        Waiting = 0,            //等待中
        Loading = 1,            //加载中
        Ready = 2,              //加载完准备中
    }

    //资源加载方式的类型
    public static class ResLoadType
    {
        //从AssetBundle中加载
        public const short AssetBundle   = 0;
        //AB中的资源
        public const short ABAsset       = 1;
        //AB中的场景
        public const short ABScene       = 2;
        //内部的资源
        public const short Internal      = 3;
        //网络图片资源
        public const short NetImageRes   = 4;
        //本地图片资源
        public const short LocalImageRes = 5;
    }
    
    //需要加载的**资源**的接口
    public interface IRes : IRefCounter,IPoolType,IEnumeratorTask
    {
        //资源的名字
        string AssetName { get; }
        //拥有的bundle的名字
        string OwnerBundleName { get; }
        //记录资源的状态 
        ResState State { get; }
        //保存资源
        UnityEngine.Object Asset { get; }
        //记录加载资源的进度
        float Progress { get; }
        //资源的类型
        Type AssetType { get; set; }
        //注册加载资源完毕的事件
        void RegisteOnResLoadDoneEvent(Action<bool, IRes> listener);
        //移除加载资源完毕的事件
        void UnRegisteOnResLoadDoneEvent(Action<bool, IRes> listener);
        //标记是否卸载Image资源
        bool UnloadImage(bool flag);
        //同步加载资源
        bool LoadSync();
        //异步加载资源
        void LoadAsync();
        //获取资源依赖集合
        string[] GetDependResList();
        //获取依赖的资源是否加载完成
        bool IsDependResLoadFinish();
        //释放资源
        bool ReleaseRes();
    }
}