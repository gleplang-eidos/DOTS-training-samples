using UnityEngine;

public static class ColorHelper
{
    public static Color GetColorByLineColor(LineColor lineColor)
    {
		switch (lineColor)
		{
			case LineColor.Blue:
				return Color.blue;
			case LineColor.Green:
				return Color.green;
			case LineColor.Orange:
				return new Color(1, 0.5f, 0);
			case LineColor.Yellow:
				return Color.yellow;
			default:
				return Color.magenta;
		}
	}
}
