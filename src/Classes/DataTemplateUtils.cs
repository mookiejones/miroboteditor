/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/15/2013
 * Time: 2:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
namespace miRobotEditor.Classes
{
	   /// <summary>
    /// Helper class for lookup of data-templates.
    /// </summary>
    internal static class DataTemplateUtils
    {
        /// <summary>
        /// Find a DataTemplate for the specified type in the visual-tree.
        /// </summary>
        public static DataTemplate FindDataTemplate(Type type, FrameworkElement element)
        {
            var dataTemplate = element.TryFindResource(new DataTemplateKey(type)) as DataTemplate;
            if (dataTemplate != null)
            {
                return dataTemplate;
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                dataTemplate = FindDataTemplate(type.BaseType, element);
                if (dataTemplate != null)
                {
                    return dataTemplate;
                }
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                dataTemplate = FindDataTemplate(interfaceType, element);
                if (dataTemplate != null)
                {
                    return dataTemplate;
                }
            }

            return null;
        }

        /// <summary>
        /// Find a data-template for the specified type and instance a visual from it.
        /// </summary>
        public static FrameworkElement InstanceTemplate(Type type, FrameworkElement element, object dataContext)
        {
            var dataTemplate = FindDataTemplate(type, element);
            return dataTemplate == null ? null : InstanceTemplate(dataTemplate, dataContext);
        }

        /// <summary>
        /// Instance a visual element from a data template.
        /// </summary>
        public static FrameworkElement InstanceTemplate(DataTemplate dataTemplate, object dataContext)
        {
            var element = (FrameworkElement)dataTemplate.LoadContent();
            element.DataContext = dataContext;
            return element;
        }
    }
}
