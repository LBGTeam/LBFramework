using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace LBFramework.ResKit
{
    public class FilePath
    {
        //持久的数据地址
        private static string mPersistentDataPath;
        //本地资源文件地址
        private static string mStreamingAssetsPath;
        //资源的持久数据地址
        private static string mPersistentDataPath4Res;
        //图片的持久数据地址
        private static string mPersistentDataPath4Photo;
        
        public static string PersistentDataPath
        {
            get
            {
                if (null == mPersistentDataPath)
                    mPersistentDataPath = Application.persistentDataPath + "/";

                return mPersistentDataPath;
            }
        }
        
        public static string StreamingAssetsPath
        {
            get
            {
#if UNITY_EDITOR
                mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
                mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE
                mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_STANDALONE_WIN
                mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_STANDALONE_OSX
                mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#else
                mStreamingAssetsPath = Application.streamingAssetsPath + "/";
#endif
                return mStreamingAssetsPath;
            }
        }
        
        public static string PersistentDataPath4Res
        {
            get
            {
                if (null == mPersistentDataPath4Res)
                {
                    mPersistentDataPath4Res = PersistentDataPath + "Res/";

                    if (!Directory.Exists(mPersistentDataPath4Res))
                    {
                        Directory.CreateDirectory(mPersistentDataPath4Res);
#if UNITY_IPHONE && !UNITY_EDITOR
						UnityEngine.iOS.Device.SetNoBackupFlag(mPersistentDataPath4Res);
#endif
                    }
                }

                return mPersistentDataPath4Res;
            }
        }
        
        public static string PersistentDataPath4Photo
        {
            get
            {
                if (null == mPersistentDataPath4Photo)
                {
                    mPersistentDataPath4Photo = PersistentDataPath + "Photos\\";

                    if (!Directory.Exists(mPersistentDataPath4Photo))
                    {
                        Directory.CreateDirectory(mPersistentDataPath4Photo);
                    }
					
                }

                return mPersistentDataPath4Photo;
            }
        }
        
        //获取资源路径，有限返回外存的资源路径
        public static string GetResPathInPersistentOrStream(string relativePath)
        {
            string resPersistentPath = string.Format("{0}{1}", FilePath.PersistentDataPath4Res, relativePath);

            if (File.Exists(resPersistentPath))
            {
                return resPersistentPath;
            }
            else
            {
                return FilePath.StreamingAssetsPath + relativePath;
            }
        }
        //获取上一级的目录
        public static string GetParentDir(string dir, int floor = 1)
        {
            string subDir = dir;
            for (int i = 0; i < floor; ++i)
            {
                int last = subDir.LastIndexOf('/');
                subDir = subDir.Substring(0, last);
            }
            return subDir;
        }
        //从文件管理器中获取文件
        public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
        {
            if (outResult == null)
            {
                return;
            }
            var dir = new DirectoryInfo(dirName);
            if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System") > -1)
            {
                return;
            }
            var fileInfos = dir.GetFiles(fileName);
            outResult.AddRange(fileInfos.Select(fileInfo => fileInfo.FullName));

            var dirInfos = dir.GetDirectories();
            foreach (var dinfo in dirInfos)
            {
                GetFileInFolder(dinfo.FullName, fileName, outResult);
            }
        }
    }
}