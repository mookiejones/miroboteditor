#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:24 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Threading;
using InlineFormParser.Common.Attributes;

#endregion

namespace InlineFormParser.ViewModel
{
	public class PropertyChangedNotifier : DispatcherObject, INotifyPropertyChanged
	{
		protected object sender;

		public PropertyChangedNotifier(object sender)
		{
			this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
			CheckAddAutoNotifyInfo();
		}

		public PropertyChangedNotifier()
		{
			sender = this;
			CheckAddAutoNotifyInfo();
		}

		public bool UsesAutoNotify => TypeUsesAutoNotify(sender.GetType());

		protected bool HasPropertyChangedListeners => PropertyChanged != null;

		public event PropertyChangedEventHandler PropertyChanged;

		public void FirePropertyChanged(params string[] propertyNames)
		{
			VerifyAccess();
			ValidatePropertyNames(propertyNames);
			InternalFireMultiPropertyChanged(propertyNames);
		}

		public void FirePropertyChanged(string propertyName)
		{
			VerifyAccess();
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));
			InternalFirePropertyChanged(propertyName);
		}

		public virtual void FirePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
		{
			FirePropertyChanged(GetMemberInfo(property).Name);
		}

		public void DelayedFirePropertyChanged(params string[] propertyNames)
		{
			VerifyAccess();
			ValidatePropertyNames(propertyNames);
			Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action<string[]>(InternalFireMultiPropertyChanged),
				propertyNames);
		}

		public static bool TypeUsesAutoNotify(Type senderType)
		{
			if (senderType == null) throw new ArgumentNullException(nameof(senderType));
			return AutoNotifyInfo.TypeUsesAutoNotify(senderType);
		}

		private static MemberInfo GetMemberInfo(LambdaExpression expression)
		{
			MemberExpression memberExpression;
			if (expression.Body is UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression) expression.Body;
				memberExpression = (MemberExpression) unaryExpression.Operand;
			}
			else
			{
				memberExpression = (MemberExpression) expression.Body;
			}

			return memberExpression.Member;
		}

		protected virtual void InternalFirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
				AutoNotifyPropertyDictionary autoNotifyPropertyDictionary =
					AutoNotifyInfo.GetAutoNotifyPropertyDictionary(sender.GetType());
				if (autoNotifyPropertyDictionary != null && autoNotifyPropertyDictionary.ContainsKey(propertyName))
				{
					HashSet<string> hashSet = new HashSet<string>();
					hashSet.Add(propertyName);
					RecFireDependentPropertyChanged(autoNotifyPropertyDictionary, propertyName, hashSet);
				}
			}
		}

		private void InternalFireMultiPropertyChanged(string[] propertyNames)
		{
			foreach (string propertyName in propertyNames) InternalFirePropertyChanged(propertyName);
		}

		private void ValidatePropertyNames(string[] propertyNames)
		{
			if (propertyNames == null) throw new ArgumentNullException(nameof(propertyNames));
			if (propertyNames.Length == 0) throw new ArgumentException("At least one property name is required");
			int num = 0;
			while (true)
			{
				if (num < propertyNames.Length)
				{
					string value = propertyNames[num];
					if (!string.IsNullOrEmpty(value))
					{
						num++;
						continue;
					}

					break;
				}

				return;
			}

			throw new ArgumentNullException($"propertyNames[{num}]");
		}

		private void CheckAddAutoNotifyInfo()
		{
			AutoNotifyInfo.CheckAddAutoNotifyInfo(sender.GetType());
		}

		private void RecFireDependentPropertyChanged(AutoNotifyPropertyDictionary autoNotifyInfo, string masterPropertyName,
			HashSet<string> processedProperties)
		{
			foreach (string item in autoNotifyInfo[masterPropertyName])
				if (!processedProperties.Contains(item))
				{
					PropertyChanged(sender, new PropertyChangedEventArgs(item));
					processedProperties.Add(item);
					if (autoNotifyInfo.ContainsKey(item)) RecFireDependentPropertyChanged(autoNotifyInfo, item, processedProperties);
				}
		}

		[Conditional("DEBUG")]
		private void VerifyProperty(string propertyName)
		{
			Type type = sender.GetType();
			if (type.GetProperty(propertyName) != null) return;
			throw new ArgumentException(
				$"PropertyChangedNotifier: \"{propertyName}\" is not a valid public property of \"{type.FullName}\"!");
		}
	}
}