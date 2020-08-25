using UnityEngine;

namespace LBFramework.LBUtils
{
    public class Platform
    {
        //是否是Android平台
        public static bool IsAndroid
        {
            get
            {
                bool retValue = false;
#if UNITY_ANDROID
                retValue = true;    
#endif
                return retValue;
            }
        }
        
        //是否是编辑器平台
        public static bool IsEditor
        {
            get
            {
                bool retValue = false;
#if UNITY_EDITOR
                retValue = true;    
#endif
                return retValue;
            }
        }
        //是否是IOS平台
        public static bool IsiOS
        {
            get
            {
                bool retValue = false;
#if UNITY_IOS
				retValue = true;    
#endif
                return retValue;
            }
        }

        //是否是StandardAlone平台
        public static bool IsStandardAlone
        {
            get
            {
                bool retValue = false;
#if UNITY_STANDALONE
                retValue = true;
#endif
                return retValue;
            }
        }
        //是否是Win平台
        public static bool IsWin
        {
            get
            {
                bool retValue = false;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                retValue = true;
#endif
                return retValue; 
            }
        }
    }
}