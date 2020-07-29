using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LBFramework.AssemblyUtil
{
    public class AssemblyUtil
    {
        public static Assembly EditorAssembly
        {
            get
            {
                // 1.获取当前项目中所有的 assembly (可以理解为 代码编译好的 dll)
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                // 2.获取编辑器环境(dll)
                var editorAssembly =
                    assemblies.First(assembly => assembly.FullName.StartsWith("Assembly-CSharp-Editor"));

                return editorAssembly;
            }
        }
    }
}

