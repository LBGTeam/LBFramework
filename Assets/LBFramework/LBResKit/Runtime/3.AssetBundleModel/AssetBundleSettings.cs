using System;
using System.Collections.Generic;
using System.IO;
using LBFramework.Log;
using UnityEditor;
using UnityEngine;

namespace LBFramework.ResKit
{
    public class AssetBundleSettings
    {
        //AB资源的配置文件
        private static IResDatas mAssetBundleConfigFile = null;
        //默认
        private static Func<IResDatas> mAssetBundleConfigFileFactory = 
            () =>  new ResDatas();
        
        //对外公开的设置默认的AB文件工厂
        public static Func<IResDatas> AssetBundleConfigFileFactory
        {
            set { mAssetBundleConfigFileFactory = value; }
        }
        //对外公开的获取和设置AB资源文件
        public static IResDatas AssetBundleConfigFile
        {
            get
            {
                if (mAssetBundleConfigFile == null)
                {
                    //返回一个资源配置文件工厂无参的委托
                    mAssetBundleConfigFile = mAssetBundleConfigFileFactory.Invoke();
                }
                return mAssetBundleConfigFile;
            }
            set { mAssetBundleConfigFile = value; }
        }
        //是否从资源文件中加载资源
        private const string kLoadResFromStreammingAssetsPath = "LoadResFromStreammingAssetsPath";
        public static bool LoadAssetResFromStreammingAssetsPath
        {
            get { return PlayerPrefs.GetInt(kLoadResFromStreammingAssetsPath, 1) == 1; }
            set { PlayerPrefs.SetInt(kLoadResFromStreammingAssetsPath, value ? 1 : 0); }
        }
        
#if UNITY_EDITOR
        //此处跟editor中保持统一，不能随意更改
        const string kSimulateAssetBundles = "SimulateAssetBundles"; 
        //是否在编辑器模式下模拟使用AB资源
        public static bool SimulateAssetBundleInEditor
        {
            get { return UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true); }
            set { UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value); }
        }
#endif
        //获取远程的ab资源名字
        public static string AssetBundleUrl2Name(string url)
        {
            string retName = null;
            string parren = FilePath.StreamingAssetsPath + "AssetBundles/" + GetPlatformName() + "/";
            retName = url.Replace(parren, "");

            parren = FilePath.PersistentDataPath + "AssetBundles/" + GetPlatformName() + "/";
            retName = retName.Replace(parren, "");
            return retName;
        }
        //从ab名字转化为对应的地址
        public static string AssetBundleName2Url(string name)
        {
            string retUrl = FilePath.PersistentDataPath + "AssetBundles/" + GetPlatformName() + "/" + name;
            if (File.Exists(retUrl))
            {
                return retUrl;
            }
            return FilePath.StreamingAssetsPath + "AssetBundles/" + GetPlatformName() + "/" + name;
        }
        
        //ab资源存放的路径
        public static string RELATIVE_AB_ROOT_FOLDER
        {
            get { return "/AssetBundles/" + GetPlatformName() + "/"; }
        }
        
        
        //从名字中获取对应的平台
        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(Application.platform);
#endif
        }
        
#if UNITY_EDITOR
        //获取对应的平台
        public static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "Linux";
#if !UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
#elif UNITY_5
			case BuildTarget.StandaloneOSXUniversal:
#else
                case BuildTarget.StandaloneOSX:
#endif
                    return "OSX";
                default:
                    return null;
            }
        }
#endif
        public static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }

        #region 设置AB资源名字
        public static void SetAssetBundleLables()
        {
            //移除掉所有没有使用的标记
            AssetDatabase.RemoveUnusedAssetBundleNames();
            string assetDirectory = "Assets/Res";
            DirectoryInfo directoryInfo = new DirectoryInfo(assetDirectory);
            DirectoryInfo[] scenesDirectories = directoryInfo.GetDirectories();
            foreach (var tempDir in scenesDirectories)
            {
                string sceneDirectory = assetDirectory + "/" + tempDir.Name;
                DirectoryInfo sceneDirectoryInfo = new DirectoryInfo(sceneDirectory);
                if (sceneDirectoryInfo == null)
                {
                    Debug.Log(sceneDirectoryInfo + "不存在");
                    return;
                }
                else
                {
                    Dictionary<string , string> namePathDictionary = new Dictionary<string, string>();
                    int index = sceneDirectory.LastIndexOf("/");
                    string sceneName = sceneDirectory.Substring(index + 1);
                    OnSceneFileSystemInfo(sceneDirectoryInfo , sceneName , namePathDictionary);
                    OnWriteConfig(sceneName , namePathDictionary);
                }
            }
            AssetDatabase.Refresh();
            LBLogWrapper.LogInfo("设置标记成功...");
        }
        private static void OnSceneFileSystemInfo(FileSystemInfo fileSystemInfo , string sceneNama , Dictionary<string, string> namePathDictionary)
        {
            if (!fileSystemInfo.Exists)
            {
                LBLogWrapper.LogError(fileSystemInfo + "不存在");
                return;
            }
            DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;

            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
            foreach (var systemInfo in fileSystemInfos)
            {
                FileInfo fileInfo = systemInfo as FileInfo;
                if (fileInfo == null)
                {
                    OnSceneFileSystemInfo(systemInfo, sceneNama , namePathDictionary);
                }
                else
                {
                    SetLables(fileInfo, sceneNama , namePathDictionary);
                }
            }
        }
        private static void OnWriteConfig(string sceneName , Dictionary<string , string> namePathDictionary)
        {
            string path = Application.dataPath + "/AssetBundles/" + sceneName ;

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Debug.Log(path);
            using (FileStream fs = new FileStream(path + "/Record.txt", FileMode.OpenOrCreate , FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(namePathDictionary.Count);
                    foreach (KeyValuePair<string , string> kv in namePathDictionary)
                    {
                        Debug.Log(kv.Value);
                        sw.WriteLine(kv.Key+"/"+kv.Value);
                    }
                }
            }
        }
        private static void SetLables(FileInfo fileInfo , string sceneName , Dictionary<string, string> namePathDictionary)
        {
            if(fileInfo.Extension == ".meta")return;
            string bundleName = GetBundleName(fileInfo , sceneName);
            int index = fileInfo.FullName.IndexOf("Assets");
            string assetPath = fileInfo.FullName.Substring(index);
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            assetImporter.assetBundleName = bundleName;
            if (fileInfo.Extension == ".unity")
                assetImporter.assetBundleVariant = "u3d";
            else
                assetImporter.assetBundleVariant = "assetbundle";
            string folderName;
            if (bundleName.Contains("/"))
                folderName = bundleName.Split('/')[1];
            else
                folderName = bundleName;
            string bundlePath = assetImporter.assetBundleName + "." + assetImporter.assetBundleVariant;
            if (!namePathDictionary.ContainsKey(folderName))
                namePathDictionary.Add(folderName, bundlePath);
        }
        private static string GetBundleName(FileInfo fileInfo, string sceneName)
        {
            string path = fileInfo.FullName;
            int index = path.IndexOf(sceneName) + sceneName.Length;
            string bundlePath = path.Substring(index + 1);
            bundlePath = bundlePath.Replace(@"\", "/");
            if (bundlePath.Contains("/"))
            {
                string[] tmp = bundlePath.Split('/');

                return sceneName + "/" + tmp[0];
            }
            return sceneName;
        }
        #endregion
    }
}