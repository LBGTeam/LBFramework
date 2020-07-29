using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBFramework.LBEditor
{
    public class LBEditorTest:IEditorPlatformModule
    {
        public void OnGUI()
        {
            GUILayout.Label("这个是一个新的模块", new GUIStyle()
            {
                fontSize = 30
            });
            GUILayout.Button("LianBai");
        }
    }
}

