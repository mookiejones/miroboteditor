/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:editor.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using CommunityToolkit.Mvvm.DependencyInjection;

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

        public ViewModelLocator()
        {

        }
        public ObjectBrowserViewModel ObjectBrowser => Ioc.Default.GetRequiredService<ObjectBrowserViewModel>();

        public StatusBarViewModel StatusBar => Ioc.Default.GetRequiredService<StatusBarViewModel>();

        /// <summary>
        ///     Gets the Main property.
        /// </summary>
        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}