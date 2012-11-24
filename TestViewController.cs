using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace LWSlideViewController
{
	public class TestViewController : UIViewController
	{
		private string btnTitle;

		public TestViewController (string buttonTitle) 
		{
			btnTitle = string.IsNullOrEmpty(btnTitle) ? buttonTitle : "Hi";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Red;

			UIButton button = new UIButton(new System.Drawing.RectangleF(50,50,100,50));
			button.SetTitle(btnTitle, UIControlState.Normal);
			button.TouchUpInside += HandleTouchUpInside;

			View.AddSubview(button);
		}

		void HandleTouchUpInside (object sender, EventArgs e)
		{
			NavigationController.PushViewController(new TestViewController(btnTitle), true);
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}
	}
}

