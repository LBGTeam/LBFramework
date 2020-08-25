using System;
using LBFramework.LBUtils;
using UnityEngine;

namespace LBFramework.ResKit
{
    public interface IResLoader : IPoolable,IPoolType
    {
        /// 添加加载的资源
        /// <param name="assetName">资源名字</param>
        /// <param name="listener">监听的事件</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        void AddLoad(string assetName, Action<bool, IRes> listener, bool lastOrder = true);
        /// 添加加载的资源
        /// <param name="ownerBundleName">所在的Bundle名字</param>
        /// <param name="assetName">资源的名字</param>
        /// <param name="listener">监听的事件</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        void AddLoad(string ownerBundleName, string assetName, Action<bool, IRes> listener, bool lastOrder = true);
        
        //释放所有的资源
        void ReleaseAllRes();
        //不加载所有实例的资源
        void UnloadAllInstantiateRes(bool flag);
    }
}