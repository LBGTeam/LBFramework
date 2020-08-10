using System;
using UnityEngine;

namespace LBFramework.LBUtils
{
    //安全的对象池
    public class SafeObjectPool<T>: Pool<T>, ISingleton where T : IPoolable, new()
    {
        #region Singleton
        void ISingleton.OnInitSingleton(){}

        public SafeObjectPool()
        {
            mFactory = new DefaultObjectFactory<T>();
        }

        public static SafeObjectPool<T> Instance
        {
            get { return SingletonProperty<SafeObjectPool<T>>.Instance; }
        }

        public void Dispose()
        {
            SingletonProperty<SafeObjectPool<T>>.Dispose();
        }
        #endregion
        
        //最大缓存个数
        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;

                if (mCacheQueue != null)
                {
                    if (mMaxCount > 0)
                    {
                        if (mMaxCount < mCacheQueue.Count)
                        {
                            int removeCount = mCacheQueue.Count - mMaxCount;
                            while (removeCount > 0)
                            {
                                mCacheQueue.Dequeue();
                                --removeCount;
                            }
                        }
                    }
                }
            }
        }
        
        //初始化对象池的数量
        public void Init(int maxCount, int initCount)
        {
            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
                mMaxCount = maxCount;
            }
            if (CurCount < initCount)
            {
                for (var i = CurCount; i < initCount; ++i)
                {
                    Recycle(mFactory.Create());
                }
            }
        }
        
        #region IPool
        
        //申请一个对象并返回该对象
        public override T Allocate()
        {
            var result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }
        
        //回收对象，和NonPublicObjectPool的作用相同
        public override bool Recycle(T t)
        {
            if (t == null || t.IsRecycled)
            {
                return false;
            }

            if (mMaxCount > 0)
            {
                if (mCacheQueue.Count >= mMaxCount)
                {
                    t.OnRecycled();
                    return false;
                }
            }
            t.IsRecycled = true;
            t.OnRecycled();
            mCacheQueue.Enqueue(t);
            return true;
        }
        
        #endregion
    }
}