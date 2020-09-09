using System.IO;
using UnityEditor;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class AssetBundleInfo
    {
        public readonly string Name = "";

        public AssetBundleInfo(string name)
        {
            this.Name = name;
        }

        public string[] assets;
    }
    public static class BuildScript
    {
        public static void BuildAssetBundles(BuildTarget buildTarget)
        {
            // Choose the output path according to the build target.
            var outputPath = Path.Combine(ResKitAssetsMenu.AssetBundlesOutputPath, GetPlatformName());
            outputPath.CreateDirIfNotExists();

            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);

            GenerateVersionConfig();

            var finalDir = Application.streamingAssetsPath + "/AssetBundles/" + GetPlatformName();

            finalDir.DeleteDirIfExists();
            finalDir.CreateDirIfNotExists();

            FileUtil.ReplaceDirectory(outputPath, finalDir);

            AssetBundleExporter.BuildDataTable();
            AssetDatabase.Refresh();
        }

        private static void GenerateVersionConfig()
        {
            if (ResKitEditorWindow.EnableGenerateClass)
            {
                WriteClass();
            }
        }

        public static void WriteClass()
        {
            "Assets/LBFrameworkData".CreateDirIfNotExists();

            var path = Path.GetFullPath(
                Application.dataPath + Path.DirectorySeparatorChar + "LBFrameworkData/QAssets.cs");
            var writer = new StreamWriter(File.Open(path, FileMode.Create));
            ResKitInfoGenerator.WriteClass(writer, "QAssetBundle");
            writer.Close();
            AssetDatabase.Refresh();
        }


        private static string GetPlatformName()
        {
            return AssetBundleSettings.GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        }
    }
}