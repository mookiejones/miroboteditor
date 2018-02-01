using System;
using System.Text;
using InlineFormParser.Common.Attributes;

namespace InlineFormParser.Model
{
	public class DisplaySource : IEquatable<DisplaySource>
	{
		private string fullName;

		public string DisplayID { get; }

		public bool IsOnMainDisplayProvider => string.IsNullOrEmpty(ProviderViewName);

		public string ProviderViewName
		{
			[return: CanBeNull]
			get;
		}

		public string FullName
		{
			get
			{
				if (fullName == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (!IsOnMainDisplayProvider)
					{
						stringBuilder.Append(ProviderViewName);
						stringBuilder.Append('.');
					}
					stringBuilder.Append(DisplayID);
					fullName = stringBuilder.ToString();
				}
				return fullName;
			}
		}

		public DisplaySource(string displayID, [CanBeNull] string providerViewName)
		{
			if (string.IsNullOrEmpty(displayID))
			{
				throw new ArgumentNullException(nameof(displayID));
			}
			DisplayID = displayID;
			ProviderViewName = providerViewName;
		}

		public override bool Equals(object obj)
		{
			DisplaySource displaySource = obj as DisplaySource;
			if (!(displaySource != (DisplaySource)null))
			{
				return false;
			}
			return FullName == displaySource.FullName;
		}

		public override int GetHashCode()
		{
			return FullName.GetHashCode();
		}

		public static bool operator ==(DisplaySource a, DisplaySource b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}
			if ((object)a != null && (object)b != null)
			{
				return a.FullName == b.FullName;
			}
			return false;
		}

		public static bool operator !=(DisplaySource a, DisplaySource b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return $"DisplaySource \"{FullName}\"";
		}

		public bool Equals(DisplaySource other)
		{
			if (!(other != (DisplaySource)null))
			{
				return false;
			}
			return FullName == other.FullName;
		}
	}
}