using LBFramework.LBUtils;
using LBFramework.Log;
using UnityEngine;

namespace LBFramework.ResKit
{
    //AB场景资源
    public class AssetBundleSceneRes : AssetRes
    {
        public static AssetBundleSceneRes Allocate(string name)
        {
            AssetBundleSceneRes res = SafeObjectPool<AssetBundleSceneRes>.Instance.Allocate();
            if (res != null)
            {
                res.AssetName = name;
                res.InitAssetBundleName();
            }
            return res;
        }

        public AssetBundleSceneRes(string assetName) : base(assetName) { }

        public AssetBundleSceneRes() { }

        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            if (string.IsNullOrEmpty(AssetBundleName))
            {
                return false;
            }

            var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName);
            
            var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);

            resSearchKeys.Recycle2Cache();

            if (abR == null || abR.AssetBundle == null)
            {
                LBLogWrapper.LogError("Failed to Load Asset, Not Find AssetBundleImage:" + abR);
                return false;
            }
            
            State = ResState.Ready;
            return true;
        }

        public override void LoadAsync()
        {
            LoadSync();
        }
        
        public override void Recycle2Cache()
        {
            SafeObjectPool<AssetBundleSceneRes>.Instance.Recycle(this);
        }
    }
}