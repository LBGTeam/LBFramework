using UnityEditor;
using UnityEngine;
using LBFramework.LBEditor;

namespace LBFramework.Nunit
{
    public class AssemblyUtilTest:IEditorPlatformModule
    {
        public void OnGUI()
        {
            if(GUILayout.Button("AssemblyUtilTest"))
                Debug.LogError(AssemblyUtil.EditorAssembly);
        }
    }
}