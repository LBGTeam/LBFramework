using System.Collections.Generic;
using System.Linq;
using LBFramework.LBBase;
using UnityEngine;

namespace LBFramework.ResKit
{
    //资源线性表
    public class ResTable : Table<IRes>
    {
        //索引资源线性表记录资源对应的索引
        public TableIndex<string, IRes> NameIndex = 
            new TableIndex<string, IRes>(res => res.AssetName.ToLower());

        //通过查找关键字资源对象获取资源
        public IRes GetResBySearchKeys(ResSearchKeys resSearchKeys)
        {
            //临时变量保留资源名字
            var assetName = resSearchKeys.AssetName;
            //获取资源对应的线性表
            var reses = NameIndex
                .Get(assetName);
            //通过资源类型筛选资源
            if (resSearchKeys.AssetType != null)
                reses = reses.Where(res => res.AssetType == resSearchKeys.AssetType);
            //通过ab资源名字筛选资源
            if (resSearchKeys.OwnerBundle != null)
                reses = reses.Where(res => res.OwnerBundleName == resSearchKeys.OwnerBundle);
            //返回资源第一个或者空的时候默认的资源
            return reses.FirstOrDefault();
        }
        //添加资源
        protected override void OnAdd(IRes item)
        {
            NameIndex.Add(item);
        }
        //删除资源
        protected override void OnRemove(IRes item)
        {
            NameIndex.Remove(item);
        }
        //清空
        protected override void OnClear()
        {
            NameIndex.Clear();
        }
        //线性扩展
        public override IEnumerator<IRes> GetEnumerator()
        {
            return NameIndex.Dictionary.SelectMany(d => d.Value)
                .GetEnumerator();
        }
        //释放对象
        protected override void OnDispose() { }
    }
}