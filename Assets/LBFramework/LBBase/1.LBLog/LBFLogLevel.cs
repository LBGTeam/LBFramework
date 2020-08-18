using UnityEngine;

namespace LBFramework.LBUtils
{
    public enum LBFLogLevel : byte
    {
        Debug = 0,        //调试
        
        Info,            //信息
        
        Warning,         //警告
        
        Error,           //错误
        
        Fatal            //严重警告
    }
}