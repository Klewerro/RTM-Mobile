//Credits: https://gist.github.com/alexrainman/82b00160ab32bef9e69dee6d460f44fa

using System.Drawing;
using Foundation;
using Rztm.DependencyInterfaces;
using Rztm.iOS.DependencyImplementations;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(TextMeter_iOS))]
namespace Rztm.iOS.DependencyImplementations
{
    public class TextMeter_iOS : ITextMeter
    {
		public double MeasureTextSize(string text, double width, double fontSize, string fontName = null)
		{
			var nsText = new NSString(text);
			var boundSize = new SizeF((float)width, float.MaxValue);
			var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

			if (fontName == null)
			{
				fontName = "HelveticaNeue";
			}

			var attributes = new UIStringAttributes
			{
				Font = UIFont.FromName(fontName, (float)fontSize)
			};

			var sizeF = nsText.GetBoundingRect(boundSize, options, attributes, null).Size;

			return (double)sizeF.Height + 5;
		}
	}
}