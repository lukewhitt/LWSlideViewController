using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace LWSlideViewController
{
	public class TestTableSource : UITableViewSource
	{
		private List<string> MenuOptions = new List<string>(5){"Spiderman","Batman","Superman","Deadpool", "Jimmy Dizzle"};
		private List<string> ImageNames = new List<string>(5){"Images/spiderman.jpeg","Images/batman.jpeg","Images/superman.jpeg","Images/deadpool.jpeg","Images/doos.png"};
		private NSString kCellID = new NSString("tableCell");

		public TestTableSource ()
		{
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return MenuOptions.Count;
		}

		public override string TitleForHeader (UITableView tableView, int section)
		{
			return "Superheroes";
		}

		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			string title = "Superheroes";
			if (string.IsNullOrEmpty(title))
				return null;

			UIImageView imageView = new UIImageView(new RectangleF(0,0,tableView.Bounds.Size.Width, 22));
			imageView.Image = UIImage.FromFile("Images/section_background.png");
			imageView.Image.StretchableImage(0,0);

			UILabel titleLabel = new UILabel(RectangleF.Inflate(imageView.Frame, -10f,0));
			titleLabel.Font = UIFont.FromName("Helvetica-Bold", 12f);
			titleLabel.TextAlignment = UITextAlignment.Left;
			titleLabel.TextColor = UIColor.FromRGBA(125f/255f, 129f/255f, 146f/255f, 1f);
			titleLabel.ShadowColor = UIColor.FromRGBA(40f/255f, 45f/255f, 57f/255, 1f);
			titleLabel.ShadowOffset = new SizeF(0,1);
			titleLabel.BackgroundColor = UIColor.Clear;
			titleLabel.Text = title;
			imageView.AddSubview(titleLabel);

			return imageView;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(kCellID);
			if (cell == null)
			{
				cell = new UITableViewCell(UITableViewCellStyle.Value1, kCellID);
			}

			UIImageView background = new UIImageView(new RectangleF(0,0,320,44));
			background.Image = UIImage.FromFile("Images/cell_background.png");
			background.Image.StretchableImage(0,0);
			cell.BackgroundView = background;

			UIImageView selectedBackground = new UIImageView(new RectangleF(0,0,320,44));
			selectedBackground.Image = UIImage.FromFile("Images/cell_selected_background.png");
			selectedBackground.Image.StretchableImage(0,0);
			cell.SelectedBackgroundView = selectedBackground;

			cell.TextLabel.TextColor = UIColor.FromRGBA(190f/255f,197f/255f,212f/255f,1f);
			cell.TextLabel.HighlightedTextColor = cell.TextLabel.TextColor;
			cell.TextLabel.ShadowColor = UIColor.FromRGBA(33f/255f,38f/255f,49f/255f,1f);
			cell.TextLabel.ShadowOffset = new SizeF(0f,1f);
			cell.TextLabel.BackgroundColor = UIColor.Clear;
			cell.TextLabel.Font = UIFont.FromName("Helvetica", 16f);

			cell.ImageView.Image = UIImage.FromFile(ImageNames[indexPath.Row]);
			cell.ImageView.ClipsToBounds = true;
			cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			cell.TextLabel.Text = MenuOptions[indexPath.Row];

			return cell;
		}
	}
}

