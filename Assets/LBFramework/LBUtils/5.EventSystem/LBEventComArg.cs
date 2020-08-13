using UnityEngine;

namespace LBFramework.LBUtils
{
    public class LBEventComArg
    {
        #region 获取多参数类型

        public bool TryGet<T>(out T t)
        {
            LBEventOneArg<T> arg = this as LBEventOneArg<T>;
            arg.Get(out t);
            return true;
        }
        public bool TryGet<T,U>(out T t,out U u)
        {
            LBEventTwoArg<T,U> arg = this as LBEventTwoArg<T,U>;
            arg.Get(out t,out u);
            return true;
        }
        public bool TryGet<T,U,K>(out T t,out U u,out K k)
        {
            LBEventThreeArg<T,U,K> arg = this as LBEventThreeArg<T,U,K>;
            arg.Get(out t,out u,out k);
            return true;
        }
        public bool TryGet<T,U,K,I>(out T t,out U u,out K k,out I i)
        {
            LBEventFourArg<T,U,K,I> arg = this as LBEventFourArg<T,U,K,I>;
            arg.Get(out t,out u,out k,out i);
            return true;
        }
        public bool TryGet<T,U,K,I,W>(out T t,out U u,out K k,out I i,out W w)
        {
            LBEventFiveArg<T,U,K,I,W> arg = this as LBEventFiveArg<T,U,K,I,W>;
            arg.Get(out t,out u,out k,out i,out w);
            return true;
        }

        #endregion
    }
    #region  多参数数据类
    public class LBEventOneArg<T> : LBEventComArg
    {
        private T m_t;
        public void Set(T t) { m_t = t; }
        public void Get(out T t) { t = m_t; }
    }
    public class LBEventTwoArg<T,U> : LBEventComArg
    {
        private T m_t; private U m_u;
        public void Set(T t,U u) { m_t = t; m_u = u; }
        public void Get(out T t,out U u) { t = m_t; u = m_u; }
    }
    public class LBEventThreeArg<T,U,K> : LBEventComArg
    {
        private T m_t; private U m_u; private K m_k;
        public void Set(T t,U u,K k) { m_t = t; m_u = u; m_k = k; }
        public void Get(out T t,out U u,out K k) { t = m_t; u = m_u; k = m_k; }
    }
    public class LBEventFourArg<T,U,K,I> : LBEventComArg
    {
        private T m_t; private U m_u; private K m_k; private I m_i;
        public void Set(T t,U u,K k,I i) { m_t = t; m_u = u; m_k = k; m_i = i; }
        public void Get(out T t,out U u,out K k,out I i) { t = m_t; u = m_u; k = m_k; i = m_i; }
    }
    public class LBEventFiveArg<T,U,K,I,W> : LBEventComArg
    {
        private T m_t; private U m_u; private K m_k; private I m_i; private W m_w;
        public void Set(T t,U u,K k,I i,W w) { m_t = t; m_u = u; m_k = k; m_i = i; m_w = w; }
        public void Get(out T t,out U u,out K k,out I i,out W w) { t = m_t; u = m_u; k = m_k; i = m_i; w = m_w; }
    }
    #endregion
    
}