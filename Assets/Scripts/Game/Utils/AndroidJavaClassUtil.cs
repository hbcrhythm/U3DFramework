
using UnityEngine;
namespace Assets.Scripts.Game.Utils
{
    public class AndroidJavaClassUtil
    {
#if UNITY_ANDROID
        private static AndroidJavaObject sActivity;
        static AndroidJavaClassUtil()
        {
            if (Application.isEditor == false)
            {
                AndroidJavaClass jc = new AndroidJavaClass("unity3d.player.UnityPlayer");
                sActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
#endif

        public static void Call(string value, params object[] args)
        {
#if UNITY_ANDROID
            if (sActivity != null)
                sActivity.Call(value, args);
#endif
        }

        public static T Call<T>(string value, params object[] args)
        {
#if UNITY_ANDROID
            if (sActivity != null)
            {
                return sActivity.Call<T>(value, args);
            }
#endif
            return default(T);
        }

        public static void AddLocalNotification(string content, string date, string hour, string minute, int type = 1, string title = "娴妃传")
        {
            //Debug.LogError(string.Format("type:{0},title:{1},content:{2},date:{3},hour:{4},minute:{5}", type, title, content, date, hour, minute));

            Call("AddLocalNotification", type, title, content, date, hour, minute);
        }

        public static void ClearLocalNotification()
        {
            Call("ClearLocalNotification");
        }

    }
}
