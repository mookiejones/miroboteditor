﻿/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/16/2013
 * Time: 09:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.ComponentModel;
using System.Linq;
using miRobotEditor.Core;
using miRobotEditor.ViewModel;
using Xceed.Wpf.AvalonDock.Layout;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public class LayoutInitializer : ILayoutUpdateStrategy
    {
        public LayoutInitializer()
        {
            Strategy = this;
        }

        public static ILayoutUpdateStrategy Strategy { get; set; }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow,
            ILayoutContainer destinationContainer)
        {
            object content = anchorableToShow.Content;
            //AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;
            LayoutAnchorablePane bottomPane =
                layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "BottomPane");
            LayoutAnchorablePane leftPane =
                layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "LeftPane");
            LayoutAnchorablePane rightPane =
                layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "RightPane");


            switch (((ToolViewModel) content).DefaultPane)
            {
                case DefaultToolPane.Bottom:
                    if (bottomPane != null)
                    {
                        bottomPane.Children.Add(anchorableToShow);
                        return true;
                    }
                    break;
                case DefaultToolPane.Left:
                    if (leftPane != null)
                    {
                        leftPane.Children.Add(anchorableToShow);
                        return true;
                    }
                    break;
                case DefaultToolPane.Right:
                    if (rightPane != null)
                    {
                        rightPane.Children.Add(anchorableToShow);
                        return true;
                    }

                    break;
            }

            //TODO Need to Expand Side to meet width


            LayoutAnchorablePane toolsPane =
                layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "ToolsPane");
            if (toolsPane != null)
            {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;
        }


        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
            var content = anchorableShown.Content as ToolViewModel;
            if (content == null) return;

            switch ((content.DefaultPane))
            {
                case DefaultToolPane.Bottom:
                    anchorableShown.AutoHideMinHeight = content.Height;
                    anchorableShown.AutoHideHeight = 100;
                    break;
                case DefaultToolPane.Right:
                case DefaultToolPane.Left:
                    anchorableShown.AutoHideMinWidth = content.Width;
                    anchorableShown.AutoHideWidth = content.Width;
                    break;
            }
        }


        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow,
            ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }
    }
}