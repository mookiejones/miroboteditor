using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using miRobotEditor.Core.Exceptions;
using miRobotEditor.Core.Services;

namespace miRobotEditor.Core
{
    /// <summary>
    /// Markup extension that retrieves localized resource strings.
    /// </summary>
    [MarkupExtensionReturnType(typeof(string))]
    public sealed class LocalizeExtension : LanguageDependentExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public LocalizeExtension(string key)
        {
            this.key = key;
            this.UsesAccessors = true;
            this.UpdateOnLanguageChange = true;
        }
        /// <summary>
        /// 
        /// </summary>
        public LocalizeExtension()
        {
            this.UsesAccessors = true;
            this.UpdateOnLanguageChange = true;
        }

        string key;

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Set whether the text uses accessors.
        /// If set to true (default), accessors will be converted to WPF syntax.
        /// </summary>
        public bool UsesAccessors { get; set; }

        public override string Value
        {
            get
            {
                try
                {
                    string result = ResourceService.GetString(key);
                    if (UsesAccessors)
                        result = MenuService.ConvertLabel(result);
                    return result;
                }
                catch (ResourceNotFoundException)
                {
                    return "{Localize:" + key + "}";
                }
            }
        }
    }
    /// <summary>
    /// LanguageDependentExtension
    /// </summary>
    public abstract class LanguageDependentExtension : MarkupExtension, INotifyPropertyChanged, IWeakEventListener
    {
        protected LanguageDependentExtension()
        {
            this.UpdateOnLanguageChange = true;
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract string Value { get; }

        /// <summary>
        /// Set whether the LocalizeExtension should use a binding to automatically
        /// change the text on language changes.
        /// The default value is true.
        /// </summary>
        public bool UpdateOnLanguageChange { get; set; }

        bool isRegisteredForLanguageChange;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (UpdateOnLanguageChange)
            {
                Binding binding = new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
                return binding.ProvideValue(serviceProvider);
            }
            else
            {
                return this.Value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use ExtensionMethods.SetValueToExtension instead of directly fetching the binding from this extension")]
        public Binding CreateBinding()
        {
            return new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
        }

        event System.ComponentModel.PropertyChangedEventHandler ChangedEvent;
        /// <summary>
        /// 
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!isRegisteredForLanguageChange)
                {
                    isRegisteredForLanguageChange = true;
                    LanguageChangeWeakEventManager.AddListener(this);
                }
                ChangedEvent += value;
            }
            remove { ChangedEvent -= value; }
        }

        static readonly System.ComponentModel.PropertyChangedEventArgs
            valueChangedEventArgs = new System.ComponentModel.PropertyChangedEventArgs("Value");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="managerType"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, System.EventArgs e)
        {
            var handler = ChangedEvent;
            if (handler != null)
                handler(this, valueChangedEventArgs);
            return true;
        }
    }
    /// <summary>
    /// LanguageChangeWeakEventManager
    /// </summary>
    public sealed class LanguageChangeWeakEventManager : WeakEventManager
    {
        /// <summary>
        /// Adds a weak event listener.
        /// </summary>
        public static void AddListener(IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(null, listener);
        }

        /// <summary>
        /// Removes a weak event listener.
        /// </summary>
        public static void RemoveListener(IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(null, listener);
        }

        /// <summary>
        /// Gets the current manager.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        private static LanguageChangeWeakEventManager CurrentManager
        {
            get
            {
                var manager = (LanguageChangeWeakEventManager)GetCurrentManager(typeof(LanguageChangeWeakEventManager));
                if (manager == null)
                {
                    manager = new LanguageChangeWeakEventManager();
                    SetCurrentManager(typeof(LanguageChangeWeakEventManager), manager);
                }
                return manager;
            }
        }

        protected override void StartListening(object source)
        {
            ResourceService.LanguageChanged += DeliverEvent;
        }

        protected override void StopListening(object source)
        {
            ResourceService.LanguageChanged -= DeliverEvent;
        }
    }

}
