using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using InlineFormParser.Common.Attributes;
using InlineFormParser.Common.Tracing;

namespace InlineFormParser.Model
{
	
public class CommandCall : AdeComponentConfigurationListItem, IComparable<CommandCall>, IStartupCheckable
{
	private IGlobalStateProvider globalStateProvider;

	private object[] orderedParameters;

	private bool getStateExceptionLogged;

	private string commandName;

	private CommandCallArgument[] arguments;

	[XmlIgnore]
	public bool IsAttached => CommandInfo != null;

	public ICommandInfo CommandInfo { get; private set; }

	[XmlIgnore]
	public bool IsEnabled => (SafeGetState(null) & CommandStates.Enabled) == CommandStates.Enabled;

	[XmlIgnore]
	public bool AreArgumentsValidated { get; private set; }

	[XmlIgnore]
	public object[] OrderedParameters
	{
		get
		{
			if (orderedParameters == null)
			{
				CheckAttached();
				var dictionary = new Dictionary<CommandCallArgument, ICommandArgument>();
				if (CommandInfo.Arguments != null && CommandInfo.Arguments.Length > 0)
				{
					var list = new List<string>();
					var array = CommandInfo.Arguments;
					foreach (var commandArgument in array)
					{
						var commandCallArgument = FindArgument(commandArgument.Name);
						if (commandCallArgument != null)
						{
							list.Add(commandCallArgument.ValueString);
							dictionary[commandCallArgument] = commandArgument;
						}
					}
					orderedParameters = ParseArguments(list.ToArray());
				}
				else
				{
					orderedParameters = new object[0];
				}
				if (arguments != null)
				{
					var array2 = arguments;
					foreach (var commandCallArgument2 in array2)
					{
						if (!dictionary.ContainsKey(commandCallArgument2))
						{
							ConfigurationHelpers.ThrowConfigurationError(this, "Arguments",
								$"The argument \"{commandCallArgument2.Name}\" is not defined for the command \"{CommandInfo.SystemName}\".");
						}
					}
				}
			}
			return orderedParameters;
		}
		set
		{
			orderedParameters = value ?? throw new ArgumentNullException("OrderedParameters");
			AreArgumentsValidated = false;
		}
	}

	[XmlIgnore]
	public bool IsRightRequired => !string.IsNullOrEmpty(RequiredRight);

	[XmlIgnore]
	public bool HasConditions
	{
		get
		{
			if (Conditions != null)
			{
				return Conditions.Length > 0;
			}
			return false;
		}
	}

	[Description("The language-independant optional (!) name of the call")]
	[XmlAttribute("SystemName")]
	public string SystemName
	{
		[return: CanBeNull]
		get;
		[param: CanBeNull]
		set;
	}

	[Description("The language-independant name of the command to be called")]
	[XmlAttribute("Target")]
	public string CommandName
	{
		get => commandName;
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("commandName");
			}
			commandName = value;
			Detach();
		}
	}

	[XmlAttribute("MergeOrder")]
	[Description("The merge order of this call if used in a collection")]
	public int MergeOrder { get; set; } = 2147483647;

	[XmlElement(ElementName = "Argument", Type = typeof(CommandCallArgument))]
	[Description("The arguments of the command")]
	public CommandCallArgument[] Arguments
	{
		[return: CanBeNull]
		get
		{
			return arguments;
		}
		[param: CanBeNull]
		set
		{
			arguments = value;
			orderedParameters = null;
			AreArgumentsValidated = false;
			IsValid = false;
		}
	}

	[XmlAttribute("Right")]
	[Description("The required right for this command call")]
	public string RequiredRight
	{
		[return: CanBeNull]
		get;
		[param: CanBeNull]
		set;
	}

	[XmlElement(ElementName = "Condition", Type = typeof(StateCondition))]
	[Description("The required conditions for this call to be enabled")]
	public StateCondition[] Conditions
	{
		[return: CanBeNull]
		get;
		[param: CanBeNull]
		set;
	}

	public CommandCall()
	{
	}

	public CommandCall(string commandName)
	{
		this.commandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
	}

	public void Attach(AdeComponent component)
	{
		if (component == null)
		{
			throw new ArgumentNullException(nameof(component));
		}
		Attach(component.Site);
	}

	public void Attach(IServiceProvider serviceProvider)
	{
		if (serviceProvider == null)
		{
			throw new ArgumentNullException(nameof(serviceProvider));
		}
		var commandManager = serviceProvider.GetService(typeof(ICommandManager)) as ICommandManager;
		if (commandManager == null)
		{
			throw new ServiceNotFoundException("ICommandManager");
		}
		CommandInfo = commandManager.GetCommandInfo(commandName);
		if (!HasConditions)
		{
			return;
		}
		globalStateProvider = (serviceProvider.GetService(typeof(IGlobalStateProvider)) as IGlobalStateProvider);
		if (globalStateProvider != null)
		{
			return;
		}
		throw new ServiceNotFoundException("IGlobalStateProvider");
	}

	public void Detach()
	{
		CommandInfo = null;
		orderedParameters = null;
		AreArgumentsValidated = false;
		globalStateProvider = null;
	}

	public void Execute([CanBeNull] object context)
	{
		CheckAttached();
		EnsureArgumentsValidated();
		CommandInfo.ExecuteCall(this, context);
	}

	public void SafeExecute([CanBeNull] object context)
	{
		try
		{
			Execute(null);
		}
		catch (Exception ex)
		{
			LogException(ex, "Execute()");
		}
	}

	public CommandStates GetState([CanBeNull] object context)
	{
		CheckAttached();
		EnsureArgumentsValidated();
		var commandStates = CommandInfo.GetCommandStateForCall(this, context);
		if (HasConditions && (commandStates & CommandStates.Enabled) == CommandStates.Enabled)
		{
			var flag = true;
			StateCondition[] array = Conditions;
			foreach (StateCondition stateCondition in array)
			{
				IGlobalState state = globalStateProvider.GetState(stateCondition.State, false);
				if (state != null && state.Value != null && !stateCondition.ResultMatches(state.Value.ToString()))
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				commandStates &= ~CommandStates.Enabled;
			}
		}
		return commandStates;
	}

	public CommandStates SafeGetState([CanBeNull] object context)
	{
		try
		{
			return GetState(context);
		}
		catch (Exception ex)
		{
			if (!getStateExceptionLogged)
			{
				getStateExceptionLogged = true;
				LogException(ex, "GetState()");
			}
		}
		return CommandStates.Nothing;
	}

	public void Check()
	{
		CheckAttached();
		CommandInfo.Check(null, OrderedParameters);
		if (HasConditions)
		{
			StateCondition[] array = Conditions;
			foreach (StateCondition stateCondition in array)
			{
				stateCondition.CheckStateKnown(globalStateProvider);
			}
		}
	}

	public int CompareTo([CanBeNull] CommandCall other)
	{
		if (other != null)
		{
			return MergeOrder.CompareTo(other.MergeOrder);
		}
		return 1;
	}

	public override void Validate()
	{
		base.Validate();
		ConfigurationHelpers.CheckOptionalStringProperty(this, "SystemName");
		ConfigurationHelpers.CheckStringProperty(this, "CommandName");
		ConfigurationHelpers.ValidateChildConfigurationArray(this, "Arguments", true);
		ConfigurationHelpers.CheckOptionalStringProperty(this, "RequiredRight");
		ConfigurationHelpers.ValidateChildConfigurationArray(this, "Conditions", true);
		IsValid = true;
	}

	public override string ToString()
	{
		return $"CommandCall \"{((commandName != null) ? commandName : "<unknown>")}\"";
	}

	private void EnsureArgumentsValidated()
	{
		if (!AreArgumentsValidated)
		{
			CommandInfo.ValidateArguments(OrderedParameters);
			AreArgumentsValidated = true;
		}
	}

	private object[] ParseArguments(string[] parameters)
	{
		var array = CommandInfo.Arguments;
		if (parameters.Length > 0 && array == null)
		{
			goto IL_001d;
		}
		if (parameters.Length > array.Length)
		{
			goto IL_001d;
		}
		var array2 = new object[parameters.Length];
		try
		{
			for (var i = 0; i < parameters.Length; i++)
			{
				var valueType = array[i].ValueType;
				if (valueType.IsEnum)
				{
					array2[i] = ConfigurationHelpers.ParseFlags(valueType, parameters[i]);
				}
				else
				{
					array2[i] = Convert.ChangeType(parameters[i], valueType, CultureInfo.InvariantCulture);
				}
			}
			return array2;
		}
		catch (Exception innerException)
		{
			throw new ArgumentException($"{ToString()}: Error parsing arguments (see inner exception for details).", innerException);
		}
		IL_001d:
		throw new ArgumentException($"{ToString()}: Too many arguments!");
	}

	private CommandCallArgument FindArgument(string argumentName)
	{
		if (arguments != null)
		{
			var array = arguments;
			foreach (var commandCallArgument in array)
			{
				if (commandCallArgument.Name == argumentName)
				{
					return commandCallArgument;
				}
			}
		}
		return null;
	}

	private void CheckAttached()
	{
		if (IsAttached)
		{
			return;
		}
		throw new InvalidOperationException($"{ToString()} is not attached.");
	}

	private void LogException(Exception ex, string method)
	{
		var source = TraceSourceFactory.GetSource(PredefinedTraceSource.UIFramework);
		source.WriteException(ex, true, "{0} failed for \"{1}\"!", method, ToString());
	}
}
}