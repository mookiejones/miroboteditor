﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;

namespace miRobotEditor.EditorControl.Classes
{
    /// <summary>
    /// Searches matching brackets .
    /// </summary>
    public class MyBracketSearcher : IBracketSearcher
    {
        private const string OpeningBrackets = "([{";
        private const string ClosingBrackets = ")]}";

        public BracketSearchResult SearchBracket(TextDocument document, int offset)
        {


            if (offset > 0)
            {
                var c = document.GetCharAt(offset - 1);
                var index = OpeningBrackets.IndexOf(c);
                var otherOffset = -1;
                if (index > -1)
                    otherOffset = SearchBracketForward(document, offset, OpeningBrackets[index], ClosingBrackets[index]);

                index = ClosingBrackets.IndexOf(c);
                if (index > -1)
                    otherOffset = SearchBracketBackward(document, offset - 2, OpeningBrackets[index], ClosingBrackets[index]);

                if (otherOffset > -1)
                    return new BracketSearchResult(Math.Min(offset - 1, otherOffset), 1,
                                                   Math.Max(offset - 1, otherOffset), 1);
            }

            return null;
        }

        #region SearchBracket helper functions
        static int ScanLineStart(ITextSource document, int offset)
        {
            for (var i = offset - 1; i > 0; --i)
            {
                if (document.GetCharAt(i) == '\n')
                    return i + 1;
            }
            return 0;
        }

        /// <summary>
        /// Gets the type of code at offset.<br/>
        /// 0 = Code,<br/>
        /// 1 = Comment,<br/>
        /// 2 = String<br/>
        /// Block comments and multiline strings are not supported.
        /// </summary>
        static int GetStartType(ITextSource document, int linestart, int offset)
        {
            var inString = false;
            var inChar = false;
            var verbatim = false;
            var result = 0;
            for (var i = linestart; i < offset; i++)
            {
                switch (document.GetCharAt(i))
                {
                    case '/':
                        if (!inString && !inChar && i + 1 < document.TextLength)
                        {
                            if (document.GetCharAt(i + 1) == '/')
                            {
                                result = 1;
                            }
                        }
                        break;
                    case '"':
                        if (!inChar)
                        {
                            if (inString && verbatim)
                            {
                                if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
                                {
                                    ++i; // skip escaped quote
                                    inString = false; // let the string go on
                                }
                                else
                                {
                                    verbatim = false;
                                }
                            }
                            else if (!inString && i > 0 && document.GetCharAt(i - 1) == '@')
                            {
                                verbatim = true;
                            }
                            inString = !inString;
                        }
                        break;
                    case '\'':
                        if (!inString) inChar = !inChar;
                        break;
                    case '\\':
                        if ((inString && !verbatim) || inChar)
                            ++i; // skip next character
                        break;
                }
            }

            return (inString || inChar) ? 2 : result;
        }
        #endregion

        #region SearchBracketBackward

        static int SearchBracketBackward(ITextSource document, int offset, char openBracket, char closingBracket)
        {


            if (offset + 1 >= document.TextLength) return -1;
            // this method parses a c# document backwards to find the matching bracket

            // first try "quick find" - find the matching bracket if there is no string/comment in the way
            var quickResult = QuickSearchBracketBackward(document, offset, openBracket, closingBracket);
            if (quickResult >= 0) return quickResult;

            // we need to parse the line from the beginning, so get the line start position
            var linestart = ScanLineStart(document, offset + 1);

            // we need to know where offset is - in a string/comment or in normal code?
            // ignore cases where offset is in a block comment
            var starttype = GetStartType(document, linestart, offset + 1);
            if (starttype == 1)
            {
                return -1; // start position is in a comment
            }

            // I don't see any possibility to parse a C# document backwards...
            // We have to do it forwards and push all bracket positions on a stack.
            var bracketStack = new Stack<int>();
            var blockComment = false;
            var lineComment = false;
            var inChar = false;
            var inString = false;
            var verbatim = false;

            for (var i = 0; i <= offset; ++i)
            {
                var ch = document.GetCharAt(i);
                switch (ch)
                {
                    case '\r':
                    case '\n':
                        lineComment = false;
                        inChar = false;
                        if (!verbatim) inString = false;
                        break;
                    case '/':
                        if (blockComment)
                        {
                            Debug.Assert(i > 0);
                            if (document.GetCharAt(i - 1) == '*')
                            {
                                blockComment = false;
                            }
                        }
                        if (!inString && !inChar && i + 1 < document.TextLength)
                        {
                            if (!blockComment && document.GetCharAt(i + 1) == '/')
                            {
                                lineComment = true;
                            }
                            if (!lineComment && document.GetCharAt(i + 1) == '*')
                            {
                                blockComment = true;
                            }
                        }
                        break;
                    case '"':
                        if (!(inChar || lineComment || blockComment))
                        {
                            if (inString && verbatim)
                            {
                                if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
                                {
                                    ++i; // skip escaped quote
                                    inString = false; // let the string go
                                }
                                else
                                {
                                    verbatim = false;
                                }
                            }
                            else if (!inString && offset > 0 && document.GetCharAt(i - 1) == '@')
                            {
                                verbatim = true;
                            }
                            inString = !inString;
                        }
                        break;
                    case '\'':
                        if (!(inString || lineComment || blockComment))
                        {
                            inChar = !inChar;
                        }
                        break;
                    case '\\':
                        if ((inString && !verbatim) || inChar)
                            ++i; // skip next character
                        break;
                    default:
                        if (ch == openBracket)
                        {
                            if (!(inString || inChar || lineComment || blockComment))
                            {
                                bracketStack.Push(i);
                            }
                        }
                        else if (ch == closingBracket)
                        {
                            if (!(inString || inChar || lineComment || blockComment))
                            {
                                if (bracketStack.Count > 0)
                                    bracketStack.Pop();
                            }
                        }
                        break;
                }
            }
            if (bracketStack.Count > 0) return bracketStack.Pop();
            return -1;
        }
        #endregion

        #region SearchBracketForward

        static int SearchBracketForward(ITextSource document, int offset, char openBracket, char closingBracket)
        {
            var inString = false;
            var inChar = false;
            var verbatim = false;

            var lineComment = false;
            var blockComment = false;

            if (offset < 0) return -1;

            // first try "quick find" - find the matching bracket if there is no string/comment in the way
            var quickResult = QuickSearchBracketForward(document, offset, openBracket, closingBracket);
            if (quickResult >= 0) return quickResult;

            // we need to parse the line from the beginning, so get the line start position
            var linestart = ScanLineStart(document, offset);

            // we need to know where offset is - in a string/comment or in normal code?
            // ignore cases where offset is in a block comment
            var starttype = GetStartType(document, linestart, offset);
            if (starttype != 0) return -1; // start position is in a comment/string

            var brackets = 1;

            while (offset < document.TextLength)
            {
                var ch = document.GetCharAt(offset);
                switch (ch)
                {
                    case '\r':
                    case '\n':
                        lineComment = false;
                        inChar = false;
                        if (!verbatim) inString = false;
                        break;
                    case '/':
                        if (blockComment)
                        {
                            Debug.Assert(offset > 0);
                            if (document.GetCharAt(offset - 1) == '*')
                            {
                                blockComment = false;
                            }
                        }
                        if (!inString && !inChar && offset + 1 < document.TextLength)
                        {
                            if (!blockComment && document.GetCharAt(offset + 1) == '/')
                            {
                                lineComment = true;
                            }
                            if (!lineComment && document.GetCharAt(offset + 1) == '*')
                            {
                                blockComment = true;
                            }
                        }
                        break;
                    case '"':
                        if (!(inChar || lineComment || blockComment))
                        {
                            if (inString && verbatim)
                            {
                                if (offset + 1 < document.TextLength && document.GetCharAt(offset + 1) == '"')
                                {
                                    ++offset; // skip escaped quote
                                    inString = false; // let the string go
                                }
                                else
                                {
                                    verbatim = false;
                                }
                            }
                            else if (!inString && offset > 0 && document.GetCharAt(offset - 1) == '@')
                            {
                                verbatim = true;
                            }
                            inString = !inString;
                        }
                        break;
                    case '\'':
                        if (!(inString || lineComment || blockComment))
                        {
                            inChar = !inChar;
                        }
                        break;
                    case '\\':
                        if ((inString && !verbatim) || inChar)
                            ++offset; // skip next character
                        break;
                    default:
                        if (ch == openBracket)
                        {
                            if (!(inString || inChar || lineComment || blockComment))
                            {
                                ++brackets;
                            }
                        }
                        else if (ch == closingBracket)
                        {
                            if (!(inString || inChar || lineComment || blockComment))
                            {
                                --brackets;
                                if (brackets == 0)
                                {
                                    return offset;
                                }
                            }
                        }
                        break;
                }
                ++offset;
            }
            return -1;
        }
        #endregion

        static int QuickSearchBracketBackward(ITextSource document, int offset, char openBracket, char closingBracket)
        {
            var brackets = -1;
            // first try "quick find" - find the matching bracket if there is no string/comment in the way
            for (var i = offset; i >= 0; --i)
            {
                var ch = document.GetCharAt(i);
                if (ch == openBracket)
                {
                    ++brackets;
                    if (brackets == 0) return i;
                }
                else if (ch == closingBracket)
                {
                    --brackets;
                }
                else if (ch == '"')
                {
                    break;
                }
                else if (ch == '\'')
                {
                    break;
                }
                else if (ch == '/' && i > 0)
                {
                    if (document.GetCharAt(i - 1) == '/') break;
                    if (document.GetCharAt(i - 1) == '*') break;
                }
            }
            return -1;
        }

        static int QuickSearchBracketForward(ITextSource document, int offset, char openBracket, char closingBracket)
        {
            var brackets = 1;
            // try "quick find" - find the matching bracket if there is no string/comment in the way
            for (var i = offset; i < document.TextLength; ++i)
            {
                var ch = document.GetCharAt(i);
                if (ch == openBracket)
                {
                    ++brackets;
                }
                else if (ch == closingBracket)
                {
                    --brackets;
                    if (brackets == 0) return i;
                }
                else if (ch == '"')
                {
                    break;
                }
                else if (ch == '\'')
                {
                    break;
                }
                else if (ch == '/' && i > 0)
                {
                    if (document.GetCharAt(i - 1) == '/') break;
                }
                else if (ch == '*' && i > 0)
                {
                    if (document.GetCharAt(i - 1) == '/') break;
                }
            }
            return -1;
        }
    }
}