using System.Windows;
using System.Collections.Generic;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;

namespace miRobotEditor.Controls.ErrorProvider
{
    public sealed class ErrorProvider : FrameworkElement
    {
        private delegate void FoundBindingCallbackDelegate(FrameworkElement element, Binding binding);
        private FrameworkElement _firstInvalidElement;
        private readonly List<IErrorDisplayStrategy> _displayStrategies;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ErrorProvider()
        {
            _displayStrategies = new List<IErrorDisplayStrategy>();
            CreateDefaultDisplayStrategies();

            DataContextChanged += ErrorProvider_DataContextChanged;
            Loaded += ErrorProvider_Loaded;
        }

        /// <summary>
        /// Sets up the default inbuilt display strategies.
        /// </summary>
        private void CreateDefaultDisplayStrategies()
        {
            AddDisplayStrategy(new TextBoxErrorDisplayStrategy());
        }

        /// <summary>
        /// Called when this component is loaded. We have a call to Validate here that way errors appear from the very 
        /// moment the page or form is visible.
        /// </summary>
        private void ErrorProvider_Loaded(object sender, RoutedEventArgs e)
        {
            Validate();
        }

        /// <summary>
        /// Adds a display strategy used to display validation errors on a given control.
        /// </summary>
        /// <param name="strategy">The strategy to add.</param>
        public void AddDisplayStrategy(IErrorDisplayStrategy strategy)
        {
            if (strategy != null && _displayStrategies.Contains(strategy) == false)
            {
                _displayStrategies.Add(strategy);
            }
        }

        /// <summary>
        /// Removes a display strategy that was added using the AddDisplayStrategy pattern.
        /// </summary>
        /// <param name="strategy">The strategy to remove.</param>
        public void RemoveDisplayStrategy(IErrorDisplayStrategy strategy)
        {
            if (strategy != null && _displayStrategies.Contains(strategy))
            {
                _displayStrategies.Remove(strategy);
            }
        }

        /// <summary>
        /// Called when our DataContext changes.
        /// </summary>
        private void ErrorProvider_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged -= DataContext_PropertyChanged;
            }

            if (e.NewValue != null && e.NewValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged += DataContext_PropertyChanged;
            }

            Validate();
        }

        /// <summary>
        /// Validates all properties on the current data source.
        /// </summary>
        /// <returns>True if there are no errors displayed, otherwise false.</returns>
        /// <remarks>
        /// Note that only errors on properties that are displayed are included. Other errors, such as errors for properties that are not displayed, 
        /// will not be validated by this method.
        /// </remarks>
        public bool Validate()
        {
            bool isValid = true;
            _firstInvalidElement = null;

            var dataErrorInfo = DataContext as IDataErrorInfo;
            if (dataErrorInfo != null)
            {
                List<Binding> allKnownBindings = ClearInternal();

                // Now show all errors
                foreach (Binding knownBinding in allKnownBindings)
                {
                    string errorMessage = dataErrorInfo[knownBinding.Path.Path];
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        isValid = false;

                        // Display the error on any elements bound to the property
                        Binding binding1 = knownBinding;
                        FindBindingsRecursively(
                            Parent,
                            delegate(FrameworkElement element, Binding binding)
                                {
                                    if (binding1.Path.Path == binding.Path.Path)
                                    {
                                        // Figure out which display strategy should be used to display the error, then display it.
                                        for (var i = _displayStrategies.Count - 1; i >= 0; i--)
                                        {
                                            if (_displayStrategies[i].CanDisplayForElement(element))
                                            {
                                                _displayStrategies[i].DisplayError(element, errorMessage);
                                                if (_firstInvalidElement == null)
                                                {
                                                    _firstInvalidElement = element;
                                                }
                                                return;
                                            }
                                        }
                                    }
                                });
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Returns the first element that this error provider has labelled as invalid. This method 
        /// is useful to set the users focus on the first visible error field on a page.
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetFirstInvalidElement()
        {
            return _firstInvalidElement;
        }

        /// <summary>
        /// Clears any error messages.
        /// </summary>
        public void Clear()
        {
            ClearInternal();
        }

        /// <summary>
        /// Clears any error messages and returns a list of all bindings on the current form/page. This is simply so 
        /// it can be reused by the Validate method.
        /// </summary>
        /// <returns>A list of all known bindings.</returns>
        private List<Binding> ClearInternal()
        {
            // Clear all errors
            var bindings = new List<Binding>();
            FindBindingsRecursively(
                    Parent,
                    delegate(FrameworkElement element, Binding binding)
                    {
                        // Remember this bound element. We'll use this to display error messages for each property.
                        bindings.Add(binding);

                        // Clear any errors on this framework element.
                        for (var i = _displayStrategies.Count - 1; i >= 0; i--)
                        {
                            if (_displayStrategies[i].CanDisplayForElement(element))
                            {
                                _displayStrategies[i].ClearError(element);
                            }
                        }
                    });
            return bindings;
        }

        /// <summary>
        /// Called when the PropertyChanged event is raised from the object we are bound to - that is, our data context.
        /// </summary>
        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Validate();
        }

        /// <summary>
        /// Recursively goes through the control tree, looking for bindings on the current data context.
        /// </summary>
        /// <param name="element">The root element to start searching at.</param>
        /// <param name="callbackDelegate">A delegate called when a binding if found.</param>
        private void FindBindingsRecursively(DependencyObject element, FoundBindingCallbackDelegate callbackDelegate)
        {

            // See if we should display the errors on this element
            MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty);

            foreach (MemberInfo member in members)
            {
                DependencyProperty dp = null;

                // Check to see if the field or property we were given is a dependency property
                if (member.MemberType == MemberTypes.Field)
                {
                    var field = (FieldInfo)member;
                    if (typeof(DependencyProperty).IsAssignableFrom(field.FieldType))
                    {
                        dp = (DependencyProperty)field.GetValue(element);
                    }
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    var prop = (PropertyInfo)member;
                    if (typeof(DependencyProperty).IsAssignableFrom(prop.PropertyType))
                    {
                        dp = (DependencyProperty)prop.GetValue(element, null);
                    }
                }

                if (dp != null)
                {
                    // Awesome, we have a dependency property. does it have a binding? If yes, is it bound to the property we're interested in?
                    Binding bb = BindingOperations.GetBinding(element, dp);
                    var frameworkElement = element as FrameworkElement;
                    if (bb != null)
                    {
                        // This element has a DependencyProperty that we know of that is bound to the property we're interested in. 
                        // Now we just tell the callback and the caller will handle it.
                        if (frameworkElement != null)
                        {
                            if (frameworkElement.DataContext == DataContext)
                            {
                                callbackDelegate(frameworkElement, bb);
                            }
                        }
                    }
                }
            }

            // Now, recurse through any child elements
            if (element is FrameworkElement || element is FrameworkContentElement)
            {
                foreach (object childElement in LogicalTreeHelper.GetChildren(element))
                {
                    var dependencyObject = childElement as DependencyObject;
                    if (dependencyObject != null)
                    {
                        FindBindingsRecursively(dependencyObject, callbackDelegate);
                    }
                }
            }
        }
    }
}
