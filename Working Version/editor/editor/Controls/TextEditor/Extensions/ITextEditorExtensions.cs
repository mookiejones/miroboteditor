using System.Collections.Generic;

namespace miRobotEditor.Controls.TextEditor.Extensions
{
    public interface ITextEditorExtensions
    {
        IEnumerable<T> Get<T>();

        IEnumerable<T> Get<T>(string extension);
    }
}