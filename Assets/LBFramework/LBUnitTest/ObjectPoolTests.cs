using LBFramework.LBUtils;
using NUnit.Framework;
using UnityEngine;

namespace LBFramework.NUnitTest
{
    public class ObjectPoolTests
    {
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
    }
}