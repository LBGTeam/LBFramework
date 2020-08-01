using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LBFramework.LBEditor
{
    public class EditorModulizationPlatformEditor : EditorWindow
    {
        // Start is called before the first frame update
        private EditorModuleContainer mContainer;

        /// <summary>
        /// 打开窗口
        /// </summary>
        /*[MenuItem("LBFramework/LBUtils/0.LBEditor")]
        public static void Open()
        {
            var editorPlatform = GetWindow<EditorModulizationPlatformEditor>();
            editorPlatform.position = new Rect(
                Screen.width / 2,
                Screen.height * 2 / 3,
                600,
                500
            );

            // 初始化 Container
            editorPlatform.mContainer = new EditorModuleContainer();

            editorPlatform.mContainer.Init();

            editorPlatform.Show();
        }*/

        private void OnGUI()
        {
            // 渲染
            mContainer.ResolveAll<IEditorPlatformModule>()
                .ForEach(editorPlatformModule => editorPlatformModule.OnGUI());
        }
    }
}
