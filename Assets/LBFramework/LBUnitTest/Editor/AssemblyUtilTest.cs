using UnityEditor;
using UnityEngine;
using LBFramework.LBEditor;

namespace LBFramework.Nunit
{
    public class AssemblyUtilTest:EditorModuleInterface,IEditorPlatformModule
    {
        [MenuItem("LBFramework/LBUtils/AssemblyUtilTest")]
        public static void Open()
        {
            OpenGUI<AssemblyUtilTest>();
        }
        public void OnGUI()
        {
            DrawLayoutHorizontalBtn(3,
                new LayoutComp("LianBai_01",()=>{Debug.LogError("LianBai_01");}),
                new LayoutComp("LianBai_02",()=>{Debug.LogError("LianBai_02");}),
                new LayoutComp("LianBai_03",()=>{Debug.LogError("LianBai_03");}),
                new LayoutComp("LianBai_04",()=>{Debug.LogError("LianBai_04");}),
                new LayoutComp("LianBai_05",()=>{Debug.LogError("LianBai_05");})
                );
        }
    }
    public class AssemblyUtilTest2:EditorModuleInterface,IEditorPlatformModule
    {
        [MenuItem("LBFramework/LBUtils/AssemblyUtilTest2")]
        public static void Open()
        {
            OpenGUI<AssemblyUtilTest2>();
        }
        public void OnGUI()
        {
            DrawLayoutHorizontalBtn(3,
                new LayoutComp("LianBai_01",()=>{Debug.LogError("LianBai_01");}),
                new LayoutComp("LianBai_02",()=>{Debug.LogError("LianBai_02");}),
                new LayoutComp("LianBai_03",()=>{Debug.LogError("LianBai_03");}),
                new LayoutComp("LianBai_04",()=>{Debug.LogError("LianBai_04");}),
                new LayoutComp("LianBai_05",()=>{Debug.LogError("LianBai_05");}),
                new LayoutComp("LianBai_06",()=>{Debug.LogError("LianBai_06");})
            );
        }
    }
}