using UnityEngine;

namespace LBFramework.LBUtils
{
    public enum LBEventTestType
    {
        EventTestType
    }
    public class LBEventSystemTest:ILBEventHandler
    {
        private string testValue;
        public void HandleEvent(int eventId, LBEventComArg arg = null)
        {
            switch ((LBEventTestType)eventId)
            {
                case LBEventTestType.EventTestType:
                    arg.TryGet(out testValue);
                    break;
            }
        }

        public void EventTest()
        {
            //LBEventDispatcher
        }
    }
}