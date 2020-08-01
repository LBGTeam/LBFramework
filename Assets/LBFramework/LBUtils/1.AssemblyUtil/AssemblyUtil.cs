using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace LBFramework.LBUtils
{
    public class AssemblyUtil
    {
        public static Assembly EditorAssembly
        {
            get
            {
                // 1.获取当前项目中所有的 Editor assembly (可以理解为 代码编译好的 dll)
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                // 2.获取编辑器环境(dll)
                var editorAssembly =
                    assemblies.First(assembly => assembly.FullName.StartsWith("Assembly-CSharp-Editor"));

                return editorAssembly;
            }
        }

        public static Assembly CShapAssembly
        {
            get
            {
                // 1.获取当前项目中所有的 CShap assembly (可以理解为 代码编译好的 dll)
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                // 2.获取编辑器环境(dll)
                var cShapAssembly =
                    assemblies.First(assembly => assembly.FullName.StartsWith("Assembly-CSharp"));
                return cShapAssembly;
            }
        }

        public static List<Type> GetEditoeAssemblyInstance<Type>()
        {
            return EditorAssembly
                // 获取所有的编辑器环境中的类型 
                .GetTypes()
                // 过滤掉抽象类型（接口/抽象类)、和未实现 Type 的类型
                .Where(type => typeof(Type).IsAssignableFrom(type) && !type.IsAbstract)
                // 获取类型的构造创建实例
                .Select(type => type.GetConstructors().First().Invoke(null))
                // 转换成 List<Type>
                .ToList()
                .OfType<Type>()
                .ToList();
        }

        public static List<Type> GetCShapAssemblyInstance<Type>()
        {
            return CShapAssembly
                // 获取所有的编辑器环境中的类型 
                .GetTypes()
                // 过滤掉抽象类型（接口/抽象类)、和未实现 Type 的类型
                .Where(type => typeof(Type).IsAssignableFrom(type) && !type.IsAbstract)
                // 获取类型的构造创建实例
                .Select(type => type.GetConstructors().First().Invoke(null))
                // 转换成 List<Type>
                .ToList()
                .OfType<Type>()
                .ToList();
        }
    }
}

