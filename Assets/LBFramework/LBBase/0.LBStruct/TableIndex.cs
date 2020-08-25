using System;
using System.Collections.Generic;
using System.Linq;
using LBFramework.LBUtils;
using UnityEngine;

namespace LBFramework.LBBase
{
    public class TableIndex<TKeyType, TDataItem>:IDisposable
    {
        //通过关键字类型存在对应的数据
        private Dictionary<TKeyType, List<TDataItem>> mIndex = new Dictionary<TKeyType, List<TDataItem>>();
        //通过数据获得对应的关键字的委托
        private Func<TDataItem, TKeyType> mGetKeyByDataItem = null;
        //构造函数传递对应的委托
        public TableIndex(Func<TDataItem, TKeyType> keyGetter)
        {
            mGetKeyByDataItem = keyGetter;
        }
        //对外公开的获取数据字典的方法
        public IDictionary<TKeyType, List<TDataItem>> Dictionary
        {
            get { return mIndex; }
        }
        //添加数据
        public void Add(TDataItem dataItem)
        {
            //首先利用委托获取对应的key
            var key = mGetKeyByDataItem(dataItem);
            //查看是否有对应的key如果有就直接添加数据
            if (mIndex.ContainsKey(key))
            {
                mIndex[key].Add(dataItem);
            }
            else
            {
                //如果没有就创建对应的数据列表
                var list = ListPool<TDataItem>.Get();
                //将数据添加到列表上
                list.Add(dataItem);
                //将列表和关键字添加到对应的字典中
                mIndex.Add(key, list);
            }
        }
        //移除对应的数据
        public void Remove(TDataItem dataItem)
        {
            //通过委托获取对应的关键字
            var key = mGetKeyByDataItem(dataItem);
            //通过关键字获取字典中对应的列表然后删除数据
            mIndex[key].Remove(dataItem);
        }
        //获取对应的数据线性表
        public IEnumerable<TDataItem> Get(TKeyType key)
        {
            //创建一个新的数据列表
            List<TDataItem> retList = null;
            //通过关键字在字典中获取对应的列表
            if (mIndex.TryGetValue(key, out retList))
            {
                //返回对应的列表
                return retList;
            }
            
            // 如果没有获取到就返回一个空的集合
            return Enumerable.Empty<TDataItem>();
        }
        //清空整个表的数据
        public void Clear()
        {
            //遍历所有的字典数据
            foreach (var value in mIndex.Values)
            {
                //将数据清空
                value.Clear();
            }
            //最后把整个字典清空
            mIndex.Clear();
        }
        //回收所有的线性表对象池
        public void Dispose()
        {
            foreach (var value in mIndex.Values)
                value.Release2Pool();
            mIndex.Release2Pool();
            mIndex = null;
        }
    }
    
}