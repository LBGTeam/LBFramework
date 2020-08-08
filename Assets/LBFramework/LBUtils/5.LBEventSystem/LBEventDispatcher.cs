using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBFramework.LBUtils
{
    //自定义事件委托类型
    public delegate void CustomEventHandler(LBEventComArg arg = null);
    
    public class LBEventDispatcher:Singleton<LBEventDispatcher>
    {
        private readonly Dictionary<int, HashSet<ILBEventHandler>> id2HandlerDict = new Dictionary<int, HashSet<ILBEventHandler>>();
        private readonly Dictionary<int, CustomEventHandler> id2DeleDict = new Dictionary<int, CustomEventHandler>();

        private LBEventDispatcher()
        {
            
        }
        public void AddListener(int eventId, ILBEventHandler handler)
        {
            if (!id2HandlerDict.TryGetValue(eventId, out HashSet<ILBEventHandler> handlerSet))
                handlerSet = id2HandlerDict[eventId] = new HashSet<ILBEventHandler>();
            handlerSet.Add(handler);
        }
        
        public void AddListener(int eventId, CustomEventHandler eventHandleDele)
        {
            if (id2DeleDict.TryGetValue(eventId, out CustomEventHandler eventDele))
            {
                Delegate[] delegates = eventHandleDele.GetInvocationList();
                if (Array.IndexOf(delegates, eventHandleDele) == -1)
                    eventDele += eventHandleDele;
            }
            else
                id2DeleDict.Add(eventId, eventHandleDele);
        }

        public void SendEvent(int eventId)
        {
            TiggerEvent(eventId,null);        //当函数参数不需要赋值的时候就赋值一个NULL,减少GC
        }

        public void SendEvent<T>(int eventId, T arg1)
        {
            //临时闯将将来可以换成对象池
            LBEventOneArg<T> eventArg = new LBEventOneArg<T>();
            eventArg.Set(arg1);
            TiggerEvent(eventId,eventArg);
            //如果用对象池，在这里就可以回收
        }
        public void SendEvent<T,U>(int eventId, T arg1,U arg2)
        {
            //临时闯将将来可以换成对象池
            LBEventTwoArg<T,U> eventArg = new LBEventTwoArg<T,U>();
            eventArg.Set(arg1,arg2);
            TiggerEvent(eventId,eventArg);
            //如果用对象池，在这里就可以回收
        }
        public void SendEvent<T,U,K>(int eventId, T arg1,U arg2,K arg3)
        {
            //临时闯将将来可以换成对象池
            LBEventThreeArg<T,U,K> eventArg = new LBEventThreeArg<T,U,K>();
            eventArg.Set(arg1,arg2,arg3);
            TiggerEvent(eventId,eventArg);
            //如果用对象池，在这里就可以回收
        }
        public void SendEvent<T,U,K,I>(int eventId, T arg1,U arg2,K arg3,I arg4)
        {
            //临时闯将将来可以换成对象池
            LBEventFourArg<T,U,K,I> eventArg = new LBEventFourArg<T,U,K,I>();
            eventArg.Set(arg1,arg2,arg3,arg4);
            TiggerEvent(eventId,eventArg);
            //如果用对象池，在这里就可以回收
        }
        
        private void TiggerEvent(int eventId, LBEventComArg arg)
        {
            if (id2HandlerDict.TryGetValue(eventId, out HashSet<ILBEventHandler> handlerSet))
            {
                foreach (var handler in handlerSet)
                {
                    handler.HandleEvent(eventId, arg);
                }
            }
            if (id2DeleDict.TryGetValue(eventId, out CustomEventHandler oldDele))
            {
                oldDele.Invoke(arg);
            }
        }
        
        public void RemoveListener(int eventId, ILBEventHandler handler)
        {
            if (id2HandlerDict.TryGetValue(eventId, out HashSet<ILBEventHandler> handlerSet))
            {
                id2HandlerDict[eventId].Remove(handler);
            }
        }

        public void RemoveListener(int eventId, CustomEventHandler eventHandlerDele)
        {
            if (id2DeleDict.TryGetValue(eventId, out CustomEventHandler oldDele))
            {
                oldDele -= eventHandlerDele;
                if (oldDele == null)
                    id2DeleDict.Remove(eventId);
            }
        }
        public void Destruct()
        {
            id2HandlerDict.Clear();
            id2DeleDict.Clear();
        }
    }
}