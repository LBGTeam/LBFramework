using LBFramework.LBUtils;
using NUnit.Framework;
using UnityEngine;

namespace LBFramework.NUnitTest
{
    public class CompExendTest
    {
        [Test]
        public static void Extensions_AllTest()
        {
            var gameObject = new GameObject();
            gameObject.transform.PositionX(10);
            Assert.AreEqual(gameObject.transform.position.x, 10);

            gameObject.transform.PositionY(20);
            Assert.AreEqual(gameObject.transform.position.y, 20);
            
            gameObject.transform.PositionZ(30);
            Assert.AreEqual(gameObject.transform.position.z, 30);

            gameObject.transform.PositionXY(40,50);
            Assert.AreEqual(gameObject.transform.position.x, 40);
            Assert.AreEqual(gameObject.transform.position.y, 50);
            
            gameObject.transform.PositionXZ(60,70);
            Assert.AreEqual(gameObject.transform.position.x, 60);
            Assert.AreEqual(gameObject.transform.position.z, 70);

            gameObject.transform.PositionYZ(80,90);
            Assert.AreEqual(gameObject.transform.position.y, 80);
            Assert.AreEqual(gameObject.transform.position.z, 90);

            gameObject.transform.LocalPositionX(100);
            Assert.AreEqual(gameObject.transform.localPosition.x, 100);

            gameObject.transform.LocalPositionY(110);
            Assert.AreEqual(gameObject.transform.localPosition.y, 110);

            gameObject.transform.LocalPositionZ(120);
            Assert.AreEqual(gameObject.transform.localPosition.z, 120);

            gameObject.transform.LocalPositionXY(130,140);
            Assert.AreEqual(gameObject.transform.localPosition.x, 130);
            Assert.AreEqual(gameObject.transform.localPosition.y, 140);
            
            gameObject.transform.LocalPositionXZ(150,160);
            Assert.AreEqual(gameObject.transform.localPosition.x, 150);
            Assert.AreEqual(gameObject.transform.localPosition.z, 160);
            
            gameObject.transform.LocalPositionYZ(170,180);
            Assert.AreEqual(gameObject.transform.localPosition.y, 170);
            Assert.AreEqual(gameObject.transform.localPosition.z, 180);
        }
    }
}