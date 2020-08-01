using LBFramework.LBEditor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LBFramework.NUnitTest
{
    public class EditorCreanScriptTest
    {
        //模板存放路径
        public static string TempPath = "Assets/LBFramework/LBUnitTest/Editor/CShapTemp/";
        
        //创建框架普通类的脚本
        [MenuItem("Assets/Create/C# LBFScript",false,80)]
        public static void CreatLBFCShap()
        {
            EditorCreatScript.Instance.CreatScriptByTemp(TempPath+"LBFClassCS.txt");
        }
        
        //创建框架静态类的脚本
        [MenuItem("Assets/Create/C# Static LBFScript",false,80)]
        public static void CreatLBFCShapStaticScript()
        {
            EditorCreatScript.Instance.CreatScriptByTemp(TempPath+"LBFStaticCS.txt");
        }
    }
}