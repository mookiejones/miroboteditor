using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace DMC_Robot_Editor.GUI.Printing
{
    /// <summary>
    /// Represents the PrintPreviewDialog class to preview documents 
    /// of type FlowDocument, IDocumentPaginatorSource or DocumentPaginatorWrapper
    /// using the PrintPreviewDocumentViewer class.
    /// </summary>
    public partial class PrintPreview : Window
    {
        private object _document;

        public PrintPreview()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the document viewer.
        /// </summary>
        public PrintPreviewDocumentViewer DocumentViewer { get; set; }



        /// <summary>

        /// Loads the specified FlowDocument document for print preview.

        /// </summary>

        public void LoadDocument(FlowDocument document)
        {

            _document = document;

            var temp = System.IO.Path.GetTempFileName();

            if (File.Exists(temp))
                File.Delete(temp);

            XpsDocument xpsDoc = new XpsDocument(temp, FileAccess.ReadWrite);
            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

            xpsWriter.Write(((FlowDocument)document as IDocumentPaginatorSource).DocumentPaginator);

            DocumentViewer.Document = xpsDoc.GetFixedDocumentSequence();

            xpsDoc.Close();

        }



        /// <summary>

        /// Loads the specified DocumentPaginatorWrapper document for print preview.

        /// </summary>

        public void LoadDocument(DocumentPaginatorWrapper document)
        {

            _document = document;

            string temp = System.IO.Path.GetTempFileName();



            if (File.Exists(temp) == true)

                File.Delete(temp);



            XpsDocument xpsDoc = new XpsDocument(temp, FileAccess.ReadWrite);

            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

            xpsWriter.Write(document);

            DocumentViewer.Document = xpsDoc.GetFixedDocumentSequence();
            xpsDoc.Close();

        }



        /// <summary>

        /// Loads the specified IDocumentPaginatorSource document for print preview.

        /// </summary>

        public void LoadDocument(IDocumentPaginatorSource document)
        {
            _document = document;
            DocumentViewer.Document = (IDocumentPaginatorSource)document;
        }



       

    }
}
