using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LBFramework.LBUtils
{
    //有限状态机
    public sealed class Fsm<T>:FsmBase, IFsm<T> where T : class
    {
        private T mOwner;    //记录有限状态机拥有者
        private readonly Dictionary<Type, FsmState<T>> mStateDic;    //字典储存状态机拥有的所有状态
        private readonly Dictionary<string, object> mDataDic;    //字典储存状态机状态的数据
        private FsmState<T> mCurrentState;    //记录有限状态机当前的状态
        private float mCurrentStateTime;      //记录当前状态所持续的时间
        private bool mIsDestroyed;            //标记当前状态机是否被销毁

        //构造函数初始化状态机的所有数据
        public Fsm()
        {
            mOwner = null;
            mStateDic = new Dictionary<Type, FsmState<T>>();
            mDataDic = new Dictionary<string, object>();
            mCurrentState = null;
            mCurrentStateTime = 0f;
            mIsDestroyed = true;
        }

        //公开的获取状态机的拥有者
        public T owner { get { return mOwner; } }
        //公开的获取邮箱状态机拥有者的类型
        public override Type ownerType { get { return typeof(T); } }
        //获取状态机存储**状态**的数量
        public override int fsmStateCount { get { return mStateDic.Count; } }
        //获取有限状态机是否正在运行
        public override bool isRunning { get { return mCurrentState != null; } }
        //获取有限状态机是否被销毁
        public override bool isDestroyed { get { return mIsDestroyed; } }
        //获取有限状态机当前的**状态**
        public FsmState<T> currentState { get { return mCurrentState; } }
        //获取当前有限状态机**状态**的名称
        public override string currentStateName { get { return mCurrentState != null ? mCurrentState.GetType().FullName : null; } }
        //获取当前有限状态机**状态**的持续时间
        public override float currentStateTime { get { return mCurrentStateTime; } }
        
        //=========================<TStatr>状态机**状态**类型=============================//
        
        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <param name="name">有限状态机的名字</param>
        /// <param name="owner">有限状态机拥有者</param>
        /// <param name="states">初始化所有的有限状态机**状态**</param>
        /// <returns></returns>
        public static Fsm<T> Creat(string name, T owner, params FsmState<T>[] states)
        {
            if (owner == null || states == null || states.Length < 1)
            {
                return null;
            }
            Fsm<T> fsm = new Fsm<T>();
            fsm.fsmName = name;
            fsm.mOwner = owner;
            fsm.mIsDestroyed = false;
            foreach (FsmState<T> state in states)
            {
                if (state == null)
                {
                    return null;
                }
                Type stateType = state.GetType();
                if (fsm.mStateDic.ContainsKey(stateType))
                {
                    return null;
                }
                fsm.mStateDic.Add(stateType, state);
                state.OnInit(fsm);
            }
            return fsm;
        }
        // 开始有限状态机。
        public void Start<TState>() where TState : FsmState<T>
        {
            if (isRunning) return;

            FsmState<T> state = GetState<TState>();
            if (state == null) return;

            mCurrentStateTime = 0f;
            mCurrentState = state;
            mCurrentState.OnEnter(this);
        }
        // 获取有限状态机状态。
        public TState GetState<TState>() where TState : FsmState<T>
        {
            FsmState<T> state = null;
            if (mStateDic.TryGetValue(typeof(TState), out state))
            {
                return (TState)state;
            }
            return null;
        }
        // 是否存在有限状态机状态。
        public bool HasState<TState>() where TState : FsmState<T>
        {
            return mStateDic.ContainsKey(typeof(TState));
        }
        //获取有限状态机里面的所有**状态**
        public FsmState<T>[] GetAllStates()
        {
            int index = 0;
            FsmState<T>[] results = new FsmState<T>[mStateDic.Count];
            foreach (KeyValuePair<Type, FsmState<T>> state in mStateDic)
            {
                results[index++] = state.Value;
            }

            return results;
        }
        //设置名字为name的有限状态机数据
        public void SetData<TData>(string name, TData data) where TData : Object
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            mDataDic[name] = data;
        }
        //切换当前有限状态机
        internal void ChangeState<TState>() where TState : FsmState<T>
        {
            ChangeState(typeof(TState));
        }
        
        //=========================stateType状态机**状态**类型=============================//
        
        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <param name="name">有限状态机的名字</param>
        /// <param name="owner">有限状态机拥有者</param>
        /// <param name="states">初始化所有的有限状态机**状态**</param>
        public static Fsm<T> Create(string name, T owner, List<FsmState<T>> states)
        {
            if (owner == null || states == null || states.Count < 1)
            {
                return null;
            }
            Fsm<T> fsm = new Fsm<T>();
            fsm.fsmName = name;
            fsm.mOwner = owner;
            fsm.mIsDestroyed = false;
            foreach (FsmState<T> state in states)
            {
                if (state == null)
                {
                    return null;
                }
                Type stateType = state.GetType();
                if (fsm.mStateDic.ContainsKey(stateType))
                {
                    return null;
                }
                fsm.mStateDic.Add(stateType, state);
                state.OnInit(fsm);
            }
            return fsm;
        }
        // 开始有限状态机。
        public void Start(Type stateType)
        {
            if (isRunning || stateType == null || !typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                return;
            }
            FsmState<T> state = GetState(stateType);
            if (state == null)
            {
                return;
            }
            mCurrentStateTime = 0f;
            mCurrentState = state;
            mCurrentState.OnEnter(this);
        }
        // 获取有限状态机状态。
        public FsmState<T> GetState(Type stateType)
        {
            if (stateType == null || !typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                return null;
            }
            FsmState<T> state = null;
            if (mStateDic.TryGetValue(stateType, out state))
            {
                return state;
            }
            return null;
        }
        // 是否存在有限状态机状态。
        public bool HasState(Type stateType)
        {
            if (stateType == null || !typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                return false;
            }

            return mStateDic.ContainsKey(stateType);
        }
        //获取有限状态机里面的所有**状态**
        public void GetAllStates(List<FsmState<T>> results)
        {
            if (results == null)
            {
                return;
            }
            results.Clear();
            foreach (KeyValuePair<Type, FsmState<T>> state in mStateDic)
            {
                results.Add(state.Value);
            }
        }
        //设置有限状态机数据
        public void SetData(string name, object data)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            mDataDic[name] = data;
        }
        //切换当前有限状态机
        internal void ChangeState(Type stateType)
        {
            if (mCurrentState == null)
            {
                return;
            }
            FsmState<T> state = GetState(stateType);
            if (state == null)
            {
                return;
            }
            mCurrentState.OnLeave(this, false);
            mCurrentStateTime = 0f;
            mCurrentState = state;
            mCurrentState.OnEnter(this);
        }
        
        
        
        //是否存在名称为name的有限**状态机**
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return mDataDic.ContainsKey(name);
        }
        
        //获取有限状态机数据
        public TData GetData<TData>(string name) where TData : Object
        {
            return (TData)GetData(name);
        }

        //得到名字为name的有限状态机数据
        public object GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            object data = null;
            if (mDataDic.TryGetValue(name, out data))
            {
                return data;
            }
            return null;
        }

        // 移除名字为name的有限状态机数据。
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            return mDataDic.Remove(name);
        }
        
        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">状态机逻辑流逝时间</param>
        /// <param name="realElapseSeconds">状态机真是流逝时间</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (mCurrentState == null)
            {
                return;
            }
            mCurrentStateTime += elapseSeconds;
            mCurrentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }

        //关闭并清理有限状态机
        internal override void CloseAndClean()
        {
            this.Clear();
            GC.SuppressFinalize(this);    //标记gc不在调用析构函数
        }
        

        //清空有限状态机
        public void Clear()
        {
            if (mCurrentState != null)
            {
                mCurrentState.OnLeave(this, true);
            }
            foreach (KeyValuePair<Type, FsmState<T>> state in mStateDic)
            {
                state.Value.OnDestroy(this);
            }
            fsmName = null;
            mOwner = null;
            mStateDic.Clear();
            mDataDic.Clear();
            mCurrentState = null;
            mCurrentStateTime = 0f;
            mIsDestroyed = true;
        }
        
        
    }
}