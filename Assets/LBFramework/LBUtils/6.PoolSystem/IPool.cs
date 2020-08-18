using System;
using System.Reflection;
using UnityEngine;

namespace LBFramework.LBUtils
{
    //对象池中对象的个数
    public interface ICountObserveAble
    {
        int CurCount { get; }    //记录对象个数
    }
    public interface IPool<T>
    {
        T Allocate();             //申请获取一个对象
        bool Recycle(T obj);        //回收一个对象
    }
    public interface IPoolable
    {
        void OnRecycled();            //响应对象池的回收
        bool IsRecycled { get; set; }    //标记是否被回收
    }
    public interface IPoolType
    {
        void Recycle2Cache();        //对象池回收进缓存
    }
    
    #region Factory
    
    //工厂接口，用来生产出来对象
    public interface IObjectFactory<T>
    {
        T Create();        //创建并返回对象
    }
    //创建一个普通对象的工厂
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();    
        }
    }
    //定制的创建对象工厂
    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            mFactoryMethod = factoryMethod;
        }
        
        protected Func<T> mFactoryMethod;

        public T Create()
        {
            return mFactoryMethod();
        }
    }
    
    //没有公有构造函数的对象创建工厂
    public class NonPublicObjectFactory<T> : IObjectFactory<T> where T : class
    {
        public T Create()
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            return ctor.Invoke(null) as T;
        }
    }
    
    #endregion
}