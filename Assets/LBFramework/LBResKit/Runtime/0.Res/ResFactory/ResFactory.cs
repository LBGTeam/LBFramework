using System.Collections.Generic;
using System.Linq;
using LBFramework.Log;
using UnityEngine;

namespace LBFramework.ResKit
{
    public static class ResFactory
    {
        static List<IResCreator> mResCreators = new List<IResCreator>()
        {
            new ResourcesResCreator(),
            new AssetBundleResCreator(),
            new AssetResCreator(),
            new AssetBundleSceneResCreator(),
            new NetImageResCreator(),
            new LocalImageResCreator()
        };
        //根据资源查找对象创建对应的资源
        public static IRes Create(ResSearchKeys resSearchKeys)
        {
            var retRes = mResCreators
                .Where(creator => creator.Match(resSearchKeys))
                .Select(creator => creator.Create(resSearchKeys))
                .FirstOrDefault();

            if (retRes == null)
            {
                LBLogWrapper.LogError("Failed to Create Res. Not Find By ResSearchKeys:" + resSearchKeys);
                return null;
            }

            return retRes;
        }
        //添加一个需要创建的资源
        public static void AddResCreator<T>() where T : IResCreator, new()
        {
            mResCreators.Add(new T());
        }
        //添加一个需要创建的资源
        public static void AddResCreator(IResCreator resCreator)
        {
            mResCreators.Add(resCreator);
        }
        //添加一个需要创建的资源
        public static void RemoveResCreator<T>() where T : IResCreator, new()
        {
            mResCreators.RemoveAll(r => r.GetType() == typeof(T));
        }
    }
    
    public class NetImageResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("netimage:");
        }
        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return NetImageRes.Allocate(resSearchKeys.AssetName);
        }
    }

    public class LocalImageResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("localimage:");
        }
        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return NetImageRes.Allocate(resSearchKeys.AssetName);
        }
    }
}