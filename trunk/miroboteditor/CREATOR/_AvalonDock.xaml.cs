using System;
using System.Linq;
using AvalonDock;
using System.Windows;
using AvalonDock.Layout;
namespace CREATOR
{
    /// <summary>
    /// Interaction logic for _AvalonDock.xaml
    /// </summary>
    public partial class _AvalonDock : Window
    {
        private int i = 0;
        public _AvalonDock()
        {
            InitializeComponent();
            this.DataContext = this;
        }
       

        private void AddDocument(object sender, RoutedEventArgs e)
        {
            // find number of documents
            var docpane = dock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane != null)
            {
                var doc = new LayoutDocument();
                i++;
                doc.ContentId = string.Format("Document#{0}", i);
                doc.Title = string.Format("Document#{0}", i);
                var ksr = new KUKAServoReader();               
                docs.Children.Add(doc);
            }
            var leftAnchorGroup = dock.Layout.LeftSide.Children.FirstOrDefault();
            if (leftAnchorGroup == null)
            {
                leftAnchorGroup = new LayoutAnchorGroup();
                dock.Layout.LeftSide.Children.Add(leftAnchorGroup);
            }

            leftAnchorGroup.Children.Add(new LayoutAnchorable() { Title = "New Anchorable" });
        }

        private void LayoutRoot_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var activecontent = ((LayoutRoot) sender).ActiveContent;
            if (e.PropertyName=="ActiveContent"&& activecontent!=null)
                Console.WriteLine(string.Format("ActiveContent->{0}",activecontent) );
        }


    }
}
