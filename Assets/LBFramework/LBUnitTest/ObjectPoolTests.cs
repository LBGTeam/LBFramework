using LBFramework.LBUtils;
using NUnit.Framework;
using UnityEngine;

namespace LBFramework.NUnitTest
{
    public class ObjectPoolTests
    {
        #region SimpleObjectPoolTest

        public class Fish
        {
        }

        [Test]
        public void SimpleObjectPool_Test()
        {
            var fishPool = new SimpleObjectPool<Fish>(() => new Fish(), null, 100);
            Assert.AreEqual(fishPool.CurCount, 100);
            var fishOne = fishPool.Allocate();
            Assert.AreEqual(fishPool.CurCount, 99);
            fishPool.Recycle(fishOne);
            Assert.AreEqual(fishPool.CurCount, 100);
            for (var i = 0; i < 10; i++)
            {
                fishPool.Allocate();
            }
            Assert.AreEqual(fishPool.CurCount, 90);
        }

        #endregion

        #region SafeObjectPoolTest

        class Msg : IPoolable
        {
            public void OnRecycled()
            {
                Debug.Log("OnRecycled");
            }
            public bool IsRecycled { get; set; }
        }

        [Test]
        public void SafeObjectPool_Test()
        {

            var msgPool = new SafeObjectPool<Msg>();
            msgPool.Init(100, 50); // max count:100 init count: 50
            Assert.AreEqual(msgPool.CurCount, 50);
            var fishOne = msgPool.Allocate();
            Assert.AreEqual(msgPool.CurCount, 49);
            msgPool.Recycle(fishOne);
            Assert.AreEqual(msgPool.CurCount, 50);
            for (var i = 0; i < 10; i++)
            {
                msgPool.Allocate();
            }
            Assert.AreEqual(msgPool.CurCount, 40);
        }

        #endregion
        
    }
}