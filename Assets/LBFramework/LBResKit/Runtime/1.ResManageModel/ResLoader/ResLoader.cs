using System;
using System.Collections.Generic;
using LBFramework.LBUtils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LBFramework.ResKit
{
    public class ResLoader : DisposableObject,IResLoader
    {
        private readonly List<IRes> mResList = new List<IRes>();    //资源列表
        private readonly LinkedList<IRes> mWaitLoadList = new LinkedList<IRes>();    //等待加载资源列表
        private Action mListener;    //监听事件
        private int mLoadingCount;    //正在加载资源个数
        private LinkedList<CallBackWrap> mCallbackRecordList;    //响应返回记录列表
        bool IPoolable.IsRecycled { get; set; }    //标记对象池是否已经回收
        
#if UNITY_EDITOR
        //编辑器模式下存放Sprite资源
        private readonly Dictionary<string, Sprite> mCachedSpriteDict = new Dictionary<string, Sprite>();
#endif
        //无参构造函数
        public ResLoader()
        {
        }
        
        //对象池申请一个资源加载器
        public static ResLoader Allocate()
        {
            //返回安全对象池申请的一个对象
            return  SafeObjectPool<ResLoader>.Instance.Allocate();
        }
        
        //释放申请的资源加载器
        public void Recycle2Cache()
        {
            SafeObjectPool<ResLoader>.Instance.Recycle(this);
        }
        
        /// 同步加载AssetBundle里的资源
        /// <param name="ownerBundle">AssetBundle名字</param>
        /// <param name="assetName">资源名字</param>
        /// <typeparam name="T">资源类型</typeparam>
        public T LoadSync<T>(string ownerBundle, string assetName) where T : Object
        {
            //获取资源的关键字
            var resSearchKeys = ResSearchKeys.Allocate(assetName,ownerBundle,typeof(T));
            //通过关键字加载出来资源
            var retAsset = LoadResSync(resSearchKeys);
            resSearchKeys.Recycle2Cache();    //对象池回收进缓存
            return retAsset.Asset as T;        //返回资源
        }
        
        /// 同步加载AssetBundle里的资源
        /// <param name="assetName">资源名字</param>
        /// <typeparam name="T">资源类型</typeparam>
        public T LoadSync<T>(string assetName) where T : Object
        {
            //获取资源的关键字
            var resSearchKeys = ResSearchKeys.Allocate(assetName, null, typeof(T));
            //通过关键字加载出来资源
            var retAsset = LoadResSync(resSearchKeys);
            resSearchKeys.Recycle2Cache();    //对象池回收进缓存
            return retAsset.Asset as T;        //返回资源
        }
        
        /// 通过名字同步加载资源
        /// <param name="name">资源名字</param>
        public Object LoadSync(string name)
        {
            //获取资源的关键字(名字)
            var resSearchRule = ResSearchKeys.Allocate(name);
            //通过关键字(名字)加载出来资源
            var retAsset = LoadResSync(resSearchRule);
            resSearchRule.Recycle2Cache();    //对象池回收进缓存
            return retAsset.Asset;        //返回资源
        }
        
        /// 通过关键字同步加载资源
        /// <param name="resSearchKeys">资源关键字</param>
        public IRes LoadResSync(ResSearchKeys resSearchKeys)
        {
            AddLoad(resSearchKeys);
            LoadSync();
            var res = ResMgr.Instance.GetRes(resSearchKeys, false);
            if (res == null)
                return null;
            return res;
        }
        
        //同步架子啊资源
        private void LoadSync()
        {
            while (mWaitLoadList.Count > 0)        //判断是否有等在加载的资源
            {
                var first = mWaitLoadList.First.Value;    //获取第一个需要加载的资源
                --mLoadingCount;    //正在加载的数量减去，接下来会加载
                mWaitLoadList.RemoveFirst();       //移除第一个等待加载的资源，要准备加载第一个资源
                if (first == null)
                    return;
                if (first.LoadSync())        //加载等待加载资源中的第一个资源
                {
                }
            }
        }
        
        //获取资源加载的进度
        public float Progress
        {
            get
            {
                if (mWaitLoadList.Count == 0)    //如果没有资源加载则直接完成返回1
                    return 1;
                var unit = 1.0f / mResList.Count;    //计算1个资源所占的百分比
                //总资源数-正在加载资源数就是已经加载完成的资源数，在*一个资源的百分比就是加载进度了
                var currentValue = unit * (mResList.Count - mLoadingCount);
                var currentNode = mWaitLoadList.First;    //获取等待加载资源第一个
                while (currentNode != null)    //循环遍历未加载的资源，
                {
                    currentValue += unit * currentNode.Value.Progress;    //如果有加载进度就累加上去
                    currentNode = currentNode.Next;    //遍历到下一个资源
                }
                return currentValue;    //返回资源加载的进度
            }
        }
        
        /// 添加进加载资源
        /// <param name="list">资源的名字列表</param>
        public void AddLoad(List<string> list)
        {
            if (list == null)
                return;
            for (var i = list.Count - 1; i >= 0; --i)
            {
                //通过名字创建资源的关键字资源
                var resSearchRule = ResSearchKeys.Allocate(list[i]);
                AddLoad(resSearchRule);
                //对象池回收进缓存
                resSearchRule.Recycle2Cache();
            }
        }
        
        /// 添加进加载资源
        /// <param name="assetName">资源的名字</param>
        /// <param name="listener">加载资源监听事件</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        public void AddLoad(string assetName, Action<bool, IRes> listener, 
            bool lastOrder = true)
        {
            //通过名字获取加载资源的关键字
            var searchRule = ResSearchKeys.Allocate(assetName);
            //添加进加载资源
            AddLoad(searchRule,listener,lastOrder);
            //对象池回收进缓存
            searchRule.Recycle2Cache();    
        }
        
        /// 添加进加载资源
        /// <param name="assetName">资源名字</param>
        /// <param name="listener">加载资源监听事件</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void AddLoad<T>(string assetName, Action<bool, IRes> listener, bool lastOrder = true)
        {
            //通过名字获取加载资源的关键字
            var searchRule = ResSearchKeys.Allocate(assetName,null,typeof(T));
            //添加进加载资源
            AddLoad(searchRule,listener,lastOrder);
            //对象池回收进缓存
            searchRule.Recycle2Cache();
        }
        
        /// 添加进加载资源
        /// <param name="ownerBundle">AB的名字</param>
        /// <param name="assetName">资源的名字</param>
        /// <param name="listener">加载资源监听事件</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        public void AddLoad(string ownerBundle, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var searchRule = ResSearchKeys.Allocate(assetName,ownerBundle);

            AddLoad(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }
        
        /// 添加进加载资源
        /// <param name="ownerBundle">AB的名字</param>
        /// <param name="assetName">资源的名字</param>
        /// <param name="listener">加载资源监听事件</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        /// <typeparam name="T">资源的类型</typeparam>
        public void AddLoad<T>(string ownerBundle, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var searchRule = ResSearchKeys.Allocate(assetName,ownerBundle,typeof(T));
            AddLoad(searchRule, listener, lastOrder);
            searchRule.Recycle2Cache();
        }
        
        /// 添加进资源加载
        /// <param name="resSearchKeys">关键字资源</param>
        /// <param name="listener">监听事件</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        private void AddLoad(ResSearchKeys resSearchKeys, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            var res = FindResInArray(mResList, resSearchKeys);    //从资源组获取资源
            if (res != null)
            {
                if (listener != null)
                {
                    AddResListenerRecord(res, listener);    //添加事件进事件记录列表
                    res.RegisteOnResLoadDoneEvent(listener);    //注册资源加载完成事件
                }
                return;
            }
            //获取资源
            res = ResMgr.Instance.GetRes(resSearchKeys, true);
            if (res == null)
            {
                return;
            }
            if (listener != null)
            {
                AddResListenerRecord(res, listener);    //添加事件进事件记录列表
                res.RegisteOnResLoadDoneEvent(listener);    //注册资源加载完成事件
            }
            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            var depends = res.GetDependResList();
            if (depends != null)
            {
                foreach (var depend in depends)
                {
                    //对象池申请一个关键字资源
                    var searchRule = ResSearchKeys.Allocate(depend,null,typeof(AssetBundle));
                    //将关键字资源添加进加载的资源
                    AddLoad(searchRule);
                    //释放对象
                    searchRule.Recycle2Cache();
                }
            }
            //添加资源进资源组
            AddResArray(res, lastOrder);
        }
        
        //从资源组里查找资源
        private static IRes FindResInArray(List<IRes> list, ResSearchKeys resSearchKeys)
        {
            if (list == null)    //如果资源组为null则返回空
                return null;
            for (var i = list.Count - 1; i >= 0; --i)    //遍历资源组
                if (resSearchKeys.MatchRes(list[i]))    //判断资源是否匹配
                    return list[i];                    //如果匹配则返回资源
            return null;
        }
        //将事件添加资源监听记录列表
        private void AddResListenerRecord(IRes res, Action<bool, IRes> listener)
        {
            //如果事件记录为null
            if (mCallbackRecordList == null)
            {
                //初始化创建一个列表
                mCallbackRecordList = new LinkedList<CallBackWrap>();
            }
            //将事件添加的事件监听记录列表
            mCallbackRecordList.AddLast(new CallBackWrap(res, listener));
        }
        //添加进资源组
        private void AddResArray(IRes res, bool lastOrder)
        {
            //申请一个关键字资源
            var searchRule = ResSearchKeys.Allocate(res.AssetName,res.OwnerBundleName,res.AssetType);
            //再次确保队列中没有它并获取该资源
            var oldRes = FindResInArray(mResList, searchRule);
            //回收关键字资源
            searchRule.Recycle2Cache();
            if (oldRes != null)
            {
                return;
            }
            res.Retain();    //资源进行增加计数
            mResList.Add(res);    //资源列表添加资源

            if (res.State != ResState.Ready)    //如果资源不处于加载完成的准备状态
            {
                ++mLoadingCount;            //正在加载的资源计数+1
                if (lastOrder)            //是否是不着急可以晚点加载的资源
                {
                    mWaitLoadList.AddLast(res);    //放在等待加载列表的后面
                }
                else
                {
                    mWaitLoadList.AddFirst(res);    //放在等待加载列表的前面
                }
            }
        }
        
        /// 加载Sprite资源
        /// <param name="bundleName">AB的名字</param>
        /// <param name="spriteName">sprite的名字</param>
        public Sprite LoadSprite(string bundleName, string spriteName)
        {
#if UNITY_EDITOR
            //编辑器模式下判断是否使用bundle
            if (AssetBundleSettings.SimulateAssetBundleInEditor)
            {
                //判断缓存的字典里面是否有
                if (mCachedSpriteDict.ContainsKey(spriteName))
                {
                    //如果有的话直接返回资源
                    return mCachedSpriteDict[spriteName];
                }
                //同步加载图片
                var texture = LoadSync<Texture2D>(bundleName, spriteName);
                //创建对应的sprite
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f);
                //将加载出来的sprite添加进字典中
                mCachedSpriteDict.Add(spriteName, sprite);
                //返回sprite
                return mCachedSpriteDict[spriteName];
            }
#endif
            //同步加载资源
            return LoadSync<Sprite>(bundleName, spriteName);
        }
        public Sprite LoadSprite(string spriteName)
        {
#if UNITY_EDITOR
            //编辑器模式下判断是否使用bundle
            if (AssetBundleSettings.SimulateAssetBundleInEditor)
            {
                //判断缓存的字典里面是否有
                if (mCachedSpriteDict.ContainsKey(spriteName))
                {
                    //如果有的话直接返回资源
                    return mCachedSpriteDict[spriteName];
                }
                //同步加载图片
                var texture = LoadSync(spriteName) as Texture2D;
                //创建对应的sprite
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f);
                //将加载出来的sprite添加进字典中
                mCachedSpriteDict.Add(spriteName, sprite);
                //返回sprite
                return mCachedSpriteDict[spriteName];
            }
#endif
            return LoadSync<Sprite>(spriteName);
        }
        
        //异步加载资源，listener加载资源监听事件
        public void LoadAsync(Action listener = null)
        {
            mListener = listener;
            DoLoadAsync();
        }
        
        /// 释放资源
        /// <param name="resName">资源的名字</param>
        public void ReleaseRes(string resName)
        {
            //如果资源名字为空，直接返回，不需要释放
            if (string.IsNullOrEmpty(resName))
                return;
#if UNITY_EDITOR
            //编辑器模式下判断是否使用bundle
            if (AssetBundleSettings.SimulateAssetBundleInEditor)
            {
                //判断缓存的字典里面是否有
                if (mCachedSpriteDict.ContainsKey(resName))
                {
                    //获取对应的资源
                    var sprite = mCachedSpriteDict[resName];
                    //删除对应的资源
                    GameObject.Destroy(sprite);
                    //字典里清楚已经删除的资源
                    mCachedSpriteDict.Remove(resName);
                }
            }
#endif
            //根据名字申请关键字资源对象
            var resSearchRule = ResSearchKeys.Allocate(resName);
            //根据关键字资源对象找到对应的资源
            var res = ResMgr.Instance.GetRes(resSearchRule);
            //对象池回收关键字资源对象
            resSearchRule.Recycle2Cache();
            //如果资源为空直接返回
            if (res == null)
                return;
            //如果等待加载的资源中存在资源，移除等待加载的资源中的资源
            if (mWaitLoadList.Remove(res))
            {
                --mLoadingCount;        //正在加载的资源数量-1
                if (mLoadingCount == 0)    //如果正在加载的资源数量为0
                    mListener = null;      //监听事件可以清空
            }
            //如果加载的资源中存在资源
            if (mResList.Remove(res))
            {
                //移除加载资源完成事件
                res.UnRegisteOnResLoadDoneEvent(OnResLoadFinish);
                //资源进行释放
                res.Release();
                //资源架子啊管理者清理并进行更新
                ResMgr.Instance.ClearOnUpdate();
            }
        }
        
        /// 释放资源
        /// <param name="names">需要释放的**资源名字组**</param>
        public void ReleaseRes(string[] names)
        {
            //判断资源名字组是否为空或者内容为0
            if (names == null || names.Length == 0)
                return;
            //遍历资源名字组合
            for (var i = names.Length - 1; i >= 0; --i)
            {
                ReleaseRes(names[i]);    //根据名字释放资源
            }
        }

        //释放所有的资源
        public void ReleaseAllRes()
        {
#if UNITY_EDITOR
            //编辑器模式下判断是否使用bundle
            if (AssetBundleSettings.SimulateAssetBundleInEditor)
            {
                //遍历Sprite字典
                foreach (var spritePair in mCachedSpriteDict)
                {
                    //销毁字典中的资源
                    GameObject.Destroy(spritePair.Value);
                }
                //清空字典
                mCachedSpriteDict.Clear();
            } 
#endif
            mListener = null;    //监听的事件清空
            mLoadingCount = 0;    //正在加载的资源数量清空
            mWaitLoadList.Clear();    //等待加载的资源清空
            //如果加载的资源不为空
            if (mResList.Count > 0)
            {
                //确保首先删除的是AB，这样能对Asset的卸载做优化
                mResList.Reverse();
                //遍历加载的资源
                for (var i = mResList.Count - 1; i >= 0; --i)
                {
                    //移除资源监听的事件
                    mResList[i].UnRegisteOnResLoadDoneEvent(OnResLoadFinish);
                    //资源计数-1
                    mResList[i].Release();
                }
                mResList.Clear();    //清空资源列表
                //如果不是程序退出
                if (!ResMgr.IsApplicationQuit)
                {
                    //资源清理并进行更新
                    ResMgr.Instance.ClearOnUpdate();
                }
            }
            //移除所有的事件
            RemoveAllCallbacks(true);
        }
        
        //不加载所有实例过的资源
        public void UnloadAllInstantiateRes(bool flag)
        {
            //如果资源列表存在资源
            if (mResList.Count > 0)
            {
                //遍历资源列表里的资源
                for (var i = mResList.Count - 1; i >= 0; --i)
                {
                    //标记资源是否加载图片
                    if (mResList[i].UnloadImage(flag))
                    {
                        //等待加载资源列表里移除本资源
                        if (mWaitLoadList.Remove(mResList[i]))
                            --mLoadingCount;        //正在加载资源的数量-1
                        //移除所有的事件
                        RemoveCallback(mResList[i], true);
                        //移除资源监听的完成事件
                        mResList[i].UnRegisteOnResLoadDoneEvent(OnResLoadFinish);
                        //资源的计数-1
                        mResList[i].Release();
                        //根据索引删除该资源
                        mResList.RemoveAt(i);
                    }
                }
                //资源管理清理并进行更新
                ResMgr.Instance.ClearOnUpdate();
            }
        }
        
        //继承清理缓存的事件
        public override void Dispose()
        {
            //释放所有的资源
            ReleaseAllRes();
            base.Dispose();
        }

        public string ToString()
        {
            string str = String.Empty;
            foreach (var res in mResList)
            {
                str = str + res.AssetName + "/n";
            }
            return str;
        }

        //执行异步加载资源
        private void DoLoadAsync()
        {
            //如果正在加载的资源数量为0
            if (mLoadingCount == 0)
            {
                //如果加载监听事件部位空
                if (mListener != null)
                {
                    //获取资源监听事件
                    var callback = mListener;
                    //清空资源监听事件
                    mListener = null;
                    //执行刚才临时存的监听事件
                    callback();
                }
                return;
            }
            //获取正在加载的第一个资源
            var nextNode = mWaitLoadList.First;
            //创建一个临时的资源列表的节点
            LinkedListNode<IRes> currentNode = null;
            //如果第一个资源不是空接循环执行
            while (nextNode != null)
            {
                //临时节点缓存第一个节点
                currentNode = nextNode;
                //获取第一个节点的资源
                var res = currentNode.Value;
                //如果依赖的资源都已经加载完毕
                if (res.IsDependResLoadFinish())
                {
                    //等待加载的资源移除当前资源
                    mWaitLoadList.Remove(currentNode);
                    //如果资源不是已经加载完成的状态
                    if (res.State != ResState.Ready)
                    {
                        //注册资源加载完成的事件
                        res.RegisteOnResLoadDoneEvent(OnResLoadFinish);
                        //异步加载资源
                        res.LoadAsync();
                    }
                    else
                    {
                        //如果已经加载完成，则正在加载的资源数量-1
                        --mLoadingCount;
                    }
                }
            }
        }
        
        /// 移除响应的资源
        /// <param name="res">资源</param>
        /// <param name="release">是否释放</param>
        private void RemoveCallback(IRes res, bool release)
        {
            //如果记录的资源加载响应事件不为空
            if (mCallbackRecordList != null)
            {
                //获取当前加载资源的第一个
                var current = mCallbackRecordList.First;
                //临时变量资源的节点
                LinkedListNode<CallBackWrap> next = null;
                //如果当前节点不为空
                while (current != null)
                {
                    //临时节点记录下一个资源节点
                    next = current.Next;
                    //如果当前资源节点是需要查找的资源
                    if (current.Value.IsRes(res))
                    {
                        if (release)    //如果需要释放
                        {
                            //释放当前资源接待你
                            current.Value.Release();
                        }
                        //移除对应的响应事件
                        mCallbackRecordList.Remove(current);
                        return;
                    }
                    //当前节点标记为下一个节点
                    current = next;
                }
            }
        }
        
        /// 移除所有资源响应事件
        /// <param name="release">是否释放</param>
        private void RemoveAllCallbacks(bool release)
        {
            //如果资源响应事件列表不是空的
            if (mCallbackRecordList != null)
            {
                //记录响应事件列表的个数
                var count = mCallbackRecordList.Count;
                //如果存在响应事件
                while (count > 0)
                {
                    --count;    //响应事件数量-1
                    if (release)    //如果需要释放资源
                    {
                        //资源列表最后一个资源进行释放
                        mCallbackRecordList.Last.Value.Release();
                    }
                    //资源响应事件列表移除最后一个
                    mCallbackRecordList.RemoveLast();
                }
            }
        }
        
        //资源加载完成事件
        private void OnResLoadFinish(bool result, IRes res)
        {
            --mLoadingCount;        //正在加载的资源数量-1
            //执行异步加载资源
            DoLoadAsync();
            //如果资源加载数量为0时
            if (mLoadingCount == 0)
            {
                //移除所有的资源加载事件
                RemoveAllCallbacks(false);
                //如果资源监听事件不为空
                if (mListener != null)
                    mListener();        //执行资源监听事件
            }
        }
        
        /// 将资源添加到资源组
        /// <param name="res">需要添加的资源</param>
        /// <param name="lastOrder">是否着急添加，不着急的排在后面添加</param>
        private void AddRes2Array(IRes res, bool lastOrder)
        {
            //对象是申请一个关键字资源
            var searchRule = ResSearchKeys.Allocate(res.AssetName,res.OwnerBundleName,res.AssetType);
            //再次确保队列中没有它，并返回对用的资源
            var oldRes = FindResInArray(mResList, searchRule);
            //对象是回收刚才的关键字资源对象
            searchRule.Recycle2Cache();
            //如果资源不是空直接返回
            if (oldRes != null)
                return;
            //资源计数+1
            res.Retain();
            //资源列表里添加资源
            mResList.Add(res);
            //如果资源不是处于已经加载完成的状态
            if (res.State != ResState.Ready)
            {
                ++mLoadingCount;    //正在加载的资源和计数+1
                //根据加载资源的优先级确定提前还是往后加载资源
                if (lastOrder)
                    mWaitLoadList.AddLast(res);    //资源加载排到最后
                else
                    mWaitLoadList.AddFirst(res);    //资源加载排在前面
            }
        }
        
        //对象池回收该对象时响应的事件
        void IPoolable.OnRecycled()
        {
            ReleaseAllRes();    //释放所有的资源
        }

        class CallBackWrap    //响应返回类
        {
            //监听的事件
            private readonly Action<bool, IRes> mListener;
            //资源
            private readonly IRes mRes;
            //构造函数初始化类
            public CallBackWrap(IRes r, Action<bool, IRes> l)
            {
                mRes = r;
                mListener = l;
            }
            //释放
            public void Release()
            {
                //释放资源监听事件
                mRes.UnRegisteOnResLoadDoneEvent(mListener);
            }
            //判断是否是该资源
            public bool IsRes(IRes res)
            {
                //判断资源的名字是否相同
                return res.AssetName == mRes.AssetName;
            }
        }
    }
}