using System.Collections.Generic;
using JetBrains.Annotations;
using LBFramework.Log;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LBFramework.LBEditor
{
    
    public class DefineSymbolsSetting : EditorModuleInterface
    {
        [MenuItem("LBTools/宏设置",false)]
        public static void Open()
        {
            OpenGUI<DefineSymbolsSetting>();
        }

        private bool isInit = false;
        private bool isChange = true;
        private const char Separator = ';';

        private void InitFilter()
        {
            filterList = new List<DefineFilter>()
            {
                new DefineFilter("是否使用打印日志","USE_LBLOGWRAPPER"),
                new DefineFilter("test2","TEST_02"),
                new DefineFilter("test3","TEST_03"),
                new DefineFilter("test4","TEST_04"),
                new DefineFilter("test5","TEST_05"),
                new DefineFilter("test6","TEST_06"),
                new DefineFilter("test7","TEST_07"),
            };
            string defineSymbolText = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string[] defineSymbols = defineSymbolText.Split(Separator);
            for (int j = 0; j < filterList.Count; j++)
            {
                DefineFilter filter = filterList[j];
                for (int i = 0; i < defineSymbols.Length; i++)
                {
                    string defineSymbol = defineSymbols[i];
                    if (defineSymbol == filter.value)
                    {
                        filter.valid = true;
                        break;
                    }
                }
            }
        }
        
        
        private string GetDefineSymbols()
        {
            string result = string.Empty;
            for (int j = 0; j < filterList.Count; j++)
            {
                DefineFilter filter = filterList[j];
                if (filter.valid)
                {
                    result = result + filter.value + Separator.ToString();
                }
            }
            return result;
        }

        public void OnGUI()
        {
            if (!isInit)
            {
                isInit = true;
                InitFilter();
            }
            
            if (mReordeList == null)
                InitFilterListDrawer();
            
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply", EditorStyles.toolbarButton))
                {
                    string defineSymbolText = GetDefineSymbols();
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbolText);
                }
            }
            GUILayout.EndHorizontal();
            DrawDefineFilterList();
            if (GUI.changed)
            {
                
            }
        }
        
        void InitFilterListDrawer()
        {
            this.mReordeList = new ReorderableList(filterList, typeof(DefineFilter));
            this.mReordeList.drawElementCallback = OnListElementGUI;
            this.mReordeList.drawHeaderCallback = OnListHeaderGUI;
            this.mReordeList.draggable = true;
            this.mReordeList.elementHeight = 22;
            this.mReordeList.onAddCallback = (list) => Add();
        }

        void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            const float GAP = 5;

            DefineFilter filter = filterList[index];
            rect.y++;

            Rect r = rect;
            r.width = 16;
            r.height = 18;
            filter.valid = GUI.Toggle(r, filter.valid, GUIContent.none);
            
            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 200;
            GUI.enabled = false;
            filter.name = GUI.TextField(r, filter.name);
            GUI.enabled = true;
        
            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            filter.value = GUI.TextField(r, filter.value);
        }
        
        void OnListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "请在DefineSymbolsSetting脚本里添加宏");
        }
        
        void Add()
        {
            var filter = new DefineFilter();
            filter.name = "名字不重要，直接后面写值";
            filterList.Add(filter);
        }
    }
}