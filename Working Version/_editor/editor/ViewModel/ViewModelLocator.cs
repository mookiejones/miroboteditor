/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:editor.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System.Diagnostics.CodeAnalysis;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc; 
using miRobotEditor.Design;
using miRobotEditor.Model;

namespace miRobotEditor.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public sealed class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }
            SimpleIoc.Default.Register<StatusBarViewModel>();
            SimpleIoc.Default.Register<ObjectBrowserViewModel>();
            SimpleIoc.Default.Register<MainViewModel>(true);
        }

        public ObjectBrowserViewModel ObjectBrowser
        {
            get { return ServiceLocator.Current.GetInstance<ObjectBrowserViewModel>(); }
        }

        public StatusBarViewModel StatusBar
        {
            get { return ServiceLocator.Current.GetInstance<StatusBarViewModel>(); }
        }

        /// <summary>
        ///     Gets the Main property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}