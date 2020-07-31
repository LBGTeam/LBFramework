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

        public LayoutComp(string label, Action onClick, float widthScale = 1)
        {
            this.label = label;
            this.onClick = onClick;
            this.widthScale = widthScale;
        }
    }
    public abstract class EditorModuleInterface : EditorWindow
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
        public void DrawLayoutBtn(int columns,params LayoutComp[] layComps)
        {
            var i = 0;
            while (i < layComps.Length)
            {
                GUILayout.BeginHorizontal("box");
                for (int j = 0; j < columns; j++)
                {
                    var laycom = layComps[i];
                    if (GUILayout.Button(laycom.label,GUILayout.Height(laycom.widthScale*100)))
                        layComps[i].onClick();
                    i++;
                    if (i >= layComps.Length) 
                        break;
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}