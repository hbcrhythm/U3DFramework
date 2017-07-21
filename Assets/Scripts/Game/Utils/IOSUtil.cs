using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Game.Utils
{
	public class IOSUtil
	{
#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void BuyDiamond(string diamondID, int count);

		[DllImport("__Internal")]
		private static extern void IOSClearLocalNotification();

		[DllImport("__Internal")]
		private static extern void IOSGotoAppStore();

		[DllImport("__Internal")]
		private static extern void IOSAddLocalNotification(string content, int second);

		[DllImport("__Internal")]
		private static extern void IOSRegisterPush(string account);

		[DllImport("__Internal")]
		private static extern void IOSSaveImageToGallery(string path);

		[DllImport("__Internal")]
		private static extern string IOSGetIPv6(string host);

		[DllImport("__Internal")]
		private static extern string IOSGetCustomPlatform();
#endif

		public static void BuyDiamonds(string diamondID, int count = 1)
		{
#if UNITY_IOS && !UNITY_EDITOR
			BuyDiamond(diamondID, count);
#endif
		}

		public static void AddLocalNotification(string content, double second)
		{
#if UNITY_IOS && !UNITY_EDITOR
			IOSAddLocalNotification(content,(int)second);
#endif
		}

		public static void ClearLocalNotification()
		{
#if UNITY_IOS && !UNITY_EDITOR
			IOSClearLocalNotification();
#endif
		}

		public static void RegisterPush(string account)
		{
#if UNITY_IOS && !UNITY_EDITOR
			IOSRegisterPush(account);
#endif
		}

		public static void SaveImageToGallery(string path)
		{
#if UNITY_IOS && !UNITY_EDITOR
			IOSSaveImageToGallery(path);
#endif
		}

		public static void GotoAppStore()
		{
#if UNITY_IOS && !UNITY_EDITOR
			IOSGotoAppStore();
#endif
		}

		public static string GetIPv6(string host)
		{
#if UNITY_IOS && !UNITY_EDITOR
			return IOSGetIPv6(host);
#endif
			return null;
		}

		public static string GetCustomPlatform()
		{
#if UNITY_IOS && !UNITY_EDITOR
			return IOSGetCustomPlatform();
#endif
			return "";
		}
	}
}
