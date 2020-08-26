using System.Collections.Generic;
using System.Linq;
using LBFramework.LBBase;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class AssetDataTable : Table<AssetData>
    {
        //创建一个资源名字和资源对应的委托的索引表
        public TableIndex<string, AssetData> NameIndex = new TableIndex<string, AssetData>(data => data.AssetName);
        //通过关键字对象获取对应的资源
        public AssetData GetAssetDataByResSearchKeys(ResSearchKeys resSearchKeys)
        {
            //获取资源的名字并全部将为小写
            var assetName = resSearchKeys.AssetName.ToLower();
            //通过名字索引表获取对应的资源列表
            var assetDatas = NameIndex
                .Get(assetName);
            //如果资源是AB资源
            if (resSearchKeys.OwnerBundle != null)
            {
                //从资源中获取左右资源AB名字相同的资源
                assetDatas = assetDatas.Where(a => a.OwnerBundleName == resSearchKeys.OwnerBundle);
            }
            //如果资源存在资源的类型
            if (resSearchKeys.AssetType != null)
            {
                //通过类型获取对应的资源数据类型
                var assetTypeCode = resSearchKeys.AssetType.ToCode();
                //如果为空直接跳过
                if (assetTypeCode == 0) { }
                else
                {
                    //从资源列表中获取所有资源类型相同的资源集合
                    var newAssetDatas = assetDatas.Where(a => a.AssetObjectTypeCode == assetTypeCode);

                    // 有可能是从旧的 AssetBundle 中加载出来的资源
                    if (newAssetDatas.Any())
                    {
                        assetDatas = newAssetDatas;
                    }
                }

            }
            //返回查找出来的第一个，如果序列中不包含任何元素，则返回默认值。 
            return assetDatas.FirstOrDefault();
        }
        //添加一个数据
        protected override void OnAdd(AssetData item)
        {
            NameIndex.Add(item);
        }
        //移除一个数据
        protected override void OnRemove(AssetData item)
        {
            NameIndex.Remove(item);
        }
        //清空数据
        protected override void OnClear()
        {
            NameIndex.Clear();
        }
        //实现线性接口可以遍历所有的数据
        public override IEnumerator<AssetData> GetEnumerator()
        {
            return NameIndex.Dictionary.SelectMany(kv => kv.Value).GetEnumerator();
        }
        //线性表进行释放
        protected override void OnDispose()
        {
            NameIndex.Dispose();
        }
    }
}