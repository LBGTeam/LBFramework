using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using System.Linq;

namespace LBFramework.ResKit
{
    public static partial class ObjectAssetTypeCode
    {
        //资源的类别
        public const short GameObject  = 1;
        public const short AudioClip   = 2;
        public const short Sprite      = 3;
        public const short Scene       = 4;
        public const short SpriteAtlas = 5;
        public const short Mesh        = 6;
        public const short Texture2D   = 7;
        public const short TextAsset   = 8;
        public const short AssetBundle   = 8;
        
        //转换资源类型的类别
        public static Type GameObjectType = typeof(GameObject);
        public static Type AudioClipType = typeof(AudioClip);
        public static Type SpriteType = typeof(Sprite);
        public static Type SceneType = typeof(Scene);
        public static Type SpriteAtlasType = typeof(SpriteAtlas);
        public static Type MeshType = typeof(Mesh);
        public static Type Texture2DType = typeof(Texture2D);
        public static Type TextAssetType = typeof(TextAsset);
        public static Type AssetBundleType = typeof(AssetBundle);

        //保留对比的字典
         static Dictionary<Type, short> typeCodeDic = new Dictionary<Type, short>
        {
            {GameObjectType,GameObject}, {AudioClipType,AudioClip}, {SpriteType,Sprite},
            {SceneType,Scene}, {SpriteAtlasType,SpriteAtlas}, {MeshType,Mesh},
            {Texture2DType,Texture2D}, {TextAssetType,TextAsset}, 
            {AssetBundleType,AssetBundle}
        };

        //通过类型获取对应的类别
        public static short ToCode(this Type type)
        {
            return typeCodeDic[type];
        }
        //通过类别获取对应的类型
        public static Type ToType(this short code)
        {
            return typeCodeDic.FirstOrDefault(q=> 
                q.Value == code).Key;
        }
    }
    [SerializeField]
    public class AssetData
    {
        private string mAssetName;    //资源名字
        private string mOwnerBundleName;    //AB的名字
        private int mABIndex;    //AB资源的索引
        private short mAssetType;    //资源的类型
        private short mAssetObjectTypeCode = 0;    //资源的类别
        
        //无参构造函数
        public AssetData() { }
        
        /// 构造函数创建资源类
        /// <param name="assetName">资源名字</param>
        /// <param name="assetType">资源类型</param>
        /// <param name="abIndex">ab资源索引</param>
        /// <param name="ownerBundleName">ab名字</param>
        /// <param name="assetObjectTypeCode">资源类别</param>
        public AssetData(string assetName, short assetType, int abIndex, string ownerBundleName,
            short assetObjectTypeCode = 0)
        {
            mAssetName = assetName;
            mAssetType = assetType;
            mABIndex = abIndex;
            mOwnerBundleName = ownerBundleName;
            mAssetObjectTypeCode = assetObjectTypeCode;
        }
        
        //对外公开的访问和设置资源名字的方法
        public string AssetName
        {
            get { return mAssetName; }
            set { mAssetName = value; }
        }
        //对外公开的访问和设置资源AB的索引的方法
        public int AssetBundleIndex
        {
            get { return mABIndex; }
            set { mABIndex = value; }
        }
        //对外公开的访问和设置资源AB的名字的方法
        public string OwnerBundleName
        {
            get { return mOwnerBundleName; }
            set { mOwnerBundleName = value; }
        }
        //对外公开的访问和设置资源类别的方法
        public short AssetObjectTypeCode
        {
            get { return mAssetObjectTypeCode; }
            set { mAssetObjectTypeCode = value; }
        }
        //对外公开的访问和设置资源类型的方法
        public short AssetType
        {
            get { return mAssetType; }
            set { mAssetType = value; }
        }
        //获取资源的标识，利用AB名字+asset名字作为标识
        public string UUID
        {
            get
            {
                return string.IsNullOrEmpty(mOwnerBundleName)
                    ? AssetName
                    : OwnerBundleName + AssetName;
            }
        }
    }
}