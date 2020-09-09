using System;
using System.Collections.Generic;
using System.Reflection;
using LBFramework.Log;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace LBFramework.LBEditor
{
    [InitializeOnLoad]
    public static class PrefabStageListener
    {
        private static Dictionary<Transform,int> oldPrefabTfDic = new Dictionary<Transform, int>();
        private static Dictionary<Transform,int> newPrefabTfDic = new Dictionary<Transform, int>();
        static PrefabStageListener()
        {
            // 打开Prefab编辑界面回调
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            // Prefab被保存之前回调
            PrefabStage.prefabSaving += OnPrefabSaving;
            // Prefab被保存之后回调
            PrefabStage.prefabSaved += OnPrefabSaved;
            // 关闭Prefab编辑界面回调
            //PrefabStage.prefabStageClosing += OnPrefabStageClosing;

            //PrefabStage.prefabStageDirtied += OnPrefabStageDirtied;

            //PrefabUtility.prefabInstanceUpdated += OnUpdate;
        }
        
        private static void OnPrefabSaved(GameObject prefab)
        {
            foreach (var prefabTf in oldPrefabTfDic)
            {
                if ((!newPrefabTfDic.ContainsKey(prefabTf.Key)) ||
                    oldPrefabTfDic[prefabTf.Key] != newPrefabTfDic[prefabTf.Key]) 
                {
                    Debug.LogError("预制体:"+PrefabUtility.GetNearestPrefabInstanceRoot(prefabTf.Key.gameObject)+"增加了物体，预制件不允许在外部添加物体，请尽快删除");
                }
            }
        }

        private static void OnPrefabSaving(GameObject prefab)
        {
            newPrefabTfDic.Clear();
            toClearConsole();
            LoadPrefab(prefab.transform, newPrefabTfDic);
        }

        private static void OnPrefabStageOpened(PrefabStage prefab)
        {
            oldPrefabTfDic.Clear();
            newPrefabTfDic.Clear();
            LoadPrefab(prefab.prefabContentsRoot.transform,oldPrefabTfDic);
        }
        /*private static void OnPrefabStageDirtied(PrefabStage obj)
        {
            
        }
        private static void OnUpdate(GameObject instance)
        {
            //Debug.LogError(instance.name);
            //Debug.LogError(PrefabUtility.GetObjectOverrides(instance.transform.GetChild(3).gameObject).Count);

        }
        private static void OnPrefabStageClosing(PrefabStage prefab)
        {
        }*/

        private static void LoadPrefab(Transform prefabTf, Dictionary<Transform,int> preDic)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(prefabTf.gameObject))
            {
                preDic.Add(prefabTf,prefabTf.childCount);
            }
            if (prefabTf.childCount > 0)
            {
                foreach (Transform child in prefabTf)
                {
                    LoadPrefab(child,preDic);
                }
            }
        }
        private static MethodInfo clearMethod = null;
        private static void toClearConsole()
        {
            if (clearMethod == null) 
            { 
                Type log = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
                clearMethod = log.GetMethod("Clear");
            }
            clearMethod.Invoke(null, null);
        }
    }
}