using System.Linq;
using TreeEditor;
using UnityEditor;
using UnityEngine;

namespace LBFramework.ResKit
{
	public class ResKitEditorWindow:EditorWindow
	{
		public static bool EnableGenerateClass{
			get {
				return EditorPrefs.GetBool (ResKitView.KEY_AUTOGENERATE_CLASS, false);
			}
		}
	}

	public class ResKitView
	{
		private const string KEY_QAssetBundleBuilder_RESVERSION = "KEY_QAssetBundleBuilder_RESVERSION";
		public const string KEY_AUTOGENERATE_CLASS = "KEY_AUTOGENERATE_CLASS";
		
		
	}
}