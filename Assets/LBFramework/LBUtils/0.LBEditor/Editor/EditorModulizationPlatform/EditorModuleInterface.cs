using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace LBFramework.LBEditor
{
    public abstract class LayoutComp
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
        public void DrawLayoutBtn(int columns,params LayoutComp[] layComps)
        {
            var i = 0;
            while (i < layComps.Length)
            {
                for (int j = 0; j < columns; j++)
                {
                    GUILayout.BeginHorizontal("box");
                    var laycom = layComps[i];
                    if (GUILayout.Button(laycom.label,GUILayout.Width(laycom.widthScale)))
                        layComps[i].onClick();
                    i++;
                }
            }
        }
    }
}