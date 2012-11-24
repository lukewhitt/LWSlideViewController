using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace LWSlideViewController
{
	public class LWSlideViewController : UIViewController
	{
		#region CONSTANTS
		private const string kLWSlideViewControllerSectionTitleKey = "kLWSlideViewControllerSectionTitleKey";
		private const string kLWSlideViewControllerSectionTitleNoTitle = "kLWSlideViewControllerSectionTitleNoTitle";
		private const string kLWSlideViewControllerSectionViewControllersKey = "kLWSlideViewControllerSectionViewControllersKey";
		private const string kLWSlideViewControllerViewControllerTitleKey = "kLWSlideViewControllerViewControllerTitleKey";
		private const string kLWSlideViewControllerViewControllerIconKey = "kLWSlideViewControllerViewControllerIconKey";
		private const string kLWSlideViewControllerViewControllerKey = "kLWSlideViewControllerViewControllerKey";

		private const float kLWLeftSlideDecisionPointX = 100f;
		private const float kLWRightSlideDecisionPointX = 265f;
		private const float kLWRightAnchorX = 270f;
		private const float kLWMinimumVelocityToTriggerSlide = 1000f;
		private const float kLWSlideAnimationDuration = 0.2f;
		#endregion

		#region PROPERTIES
		// Properties
		private UINavigationController slideNavigationController;
		private UITableView tableView;
		private UISearchBar searchBar;
		private UIImageView searchBarBackgroundView;
		private PointF startingDragPoint;
		private float startingDragTransformTx;
		private UITapGestureRecognizer tableViewTapGestureRecogniser;
		private UITapGestureRecognizer slideInTapGestureRecognizer;
		private LWSlideViewControllerState slideState;
		private LWSlideViewControllerMode slideMode;
		private UIViewController initViewController;
		private UITableViewSource tableSource;
		#endregion

		#region INIT
		public LWSlideViewController (UIViewController initVC, UITableViewSource tblSource)
		{
			tableSource = tblSource;
			initViewController = initVC;
			//rotationEnabled = true;
			slideMode = LWSlideViewControllerMode.AllViewController | LWSlideViewControllerMode.WholeView;
			slideState = LWSlideViewControllerState.Normal;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			searchBarBackgroundView = new UIImageView(new RectangleF(0,0,320,44));
			searchBarBackgroundView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			View.AddSubview(searchBarBackgroundView);

			searchBar = new UISearchBar(new RectangleF(0,0,kLWRightAnchorX,44));
			//searchBar.Delegate = TODO;
			searchBar.TintColor = UIColor.FromRGBA(36f,43f,57f,1f);
			View.AddSubview(searchBar);


			tableView = new UITableView(new RectangleF(0,0,320, View.Bounds.Size.Height-44f), UITableViewStyle.Plain);
			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			tableView.BackgroundColor = UIColor.FromRGBA(50f/255f,57f/255f,74f/255f,1f);
			tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			tableView.Source = tableSource;
			View.AddSubview(tableView);

			ConfigureViewController(initViewController);

			slideNavigationController = new UINavigationController(initViewController);
			LWSlideNavigationControllerDelegate navDelegate = new LWSlideNavigationControllerDelegate();
			navDelegate.DidShowViewControllerEvent += HandleDidShowViewControllerEvent;
			slideNavigationController.Delegate = navDelegate;
			slideNavigationController.View.Layer.ShadowColor = UIColor.Black.CGColor;
			slideNavigationController.View.Layer.ShadowOffset = new SizeF(0,0);
			slideNavigationController.View.Layer.ShadowRadius = 4;
			slideNavigationController.View.Layer.ShadowOpacity = 0.75f;
			slideNavigationController.WillMoveToParentViewController(this);
			AddChildViewController(slideNavigationController);
			View.AddSubview(slideNavigationController.View);
			slideNavigationController.DidMoveToParentViewController(this);

			UIBezierPath path = UIBezierPath.FromRoundedRect(slideNavigationController.View.Bounds, 4.0f);
			slideNavigationController.View.Layer.ShadowPath = path.CGPath;

			UIPanGestureRecognizer panRecogniser = new UIPanGestureRecognizer(HandlePan);
			slideNavigationController.NavigationBar.AddGestureRecognizer(panRecogniser);
			slideNavigationController.View.AddGestureRecognizer(panRecogniser);

			slideInTapGestureRecognizer = new UITapGestureRecognizer(HandleSlideInTap);
			slideInTapGestureRecognizer.Enabled = false;
			slideNavigationController.View.AddGestureRecognizer(slideInTapGestureRecognizer);

			tableViewTapGestureRecogniser = new UITapGestureRecognizer(HandleTableViewTap);
			tableViewTapGestureRecogniser.Enabled = false;
			tableView.AddGestureRecognizer(tableViewTapGestureRecogniser);


			UIImage searchBarBG = UIImage.FromFile("Images/search_bar_background.png");
			searchBar.BackgroundImage = searchBarBG;
			searchBar.BackgroundImage.StretchableImage(0,0);
			searchBarBackgroundView.Image = searchBarBG;
			searchBarBackgroundView.Image.StretchableImage(0,0);
			searchBar.Placeholder = "Search";


		}

		void HandleDidShowViewControllerEvent (LWSlideViewControllerState state)
		{
			slideState = state;
		}
		#endregion

		#region GESTURE HANDLERS
		private void HandleTableViewTap(UITapGestureRecognizer rec)
		{
			searchBar.ResignFirstResponder();
		}

		private void HandleSlideInTap(UITapGestureRecognizer rec)
		{
			if (slideState == LWSlideViewControllerState.Peeking)
			{
				SlideInSlideNavigationView();
			}
		}

		private void HandlePan(UIPanGestureRecognizer rec)
		{
			if (rec.State == UIGestureRecognizerState.Began)
			{
				HandleTouchesBeganAtLocation(rec.LocationInView(View));
			}
			else if (rec.State == UIGestureRecognizerState.Changed)
			{
				HandleTouchesMovedToLocation(rec.LocationInView(View));
			}
			else if (rec.State == UIGestureRecognizerState.Ended || 
			         rec.State == UIGestureRecognizerState.Cancelled ||
			         rec.State == UIGestureRecognizerState.Failed)
			{
				float velocity = rec.VelocityInView(View).X;
				if (Math.Abs(velocity) > kLWMinimumVelocityToTriggerSlide)
				{
					if (velocity > 0f)
					{
						SlideOutSlideNavigationView();
					}
					else
					{
						SlideInSlideNavigationView();
					}
				}
				else
				{
					HandleTouchesEndedAtLocation(rec.LocationInView(View));
				}
			}
		}

		private void HandleTouchesBeganAtLocation(PointF location)
		{
			if ((slideMode & LWSlideViewControllerMode.AllViewController) == 0 && (slideState == LWSlideViewControllerState.DrilledDown))
			    return;

			if (slideState == LWSlideViewControllerState.Searching)
				return;

			startingDragPoint = location;

			if (slideNavigationController.View.Frame.Contains(startingDragPoint) && slideState == LWSlideViewControllerState.Peeking)
			{
				slideState = LWSlideViewControllerState.Dragging;
				startingDragTransformTx = slideNavigationController.View.Transform.x0;
			}

			if ((slideMode & LWSlideViewControllerMode.WholeView) != 0 || startingDragPoint.Y <= slideNavigationController.NavigationBar.Frame.Size.Height)
			{
				slideState = LWSlideViewControllerState.Dragging;
				startingDragTransformTx = slideNavigationController.View.Transform.x0;
			}
		}

		private void HandleTouchesMovedToLocation(PointF location)
		{
			if (slideState != LWSlideViewControllerState.Dragging)
				return;

			UIView.Animate(0.05, 0, UIViewAnimationOptions.CurveLinear | UIViewAnimationOptions.BeginFromCurrentState,
			               delegate {
				slideNavigationController.View.Transform = CGAffineTransform.MakeTranslation(Math.Max(startingDragTransformTx + (location.X - startingDragPoint.X), 0f), 0f);
			}, delegate{});
		}

		private void HandleTouchesEndedAtLocation(PointF location)
		{
			if (slideState == LWSlideViewControllerState.Dragging)
			{
				if (location.X < startingDragPoint.X)
				{
					if (slideNavigationController.View.Transform.x0 <= kLWRightSlideDecisionPointX)
					{
						SlideInSlideNavigationView();
					}
					else
					{
						SlideOutSlideNavigationView();
					}
				} 
				else
				{
					if (slideNavigationController.View.Transform.x0 >= kLWLeftSlideDecisionPointX)
					{
						SlideOutSlideNavigationView();
					}
					else
					{
						SlideInSlideNavigationView();
					}
				}
			}
		}
		#endregion

		#region PRIVATE METHODS
		private void ShowViewController(UIViewController controller)
		{
			if (controller != null)
			{
				ConfigureViewController(controller);
				slideNavigationController.SetViewControllers(new UIViewController[]{controller}, false);
				SlideInSlideNavigationView();
			}
		}

		private void ConfigureViewController(UIViewController initVC)
		{
			UIBarButtonItem barButtonItem = new UIBarButtonItem(UIImage.FromFile("Images/menu_icon.png"), UIBarButtonItemStyle.Plain, MenuBarButtonItemPressed);
			initVC.NavigationItem.SetLeftBarButtonItem(barButtonItem, false);
		}

		private void MenuBarButtonItemPressed(object sender, EventArgs e)
		{
			if (slideState == LWSlideViewControllerState.Peeking)
			{
				SlideInSlideNavigationView();
			}
			else
			{
				SlideOutSlideNavigationView();
			}
		}

		private void SlideInSlideNavigationView()
		{
			UIView.Animate(kLWSlideAnimationDuration, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.BeginFromCurrentState,
			               delegate {
				slideNavigationController.View.Transform = CGAffineTransform.MakeIdentity();
			}, delegate {
				slideNavigationController.TopViewController.View.UserInteractionEnabled = true;
				slideInTapGestureRecognizer.Enabled = false;
				//CancelSearching();
				slideState = LWSlideViewControllerState.Normal;
			});
		}
		#endregion

		#region ANIMATIONS

		private void SlideOutSlideNavigationView()
		{
			slideState = LWSlideViewControllerState.Peeking;
			slideNavigationController.TopViewController.View.UserInteractionEnabled = false;

			UIView.Animate(kLWSlideAnimationDuration, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.BeginFromCurrentState,
			               delegate{
				slideNavigationController.View.Transform = CGAffineTransform.MakeTranslation(kLWRightAnchorX, 0f);
			}, delegate {
				searchBar.Frame = new RectangleF(0,0,kLWRightAnchorX, searchBar.Frame.Size.Height);
				slideInTapGestureRecognizer.Enabled = true;
			});
		}

		private void SlideSlideNavigationControllerViewOffScreen()
		{
			float width;
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
				width = 480f;
			else
				width = 320f;

			slideState = LWSlideViewControllerState.Searching;
			slideInTapGestureRecognizer.Enabled = false;

			UIView.Animate(kLWSlideAnimationDuration, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.BeginFromCurrentState,
			               delegate {
				slideNavigationController.View.Transform = CGAffineTransform.MakeTranslation(width, 0);
				searchBar.Frame = new RectangleF(0,0,width,searchBar.Frame.Size.Height);
			}, delegate {});
		}
		#endregion
	}

	public class LWSlideNavigationControllerDelegate : MonoTouch.UIKit.UINavigationControllerDelegate
	{
		public delegate void DidShowViewControllerDelegate(LWSlideViewControllerState state);
		public event DidShowViewControllerDelegate DidShowViewControllerEvent;

		public LWSlideNavigationControllerDelegate()
		{}

		public override void DidShowViewController (UINavigationController navigationController, UIViewController viewController, bool animated)
		{
			if (navigationController.ViewControllers.Length > 1) 
			{
				if (DidShowViewControllerEvent != null)
					DidShowViewControllerEvent(LWSlideViewControllerState.DrilledDown);
			}
			else
				if (DidShowViewControllerEvent != null)
					DidShowViewControllerEvent(LWSlideViewControllerState.Normal);
		}
	}
}

