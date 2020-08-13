using System;
using System.Collections.Generic;

namespace LBFramework.LBUtils
{
    public interface IFsm<T> where T : class
    {
        string fsmName { get; }    //获取有限状态机的名字
        string fullName { get; }    //获取有限状态机的完整名字
        T owner { get; }        //获取状态机的拥有者
        int fsmStateCount { get; }    //获取当前状态机中存储的状态数量
        bool isRunning { get; }    //标记当前状态机是否正在运行
        bool isDestroyed { get; }    //标记当前状态机是否被销毁
        
        FsmState<T> currentState { get; }    //获取状态状态机的**状态**
        float currentStateTime { get; }    //获取当前**状态**的持续时间
        
        //=========================<TStatr>状态机**状态**类型=============================//
        
        //开始执行有限**状态机**
        void Start<TState>() where TState : FsmState<T>;
        //有限状态机是否存在**状态**
        bool HasState<TState>() where TState : FsmState<T>;
        //获取有限状态机的**状态**
        TState GetState<TState>() where TState : FsmState<T>;
        //获取有限状态机所有**状态**
        FsmState<T>[] GetAllStates();
        
        //======================<Type stateType>状态机**状态**类型========================//
        
        //开始执行有限**状态机**
        void Start(Type stateType);
        //有限状态机是否存在**状态**
        bool HasState(Type stateType);
        //获取有限状态机的**状态**
        FsmState<T> GetState(Type stateType);
        //获取有限状态机所有**状态**
        void GetAllStates(List<FsmState<T>> results);
        
        
        //是否存在名字为name的有限状态机
        bool HasData(string name);
        //获取名字为name的有限**状态机**数据
        //获取名字为name的有限**状态机**数据
        object GetData(string name);
        //设置有限**状态机**的数据
        void SetData(string name, object data);
        //移除有限**状态机**的数据
        bool RemoveData(string name);
    }
}