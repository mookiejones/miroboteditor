using System;
using System.Text;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public abstract class TokenFunction
    {
        private readonly string startDelimiter = string.Empty;
        private readonly string endDelimiter = ")}";
        protected TokenFunction(string tokenStartDelimiter)
        {
            if (string.IsNullOrEmpty(tokenStartDelimiter))
            {
                throw new ArgumentNullException("tokenStartDelimiter");
            }
            startDelimiter = tokenStartDelimiter;
        }
        protected TokenFunction(string tokenStartDelimiter, string tokenEndDelimiter)
        {
            if (string.IsNullOrEmpty(tokenStartDelimiter))
            {
                throw new ArgumentNullException("tokenStartDelimiter");
            }
            if (string.IsNullOrEmpty(tokenEndDelimiter))
            {
                throw new ArgumentNullException("tokenEndDelimiter");
            }
            startDelimiter = tokenStartDelimiter;
            endDelimiter = tokenEndDelimiter;
        }
        public virtual void Format(StringBuilder messageBuilder, LogEntry log)
        {
            int i = 0;
            while (i < messageBuilder.Length)
            {
                string text = messageBuilder.ToString();
                if (text.IndexOf(startDelimiter) == -1)
                {
                    return;
                }
                string innerTemplate = GetInnerTemplate(i, text);
                string text2 = startDelimiter + innerTemplate + endDelimiter;
                i = messageBuilder.ToString().IndexOf(text2);
                string newValue = FormatToken(innerTemplate, log);
                messageBuilder.Replace(text2, newValue);
            }
        }
        public abstract string FormatToken(string tokenTemplate, LogEntry log);
        protected virtual string GetInnerTemplate(int startPos, string message)
        {
            int num = message.IndexOf(startDelimiter, startPos) + startDelimiter.Length;
            int num2 = message.IndexOf(endDelimiter, num);
            return message.Substring(num, num2 - num);
        }
    }
}
