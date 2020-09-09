using System.IO;
using LBFramework.LBUtils;
using UnityEditor;
using UnityEngine;

namespace LBFramework.ResKit
{
    [InitializeOnLoad]
    public class ResKitAssetsMenu
    {
        public const   string AssetBundlesOutputPath       = "AssetBundles";
        private static int    mSimulateAssetBundleInEditor = -1;


        private const string Mark_AssetBundle   = "Assets/@ResKit - AssetBundle Mark";

        static ResKitAssetsMenu()
        {
            Selection.selectionChanged = OnSelectionChanged;
        }

        public static void OnSelectionChanged()
        {
            var path = EditorUtils.GetSelectedPathOrFallback();
            if (!string.IsNullOrEmpty(path))
            {
                Menu.SetChecked(Mark_AssetBundle, Marked(path));
            }
        }

        public static bool Marked(string path)
        {
            var ai = AssetImporter.GetAtPath(path);
            var dir = new DirectoryInfo(path);
            return string.Equals(ai.assetBundleName, dir.Name.Replace(".", "_").ToLower());
        }

        public static void MarkAB(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var ai = AssetImporter.GetAtPath(path);
                var dir = new DirectoryInfo(path);

                if (Marked(path))
                {
                    Menu.SetChecked(Mark_AssetBundle, false);
                    ai.assetBundleName = null;
                }
                else
                {
                    Menu.SetChecked(Mark_AssetBundle, true);
                    ai.assetBundleName = dir.Name.Replace(".", "_");
                }

                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
        }


        [MenuItem(Mark_AssetBundle)]
        public static void MarkPTABDir()
        {
            var path = EditorUtils.GetSelectedPathOrFallback();
            MarkAB(path);
        }
    }
}