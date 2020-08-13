using System;
using UnityEngine;

namespace LBFramework.LBUtils
{
    //简单的对象池
    public class SimpleObjectPool<T>: Pool<T>
    {
        readonly Action<T> mResetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null,int initCount = 0)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;

            for (int i = 0; i < initCount; i++)
            {
                mCacheQueue.Enqueue(mFactory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            if (mResetMethod != null)
            {
                mResetMethod.Invoke(obj);
            }
            
            mCacheQueue.Enqueue(obj);
            return true;
        }
    }
}