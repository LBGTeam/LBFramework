using System.Collections.Generic;
using UnityEngine;

namespace LBFramework.LBUtils
{
    public static class CompExtend
    {
        #region SetPos
        
        //设置世界坐标X的值
        public static T PositionX<T>(this T self, float x) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var position = transform.position;           //临时变量存储坐标信息
            position.x = x;                                    //更改临时变量的坐标信息
            transform.position = position;                    //重新给位置信息赋值
            return self;
        }
        //设置世界坐标Y的值
        public static T PositionY<T>(this T self, float y) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var position = transform.position;           //临时变量存储坐标信息
            position.y = y;                                    //更改临时变量的坐标信息
            transform.position = position;                    //重新给位置信息赋值
            return self;
        }
        //设置世界坐标Z的值
        public static T PositionZ<T>(this T self, float z) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var position = transform.position;           //临时变量存储坐标信息
            position.z = z;                                    //更改临时变量的坐标信息
            transform.position = position;                    //重新给位置信息赋值
            return self;
        }
        //设置世界坐标XY的值
        public static T PositionXY<T>(this T self, float x,float y) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var position = transform.position;           //临时变量存储坐标信息
            position.x = x;                                    //更改临时变量的坐标信息
            position.y = y;                                    
            transform.position = position;                    //重新给位置信息赋值
            return self;
        }
        //设置世界坐标XZ的值
        public static T PositionXZ<T>(this T self, float x,float z) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var position = transform.position;           //临时变量存储坐标信息
            position.x = x;                                    //更改临时变量的坐标信息
            position.z = z;                                    
            transform.position = position;                    //重新给位置信息赋值
            return self;
        }
        //设置世界坐标YZ的值
        public static T PositionYZ<T>(this T self, float y,float z) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var position = transform.position;           //临时变量存储坐标信息
            position.y = y;                                    //更改临时变量的坐标信息
            position.z = z;                                    
            transform.position = position;                    //重新给位置信息赋值
            return self;
        }
        //设置本地坐标X的值
        public static T LocalPositionX<T>(this T self, float x) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var localPosition = transform.localPosition;           //临时变量存储坐标信息
            localPosition.x = x;                                    //更改临时变量的坐标信息
            transform.localPosition = localPosition;                    //重新给位置信息赋值
            return self;
        }
        //设置本地坐标Y的值
        public static T LocalPositionY<T>(this T self, float y) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var localPosition = transform.localPosition;           //临时变量存储坐标信息
            localPosition.y = y;                                    //更改临时变量的坐标信息
            transform.localPosition = localPosition;                    //重新给位置信息赋值
            return self;
        }
        //设置本地坐标Z的值
        public static T LocalPositionZ<T>(this T self, float z) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var localPosition = transform.localPosition;           //临时变量存储坐标信息
            localPosition.z = z;                                    //更改临时变量的坐标信息
            transform.localPosition = localPosition;                    //重新给位置信息赋值
            return self;
        }
        //设置本地坐标XY的值
        public static T LocalPositionXY<T>(this T self, float x,float y) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var localPosition = transform.localPosition;           //临时变量存储坐标信息
            localPosition.x = x;                                    //更改临时变量的坐标信息
            localPosition.y = y;                                    
            transform.localPosition = localPosition;                    //重新给位置信息赋值
            return self;
        }
        //设置本地坐标XZ的值
        public static T LocalPositionXZ<T>(this T self, float x,float z) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var localPosition = transform.localPosition;           //临时变量存储坐标信息
            localPosition.x = x;                                    //更改临时变量的坐标信息
            localPosition.z = z;                                    
            transform.localPosition = localPosition;                    //重新给位置信息赋值
            return self;
        }
        //设置本地坐标YZ的值
        public static T LocalPositionYZ<T>(this T self, float y,float z) where T : Component
        {
            var transform = self.transform;                    //获取Component的transfrom
            var localPosition = transform.localPosition;           //临时变量存储坐标信息
            localPosition.y = y;                                    //更改临时变量的坐标信息
            localPosition.z = z;                                    
            transform.localPosition = localPosition;                    //重新给位置信息赋值
            return self;
        }
        #endregion

        #region Indentity
        
        //Transfroam本地数据重置
        public static T LocalIdentity<T>(this T self) where T : Component
        {
            var transform = self.transform;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            return self;
        }
        
        //Transfroam世界数据重置
        public static T Identity<T>(this T self) where T : Component
        {
            var transform = self.transform;

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            
            return self;
        }
        
        #endregion

        #region Active

        public static T Show<T>(this T self) where T : Component
        {
            self.gameObject.SetActive(true);
            return self;
        }
        public static T Hide<T>(this T self) where T : Component
        {
            self.gameObject.SetActive(false);
            return self;
        }

        public static GameObject Show(this GameObject self)
        {
            self.SetActive(true);
            return self;
        }

        public static GameObject Hide(this GameObject self)
        {
            self.SetActive(false);
            return self;
        }

        #endregion
    }
}