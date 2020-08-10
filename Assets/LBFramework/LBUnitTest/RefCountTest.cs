using LBFramework.LBUtils;
using NUnit.Framework;
using UnityEngine;

namespace LBFramework.NUnitTest
{
    public class RefCountTest
    {
        class Light
        {
            public bool Opening { get; private set; }    //灯是否打开
            public void Open()            //开灯
            {
                Opening = true;
            }
            public void Close()
            {
                Opening = false;        //关灯
            }
        }

        class Room : SimpleRC
        {
            public readonly Light Light = new Light();

            public void EnterPeople()        //人进入
            {
                if (RefCount == 0)        
                {
                    Light.Open();        //如果没有引用说明没有人，要执行开灯，也就是创建
                }
                Retain();                //添加一个计数
            }
            public void LeavePeople()
            {
                Release();                //人离开，取消一个引用
            }
            protected override void OnZeroRef()
            {
                Light.Close();            //当没有人的时候关灯，也就是没有引用的时候注销
            }
        }
        
        [Test]
        public void SimpleRC_Test()
        {
            var room = new Room();
            Assert.AreEqual(room.RefCount, 0);
            Assert.IsFalse(room.Light.Opening);
            
            room.EnterPeople();
            Assert.AreEqual(room.RefCount, 1);
            Assert.IsTrue(room.Light.Opening);
            
            room.EnterPeople();
            room.EnterPeople();
            Assert.AreEqual(room.RefCount, 3);

            room.LeavePeople();
            room.LeavePeople();
            room.LeavePeople();
            Assert.IsFalse(room.Light.Opening);
            Assert.AreEqual(room.RefCount, 0);

            room.EnterPeople();
            Assert.AreEqual(room.RefCount, 1);
        }
    }
}