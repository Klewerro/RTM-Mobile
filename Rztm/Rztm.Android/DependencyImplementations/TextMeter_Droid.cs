//Credits: https://gist.github.com/alexrainman/82b00160ab32bef9e69dee6d460f44fa

using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Rztm.DependencyInterfaces;
using Rztm.Droid.DependencyImplementations;
using Xamarin.Forms;

[assembly: Dependency(typeof(TextMeter_Droid))]
namespace Rztm.Droid.DependencyImplementations
{
	public class TextMeter_Droid : ITextMeter
    {
		private Typeface _textTypeface;

		public double MeasureTextSize(string text, double width, double fontSize, string fontName = null)
		{
			var textView = new TextView(global::Android.App.Application.Context);
			textView.Typeface = GetTypeface(fontName);
			textView.SetText(text, TextView.BufferType.Normal);
			textView.SetTextSize(ComplexUnitType.Px, (float)fontSize);

			int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
				(int)width, MeasureSpecMode.AtMost);
			int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(
				0, MeasureSpecMode.Unspecified);

			textView.Measure(widthMeasureSpec, heightMeasureSpec);

			//return new Xamarin.Forms.Size((double)textView.MeasuredWidth, (double)textView.MeasuredHeight);
			return (double)textView.MeasuredHeight;
		}

		private Typeface GetTypeface(string fontName)
		{
			if (fontName == null)
			{
				return Typeface.Default;
			}

			if (_textTypeface == null)
			{
				_textTypeface = Typeface.Create(fontName, TypefaceStyle.Normal);
			}

			return _textTypeface;
		}
	}
}