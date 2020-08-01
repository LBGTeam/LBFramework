using UnityEngine;

namespace LBFramework.LBUtils
{
    //检测屏幕的分辨率
    public class ResolutionCheck
    {
        //判断是否横屏
        public static bool IsLandScape
        {
            get { return Screen.width > Screen.height; }
        }
        
        //Pad、Android 大部分平板的宽高比是 4:3
        public static bool IsPad
        {
            get { return IsRatito(4,3);; }
        }
        //1280 / 720（一般安卓设备，结果为 1.777778） 还是 1136 / 640（iPhone 5s，结果为 1.775）
        //或者是 1920 / 1080（iPhone 6p ，结果为 1.777778），它们都属于 16 ： 9 这个宽高比的范畴
        public static bool IsPhone16_9
        {
            get { return IsRatito(16,9); }
        }

        public static bool IsRatito(float width, float height)
        {
            var aspectRatio = IsLandScape
                ? (float) Screen.width / Screen.height
                : (float) Screen.height / Screen.width;
            var destinationRatio = width / height > 1 ? width / height : height / width;
            return aspectRatio > destinationRatio - 0.05f && aspectRatio < destinationRatio + 0.05f;
        }
    }
}