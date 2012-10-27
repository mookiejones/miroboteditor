using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using miRobotEditor.Core.Services;

namespace miRobotEditor.Core
{
    /// <summary>
    /// Allows printing using the IPrintable interface.
    /// </summary>
    public class WindowsFormsPrinting
    {
        /// <summary>
        /// Print
        /// </summary>
        /// <param name="printable"></param>
        public static void Print(IPrintable printable)
        {
            using (PrintDocument pdoc = printable.PrintDocument)
            {
                if (pdoc != null)
                {
                    using (var ppd = new PrintDialog())
                    {
                        ppd.Document = pdoc;
                        ppd.AllowSomePages = true;
                        if (ppd.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK)
                        { // fixed by Roger Rubin
                            pdoc.Print();
                        }
                    }
                }
                else
                {
                    MessageService.ShowError("Error on Printing Document");
                }
            }
        }

        /// <summary>
        /// Print Preview
        /// </summary>
        /// <param name="printable"></param>
        public static void PrintPreview(IPrintable printable)
        {
            using (PrintDocument pdoc = printable.PrintDocument)
            {
                if (pdoc != null)
                {
                    var ppd = new PrintPreviewDialog {TopMost = true, Document = pdoc};
                    ppd.Show(WorkbenchSingleton.MainWin32Window);
                }
                else
                {
                    MessageService.ShowError("Error on Printing Document");
                }
            }
        }
    }
}
