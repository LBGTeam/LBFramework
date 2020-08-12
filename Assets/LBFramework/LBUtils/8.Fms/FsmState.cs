using System;

namespace LBFramework.LBUtils
{
    //有限状态机的**状态**
    public class FsmState<T> where T : class
    {
        public FsmState(){}    //构造函数，初始化状态机状态基类的实例
        
        //有限**状态机**初始化时统一调用
        protected internal virtual void OnInit(IFsm<T> fsm){} 
        
        //有限状态机**进入该状态**的时候调用
        protected internal virtual void OnEnter(IFsm<T> fsm){}
        
        /// <summary>
        /// 有限状态机运行时所有**状态**都运行的做记录的属性
        /// </summary>
        /// <param name="fsm">调用状态的状态机</param>
        /// <param name="elapseSeconds">逻辑所流动的时间</param>
        /// <param name="realElapseSeconds">游戏真实所流逝的时间（因为存在逻辑暂停的情况）</param>
        protected internal virtual void OnUpdate(IFsm<T> fsm, float elapseSeconds, float realElapseSeconds){}
        
        /// <summary>
        /// 有限状态机离开该**状态**时进行调用
        /// </summary>
        /// <param name="fsm">调用状态的状态机</param>
        /// <param name="isShutdown">标记**状态机**是否结束</param>
        protected internal virtual void OnLeave(IFsm<T> fsm, bool isShutdown){}
        
        //销毁整个状态机的**状态**
        protected internal virtual void OnDestroy(IFsm<T> fsm){}

        //状态机想要切换为当前**状态**的时候进行调用
        protected void ChangeState<TState>(IFsm<T> fsm) where TState : FsmState<T>
        {
            /*Fsm<T> fsmImplement = (Fsm<T>)fsm;
            if (fsmImplement == null)
            {
                throw new GameFrameworkException("FSM is invalid.");
            }
            fsmImplement.ChangeState<TState>();*/
        }

        /// <summary>
        /// 状态机切换为当前状态的时候调用，像比与上一个，这个是上一个状态机类型切换到该状态代表的状态机类型
        /// </summary>
        /// <param name="fsm"></param>
        /// <param name="stateType"></param>
        protected void ChangeState(IFsm<T> fsm, Type stateType)
        {
            /*Fsm<T> fsmImplement = (Fsm<T>)fsm;
            if (fsmImplement == null)
            {
                throw new GameFrameworkException("FSM is invalid.");
            }
            if (stateType == null)
            {
                throw new GameFrameworkException("State type is invalid.");
            }
            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                throw new GameFrameworkException(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));
            }
            fsmImplement.ChangeState(stateType);*/
        }
    }
}