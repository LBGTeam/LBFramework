using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
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

    [System.Serializable]
    public class DefineFilter
    {
        public bool valid = false;
        public string name = string.Empty;
        public string value = String.Empty;
        
        public DefineFilter() { }
        public DefineFilter(string name = "名字未进行初始化", string value = "请输入正确的值")
        {
            this.name = name;
            this.value = value;
        }
    }
    public abstract class EditorModuleInterface : EditorWindow,IEditorPlatformModule
    {
        private EditorModuleContainer mContainer;
        protected ReorderableList mReordeList;
        protected List<DefineFilter> filterList;
        
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

        public void DrawDefineFilterList()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(Vector2.zero);
            mReordeList.DoLayoutList();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            
            if (GUI.changed)
            {
                
            }
        }
        
        
        

        public void OnGUI()
        {
        }
    }
}