using System;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class DisposableObject : IDisposable
    {
        //标记资源是否释放
        private Boolean mDisposed = false;
        
        //析构函数进行资源释放
        ~DisposableObject()
        {
            Dispose(false);
        }
        
        //释放资源
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        //释放GC
        protected virtual void DisposeGC() { }
        
        //释放不是GC的
        protected virtual void DisposeNGC() { }
        
        //释放资源
        private void Dispose(Boolean disposing)
        {
            if (mDisposed)
                return;
            if (disposing)
                DisposeGC();
            
            DisposeNGC();
            mDisposed = true;
        }
    }
}