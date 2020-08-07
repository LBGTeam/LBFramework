using System;
using System.Reflection;
#if UNITY_5_6_OR_NEWER
using UnityEngine;
using Object = UnityEngine.Object;
#endif

namespace LBFramework.Singleton
{
    public interface ISingleton
    {
        void OnInitSingleton();
    }

    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        protected static T mInstance;
        private static object mLock = new object();        //用来锁定，防止多线程问题

        public static T Instabce
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }
                return mInstance;
            }
        }
        public void OnInitSingleton() { }
    }
    public static class SingletonCreator
    {
	    public static T CreateSingleton<T>() where T : class, ISingleton
	    {
		    // 获取私有构造函数
		    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
		    // 获取无参构造函数
		    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
		    if (ctor == null)
		    {
			    throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
		    }
		    // 通过构造函数，常见实例
		    var retInstance = ctor.Invoke(null) as T;
		    retInstance.OnInitSingleton();
		    return retInstance;
	    }
    }

#if UNITY_5_6_OR_NEWER
    [Obsolete]
    public class QMonoSingletonPath : MonoSingletonPath
    {
        public QMonoSingletonPath(string pathInHierarchy) : base(pathInHierarchy)
        {
        }
    }
    //定义一个特性并控制定义特性的使用
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonPath : Attribute
    {
        private string mPathInHierarchy;
        public MonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }
        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        protected static T mInstance;
        protected static bool mOnApplicationQuit = false;
        public static T Instance
        {
            get
            {
                if (mInstance == null && !mOnApplicationQuit)
                {
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }
                return mInstance;
            }
        }
        public virtual void OnInitSingleton() { }
        public virtual void Dispose()
        {
	        if (MonoSingletonCreator.IsUnitTestMode)
	        {
		        var curTrans = transform;
		        do
		        {
			        var parent = curTrans.parent;
			        DestroyImmediate(curTrans.gameObject);
			        curTrans = parent;
		        } while (curTrans != null);
		        mInstance = null;
	        }
	        else { Destroy(gameObject); }
        }
        //应用退出前进行处理
        protected virtual void OnApplicationQuit()
        {
            mOnApplicationQuit = true;
            if (mInstance == null) return;
            Destroy(mInstance.gameObject);
            mInstance = null;
        }
        protected virtual void OnDestroy() { mInstance = null; }
        //判断对象是否已经退出
        public static bool IsApplicationQuit
        {
            get { return mOnApplicationQuit; }
        }
    }
    //创建单例
    public static class MonoSingletonCreator
	{
		public static bool IsUnitTestMode { get; set; }
		public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
		{
			T instance = null;
			//判断是否需要创建
			if (!IsUnitTestMode && !Application.isPlaying) return instance;
			instance = Object.FindObjectOfType<T>();
			//如果已经存在了就初始化并且返回
			if (instance != null)
			{
				instance.OnInitSingleton();
				return instance;
			}
			//获取成员信息
			MemberInfo info = typeof(T);
			var attributes = info.GetCustomAttributes(true);
			foreach (var atribute in attributes)
			{
				var defineAttri = atribute as MonoSingletonPath;
				if (defineAttri == null)
				{
					continue;
				}

				instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy, true);
				break;
			}
			if (instance == null)
			{
				var obj = new GameObject(typeof(T).Name);
				if (!IsUnitTestMode)
					Object.DontDestroyOnLoad(obj);
				instance = obj.AddComponent<T>();
			}
			instance.OnInitSingleton();
			return instance;
		}
		private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : MonoBehaviour
		{
			var obj = FindGameObject(path, true, dontDestroy);
			if (obj == null)
			{
				obj = new GameObject("Singleton of " + typeof(T).Name);
				if (dontDestroy && !IsUnitTestMode)
				{
					Object.DontDestroyOnLoad(obj);
				}
			}
			return obj.AddComponent<T>();
		}

		private static GameObject FindGameObject(string path, bool build, bool dontDestroy)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			var subPath = path.Split('/');
			if (subPath == null || subPath.Length == 0)
			{
				return null;
			}

			return FindGameObject(null, subPath, 0, build, dontDestroy);
		}

		private static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build,
			bool dontDestroy)
		{
			GameObject client = null;
			if (root == null)
			{
				client = GameObject.Find(subPath[index]);
			}
			else
			{
				var child = root.transform.Find(subPath[index]);
				if (child != null)
				{
					client = child.gameObject;
				}
			}
			if (client == null)
			{
				if (build)
				{
					client = new GameObject(subPath[index]);
					if (root != null)
					{
						client.transform.SetParent(root.transform);
					}

					if (dontDestroy && index == 0 && !IsUnitTestMode)
					{
						GameObject.DontDestroyOnLoad(client);
					}
				}
			}
			if (client == null)
			{
				return null;
			}
			return ++index == subPath.Length ? client : FindGameObject(client, subPath, index, build, dontDestroy);
		}
	}
#endif
}