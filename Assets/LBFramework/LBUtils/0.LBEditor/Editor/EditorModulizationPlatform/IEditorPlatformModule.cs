using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBFramework.LBEditor
{
    public interface IEditorPlatformModule
    {
        /// <summary>
        /// 渲染 IMGUI
        /// </summary>
        void OnGUI();
    }
}

