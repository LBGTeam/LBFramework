using System;
using UnityEngine;

namespace LBFramework.LBUtils
{
    //有限**状态机**的基类
    public abstract class FsmBase
    {
        private string mName;    //**状态机**名字

        public FsmBase()        //构造函数初始化状态机的名字
        {
            mName = string.Empty;
        }
        
        public string fsmName        //状态机公开的状态机名字属性
        {
            get
            {
                return mName;    //获得状态机名字
            }
            protected set        //保护属性，只允许继承的子类进行修改
            {
                mName = value ?? string.Empty;
            }
        }

        public abstract Type ownerType { get; } //有限状态机持有者的类型

        public string fullName            //获取有限**状态机**的完整名字
        {
            get
            {
                return string.IsNullOrEmpty(mName) ? 
                    ownerType.FullName 
                    : string.Format("{0}.{1}", ownerType.FullName, mName);
            }
        }
        
        //获取有限状态机中**状态**的数量
        public abstract int fsmStateCount { get; }
        
        //标记有限**状态机**是否在运行
        public abstract bool isRunning { get; }
        
        //标记有限**状态机**是否被销毁
        public abstract bool isDestroyed { get; }
        
        //获取当前有限状态机执行**状态**名称
        public abstract string currentStateName { get; }   
        
        //获取当前有限状态机的**状态**持续时间
        public abstract float currentStateTime { get; }
        
        /// <summary>
        /// 有限**状态机**运行时实时记录运行时间
        /// </summary>
        /// <param name="elapseSeconds">逻辑所流动的时间</param>
        /// <param name="realElapseSeconds">游戏真实所流逝的时间（因为存在逻辑暂停的情况）</param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);
        
        //有限**状态机**关闭并清理
        internal abstract void CloseAndClean();
    }
}