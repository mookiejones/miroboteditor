using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
namespace DMC_Robot_Editor.Globals.Exceptionhandling
{
    public class TextExceptionFormatter : ExceptionFormatter
    {
        private string exceptionFullName;
        private int innerDepth;
        public TextWriter Writer { get; private set; }
        private string FullName
        {
            get {
                return exceptionFullName ??
                       (exceptionFullName =
                        string.Format(Properties.Culture, Properties.ExceptionWasCaught,
                                      new object[] {Exception.GetType().FullName}));
            }
        }
        public TextExceptionFormatter(TextWriter writer, Exception exception)
            : base(exception)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            Writer = writer;
        }
        private void WriteSeparator()
        {
            var value = new string('-', FullName.Length);
            Writer.WriteLine(value);
        }
        public override void Format()
        {
            WriteSeparator();
            WriteDescription();
            WriteDateTime(DateTime.UtcNow);
            WriteException(Exception, null);
            WriteSeparator();
        }
        protected override void WriteDescription()
        {
            Writer.WriteLine(FullName);
        }
        protected override void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            if (outerException != null)
            {
                innerDepth++;
                Indent();
                string innerException = Properties.InnerException;
                var value = new string('-', innerException.Length);
                Writer.WriteLine(innerException);
                Indent();
                Writer.WriteLine(value);
                base.WriteException(exceptionToFormat, outerException);
                innerDepth--;
                return;
            }
            base.WriteException(exceptionToFormat, outerException);
        }
        protected override void WriteDateTime(DateTime utcNow)
        {
            string value = utcNow.ToLocalTime().ToString("G", DateTimeFormatInfo.InvariantInfo);
            Writer.WriteLine(value);
        }
        protected override void WriteExceptionType(Type exceptionType)
        {
            IndentAndWriteLine(Properties.TypeString, new object[]
			{
				exceptionType.AssemblyQualifiedName
			});
        }
        protected override void WriteMessage(string message)
        {
            IndentAndWriteLine(Properties.Message, new object[]
			{
				message
			});
        }
        protected override void WriteSource(string source)
        {
            IndentAndWriteLine(Properties.Source, new object[]
			{
				source
			});
        }
        protected override void WriteHelpLink(string helpLink)
        {
            IndentAndWriteLine(Properties.HelpLink, new object[]
			{
				helpLink
			});
        }
        protected override void WritePropertyInfo(PropertyInfo propertyInfo, object value)
        {
            Indent();
            Writer.Write(propertyInfo.Name);
            Writer.Write(" : ");
            Writer.WriteLine(value);
        }
        protected override void WriteFieldInfo(FieldInfo fieldInfo, object value)
        {
            Indent();
            Writer.Write(fieldInfo.Name);
            Writer.Write(" : ");
            Writer.WriteLine(value);
        }
        protected override void WriteStackTrace(string stackTrace)
        {
            Indent();
            Writer.Write(Properties.StackTrace);
            Writer.Write(" : ");
            if (string.IsNullOrEmpty(stackTrace))
            {
                Writer.WriteLine(Properties.StackTraceUnavailable);
                return;
            }
            var str = new string('\t', innerDepth);
            string value = stackTrace.Replace("\n", "\n" + str);
            Writer.WriteLine(value);
            Writer.WriteLine();
        }
        protected override void WriteAdditionalInfo(NameValueCollection additionalInformation)
        {
            Writer.WriteLine(Properties.AdditionalInfo);
            Writer.WriteLine();
            string[] allKeys = additionalInformation.AllKeys;
            for (var i = 0; i < allKeys.Length; i++)
            {
                string text = allKeys[i];
                Writer.Write(text);
                Writer.Write(" : ");
                Writer.Write(additionalInformation[text]);
                Writer.Write("\n");
            }
        }
        protected virtual void Indent()
        {
            for (var i = 0; i < innerDepth; i++)
            {
                Writer.Write("\t");
            }
        }
        private void IndentAndWriteLine(string format, params object[] arg)
        {
            Indent();
            Writer.WriteLine(format, arg);
        }
    }
}
