using System.Collections;
using UnityEngine;

namespace LBFramework.ResKit
{
    //资源数据集合类
    public interface IResDatas
    {
        //资源文件的名字
        string FileName { get; }
        //获取资源的所有依赖
        string[] GetAllDependenciesByUrl(string url);
        /// 从文件中加载资源
        /// <param name="outRes">资源文件的名字</param>
        void LoadFromFile(string outRes);
        //重置资源
        void Reset();
        /// 异步从文件中加载资源
        /// <param name="outRes">资源文件的名字</param>
        IEnumerator LoadFromFileAsync(string outRes);
        //通过关键字资源对象获取资源
        AssetData GetAssetData(ResSearchKeys resSearchKeys);
        /// 添加AssetBundle名字
        /// <param name="abName">ab的名字</param>
        /// <param name="depends">依赖的名字</param>
        /// <param name="group">资源数据组</param>
        /// <returns></returns>
        int AddAssetBundleName(string abName, string[] depends, out AssetDataGroup @group);
    }
}