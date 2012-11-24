using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace LWSlideViewController
{
	public class TestViewController : UIViewController
	{
		public TestViewController () 
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Red;
		}
	}
}

