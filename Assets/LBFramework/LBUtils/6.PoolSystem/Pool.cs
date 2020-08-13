using System.Collections.Generic;
using UnityEngine;

namespace LBFramework.LBUtils
{
    public abstract class Pool<T> : IPool<T>, ICountObserveAble
    {
        protected readonly Queue<T> mCacheQueue = new Queue<T>();        //对象池队列
        protected IObjectFactory<T> mFactory;        //创建的工厂，用来创建新的对象
        protected int mMaxCount = 12;                //记录对象池最大的个数
        
        #region ICountObserverable
        
        public int CurCount    //继承对象池数量接口
        {
            get { return mCacheQueue.Count; }
        }
        
        #endregion
        
        # region IPool<T>
        
        public virtual T Allocate()
        {
            return mCacheQueue.Count == 0
                ? mFactory.Create()            //如果队列没有了就创建一个对象返回
                : mCacheQueue.Dequeue();        //队列还有的时候移除并返回队列头部的对象
        }

        public abstract bool Recycle(T obj);    //回收对象
        
        #endregion
    }
}