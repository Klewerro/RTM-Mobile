using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.DependencyInterfaces
{
	public interface ITextMeter
	{
		double MeasureTextSize(string text, double width, double fontSize, string fontName = null);
	}
}
