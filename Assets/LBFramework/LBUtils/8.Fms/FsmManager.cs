using System;
using System.Collections.Generic;
using LBFramework.LBBase;
using UnityEngine;

namespace LBFramework.LBUtils
{
    public abstract class FsmManager : IFsmManager
    {
        private readonly Dictionary<TypeNamePair, FsmBase> mFsmDic;
        private readonly List<FsmBase> mTempFsmDic;

        //构造函数构造所有变量初始值
        public FsmManager()
        {
            mFsmDic = new Dictionary<TypeNamePair, FsmBase>();
            mTempFsmDic = new List<FsmBase>();
        }

        //获取有限**状态机**的数量
        public int count
        {
            get { return mFsmDic.Count; }
        }

        //实时更新状态机的管理，就是状态机的Update
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            mTempFsmDic.Clear();
            if (mFsmDic.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in mFsmDic)
            {
                mTempFsmDic.Add(fsm.Value);
            }

            foreach (FsmBase fsm in mTempFsmDic)
            {
                if (fsm.isDestroyed)
                {
                    continue;
                }

                fsm.Update(elapseSeconds, realElapseSeconds);
            }
        }

        //关闭并清理有限状态机
        public void CloseAndClean()
        {
            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in mFsmDic)
            {
                fsm.Value.CloseAndClean();
            }

            mFsmDic.Clear();
            mTempFsmDic.Clear();
        }
        ///判断是否存在状态机
        /// <typeparam name="T">状态机拥有者类型</typeparam>
        public bool HasFsm<T>() where T : class
        {
            return InternalHasFsm(new TypeNamePair(typeof(T)));
        }
        /// 判断是否存在状态机
        /// <param name="ownerType">状态机拥有者类型</param>
        public bool HasFsm(Type ownerType)
        {
            if (ownerType == null)
                return false;
            return InternalHasFsm(new TypeNamePair(ownerType));
        }
        /// 判断是否存在状态机
        /// <param name="name">状态机名称</param>
        /// <typeparam name="T">状态机拥有者类型</typeparam>
        public bool HasFsm<T>(string name) where T : class
        {
            return InternalHasFsm(new TypeNamePair(typeof(T), name));
        }
        /// 判断是否存在状态机
        /// <param name="name">状态机名称</param>
        /// <param name="ownerType">状态机拥有者类型</param>
        public bool HasFsm(Type ownerType, string name)
        {
            if (ownerType == null)
                return false;
            return InternalHasFsm(new TypeNamePair(ownerType, name));
        }
        //检测字典中是否存在状态机
        private bool InternalHasFsm(TypeNamePair typeNamePair)
        {
            return mFsmDic.ContainsKey(typeNamePair);
        }
        
        
        /// 获取有限状态机
        /// <typeparam name="T">状态机拥有者类型</typeparam>
        public IFsm<T> GetFsm<T>() where T : class
        {
            return (IFsm<T>)InternalGetFsm(new TypeNamePair(typeof(T)));
        }
        /// 获取有限状态机
        /// <param name="ownerType">状态机拥有者类型</param>
        public FsmBase GetFsm(Type ownerType)
        {
            if (ownerType == null)
                return null;
            return InternalGetFsm(new TypeNamePair(ownerType));
        }
        /// 获取有限状态机
        /// <param name="name">状态机名称</param>
        /// <typeparam name="T">状态机拥有者类型</typeparam>
        public IFsm<T> GetFsm<T>(string name) where T : class
        {
            return (IFsm<T>)InternalGetFsm(new TypeNamePair(typeof(T), name));
        }
        /// 获取有限状态机
        /// <param name="name">状态机名称</param>
        /// <param name="ownerType">状态机拥有者类型</param>
        public FsmBase GetFsm(Type ownerType, string name)
        {
            if (ownerType == null)
                return null;
            return InternalGetFsm(new TypeNamePair(ownerType, name));
        }
        //字典中获取有限状态机
        private FsmBase InternalGetFsm(TypeNamePair typeNamePair)
        {
            FsmBase fsm = null;
            if (mFsmDic.TryGetValue(typeNamePair, out fsm))
            {
                return fsm;
            }
            return null;
        }
        
        
        //获取所有的有限状态机
        public FsmBase[] GetAllFsms()
        {
            int index = 0;
            FsmBase[] results = new FsmBase[mFsmDic.Count];
            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in mFsmDic)
            {
                results[index++] = fsm.Value;
            }

            return results;
        }
        /// 获取所有的有限状态机
        /// <param name="results">得到所有有限状态机结果存储的链表</param>
        public void GetAllFsms(List<FsmBase> results)
        {
            if (results == null)
                return;
            results.Clear();
            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in mFsmDic)
            {
                results.Add(fsm.Value);
            }
        }
        
        
        /// 创建一个状态机
        /// <param name="owner">状态机的持有者</param>
        /// <param name="states">状态机的所有状态</param>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        public IFsm<T> CreateFsm<T>(T owner, params FsmState<T>[] states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }
        /// 创建一个状态机
        /// /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">状态机的持有者</param>
        /// <param name="states">状态机的所有状态</param>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        public IFsm<T> CreateFsm<T>(string name, T owner, params FsmState<T>[] states) where T : class
        {
            TypeNamePair typeNamePair = new TypeNamePair(typeof(T), name);
            if (HasFsm<T>(name))
                return null;
            Fsm<T> fsm = Fsm<T>.Create(name, owner, states);
            mFsmDic.Add(typeNamePair, fsm);
            return fsm;
        }
        /// 创建一个状态机
        /// <param name="owner"></param>
        /// <param name="states"></param>
        /// <typeparam name="T"></typeparam>
        public IFsm<T> CreateFsm<T>(T owner, List<FsmState<T>> states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }
        /// 创建一个状态机
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">状态机的持有者</param>
        /// <param name="states">状态机的所有状态</param>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        public IFsm<T> CreateFsm<T>(string name, T owner, List<FsmState<T>> states) where T : class
        {
            TypeNamePair typeNamePair = new TypeNamePair(typeof(T), name);
            if (HasFsm<T>(name))
                return null;
            Fsm<T> fsm = Fsm<T>.Create(name, owner, states);
            mFsmDic.Add(typeNamePair, fsm);
            return fsm;
        }
        
        
        //销毁有限状态机
        /// <typeparam name="T">状态机持有者类型</typeparam>
        public bool DestroyFsm<T>() where T : class
        {
            return InternalDestroyFsm(new TypeNamePair(typeof(T)));
        }
        /// 销毁有限状态机
        /// <param name="ownerType">状态机持有者类型</param>
        public bool DestroyFsm(Type ownerType)
        {
            if (ownerType == null)
                return false;
            return InternalDestroyFsm(new TypeNamePair(ownerType));
        }
        /// 销毁有限状态机
        /// <param name="name">有限状态机名称。</param>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        public bool DestroyFsm<T>(string name) where T : class
        {
            return InternalDestroyFsm(new TypeNamePair(typeof(T), name));
        }
        /// 销毁有限状态机
        /// <param name="name">有限状态机名称。</param>
        /// <param name="ownerType">状态机持有者类型</param>
        public bool DestroyFsm(Type ownerType, string name)
        {
            if (ownerType == null)
                return false;
            return InternalDestroyFsm(new TypeNamePair(ownerType, name));
        }
        /// 销毁有限状态机
        /// <param name="fsm">需要销毁的有限状态机</param>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        public bool DestroyFsm<T>(IFsm<T> fsm) where T : class
        {
            if (fsm == null)
                return false;
            return InternalDestroyFsm(new TypeNamePair(typeof(T), fsm.fsmName));
        }
        /// 销毁有限状态机
        /// <param name="fsm">需要销毁的有限状态机</param>
        public bool DestroyFsm(FsmBase fsm)
        {
            if (fsm == null)
                return false;
            return InternalDestroyFsm(new TypeNamePair(fsm.ownerType, fsm.fsmName));
        }
        //字典中移除有限状态机
        private bool InternalDestroyFsm(TypeNamePair typeNamePair)
        {
            FsmBase fsm = null;
            if (mFsmDic.TryGetValue(typeNamePair, out fsm))
            {
                fsm.CloseAndClean();
                return mFsmDic.Remove(typeNamePair);
            }
            return false;
        }
    }
}
