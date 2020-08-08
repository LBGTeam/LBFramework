using UnityEngine;
using LBFramework.LBUtils;
using NUnit.Framework;

namespace LBFramework.LBUtils
{
    public class TestSingletonClass : Singleton<TestSingletonClass>
    {
        private TestSingletonClass()
        {
            
        }
    }
    public class LBSingletonTest
    {
        
        [Test]
        public void Test()
        {
            TestSingletonClass objA = TestSingletonClass.Instabce;
            TestSingletonClass objB = TestSingletonClass.Instabce;
            
            Assert.AreSame(objA, objB);
        }
    }
}