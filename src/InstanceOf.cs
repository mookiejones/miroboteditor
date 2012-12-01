using miRobotEditor.GUI;

namespace miRobotEditor
{
    /// <summary>
    /// Trying to Only Have one _instance of Each here
    /// </summary>
    class InstanceOf
    {
        private static Editor _ieditor;
        public static Editor IEditor
        {
            get { return _ieditor ?? (_ieditor = new Editor()); }
            set { _ieditor = value; }
        }

        private static DummyDoc _idocument;
        public static DummyDoc IDocument
        {
            get { return _idocument ?? (_idocument = new DummyDoc()); }
            set { _idocument = value; }
        }
    }
}
