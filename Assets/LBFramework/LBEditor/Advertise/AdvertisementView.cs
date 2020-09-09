using System.ComponentModel;

namespace LBFramework.PackageKit
{
	[DisplayName("技术支持")]
	[PackageKitRenderOrder(int.MinValue)]
	public class AdvertisementView : LBFramework.IPackageKitView
	{
		public int RenderOrder
		{
			get { return -1; }
		}

		public bool Ignore { get; private set; }

		public bool Enabled
		{
			get { return true; }
		}

		private VerticalLayout mRootLayout = null;

		public AdvertisementView()
		{
			mRootLayout = new VerticalLayout();

			var verticalLayout = new VerticalLayout()
				.AddTo(mRootLayout);

			new LabelView("技术支持").FontBold().FontSize(12).AddTo(verticalLayout);
			
			new AdvertisementItemView("github",
					"https://github.com/lianbai")
				.AddTo(verticalLayout);
			
			new AdvertisementItemView("blog",
					"lianbai.github.io")
				.AddTo(verticalLayout);
		}

		public void OnUpdate()
		{

		}

		public void OnGUI()
		{
			mRootLayout.DrawGUI();
		}

		public void OnDispose()
		{
			mRootLayout.Dispose();
			mRootLayout = null;
		}

		public class LocalText
		{
			public static string TechSupport
			{
				get { return Language.IsChinese ? "技术支持" : "Tech Support"; }
			}
		}
	}
}