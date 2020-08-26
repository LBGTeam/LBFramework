using LBFramework.Log;
using UnityEditor;
using UnityEngine;

namespace LBFramework.ResKit
{
    public static class EditorRuntimeAssetDataCollector
    {
        public static ResDatas BuildDataTable()
        {
            LBLogWrapper.LogError("Start BuildAssetDataTable!");

            var resDatas = new ResDatas();
            AddABInfo2ResDatas(resDatas);
            return resDatas;
        }
        //从ab资源路径中取出名字
        private static string AssetPath2Name(string assetPath)
        {
            var startIndex = assetPath.LastIndexOf("/") + 1;
            var endIndex = assetPath.LastIndexOf(".");

            if (endIndex > 0)
            {
                var length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length).ToLower();
            }

            return assetPath.Substring(startIndex).ToLower();
        }
        //将AB信息添加进资源数据中
        public static void AddABInfo2ResDatas(IResDatas assetBundleConfigFile)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            var abNames = AssetDatabase.GetAllAssetBundleNames();
            if (abNames != null && abNames.Length > 0)
            {
                foreach (var abName in abNames)
                {
                    var depends = AssetDatabase.GetAssetBundleDependencies(abName, false);
                    AssetDataGroup group;
                    var abIndex = assetBundleConfigFile.AddAssetBundleName(abName, depends, out @group);
                    if (abIndex < 0)
                        continue;
                    var assets = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                    foreach (var cell in assets)
                    {
                        var type = AssetDatabase.GetMainAssetTypeAtPath(cell);
                        var code = type.ToCode();
                        @group.AddAssetData(cell.EndsWith(".unity")
                            ? new AssetData(AssetPath2Name(cell), ResLoadType.ABScene, abIndex, abName, code)
                            : new AssetData(AssetPath2Name(cell), ResLoadType.ABAsset, abIndex, abName, code));
                    }
                }
            }
        }
    }
}