using LBFramework.LBUtils;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace LBFramework.NUnitTest
{
    public enum LBEventTestType
    {
        EventTestType = 100,
    }

    public class LBEventSystemClass:ILBEventHandler
    {
        public int testValue = 5;
        
        public void HandleEvent(int eventId, LBEventComArg arg = null)
        {
            switch ((LBEventTestType)eventId)
            {
                case LBEventTestType.EventTestType:
                    arg.TryGet(out testValue);
                    break;
            }
        }
    }
    public class LBEventSystemTest
    {
        [Test]
        public void EventTest()
        {
            LBEventSystemClass testClass = new LBEventSystemClass();
            LBEventDispatcher.Instabce.AddListener((int)LBEventTestType.EventTestType, testClass);
            LBEventDispatcher.Instabce.SendEvent((int)LBEventTestType.EventTestType,10);
            
            Assert.AreEqual(testClass.testValue,10);
        }
    }
}