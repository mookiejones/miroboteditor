using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace miRobotEditor.Core.Services
{
    /// <summary>
    /// Creates WPF menu controls from the AddIn Tree.
    /// </summary>
    public static class MenuService
    {
        internal sealed class MenuCreateContext
        {
            public UIElement InputBindingOwner;
            public string ActivationMethod;
            public bool ImmediatelyExpandMenuBuildersForShortcuts;
        }

        static Dictionary<string, System.Windows.Input.ICommand> knownCommands = LoadDefaultKnownCommands();

        static Dictionary<string, System.Windows.Input.ICommand> LoadDefaultKnownCommands()
        {
            var knownCommands = new Dictionary<string, System.Windows.Input.ICommand>();
            foreach (Type t in new Type[] { typeof(ApplicationCommands), typeof(NavigationCommands) })
            {
                foreach (PropertyInfo p in t.GetProperties())
                {
                    knownCommands.Add(p.Name, (System.Windows.Input.ICommand)p.GetValue(null, null));
                }
            }
            return knownCommands;
        }

        

        /// <summary>
        /// Registers a WPF command for use with the &lt;MenuItem command="name"&gt; syntax.
        /// </summary>
        public static void RegisterKnownCommand(string name, System.Windows.Input.ICommand command)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (command == null)
                throw new ArgumentNullException("command");
            lock (knownCommands)
            {
                knownCommands.Add(name, command);
            }
        }

        public static void UpdateStatus(IEnumerable menuItems)
        {
            if (menuItems == null)
                return;
            foreach (object o in menuItems)
            {
                IStatusUpdate cmi = o as IStatusUpdate;
                if (cmi != null)
                    cmi.UpdateStatus();
            }
        }

        public static void UpdateText(IEnumerable menuItems)
        {
            if (menuItems == null)
                return;
            foreach (object o in menuItems)
            {
                IStatusUpdate cmi = o as IStatusUpdate;
                if (cmi != null)
                    cmi.UpdateText();
            }
        }

      
        public static ContextMenu ShowContextMenu(UIElement parent, object owner, string addInTreePath)
        {
            var menu = new ContextMenu();
            menu.ItemsSource = CreateMenuItems(menu, owner, addInTreePath, "ContextMenu");
            menu.PlacementTarget = parent;
            menu.IsOpen = true;
            return menu;
        }

        internal static ContextMenu CreateContextMenu(IList subItems)
        {
            var contextMenu = new ContextMenu()
            {
                ItemsSource = new object[1]
            };
            contextMenu.Opened += (sender, args) =>
            {
                contextMenu.ItemsSource = ExpandMenuBuilders(subItems, true);
                args.Handled = true;
            };
            return contextMenu;
        }

        public static IList CreateMenuItems(UIElement inputBindingOwner, object owner, string addInTreePath, string activationMethod = null, bool immediatelyExpandMenuBuildersForShortcuts = false)
        {
            IList items = CreateUnexpandedMenuItems(
                new MenuCreateContext
                {
                    InputBindingOwner = inputBindingOwner,
                    ActivationMethod = activationMethod,
                    ImmediatelyExpandMenuBuildersForShortcuts = immediatelyExpandMenuBuildersForShortcuts
                },
           
            return ExpandMenuBuilders(items, false);
        }

        sealed class MenuItemBuilderPlaceholder
        {
            }

        internal static IList CreateUnexpandedMenuItems(MenuCreateContext context, IEnumerable descriptors)
        {
            ArrayList result = new ArrayList();
            
            return result;
        }

        static IList ExpandMenuBuilders(ICollection input, bool addDummyEntryIfMenuEmpty)
        {
            var result = new ArrayList(input.Count);
            foreach (object o in input)
            {
                var p = o as MenuItemBuilderPlaceholder;
                if (p != null)
                {
                    
                   
                }
                else
                {
                    result.Add(o);
                    var statusUpdate = o as IStatusUpdate;
                    if (statusUpdate != null)
                    {
                        statusUpdate.UpdateStatus();
                        statusUpdate.UpdateText();
                    }
                }
            }
            if (addDummyEntryIfMenuEmpty && result.Count == 0)
            {
                result.Add(new MenuItem { Header = "(empty menu)", IsEnabled = false });
            }
            return result;
        }



        /// <summary>
        /// Converts from the Windows-Forms style label format (accessor key marked with '&amp;')
        /// to a WPF label format (accessor key marked with '_').
        /// </summary>
        public static string ConvertLabel(string label)
        {
            return label.Replace("_", "__").Replace("&", "_");
        }

        // HACK: find a better way to allow the host app to process link commands
        public static Func<string, ICommand> LinkCommandCreator { get; set; }

        /// <summary>
        /// Creates an KeyGesture for a shortcut.
        /// </summary>
        public static KeyGesture ParseShortcut(string text)
        {
            return (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString(text.Replace(',', '+').Replace('|', '+'));
        }

   
     
    }
}
