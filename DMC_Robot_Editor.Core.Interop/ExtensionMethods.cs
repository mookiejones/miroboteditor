using System.Xml.Linq;
using miRobotEditor.Core.Services;

namespace miRobotEditor.Core
{
    // Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
    // This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Media;
    using System.Xml;
    using WinForms = System.Windows.Forms;

        /// <summary>
        /// Extension methods used in SharpDevelop.
        /// </summary>
        public static class ExtensionMethods
        {
            /// <summary>
            /// Raises the event.
            /// Does nothing if eventHandler is null.
            /// Because the event handler is passed as parameter, it is only fetched from the event field one time.
            /// This makes
            /// <code>MyEvent.RaiseEvent(x,y);</code>
            /// thread-safe
            /// whereas
            /// <code>if (MyEvent != null) MyEvent(x,y);</code>
            /// would not be safe.
            /// </summary>
            /// <remarks>Using this method is only thread-safe under the Microsoft .NET memory model,
            /// not under the less strict memory model in the CLI specification.</remarks>
            public static void RaiseEvent(this EventHandler eventHandler, object sender, System.EventArgs e)
            {
                if (eventHandler != null)
                {
                    eventHandler(sender, e);
                }
            }

            /// <summary>
            /// Raises the event.
            /// Does nothing if eventHandler is null.
            /// Because the event handler is passed as parameter, it is only fetched from the event field one time.
            /// This makes
            /// <code>MyEvent.RaiseEvent(x,y);</code>
            /// thread-safe
            /// whereas
            /// <code>if (MyEvent != null) MyEvent(x,y);</code>
            /// would not be safe.
            /// </summary>
            public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : System.EventArgs
            {
                if (eventHandler != null)
                {
                    eventHandler(sender, e);
                }
            }

            /// <summary>
            /// Runs an action for all elements in the input.
            /// </summary>
            public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
            {
                if (input == null)
                    throw new ArgumentNullException("input");
                foreach (T element in input)
                {
                    action(element);
                }
            }

            /// <summary>
            /// Adds all <paramref name="elements"/> to <paramref name="list"/>.
            /// </summary>
            public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
            {
                foreach (T o in elements)
                    list.Add(o);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="arrayList"></param>
            /// <param name="elements"></param>
            public static void AddRange(this IList arrayList, IEnumerable elements)
            {
                foreach (object o in elements)
                    arrayList.Add(o);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="arr"></param>
            /// <returns></returns>
            public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> arr)
            {
                return new ReadOnlyCollection<T>(arr);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="arr"></param>
            /// <returns></returns>
            public static ReadOnlyCollectionWrapper<T> AsReadOnly<T>(this ICollection<T> arr)
            {
                return new ReadOnlyCollectionWrapper<T>(arr);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="collection"></param>
            /// <returns></returns>
            public static IEnumerable<WinForms.Control> GetRecursive(this WinForms.Control.ControlCollection collection)
            {
                return collection.Cast<WinForms.Control>().Flatten(c => c.Controls.Cast<WinForms.Control>());
            }

            /// <summary>
            /// Converts a recursive data structure into a flat list.
            /// </summary>
            /// <param name="input">The root elements of the recursive data structure.</param>
            /// <param name="recursion">The function that gets the children of an element.</param>
            /// <returns>Iterator that enumerates the tree structure in preorder.</returns>
            public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
            {
                var stack = new Stack<IEnumerator<T>>();
                try
                {
                    stack.Push(input.GetEnumerator());
                    while (stack.Count > 0)
                    {
                        while (stack.Peek().MoveNext())
                        {
                            T element = stack.Peek().Current;
                            yield return element;
                            IEnumerable<T> children = recursion(element);
                            if (children != null)
                            {
                                stack.Push(children.GetEnumerator());
                            }
                        }
                        stack.Pop().Dispose();
                    }
                }
                finally
                {
                    while (stack.Count > 0)
                    {
                        stack.Pop().Dispose();
                    }
                }
            }

            /// <summary>
            /// Creates an array containing a part of the array (similar to string.Substring).
            /// </summary>
            public static T[] Splice<T>(this T[] array, int startIndex)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                return Splice(array, startIndex, array.Length - startIndex);
            }

            /// <summary>
            /// Creates an array containing a part of the array (similar to string.Substring).
            /// </summary>
            public static T[] Splice<T>(this T[] array, int startIndex, int length)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                if (startIndex < 0 || startIndex > array.Length)
                    throw new ArgumentOutOfRangeException("startIndex", startIndex, "Value must be between 0 and " + array.Length);
                if (length < 0 || length > array.Length - startIndex)
                    throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (array.Length - startIndex));
                var result = new T[length];
                Array.Copy(array, startIndex, result, 0, length);
                return result;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="K"></typeparam>
            /// <param name="input"></param>
            /// <param name="keySelector"></param>
            /// <returns></returns>
            public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> input, Func<T, K> keySelector)
            {
                return input.Distinct(KeyComparer.Create(keySelector));
            }

            /// <summary>
            /// Sets the Content property of the specified ControlControl to the specified content.
            /// If the content is a Windows-Forms control, it is wrapped in a WindowsFormsHost.
            /// If the content control already contains a WindowsFormsHost with that content,
            /// the old WindowsFormsHost is kept.
            /// When a WindowsFormsHost is replaced with another content, the host is disposed (but the control
            /// inside the host isn't)
            /// </summary>
            public static void SetContent(this ContentControl contentControl, object content)
            {
                SetContent(contentControl, content, null);
            }

            public static void SetContent(this ContentPresenter contentControl, object content)
            {
                SetContent(contentControl, content, null);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="contentControl"></param>
            /// <param name="content"></param>
            /// <param name="serviceObject"></param>
            public static void SetContent(this ContentControl contentControl, object content, object serviceObject)
            {
                if (contentControl == null)
                    throw new ArgumentNullException("contentControl");
                // serviceObject = object implementing the old clipboard/undo interfaces
                // to allow WinForms AddIns to handle WPF commands

                var host = contentControl.Content as WindowsFormsHost;
                if (host != null)
                {
                    if (host.Child == content)
                    {
                        host.ServiceObject = serviceObject;
                        return;
                    }
                    host.Dispose();
                }
                if (content is WinForms.Control)
                {
                    contentControl.Content = new WindowsFormsHost
                    {
                        Child = (WinForms.Control)content,ServiceObject = serviceObject,DisposeChild = false
                    };
                }
                else if (content is string)
                {
                    contentControl.Content = new TextBlock
                    {
                        Text = content.ToString(),
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    contentControl.Content = content;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="contentControl"></param>
            /// <param name="content"></param>
            /// <param name="serviceObject"></param>
            public static void SetContent(this ContentPresenter contentControl, object content, object serviceObject)
            {
                if (contentControl == null)
                    throw new ArgumentNullException("contentControl");
                // serviceObject = object implementing the old clipboard/undo interfaces
                // to allow WinForms AddIns to handle WPF commands

                var host = contentControl.Content as WindowsFormsHost;
                if (host != null)
                {
                    if (host.Child == content)
                    {
                        host.ServiceObject = serviceObject;
                        return;
                    }
                    host.Dispose();
                }
                if (content is WinForms.Control)
                {
                    contentControl.Content = new WindowsFormsHost
                    {
                        Child = (WinForms.Control)content,
                        ServiceObject = serviceObject,
                        DisposeChild = false
                    };
                }
                else if (content is string)
                {
                    contentControl.Content = new TextBlock
                    {
                        Text = content.ToString(),
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    contentControl.Content = content;
                }
            }

            #region System.Drawing <-> WPF conversions
            /// <summary>
            /// 
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public static System.Drawing.Point ToSystemDrawing(this Point p)
            {
                return new System.Drawing.Point((int)p.X, (int)p.Y);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public static System.Drawing.Size ToSystemDrawing(this Size s)
            {
                return new System.Drawing.Size((int)s.Width, (int)s.Height);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            public static System.Drawing.Rectangle ToSystemDrawing(this Rect r)
            {
                return new System.Drawing.Rectangle(r.TopLeft.ToSystemDrawing(), r.Size.ToSystemDrawing());
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static System.Drawing.Color ToSystemDrawing(this Color c)
            {
                return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public static Point ToWpf(this System.Drawing.Point p)
            {
                return new Point(p.X, p.Y);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public static Size ToWpf(this System.Drawing.Size s)
            {
                return new Size(s.Width, s.Height);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="rect"></param>
            /// <returns></returns>
            public static Rect ToWpf(this System.Drawing.Rectangle rect)
            {
                return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static Color ToWpf(this System.Drawing.Color c)
            {
                return Color.FromArgb(c.A, c.R, c.G, c.B);
            }
            #endregion

            #region DPI independence
            /// <summary>
            /// 
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="visual"></param>
            /// <returns></returns>
            public static Rect TransformToDevice(this Rect rect, Visual visual)
            {
                var presentationSource = PresentationSource.FromVisual(visual);
                if (presentationSource != null)
                {
                    if (presentationSource.CompositionTarget != null)
                    {
                        var matrix = presentationSource.CompositionTarget.TransformToDevice;
                        return Rect.Transform(rect, matrix);
                    }
                }
                return new Rect();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="visual"></param>
            /// <returns></returns>
            public static Rect TransformFromDevice(this Rect rect, Visual visual)
            {
                var presentationSource = PresentationSource.FromVisual(visual);
               
                        var matrix = presentationSource.CompositionTarget.TransformFromDevice;
                        return Rect.Transform(rect, matrix);
                    
               
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="size"></param>
            /// <param name="visual"></param>
            /// <returns></returns>
            public static Size TransformToDevice(this Size size, Visual visual)
            {
                var presentationSource = PresentationSource.FromVisual(visual);
                
                    var matrix = presentationSource.CompositionTarget.TransformToDevice;
                    return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
                
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="size"></param>
            /// <param name="visual"></param>
            /// <returns></returns>
            public static Size TransformFromDevice(this Size size, Visual visual)
            {
                Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
                return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <param name="visual"></param>
            /// <returns></returns>
            public static Point TransformToDevice(this Point point, Visual visual)
            {
                Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
                return matrix.Transform(point);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <param name="visual"></param>
            /// <returns></returns>
            public static Point TransformFromDevice(this Point point, Visual visual)
            {
                var matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
                return matrix.Transform(point);
            }
            #endregion

            /// <summary>
            /// Removes <param name="stringToRemove" /> from the start of this string.
            /// Throws ArgumentException if this string does not start with <param name="stringToRemove" />.
            /// </summary>
            public static string RemoveFromStart(this string s, string stringToRemove)
            {
                if (s == null)
                    return null;
                if (string.IsNullOrEmpty(stringToRemove))
                    return s;
                if (!s.StartsWith(stringToRemove))
                    throw new ArgumentException(string.Format("{0} does not start with {1}", s, stringToRemove));
                return s.Substring(stringToRemove.Length);
            }

            /// <summary>
            /// Removes <paramref name="stringToRemove" /> from the end of this string.
            /// Throws ArgumentException if this string does not end with <paramref name="stringToRemove" />.
            /// </summary>
            public static string RemoveFromEnd(this string s, string stringToRemove)
            {
                if (s == null) return null;
                if (string.IsNullOrEmpty(stringToRemove))
                    return s;
                if (!s.EndsWith(stringToRemove))
                    throw new ArgumentException(string.Format("{0} does not end with {1}", s, stringToRemove));
                return s.Substring(0, s.Length - stringToRemove.Length);
            }

            /// <summary>
            /// Trims the string from the first occurence of <paramref name="cutoffStart" /> to the end, including <paramref name="cutoffStart" />.
            /// If the string does not contain <paramref name="cutoffStart" />, just returns the original string.
            /// </summary>
            public static string CutoffEnd(this string s, string cutoffStart)
            {
                if (s == null) return null;
                var pos = s.IndexOf(cutoffStart);
                return pos != -1 ? s.Substring(0, pos) : s;
            }

            /// <summary>
            /// Takes at most <param name="length" /> first characters from string.
            /// String can be null.
            /// </summary>
            public static string TakeStart(this string s, int length)
            {
                if (string.IsNullOrEmpty(s) || length >= s.Length)
                    return s;
                return s.Substring(0, length);
            }

            /// <summary>
            /// Takes at most <param name="length" /> first characters from string, and appends '...' if string is longer.
            /// String can be null.
            /// </summary>
            public static string TakeStartEllipsis(this string s, int length)
            {
                if (string.IsNullOrEmpty(s) || length >= s.Length)
                    return s;
                return s.Substring(0, length) + "...";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="original"></param>
            /// <param name="pattern"></param>
            /// <param name="replacement"></param>
            /// <param name="comparisonType"></param>
            /// <returns></returns>
            public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
            {
                if (original == null)
                    throw new ArgumentNullException("original");
                if (pattern == null)
                    throw new ArgumentNullException("pattern");
                if (pattern.Length == 0)
                    throw new ArgumentException("String cannot be of zero length.", "pattern");
                if (comparisonType != StringComparison.Ordinal && comparisonType != StringComparison.OrdinalIgnoreCase)
                    throw new NotSupportedException("Currently only ordinal comparisons are implemented.");

                var result = new StringBuilder(original.Length);
                int currentPos = 0;
                int nextMatch = original.IndexOf(pattern, comparisonType);
                while (nextMatch >= 0)
                {
                    result.Append(original, currentPos, nextMatch - currentPos);
                    // The following line restricts this method to ordinal comparisons:
                    // for non-ordinal comparisons, the match length might be different than the pattern length.
                    currentPos = nextMatch + pattern.Length;
                    result.Append(replacement);

                    nextMatch = original.IndexOf(pattern, currentPos, comparisonType);
                }

                result.Append(original, currentPos, original.Length - currentPos);
                return result.ToString();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="encoding"></param>
            /// <param name="text"></param>
            /// <returns></returns>
            public static byte[] GetBytesWithPreamble(this Encoding encoding, string text)
            {
                byte[] encodedText = encoding.GetBytes(text);
                byte[] bom = encoding.GetPreamble();
                if (bom != null && bom.Length > 0)
                {
                    byte[] result = new byte[bom.Length + encodedText.Length];
                    bom.CopyTo(result, 0);
                    encodedText.CopyTo(result, bom.Length);
                    return result;
                }
                else
                {
                    return encodedText;
                }
            }

            /// <summary>
            /// Creates a new image for the image source.
            /// </summary>
            public static Image CreateImage(this IImage image)
            {
                if (image == null)
                    throw new ArgumentNullException("image");
                return new Image { Source = image.ImageSource };
            }

            /// <summary>
            /// Creates a new image for the image source.
            /// </summary>
            [ObsoleteAttribute("Use layout rounding instead")]
            public static UIElement CreatePixelSnappedImage(this IImage image)
            {
                return CreateImage(image);
            }

            /// <summary>
            /// Translates a WinForms menu to WPF.
            /// </summary>
            public static ICollection TranslateToWpf(this ToolStripItem[] items)
            {
                return items.OfType<ToolStripMenuItem>().Select(item => TranslateMenuItemToWpf(item)).ToList();
            }

            static System.Windows.Controls.MenuItem TranslateMenuItemToWpf(ToolStripMenuItem item)
            {
                var r = new System.Windows.Controls.MenuItem();
                
                return r;
            }

            /// <summary>
            /// Returns the index of the first element for which <paramref name="predicate"/> returns true.
            /// If none of the items in the list fits the <paramref name="predicate"/>, -1 is returned.
            /// </summary>
            public static int FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (predicate(list[i]))
                        return i;
                }

                return -1;
            }

            /// <summary>
            /// Adds item to the list if the item is not null.
            /// </summary>
            public static void AddIfNotNull<T>(this IList<T> list, T itemToAdd)
            {
                if (itemToAdd != null)
                    list.Add(itemToAdd);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="condition"></param>
            public static void RemoveWhere<T>(this IList<T> list, Predicate<T> condition)
            {
                if (list == null)
                    throw new ArgumentNullException("list");
                int i = 0;
                while (i < list.Count)
                {
                    if (condition(list[i]))
                        list.RemoveAt(i);
                    else
                        i++;
                }
            }

          
            public static void WriteTo(this Stream sourceStream, Stream targetStream)
            {
                byte[] buffer = new byte[4096];
                int bytes;
                while ((bytes = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    targetStream.Write(buffer, 0, bytes);
            }

            public static XElement FormatXml(this XElement element, int indentationLevel)
            {
                StringWriter sw = new StringWriter();
                using (XmlTextWriter xmlW = new XmlTextWriter(sw))
                {
                    if (EditorControlService.GlobalOptions.ConvertTabsToSpaces)
                    {
                        xmlW.IndentChar = ' ';
                        xmlW.Indentation = EditorControlService.GlobalOptions.IndentationSize;
                    }
                    else
                    {
                        xmlW.Indentation = 1;
                        xmlW.IndentChar = '\t';
                    }
                    xmlW.Formatting = Formatting.Indented;
                    element.WriteTo(xmlW);
                }
                string xmlText = sw.ToString();
                xmlText = xmlText.Replace(sw.NewLine, sw.NewLine + GetIndentation(indentationLevel));
                return XElement.Parse(xmlText, LoadOptions.PreserveWhitespace);
            }

            static string GetIndentation(int level)
            {
                StringBuilder indentation = new StringBuilder();
                for (int i = 0; i < level; i++)
                {
                    indentation.Append(EditorControlService.GlobalOptions.IndentationString);
                }
                return indentation.ToString();
            }

            public static XElement AddWithIndentation(this XElement element, XElement newContent)
            {
                int indentationLevel = 0;
                XElement tmp = element;
                while (tmp != null)
                {
                    tmp = tmp.Parent;
                    indentationLevel++;
                }
                if (!element.Nodes().Any())
                {
                    element.Add(new XText(Environment.NewLine + GetIndentation(indentationLevel - 1)));
                }
                var whitespace = element.Nodes().Last() as XText;
                if (whitespace != null && string.IsNullOrWhiteSpace(whitespace.Value))
                {
                    whitespace.AddBeforeSelf(new XText(Environment.NewLine + GetIndentation(indentationLevel)));
                    whitespace.AddBeforeSelf(newContent = FormatXml(newContent, indentationLevel));
                }
                else
                {
                    element.Add(new XText(Environment.NewLine + GetIndentation(indentationLevel)));
                    element.Add(newContent = FormatXml(newContent, indentationLevel));
                }
                return newContent;
            }

            public static XElement AddFirstWithIndentation(this XElement element, XElement newContent)
            {
                int indentationLevel = 0;
                var indentation = new StringBuilder();
                XElement tmp = element;
                while (tmp != null)
                {
                    tmp = tmp.Parent;
                    indentationLevel++;
                    indentation.Append(EditorControlService.GlobalOptions.IndentationString);
                }
                if (!element.Nodes().Any())
                {
                    element.Add(new XText(Environment.NewLine + GetIndentation(indentationLevel - 1)));
                }
                element.AddFirst(newContent = FormatXml(newContent, indentationLevel));
                element.AddFirst(new XText(Environment.NewLine + indentation));
                return newContent;
            }

            #region Dom, AST, Editor, Document
            
            public static int PositionToOffset(this IDocument document, Location location)
            {
                return document.PositionToOffset(location.Line, location.Column);
            }

            public static string GetText(this IDocument document, Location startPos, Location endPos)
            {
                int startOffset = document.PositionToOffset(startPos);
                return document.GetText(startOffset, document.PositionToOffset(endPos) - startOffset);
            }

            public static void ClearSelection(this ITextEditor editor)
            {
                editor.Select(editor.Document.PositionToOffset(editor.Caret.Position), 0);
            }

           
            #endregion
        }
        /// <summary>
        /// Wraps any collection to make it read-only.
        /// </summary>
        public sealed class ReadOnlyCollectionWrapper<T> : ICollection<T>
        {
            readonly ICollection<T> c;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            public ReadOnlyCollectionWrapper(ICollection<T> c)
            {
                if (c == null)
                    throw new ArgumentNullException("c");
                this.c = c;
            }

            public int Count
            {
                get
                {
                    return c.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            void ICollection<T>.Add(T item)
            {
                throw new NotSupportedException();
            }

            void ICollection<T>.Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(T item)
            {
                return c.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                c.CopyTo(array, arrayIndex);
            }

            bool ICollection<T>.Remove(T item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return c.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)c).GetEnumerator();
            }
        }
    /// <summary>
    /// 
    /// </summary>
        public static class KeyComparer
        {
            /// <summary>
            /// Create
            /// </summary>
            /// <typeparam name="TElement"></typeparam>
            /// <typeparam name="TKey"></typeparam>
            /// <param name="keySelector"></param>
            /// <returns></returns>
            public static KeyComparer<TElement, TKey> Create<TElement, TKey>(Func<TElement, TKey> keySelector)
            {
                return new KeyComparer<TElement, TKey>(keySelector, Comparer<TKey>.Default, EqualityComparer<TKey>.Default);
            }
        }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TKey"></typeparam>
        public class KeyComparer<TElement, TKey> : IComparer<TElement>, IEqualityComparer<TElement>
        {
            readonly Func<TElement, TKey> keySelector;
            readonly IComparer<TKey> keyComparer;
            readonly IEqualityComparer<TKey> keyEqualityComparer;
/// <summary>
/// 
/// </summary>
/// <param name="keySelector"></param>
/// <param name="keyComparer"></param>
/// <param name="keyEqualityComparer"></param>
            public KeyComparer(Func<TElement, TKey> keySelector, IComparer<TKey> keyComparer, IEqualityComparer<TKey> keyEqualityComparer)
            {
                this.keySelector = keySelector;
                this.keyComparer = keyComparer;
                this.keyEqualityComparer = keyEqualityComparer;
            }

            public int Compare(TElement x, TElement y)
            {
                return keyComparer.Compare(keySelector(x), keySelector(y));
            }

            public bool Equals(TElement x, TElement y)
            {
                return keyEqualityComparer.Equals(keySelector(x), keySelector(y));
            }

            public int GetHashCode(TElement obj)
            {
                return keyEqualityComparer.GetHashCode(keySelector(obj));
            }
        }
        /// <summary>
        /// Structure containing the result of a call to an expression finder.
        /// </summary>
        public struct ExpressionResult
        {
            public static readonly ExpressionResult Empty = new ExpressionResult(null);

            /// <summary>The expression that has been found at the specified offset.</summary>
            public string Expression;
            /// <summary>The exact source code location of the expression.</summary>
            /// <summary>Specifies the context in which the expression was found.</summary>
            public ExpressionContext Context;
            /// <summary>An object carrying additional language-dependend data.</summary>
            public object Tag;

            public ExpressionResult(string expression) : this(expression,  ExpressionContext.Default, null) { }
            public ExpressionResult(string expression, ExpressionContext context) : this(expression,  context, null) { }

            public ExpressionResult(string expression,  ExpressionContext context, object tag)
            {
                this.Expression = expression;
                this.Context = context;
                this.Tag = tag;
            }

            public override string ToString()
            {
                if (Context == ExpressionContext.Default)
                    return "<" + Expression + ">";
                else
                    return "<" + Expression + "> (" + Context.ToString() + ")";
            }
        }
        /// <summary>
        /// Class describing a context in which an expression can be.
        /// Serves as filter for code completion results, but the contexts exposed as static fields
        /// can also be used as a kind of enumeration for special behaviour in the resolver.
        /// </summary>
        public abstract class ExpressionContext
        {
            #region Instance members
            public abstract bool ShowEntry(ICompletionEntry o);

            protected bool readOnly = true;
            object suggestedItem;

            /// <summary>
            /// Gets if the expression is in the context of an object creation.
            /// </summary>
            public virtual bool IsObjectCreation
            {
                get
                {
                    return false;
                }
                set
                {
                    if (value)
                        throw new NotSupportedException();
                }
            }

            /// <summary>
            /// Gets/Sets the default item that should be included in a code completion popup
            /// in this context and selected as default value.
            /// </summary>
            /// <example>
            /// "List&lt;TypeName&gt; var = new *expr*();" has as suggested item the pseudo-class
            /// "List&lt;TypeName&gt;".
            /// </example>
            public object SuggestedItem
            {
                get
                {
                    return suggestedItem;
                }
                set
                {
                    if (readOnly)
                        throw new NotSupportedException();
                    suggestedItem = value;
                }
            }

            public virtual bool IsTypeContext
            {
                get { return false; }
            }
            #endregion

            #region VB specific contexts (public static fields) * MOVE TO ANOTHER CLASS *
            /// <summary>The context expects a new parameter declaration</summary>
            /// <example>Function Test(*expr*, *expr*, ...)</example>
            public static readonly ExpressionContext Parameter = new DefaultExpressionContext("Parameter");
            #endregion

            #region Default contexts (public static fields)
            /// <summary>Default/unknown context</summary>
            public readonly static ExpressionContext Default = new DefaultExpressionContext("Default");

            /// <summary>The context expects the base type of an enum.</summary>
            /// <example>enum Name : *expr* {}</example>
            public readonly static ExpressionContext EnumBaseType = new EnumBaseTypeExpressionContext();

            /// <summary>Context expects a non-sealed type or interface</summary>
            /// <example>class C : *expr* {}</example>
            public readonly static ExpressionContext InheritableType = new InheritableTypeExpressionContext();

            /// <summary>Context expects a namespace name</summary>
            /// <example>using *expr*;</example>
            public readonly static ExpressionContext Namespace = new ImportableExpressionContext(false);

            /// <summary>Context expects an importable type (namespace or class with public static members)</summary>
            /// <example>Imports *expr*;</example>
            public readonly static ExpressionContext Importable = new ImportableExpressionContext(true);

            /// <summary>Context expects a type name</summary>
            /// <example>typeof(*expr*)</example>
            public readonly static ExpressionContext Type = new TypeExpressionContext(null, false, true);

            /// <summary>Context expects the name of a non-static, non-void type</summary>
            /// <example>is *expr*, *expr* variableName</example>
            public readonly static ExpressionContext NonStaticNonVoidType = new NonStaticTypeExpressionContext("NonStaticType", false);

            /// <summary>Context expects a non-abstract type that has accessible constructors</summary>
            /// <example>new *expr*();</example>
            /// <remarks>When using this context, a resolver should treat the expression as object creation,
            /// even when the keyword "new" is not part of the expression.</remarks>
            public readonly static ExpressionContext ObjectCreation = new TypeExpressionContext(null, true, true);

            /// <summary>Context expects a type deriving from System.Attribute.</summary>
            /// <example>[*expr*()]</example>
            /// <remarks>When using this context, a resolver should try resolving typenames with an
            /// appended "Attribute" suffix and treat "invocations" of the attribute type as
            /// object creation.</remarks>
            public readonly static ExpressionContext Attribute = new AttributeExpressionContext();

            /// <summary>Context expects a type name which has special base type</summary>
            /// <param name="baseType"> </param>
            /// <param name="isObjectCreation">Specifies whether classes must be constructable.</param>
            /// <example>catch(*expr* ...), using(*expr* ...), throw new *expr*();</example>
            public static ExpressionContext TypeDerivingFrom(IReturnType baseType, bool isObjectCreation)
            {
                return new TypeExpressionContext(baseType, isObjectCreation, false);
            }

            /// <summary>Context expects an interface</summary>
            /// <example>interface C : *expr* {}</example>
            /// <example>Implements *expr*</example>
            public readonly static ExpressionContext Interface = new ClassTypeExpressionContext(ClassType.Interface);

            /// <summary>Context expects a delegate</summary>
            /// <example>public event *expr*</example>
            public readonly static ExpressionContext DelegateType = new ClassTypeExpressionContext(ClassType.Delegate);

            /// <summary>The context expects a new identifier</summary>
            /// <example>class *expr* {}; string *expr*;</example>
            public readonly static ExpressionContext IdentifierExpected = new DefaultExpressionContext("IdentifierExpected");

            /// <summary>The context is outside of any type declaration, expecting a global-level keyword.</summary>
            public readonly static ExpressionContext Global = new DefaultExpressionContext("Global");

            /// <summary>The context is the body of a type declaration.</summary>
            public readonly static ExpressionContext TypeDeclaration = new ExpressionContext.NonStaticTypeExpressionContext("TypeDeclaration", true);

            /// <summary>The context is the body of a method.</summary>
            /// <example>void Main () { *expr* }</example>
            public readonly static ExpressionContext MethodBody = new ExpressionContext.DefaultExpressionContext("MethodBody");
            #endregion

            #region DefaultExpressionContext
            internal sealed class DefaultExpressionContext : ExpressionContext
            {
                string name;

                public DefaultExpressionContext(string name)
                {
                    this.name = name;
                }

                public override bool ShowEntry(ICompletionEntry o)
                {
                    return true;
                }

                public override string ToString()
                {
                    return "[" + GetType().Name + ": " + name + "]";
                }
            }
            #endregion

            #region NamespaceExpressionContext
            sealed class ImportableExpressionContext : ExpressionContext
            {
                bool allowImportClasses;

                public ImportableExpressionContext(bool allowImportClasses)
                {
                    this.allowImportClasses = allowImportClasses;
                }

                public override bool ShowEntry(ICompletionEntry o)
                {
                    if (!(o is IEntity))
                        return true;
                    IClass c = o as IClass;
                    if (allowImportClasses && c != null)
                    {
                        return c.HasPublicOrInternalStaticMembers;
                    }
                    return false;
                }

                public override string ToString()
                {
                    return "[" + GetType().Name + " AllowImportClasses=" + allowImportClasses.ToString() + "]";
                }
            }
            #endregion

            #region TypeExpressionContext
            sealed class TypeExpressionContext : ExpressionContext
            {
                IClass baseClass;
                bool isObjectCreation;

                public TypeExpressionContext(IReturnType baseType, bool isObjectCreation, bool readOnly)
                {
                    if (baseType != null)
                        baseClass = baseType.GetUnderlyingClass();
                    this.isObjectCreation = isObjectCreation;
                    this.readOnly = readOnly;
                }

                public override bool ShowEntry(ICompletionEntry o)
                {
                    if (!(o is IEntity))
                        return true;
                    var c = o as IClass;
                    if (c == null)
                        return false;
                    if (isObjectCreation)
                    {
                        if (c.IsAbstract || c.IsStatic) return false;
                        if (c.ClassType == ClassType.Enum || c.ClassType == ClassType.Interface)
                            return false;
                    }
                    if (baseClass == null)
                        return true;
                    return c.IsTypeInInheritanceTree(baseClass);
                }

                public override bool IsObjectCreation
                {
                    get
                    {
                        return isObjectCreation;
                    }
                    set
                    {
                        if (readOnly && value != isObjectCreation)
                            throw new NotSupportedException();
                        isObjectCreation = value;
                    }
                }

                public override bool IsTypeContext
                {
                    get { return true; }
                }

                public override string ToString()
                {
                    if (baseClass != null)
                        return "[" + GetType().Name + ": " + baseClass.FullyQualifiedName
                            + " IsObjectCreation=" + IsObjectCreation + "]";
                    else
                        return "[" + GetType().Name + " IsObjectCreation=" + IsObjectCreation + "]";
                }

                public override bool Equals(object obj)
                {
                    var o = obj as TypeExpressionContext;
                    return o != null && object.Equals(baseClass, o.baseClass) && IsObjectCreation == o.IsObjectCreation;
                }

                public override int GetHashCode()
                {
                    return ((baseClass != null) ? baseClass.GetHashCode() : 0)
                        ^ isObjectCreation.GetHashCode();
                }
            }
            #endregion

            #region CombinedExpressionContext
            public static ExpressionContext operator |(ExpressionContext a, ExpressionContext b)
            {
                return new CombinedExpressionContext(0, a, b);
            }

            public static ExpressionContext operator &(ExpressionContext a, ExpressionContext b)
            {
                return new CombinedExpressionContext(1, a, b);
            }

            public static ExpressionContext operator ^(ExpressionContext a, ExpressionContext b)
            {
                return new CombinedExpressionContext(2, a, b);
            }

            sealed class CombinedExpressionContext : ExpressionContext
            {
                byte opType; // 0 = or ; 1 = and ; 2 = xor
                ExpressionContext a;
                ExpressionContext b;

                public CombinedExpressionContext(byte opType, ExpressionContext a, ExpressionContext b)
                {
                    if (a == null)
                        throw new ArgumentNullException("a");
                    if (b == null)
                        throw new ArgumentNullException("a");
                    this.opType = opType;
                    this.a = a;
                    this.b = b;
                }

                public override bool ShowEntry(ICompletionEntry o)
                {
                    if (opType == 0)
                        return a.ShowEntry(o) || b.ShowEntry(o);
                    else if (opType == 1)
                        return a.ShowEntry(o) && b.ShowEntry(o);
                    else
                        return a.ShowEntry(o) ^ b.ShowEntry(o);
                }

                public override string ToString()
                {
                    string op;
                    if (opType == 0)
                        op = " OR ";
                    else if (opType == 1)
                        op = " AND ";
                    else
                        op = " XOR ";
                    return "[" + GetType().Name + ": " + a + op + b + "]";
                }

                public override int GetHashCode()
                {
                    int hashCode = 0;
                    unchecked
                    {
                        hashCode += opType.GetHashCode();
                        if (a != null) hashCode += a.GetHashCode() * 3;
                        if (b != null) hashCode += b.GetHashCode() * 181247123;
                    }
                    return hashCode;
                }

                public override bool Equals(object obj)
                {
                    var cec = obj as CombinedExpressionContext;
                    return cec != null && this.opType == cec.opType && object.Equals(this.a, cec.a) && object.Equals(this.b, cec.b);
                }
            }
            #endregion

            #region EnumBaseTypeExpressionContext
            sealed class EnumBaseTypeExpressionContext : ExpressionContext
            {
                public override bool ShowEntry(ICompletionEntry o)
                {
                    var c = o as IClass;
                    if (c != null)
                    {
                        // use this hack to show dummy classes like "short"
                        // (go from the dummy class to the real class)
                        if (c.Methods.Count > 0)
                        {
                            c = c.Methods[0].DeclaringType;
                        }
                        switch (c.FullyQualifiedName)
                        {
                            case "System.Byte":
                            case "System.SByte":
                            case "System.Int16":
                            case "System.UInt16":
                            case "System.Int32":
                            case "System.UInt32":
                            case "System.Int64":
                            case "System.UInt64":
                                return true;
                            default:
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                public override string ToString()
                {
                    return "[" + GetType().Name + "]";
                }
            }
            #endregion

            #region AttributeExpressionContext
            sealed class AttributeExpressionContext : ExpressionContext
            {
                public override bool ShowEntry(ICompletionEntry o)
                {
                    if (!(o is IEntity))
                        return true;
                    var c = o as IClass;
                    if (c != null && !c.IsAbstract)
                    {
                        
                    }
                    else
                    {
                        return false;
                    }
                }

                public override bool IsTypeContext
                {
                    get { return true; }
                }

                public override string ToString()
                {
                    return "[" + GetType().Name + "]";
                }
            }
            #endregion

            #region InheritableTypeExpressionContext
            sealed class InheritableTypeExpressionContext : ExpressionContext
            {
                public override bool ShowEntry(ICompletionEntry o)
                {
                    if (!(o is IEntity)) return true;
                    var c = o as IClass;
                    if (c != null)
                    {
                        foreach (IClass innerClass in c.InnerClasses)
                        {
                            if (ShowEntry(innerClass)) return true;
                        }
                        if (c.ClassType == ClassType.Interface) return true;
                        if (c.ClassType == ClassType.Class)
                        {
                            if (!c.IsSealed && !c.IsStatic) return true;
                        }
                    }
                    return false;
                }

                public override string ToString()
                {
                    return "[" + GetType().Name + "]";
                }
            }
            #endregion

            #region ClassTypeExpressionContext
            sealed class ClassTypeExpressionContext : ExpressionContext
            {
                readonly ClassType expectedType;

                public ClassTypeExpressionContext(ClassType expectedType)
                {
                    this.expectedType = expectedType;
                }

                public override bool ShowEntry(ICompletionEntry o)
                {
                    if (!(o is IEntity)) return true;
                    var c = o as IClass;
                    if (c != null)
                    {
                        foreach (IClass innerClass in c.InnerClasses)
                        {
                            if (ShowEntry(innerClass)) return true;
                        }
                        if (c.ClassType == expectedType) return true;
                    }
                    return false;
                }

                public override string ToString()
                {
                    return "[" + GetType().Name + " expectedType=" + expectedType.ToString() + "]";
                }
            }
            #endregion

            #region NonStaticTypeExpressionContext
            internal sealed class NonStaticTypeExpressionContext : ExpressionContext
            {
                string name;
                bool allowVoid;

                public NonStaticTypeExpressionContext(string name, bool allowVoid)
                {
                    this.name = name;
                    this.allowVoid = allowVoid;
                }

                public override bool ShowEntry(ICompletionEntry o)
                {
                    if (!(o is IEntity)) return true;
                    IClass c = o as IClass;
                    if (c != null)
                    {
                        if (!allowVoid)
                        {
                            if (c.FullyQualifiedName == "System.Void" || c.FullyQualifiedName == "void") return false;
                        }

                        foreach (IClass innerClass in c.InnerClasses)
                        {
                            if (ShowEntry(innerClass)) return true;
                        }
                        if (!c.IsStatic && c.ClassType != ClassType.Module) return true;
                    }
                    return false;
                }

                public override string ToString()
                {
                    return "[" + GetType().Name + " " + name + "]";
                }
            }
            #endregion
        }
        public enum ClassType
        {
            Class ,
            Enum ,
            Interface ,
            Struct ,
            Delegate = 0x5,
            Module
        }
}


