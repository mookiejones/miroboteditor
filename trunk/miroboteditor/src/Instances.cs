using miRobotEditor.Controls;

namespace miRobotEditor
{
    /// <summary>
    /// Trying to Only Have one _instance of Each here
    /// </summary>
    class InstanceOf
    {
        private static Editor _IEditor;
        public static Editor IEditor
        {
            get { return _IEditor ?? (_IEditor = new Editor()); }
            set { _IEditor = value; }
        }

        private static DummyDoc _IDocument;
        public static DummyDoc IDocument
        {
            get { return _IDocument ?? (_IDocument = new DummyDoc()); }
            set { _IDocument = value; }
        }





    }
}
