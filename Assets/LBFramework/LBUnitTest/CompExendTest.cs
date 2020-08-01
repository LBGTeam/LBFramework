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
            var position = gameObject.transform.position;
            Assert.AreEqual(position.x, 10);

            gameObject.transform.PositionY(20);
            Assert.AreEqual(position.y, 20);
            
            gameObject.transform.PositionZ(30);
            Assert.AreEqual(position.z, 30);

            gameObject.transform.PositionXY(40,50);
            Assert.AreEqual(position.x, 40);
            Assert.AreEqual(position.y, 50);
            
            gameObject.transform.PositionXZ(60,70);
            Assert.AreEqual(position.x, 60);
            Assert.AreEqual(position.z, 70);

            gameObject.transform.PositionYZ(80,90);
            Assert.AreEqual(position.y, 80);
            Assert.AreEqual(position.z, 90);

            gameObject.transform.LocalPositionX(100);
            var localPosition = gameObject.transform.localPosition;
            Assert.AreEqual(localPosition.x, 100);

            gameObject.transform.LocalPositionY(110);
            Assert.AreEqual(localPosition.y, 110);

            gameObject.transform.LocalPositionZ(120);
            Assert.AreEqual(localPosition.z, 120);

            gameObject.transform.LocalPositionXY(130,140);
            Assert.AreEqual(localPosition.x, 130);
            Assert.AreEqual(localPosition.y, 140);
            
            gameObject.transform.LocalPositionXZ(150,160);
            Assert.AreEqual(localPosition.x, 150);
            Assert.AreEqual(localPosition.z, 160);
            
            gameObject.transform.LocalPositionYZ(170,180);
            Assert.AreEqual(localPosition.y, 170);
            Assert.AreEqual(localPosition.z, 180);
            
            gameObject.transform.LocalIdentity();
            Assert.AreEqual(gameObject.transform.localPosition, Vector3.zero);
            Assert.AreEqual(gameObject.transform.localRotation,Quaternion.identity);
            Assert.AreEqual(gameObject.transform.localScale,Vector3.one);
            
            gameObject.transform.Identity();
            Assert.AreEqual(gameObject.transform.position, Vector3.zero);
            Assert.AreEqual(gameObject.transform.rotation,Quaternion.identity);
            Assert.AreEqual(gameObject.transform.lossyScale,Vector3.one);
        }
    }
}