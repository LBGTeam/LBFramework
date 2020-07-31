using LBFramework.LBEditor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LBFramework.Nunit
{
    public class EditorCreanScriptTest : EditorCreatScript
    {
        [MenuItem("Assets/Create/LBFCShap",false,80)]
        public static void CreatLBFCShap()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<CraetEventCSScriptAsset>(),
                GetSelectedPathOrFallBack()+"/New Script.cs",
                null,
                "Assets/LBFramework/LBUnitTest/" +
                "Editor/CShapTemp/LBFCSTemp.txt"         // 只需要在此文件夹下创建你需要的模板，命名与EventCSClass.cs一致就行
            );

        }
    }
}