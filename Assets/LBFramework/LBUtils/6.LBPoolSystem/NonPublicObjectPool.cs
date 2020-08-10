using System;
using UnityEngine;

namespace LBFramework.LBUtils
{
    //没有公有的构造函数的对象池 例如单例就是私有的构造函数
    public class NonPublicObjectPool<T>:Pool<T>,ISingleton where T : class,IPoolable
    {
        #region Singleton
        
        void ISingleton.OnInitSingleton(){}

        public static NonPublicObjectPool<T> Instance
        {
            get { return SingletonProperty<NonPublicObjectPool<T>>.Instance; }
        }
        
        protected NonPublicObjectPool()
        {
            mFactory = new NonPublicObjectFactory<T>();
        }

        public void Dispose()
        {
            SingletonProperty<NonPublicObjectPool<T>>.Dispose();
        }
        
        #endregion
        
        //初始化对象池的数量
        public void Init(int maxCount, int initCount)
        {
            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);        //初始化数量为最小的个数
            }

            if (CurCount >= initCount) return;            //如果当前数量大于初始化数量返回
			
            for (var i = CurCount; i < initCount; ++i)
            {   
                Recycle(mFactory.Create());            //工厂开始创建当前的初始化数量
            }
        }
        //设置最大的缓存数量
        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;
                if (mCacheQueue == null) return;
                if (mMaxCount <= 0) return;
                if (mMaxCount >= mCacheQueue.Count) return;
                var removeCount = mMaxCount - mCacheQueue.Count;
                while (removeCount > 0)
                {
                    mCacheQueue.Dequeue();
                    --removeCount;
                }
            }
        }

        #region IPool

        public override T Allocate()    //申请一个对象并返回该对象
        {
            var result = base.Allocate();    //申请对象
            result.IsRecycled = false;        //该对象标记为没有被回收
            return result;                    //返回该对象
        }

        public override bool Recycle(T t)        //回收该对象
        {
            if (t == null || t.IsRecycled)        //如果已经被回收就返回
            {
                return false;
            }
            if (mMaxCount > 0)                
            {
                if (mCacheQueue.Count >= mMaxCount)    //如果当前数量大于最大数量，不放回原来的队列中
                {
                    t.OnRecycled();
                    return false;
                }
            }
            t.IsRecycled = true;                //标记为已经被回收
            t.OnRecycled();                    //相应被回收的事件
            mCacheQueue.Equals(t);            //回收并放回原来的队列中
            return true;
        }
        
        #endregion
        
    }
}