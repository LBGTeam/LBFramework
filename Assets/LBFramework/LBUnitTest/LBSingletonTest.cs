using UnityEngine;
using LBFramework.LBUtils;
using NUnit.Framework;

namespace LBFramework.NUnitTest
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
            TestSingletonClass objA = TestSingletonClass.Instance;
            TestSingletonClass objB = TestSingletonClass.Instance;
            
            Assert.AreSame(objA, objB);
        }
    }
}