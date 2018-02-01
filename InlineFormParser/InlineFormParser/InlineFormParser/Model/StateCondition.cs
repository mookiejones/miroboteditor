using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace InlineFormParser.Model
{
	[XmlRoot("Condition")]
	public class StateCondition : AdeComponentConfigurationListItem, IEquatable<StateCondition>
	{
		private Regex resultRegEx;

		[XmlAttribute("State")]
		[Description("The global state checked in this condition")]
		public string State { get; set; }

		[Description("A regular expression to be matched with the state value")]
		[XmlAttribute("Result")]
		public string Result
		{
			get
			{
				if (resultRegEx == null)
				{
					return null;
				}
				return resultRegEx.ToString();
			}
			set => resultRegEx = ((!string.IsNullOrEmpty(value)) ? new Regex(value, RegexOptions.Compiled) : null);
		}

		public bool ResultMatches(string probe)
		{
			Match match = resultRegEx.Match(probe);
			if (match != null && match.Index == 0)
			{
				return match.Length == probe.Length;
			}
			return false;
		}

		public void CheckStateKnown(IGlobalStateProvider provider)
		{
			if (provider.GetState(State, false) == null)
			{
				ConfigurationHelpers.ThrowConfigurationError(this, "State", $"The state \"{State}\" is not known.");
			}
		}

		public override void Validate()
		{
			base.Validate();
			ConfigurationHelpers.CheckStringProperty(this, "State");
			ConfigurationHelpers.CheckPropertyNotNull(this, "Result");
			IsValid = true;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as StateCondition);
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (State != null)
			{
				num ^= State.GetHashCode();
			}
			if (resultRegEx != null)
			{
				num ^= resultRegEx.ToString().GetHashCode();
			}
			return num;
		}

		public bool Equals(StateCondition other)
		{
			if (other != null && string.Equals(State, other.State, StringComparison.InvariantCulture))
			{
				return string.Equals(Result, other.Result, StringComparison.InvariantCulture);
			}
			return false;
		}
	}
}