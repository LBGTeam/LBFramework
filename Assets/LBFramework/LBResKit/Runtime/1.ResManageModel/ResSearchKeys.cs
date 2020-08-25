using System;
using LBFramework.LBUtils;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class ResSearchKeys : IPoolable,IPoolType
    {
        //资源的名字
        public string AssetName { get; set; }
        //资源的AssetBundle名字
        public string OwnerBundle { get;  set; }
        //资源的类型
        public Type AssetType { get; set; }
        
        //对象池获取对象
        public static ResSearchKeys Allocate(string assetName, string ownerBundleName = null, Type assetType = null)
        {
            //从安全对象池获取一个对象
            var resSearchRule = SafeObjectPool<ResSearchKeys>.Instance.Allocate();
            //设置资源的名字，全部用小写
            resSearchRule.AssetName = assetName.ToLower();
            //判断是否有AB名字，如果有就设置进去
            resSearchRule.OwnerBundle = ownerBundleName == null ? null : ownerBundleName.ToLower();
            //设置资源的类型
            resSearchRule.AssetType = assetType;
            //返回该资源关键字对象
            return resSearchRule;
        }
        //释放对象
        public void Recycle2Cache()
        {
            //安全对象池回收对象
            SafeObjectPool<ResSearchKeys>.Instance.Recycle(this);
        }

        //匹配两个资源是否相同
        public bool MatchRes(IRes res)
        {
            if (res.AssetName == AssetName)    //首先判断资源名字是否相同
            {
                var isMatch = true;    //首先标记相同
                if (AssetType != null)    //如果存在资源类型，就比对资源类型是否相同
                {
                    isMatch = res.AssetType == AssetType;
                }
                if (OwnerBundle != null)    //如果存在ab名字就匹配是否相同并和上述判断进行且判断
                {
                    isMatch = isMatch && res.OwnerBundleName == OwnerBundle;
                }
                return isMatch;    //返回匹配的结果
            }
            return false;
        }
        
        //响应对象回收事件
        void IPoolable.OnRecycled()
        {
            AssetName = null;
            OwnerBundle = null;
            AssetType = null;
        }
        
        //标记对象是否被回收
        bool IPoolable.IsRecycled { get; set; }
        
        //返回对象可展示数据
        public override string ToString()
        {
            return string.Format("AssetName:{0} BundleName:{1} TypeName:{2}", AssetName, OwnerBundle,
                AssetType);
        }
    }
}