using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBFramework.LBBase
{
    public abstract class Table<TDataItem> : IEnumerable<TDataItem>, IDisposable
    {

        //线性表直接添加数据
        public void Add(TDataItem item)
        {
            OnAdd(item);
        }
        //移除一个数据
        public void Remove(TDataItem item)
        {
            OnRemove(item);
        }
        //清空线性表
        public void Clear()
        {
            OnClear();
        }
        protected abstract void OnAdd(TDataItem item);
        protected abstract void OnRemove(TDataItem item);
        protected abstract void OnClear();
        public abstract IEnumerator<TDataItem> GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            OnDispose();
        }
        protected abstract void OnDispose();
    }
}