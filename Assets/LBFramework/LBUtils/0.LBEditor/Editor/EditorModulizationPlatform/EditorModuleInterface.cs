using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace LBFramework.LBEditor
{
    public struct LayoutComp
    {
        public string label;
        public Action onClick;
        public float widthScale;
        public float hightScale;

        public LayoutComp(string label, Action onClick, float widthScale = 100,float hightScale = 20)
        {
            this.label = label;
            this.onClick = onClick;
            this.widthScale = widthScale;
            this.hightScale = hightScale;
        }
    }
    public abstract class EditorModuleInterface : EditorWindow,IEditorPlatformModule
    {
        private EditorModuleContainer mContainer;
        
        public static void OpenGUI<Type>() where Type : EditorModuleInterface
        {
            var editorPlatform = GetWindow<Type>();
            editorPlatform.position = new Rect(
                Screen.width / 2,
                Screen.height * 2 / 3,
                600,
                500
            );
            
            if (editorPlatform == null)
            {
                // 初始化 Container
                editorPlatform.mContainer = new EditorModuleContainer();
                editorPlatform.mContainer.Init();
            }
            
            editorPlatform.Show();
        }
        public void DrawLayoutHorizontalBtn(int columns,params LayoutComp[] layComps)
        {
            var i = 0;
            while (i < layComps.Length)
            {
                GUILayout.BeginHorizontal("box");
                for (int j = 0; j < columns; j++)
                {
                    var laycom = layComps[i];
                    if (GUILayout.Button(laycom.label,GUILayout.Height(laycom.hightScale)))
                        layComps[i].onClick();
                    i++;
                    if (i >= layComps.Length) 
                        break;
                }
                GUILayout.EndHorizontal();
            }
        }

        public void OnGUI()
        {
        }
    }
}