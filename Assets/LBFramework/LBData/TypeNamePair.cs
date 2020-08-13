using System;
using UnityEngine;

namespace LBFramework.LBData
{
    public struct TypeNamePair
    {
        private readonly Type mType;        //类型
        private readonly string mName;        //名字
        
        /// 构造函数初始化数据
        /// <param name="type">类型</param>
        /// <param name="name">名字</param>
        public TypeNamePair(Type type, string name)
        {
            mType = type;
            mName = name ?? string.Empty;
        }
        //构造函数只对类型进行初始化
        public TypeNamePair(Type type) : this(type, string.Empty){}
        
        //获取类型
        public Type type { get { return mType; } }
        //获取名字
        public string name { get { return mName; } }
        //获取类型和名字的组合字符串值
        public override string ToString()
        {
            if (mType == null)
            {
                return string.Empty;
            }
            string typeName = mType.FullName;
            return string.IsNullOrEmpty(mName) ? typeName : string.Format("{0}.{1}", typeName, mName);
        }
        //获取哈希值
        public override int GetHashCode()
        {
            return mType.GetHashCode() ^ mName.GetHashCode();
        }
        //类型进行比较
        public override bool Equals(object obj)
        {
            return obj is TypeNamePair && Equals((TypeNamePair)obj);
        }
        //类型进行比较
        public bool Equals(TypeNamePair value)
        {
            return mType == value.mType && mName == value.mName;
        }
        //判断值是否相同
        public static bool operator ==(TypeNamePair a, TypeNamePair b)
        {
            return a.Equals(b);
        }
        //判断值是否相同
        public static bool operator !=(TypeNamePair a, TypeNamePair b)
        {
            return !(a == b);
        }
    }
}