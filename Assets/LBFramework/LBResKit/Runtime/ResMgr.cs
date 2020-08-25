using System;
using System.Collections;
using System.Collections.Generic;
using LBFramework.LBUtils;
using UnityEditor.XR;
using UnityEngine;

namespace LBFramework.ResKit
{
    [MonoSingletonPath("[Framework]/ResMgr")]
    public class ResMgr : MonoSingleton<ResMgr>
    {
        #region StaticInit

        private static bool mResMgrInited = false;    //标记资源管理器是否被初始化

        //初始化bin文件
        public static void Init()
        {
            if (mResMgrInited) return;    //如果已经初始化就不用初始化了
            mResMgrInited = true;         //标记已经初始化
            
            //对象池初始化AssetBundle资源
            SafeObjectPool<AssetBundleRes>.Instance.Init(40,20);
            //对象池初始化普通资源
            SafeObjectPool<AssetRes>.Instance.Init(40,20);
            //对象池初始化Resources资源
            SafeObjectPool<ResourcesRes>.Instance.Init(40,20);
            //对象池初始化网络图片资源
            SafeObjectPool<NetImageRes>.Instance.Init(40, 20);
            //对象池初始化资关键字资源对象池
            SafeObjectPool<ResSearchKeys>.Instance.Init(40, 20);
            //对象池初始化资源加载者
            SafeObjectPool<ResLoader>.Instance.Init(40, 20);
            
            //初始化资源管理器
            Instance.InitResMgr();
        }
        
        //异步初始化bin文件
        public static IEnumerator InitAsync()
        {
            if (mResMgrInited) yield break;    //如果已经初始化就不用初始化了
            mResMgrInited = true;                //标记已经初始化

            //对象池初始化AssetBundle资源
            SafeObjectPool<AssetBundleRes>.Instance.Init(40,20);
            //对象池初始化普通资源
            SafeObjectPool<AssetRes>.Instance.Init(40,20);
            //对象池初始化Resources资源
            SafeObjectPool<ResourcesRes>.Instance.Init(40,20);
            //对象池初始化网络图片资源
            SafeObjectPool<NetImageRes>.Instance.Init(40, 20);
            //对象池初始化资关键字资源对象池
            SafeObjectPool<ResSearchKeys>.Instance.Init(40, 20);
            //对象池初始化资源加载者
            SafeObjectPool<ResLoader>.Instance.Init(40, 20);

            //异步初始化资源管理器
            yield return Instance.InitResMgrAsync();
        }

        #endregion

        //资源存放表
        private ResTable mTable = new ResTable();
        //当前携程的数量
        [SerializeField] private int mCurrentCoroutineCount;
        private int mMaxCoroutineCount = 8;    //最大携程数量(最快携程大概在6~8)
        //携程储存的列表
        private LinkedList<IEnumeratorTask> mIEnumeratorTaskStack = new LinkedList<IEnumeratorTask>();

        //ResMgr定时收集列表中的Res然后删除
        private bool mIsResMapDirty;

        //异步初始化资源管理器
        public IEnumerator InitResMgrAsync()
        {
#if UNITY_EDITOR
            //编辑器模式下判断是否使用AB
            if (AssetBundleSettings.SimulateAssetBundleInEditor)
            {
                AssetBundleSettings.AssetBundleConfigFile = EditorRuntimeAssetDataCollector.BuildDataTable();
                yield return null;
            }
            else
#endif
            {
                //AB配置文件重置
                AssetBundleSettings.AssetBundleConfigFile.Reset();
                //资源地址列表
                var outResult = new List<string>();
                //临时变量保存地址
                string pathPrefix = String.Empty;
#if UNITY_EDITOR || UNITY_IOS
                //编辑器或者IOS环境下加上固定的头
                pathPrefix = "file://";
#endif
                // 未进行过热更
                if (AssetBundleSettings.LoadAssetResFromStreammingAssetsPath)
                {
                    //获取资源的地址
                    string streamingPath = Application.streamingAssetsPath + "/AssetBundles/" +
                                           AssetBundleSettings.GetPlatformName() + "/" + 
                                           AssetBundleSettings.AssetBundleConfigFile.FileName;
                    //添加资源的地址
                    outResult.Add(pathPrefix + streamingPath);
                }
                // 进行过热更
                else
                {
                    //获取资源的地址
                    string persistenPath = Application.persistentDataPath + "/AssetBundles/" +
                                           AssetBundleSettings.GetPlatformName() + "/" + 
                                           AssetBundleSettings.AssetBundleConfigFile.FileName;
                    //添加资源的地址
                    outResult.Add(pathPrefix + persistenPath);
                }
                foreach (var outRes in outResult)
                {
                    yield return AssetBundleSettings.AssetBundleConfigFile.LoadFromFileAsync(outRes);
                }
                yield return null;
            }
        }
        
        //初始化资源管理器
        public void InitResMgr()
        {
#if UNITY_EDITOR
            if (AssetBundleSettings.SimulateAssetBundleInEditor)
            {
                AssetBundleSettings.AssetBundleConfigFile = EditorRuntimeAssetDataCollector.BuildDataTable();
            }
            else
#endif
            {
                //AB配置文件重置
                AssetBundleSettings.AssetBundleConfigFile.Reset();
                //资源地址列表
                var outResult = new List<string>();

                // 未进行过热更
                if (AssetBundleSettings.LoadAssetResFromStreammingAssetsPath)
                    FileMgr.Instance.GetFileInInner(AssetBundleSettings.AssetBundleConfigFile.FileName, outResult);
                // 进行过热更
                else
                    FilePath.GetFileInFolder(FilePath.PersistentDataPath, 
                        AssetBundleSettings.AssetBundleConfigFile.FileName, outResult);
                foreach (var outRes in outResult)
                    AssetBundleSettings.AssetBundleConfigFile.LoadFromFile(outRes);
            }
        }
        
        //资源管理器清理并且更新
        public void ClearOnUpdate()
        {
            //将资源更新标记为true，表示规定事件进行删除清理
            mIsResMapDirty = true;
        }
        //添加携程任务
        public void PushIEnumeratorTask(IEnumeratorTask task)
        {
            if (task == null)    //如果携程为空就不添加
                return;
            //携程列表添加任务
            mIEnumeratorTaskStack.AddLast(task);
            TryStartNextIEnumeratorTask();
        }
        
        /// 获取资源
        /// <param name="resSearchKeys">关键字资源对象</param>
        /// <param name="createNew">是否创建一个新的</param>
        public IRes GetRes(ResSearchKeys resSearchKeys, bool createNew = false)
        {
            //先从资源表里获取资源
            var res = mTable.GetResBySearchKeys(resSearchKeys);
            //如果从资源表获取到资源直接返回资源
            if (res != null)
                return res;
            //如果资源表里没有资源并且不创建新的资源就直接返回空
            if (!createNew)
                return null;
            //利用资源工厂创建新的资源
            res = ResFactory.Create(resSearchKeys);
            //如果创建的资源不是空的就添加到资源表里
            if (res != null)
                mTable.Add(res);
            //返回创建的资源
            return res;
        }

        private void Update()
        {
            if (mIsResMapDirty)    //如果需要更新资源
            {
                RemoveUnusedRes();    //移除所有未被使用的资源
            }
        }
        
        //移除所有未被使用的资源
        private void RemoveUnusedRes()
        {
            if (!mIsResMapDirty)    //如果标记不需要更新资源直接返回
                return;
            //标记资源不需要更新
            mIsResMapDirty = false;
            //遍历所有的资源
            foreach (var res in mTable.ToArray())
            {
                //如果资源引用数量小于0并且资源不是处于正在加载的状态
                if (res.RefCount <= 0 && res.State != ResState.Loading)
                {
                    //释放资源并判断资源是否释放成功
                    if (res.ReleaseRes())
                    {
                        mTable.Remove(res);    //表里移除资源
                        res.Recycle2Cache();    //资源的对象池进行回收
                    }
                }
            }
        }

        /// 获取T类型的资源
        /// <param name="resSearchKeys">关键字资源对象</param>
        public T GetRes<T>(ResSearchKeys resSearchKeys) where T : class, IRes
        {
            return GetRes(resSearchKeys) as T;
        }
        
        //尝试开启下一个携程
        private void TryStartNextIEnumeratorTask()
        {
            if (mIEnumeratorTaskStack.Count == 0)    //如果携程数量为0则直接返回
                return;
            if (mCurrentCoroutineCount >= mMaxCoroutineCount)    //如果当前执行携程数量大于最大执行携程数也直接返回，先不执行
                return;

            var task = mIEnumeratorTaskStack.First.Value;    //获取携程列表里的第一个协亨
            mIEnumeratorTaskStack.RemoveFirst();    //移除携程列表第一个携程，因为地下要执行

            ++mCurrentCoroutineCount;    //当前执行携程数量+1
            StartCoroutine(task.DoLoadAsync(OnIEnumeratorTaskFinish));    //执行刚才保留的第一个携程
        }
        
        //携程执行结束响应的事件
        private void OnIEnumeratorTaskFinish()
        {
            --mCurrentCoroutineCount;    //当前执行携程数量-1
            TryStartNextIEnumeratorTask();    //尝试是否可以执行下一个携程
        }
        
        //编辑器扩展显示资源信息
        private void OnGUI()
        {
            if (Platform.IsEditor && Input.GetKey(KeyCode.F1))
            {
                GUILayout.BeginVertical("box");

                GUILayout.Label("ResKit", new GUIStyle {fontSize = 30});
                GUILayout.Space(10);
                GUILayout.Label("ResInfo", new GUIStyle {fontSize = 20});
                mTable.ToList().ForEach(res => { GUILayout.Label((res as Res).ToString()); });
                GUILayout.Space(10);

                GUILayout.Label("Pools", new GUIStyle() {fontSize = 20});
                GUILayout.Label(string.Format("ResSearchRule:{0}",
                    SafeObjectPool<ResSearchKeys>.Instance.CurCount));
                GUILayout.Label(string.Format("ResLoader:{0}",
                    SafeObjectPool<ResLoader>.Instance.CurCount));
                GUILayout.EndVertical();
            }
        }
    }
}