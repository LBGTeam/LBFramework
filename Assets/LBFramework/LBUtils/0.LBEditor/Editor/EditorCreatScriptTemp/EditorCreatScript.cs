using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace LBFramework.LBEditor
{
    public class EditorCreatScript
    {
        private static EditorCreatScript mInstacne; //EditorCreatScript脚本单例
        EditorCreatScript(){}    //通过private阻止外界创建单例
        public static EditorCreatScript Instance    //外界调用的EditorCreatScript单例接口
        {
            get
            {
                if (mInstacne==null)
                    mInstacne = new EditorCreatScript();
                return mInstacne;
            }
        }
        //获取选中的文件夹路径
        public string GetSelectedPathOrFallBack()
        {
            string path = "Assets";
            foreach (UnityEngine.Object obj  in Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
        //根据路径模板创建脚本,传入参数需要带模板文件格式后缀
        public void CreatScriptByTemp(string FilePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                    ScriptableObject.CreateInstance<CraetEventCSScriptAsset>(),
                    GetSelectedPathOrFallBack()+"/New Script.cs",
                    null,
                    FilePath         // 只需要在此文件夹下创建你需要的模板，命名与EventCSClass.cs一致就行
                );
        }
    }
    public //定义一个创建资源的Action类并实现其Action方法
        class CraetEventCSScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //创建资源
            UnityEngine.Object o = CreatScriptAssetFormTemplate(pathName, resourceFile);
            //高亮显示该资源
            ProjectWindowUtil.ShowCreatedAsset(o);
        }


        internal static UnityEngine.Object CreatScriptAssetFormTemplate(string pathName, string resourcesFile)
        {
            //获取创建资源的绝对路径
            string fullName = Path.GetFullPath(pathName);
            //读取本地模板
            StreamReader streamReader = new StreamReader(resourcesFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
                
            //获取文件名字
            string fileName = Path.GetFileNameWithoutExtension(pathName);
            //替换文件中的内容
            text = text.Replace("#NAME",fileName);

            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding uTF8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;

            //写入新文件
            StreamWriter streamWriter = new StreamWriter(fullName, append, uTF8Encoding);
            streamWriter.Write(text);
            streamWriter.Close();
                
            //刷新本地资源
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }
    }
}
