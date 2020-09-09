using LBFramework.Log;
using UnityEditor;
using UnityEngine;

namespace LBFramework.ResKit
{
    public static class AssetBundleExporter
    {
        public static void BuildDataTable()
        {
            LBLogWrapper.LogInfo("Start BuildAssetDataTable!");
            ResDatas table = new ResDatas();
            EditorRuntimeAssetDataCollector.AddABInfo2ResDatas(table);

            var filePath =
                (FilePath.StreamingAssetsPath + AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER).CreateDirIfNotExists() +
                table.FileName;
			
            table.Save(filePath);
            AssetDatabase.Refresh();
        }
    }
}