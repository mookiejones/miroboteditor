#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:27 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Drawing;
using System.Globalization;

#endregion

namespace InlineFormParser.Model
{
	public interface ICommonResourceService2
	{
		string GetString(string resourceId);

		string GetString(string resourceId, CultureInfo culture);

		string GetString(string resourceId, string defaultValue);

		string GetString(string resourceId, string defaultValue, CultureInfo culture);

		string GetString(string resourceId, bool throwIfNotFound);

		string GetString(string resourceId, bool throwIfNotFound, CultureInfo culture);

		Icon GetIcon(string resourceId);

		Icon GetIcon(string resourceId, CultureInfo culture);

		Icon GetIcon(string resourceId, bool throwIfNotFound);

		Icon GetIcon(string resourceId, bool throwIfNotFound, CultureInfo culture);

		Icon GetIcon(string resourceId, int width, int height);

		Icon GetIcon(string resourceId, int width, int height, CultureInfo culture);

		Icon GetIcon(string resourceId, int width, int height, bool throwIfNotFound);

		Icon GetIcon(string resourceId, int width, int height, bool throwIfNotFound, CultureInfo culture);

		Image GetImage(string resourceId);

		Image GetImage(string resourceId, CultureInfo culture);

		Image GetImage(string resourceId, bool throwIfNotFound);

		Image GetImage(string resourceId, bool throwIfNotFound, CultureInfo culture);
	}
}