using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Controls;
namespace DMC_Robot_Editor.Controls
{
    class CustomToolTip:ToolTip,IDisposable
    {       
        public string Content
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public CustomToolTip()
        {
            base.Draw += CustomToolTip_Draw;
            base.Popup += CustomToolTip_Popup;
        }
        public bool IsOpen
        {
            get { return base.Active; }
            set { base.Active = value; }
        }
        void CustomToolTip_Popup(object sender, PopupEventArgs e)
        {
            using (Font f = new Font("Tahoma", 9))
            {
                e.ToolTipSize = TextRenderer.MeasureText(GetToolTip(e.AssociatedControl), f);
            }
        }

        void CustomToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            // Draw the ToolTip differently depending on which  
            // control this ToolTip is for. 
            // Draw a custom 3D border if the ToolTip is for button1. 
          // if (e.AssociatedControl == )
                e.DrawBackground();
                // Draw the standard background.
          // {

                // Draw the custom border to appear 3-dimensional.
                e.Graphics.DrawLines(SystemPens.ControlLightLight, new Point[] {
                    new Point (0, e.Bounds.Height - 1), 
                    new Point (0, 0), 
                    new Point (e.Bounds.Width - 1, 0)
                });
                e.Graphics.DrawLines(SystemPens.ControlDarkDark, new Point[] {
                    new Point (0, e.Bounds.Height - 1), 
                    new Point (e.Bounds.Width - 1, e.Bounds.Height - 1), 
                    new Point (e.Bounds.Width - 1, 0)
                });

                // Specify custom text formatting flags.
                TextFormatFlags sf = TextFormatFlags.VerticalCenter |
                                     TextFormatFlags.HorizontalCenter |
                                     TextFormatFlags.NoFullWidthCharacterBreak;

                // Draw the standard text with customized formatting options.
                e.DrawText(sf);
          // }
          // // Draw a custom background and text if the ToolTip is for button2. 
          // else if (e.AssociatedControl == button2)
          // {
          //     // Draw the custom background.
          //     e.Graphics.FillRectangle(SystemBrushes.ActiveCaption, e.Bounds);
          //
          //     // Draw the standard border.
          //     e.DrawBorder();
          //
          //     // Draw the custom text. 
          //     // The using block will dispose the StringFormat automatically. 
          //     using (StringFormat sf = new StringFormat())
          //     {
          //         sf.Alignment = StringAlignment.Center;
          //         sf.LineAlignment = StringAlignment.Center;
          //         sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
          //         sf.FormatFlags = StringFormatFlags.NoWrap;
          //         using (Font f = new Font("Tahoma", 9))
          //         {
          //             e.Graphics.DrawString(e.ToolTipText, f, 
          //                 SystemBrushes.ActiveCaptionText, e.Bounds, sf);
          //         }
          //     }
          // }
          // // Draw the ToolTip using default values if the ToolTip is for button3. 
          // else if (e.AssociatedControl == button3)
          // {
          //     e.DrawBackground();
          //     e.DrawBorder();
          //     e.DrawText();
          // }
        }
    }
}
