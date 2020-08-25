using System;
using System.IO;
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
    }
}