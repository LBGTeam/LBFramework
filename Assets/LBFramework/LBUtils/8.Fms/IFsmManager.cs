using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBFramework.LBUtils
{
    //有限状态机管理器
    public interface IFsmManager
    {
        int count { get; }    //获取有限**状态机**的数量

        //======================================================//
        
        /// 检查是否存在有限**状态机**
        /// <typeparam name="T">状态机持有者类型</typeparam>
        bool hasFsm<T>() where T : class;
        /// 检查是否存在有限**状态机**
        /// <typeparam name="T">状态机持有者类型</typeparam>
        /// <param name="name">状态机的名称</param>
        bool HasFsm<T>(string name) where T : class;
        /// 获取有限**状态机**
        /// <typeparam name="T">状态机持有者类型</typeparam>
        IFsm<T> GetFsm<T>() where T : class;
        /// 获取有限**状态机**
        /// <typeparam name="T">状态机持有者类型</typeparam>
        /// <param name="name">状态机的名称</param>
        IFsm<T> GetFsm<T>(string name) where T : class;
        
        //======================================================//
        
        /// 检查是否存在有限**状态机**
        /// <param name="ownerType">状态机持有者类型</param>
        bool hasFsm(Type ownerType);
        /// 检查是否存在有限**状态机**
        /// <param name="ownerType">状态机的持有者类型</param>
        /// <param name="name">状态机的名称</param>
        bool HasFsm(Type ownerType, string name);
        /// 获取有限**状态机**
        /// <param name="ownerType">状态机持有者类型</param>
        FsmBase GetFsm(Type ownerType);
        /// 获取有限**状态机**
        /// <param name="ownerType">状态机持有者类型</param>
        /// <param name="name">状态机的名称</param>
        FsmBase GetFsm(Type ownerType, string name);
        
        
        //获取所有的有限状态机
        FsmBase[] GetAllFsms();
        //获取所有的有限状态机
        void GetAllFsms(List<FsmBase> results);
        
        /// 创建有限状态机
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="states">有限状态机持有的**状态**</param>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        IFsm<T> CreateFsm<T>(T owner, params FsmState<T>[] states) where T : class;
        /// 创建有限状态机
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="states">有限状态机持有的**状态**</param>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        IFsm<T> CreateFsm<T>(string name, T owner, params FsmState<T>[] states) where T : class;
        /// 创建有限状态机
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="states">有限状态机持有的**状态**</param>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        IFsm<T> CreateFsm<T>(T owner, List<FsmState<T>> states) where T : class;
        /// 创建有限状态机
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="states">有限状态机持有的**状态**</param>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        IFsm<T> CreateFsm<T>(string name, T owner, List<FsmState<T>> states) where T : class;
        
        /// 销毁有限状态机
        /// <typeparam name="T">状态持有者类型</typeparam>
        bool DestroyFsm<T>() where T : class;
        /// 销毁有限状态机
        /// <param name="ownerType">状态持有者类型</param>
        bool DestroyFsm(Type ownerType);
        /// 销毁有限状态机
        /// <param name="name">状态机名字</param>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        bool DestroyFsm<T>(string name) where T : class;
        /// 销毁有限状态机
        /// <param name="ownerType">状态机持有者类型</param>
        /// <param name="name">状态机名字</param>
        bool DestroyFsm(Type ownerType, string name);
        /// 销毁有限状态机
        /// <param name="fsm">要销毁的有限状态机</param>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        bool DestroyFsm<T>(IFsm<T> fsm) where T : class;
        /// 销毁有限状态机
        /// <param name="fsm">要销毁的有限状态机</param>
        bool DestroyFsm(FsmBase fsm);
    }
}