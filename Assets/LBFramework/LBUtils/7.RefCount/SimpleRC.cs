using UnityEngine;

namespace LBFramework.LBUtils
{
    //引用计数接口
    public interface IRefCounter
    {
        int RefCount { get; }

        void Retain(object refOwner = null);
        void Release(object refOwner = null);
    }
    //简单的引用计数
    public abstract class SimpleRC : IRefCounter
    {
        public int RefCount { get; private set; }        //统计引用个数
        
        public SimpleRC()
        {
            RefCount = 0;                            //构造函数初始化引用个数
        }

        public void Retain(object refOwner = null)
        {
            ++RefCount;                        //添加一个新的引用
        }
        public void Release(object refOwner = null)
        {
            --RefCount;                       //减少一个引用
            if (RefCount == 0)
            {
                OnZeroRef();                //清空引用
            }
        }
        protected virtual void OnZeroRef()        //清空引用
        {
        }
    }
}