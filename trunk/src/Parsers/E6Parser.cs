using System.ComponentModel;

namespace miRobotEditor.Parsers
{
    // This file was Auto Generated with TokenIcer
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// TokenParser
    /// </summary>
    /// <remarks>
    /// TokenParser is the main parser engine for converting input into lexical tokens.
    /// </remarks>
    [Localizable(false)]
    public class E6Parser : AbstractParser
    {
        // This dictionary will store our RegEx rules
        private readonly Dictionary<Tokens, string> _tokens;
        // This dictionary will store our matches
        private readonly Dictionary<Tokens, MatchCollection> _regExMatchCollection;
        // This input string will store the string to parse
        private string _inputString;
        // This index is used internally so the parser knows where it left off
        private int _index;

        // This is our token enumeration. It holds every token defined in the grammar
        /// <summary>
        /// Tokens is an enumeration of all possible token values.
        /// </summary>
        public enum Tokens
        {
            // ReSharper disable InconsistentNaming

            UNDEFINED = 0,
            DECLARATION = 1,
            E6POS = 2,
            IDENTIFIER = 3,
            END = 4,
            COLON = 5,
            EQUALS = 6,
            STRING = 7,
            WHITESPACE = 8,
            NEWLINE = 9,
            REAL = 10,
            INTEGER = 11,
            APOSTROPHE = 12,
            LPAREN = 13,
            RPAREN = 14,
            LBRACE = 15,
            RBRACE = 16,
            ASTERISK = 17,
            SLASH = 18,
            PLUS = 19,
            COMMA = 20,
            MINUS = 21
// ReSharper restore InconsistentNaming

        }

        // A public setter for our input string
        /// <summary>
        /// InputString Property
        /// </summary>
        /// <value>
        /// The string value that holds the input string.
        /// </value>
        public string InputString
        {
            set
            {
                _inputString = value;
                PrepareRegex();
            }
        }

        // Our Constructor, which simply initializes values
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <remarks>
        /// The constructor initalizes memory and adds all of the tokens to the token dictionary.
        /// </remarks>
        public E6Parser()
        {
            _tokens = new Dictionary<Tokens, string>();
            _regExMatchCollection = new Dictionary<Tokens, MatchCollection>();
            _index = 0;
            _inputString = string.Empty;

            // These lines add each grammar rule to the dictionary
            _tokens.Add(Tokens.DECLARATION, "[Dd][Ee][Cc][Ll]");
            _tokens.Add(Tokens.E6POS, "[Ee][6][Pp][Oo][Ss]");
            _tokens.Add(Tokens.IDENTIFIER, "[a-zA-Z_][a-zA-Z0-9_]*");
            _tokens.Add(Tokens.END, "[Ee][Nn][Dd]");
            _tokens.Add(Tokens.COLON, "\\:");
            _tokens.Add(Tokens.EQUALS, "=");
            _tokens.Add(Tokens.STRING, "\".*?\"");
            _tokens.Add(Tokens.WHITESPACE, "[ \\t]+");
            _tokens.Add(Tokens.NEWLINE, "[\\r\\n]+");
            _tokens.Add(Tokens.REAL, "[\\d.-]*");
            _tokens.Add(Tokens.INTEGER, "[\\d]+");
            _tokens.Add(Tokens.APOSTROPHE, "'.*");
            _tokens.Add(Tokens.LPAREN, "\\(");
            _tokens.Add(Tokens.RPAREN, "\\)");
            _tokens.Add(Tokens.LBRACE, "\\}");
            _tokens.Add(Tokens.RBRACE, "\\{");
            _tokens.Add(Tokens.ASTERISK, "\\*");
            _tokens.Add(Tokens.SLASH, "\\/");
            _tokens.Add(Tokens.PLUS, "\\+");
            _tokens.Add(Tokens.COMMA, ",");
            _tokens.Add(Tokens.MINUS, "\\-");
        }

        // This function preloads the matches based on our rules and the input string
        /// <summary>
        /// PrepareRegex prepares the regex for parsing by pre-matching the Regex tokens.
        /// </summary>
        private void PrepareRegex()
        {
            _regExMatchCollection.Clear();
            foreach (var pair in _tokens)
            {
                _regExMatchCollection.Add(pair.Key, Regex.Matches(_inputString, pair.Value));
            }
        }

        // ResetParser() will reset the parser.
        // Keep in mind that you must set the input string again
        /// <summary>
        /// ResetParser resets the parser to its inital state. Reloading InputString is required.
        /// </summary>
        /// <seealso cref="InputString"/>
        public void ResetParser()
        {
            _index = 0;
            _inputString = string.Empty;
            _regExMatchCollection.Clear();
        }

        // GetToken() retrieves the next token and returns a token object
        /// <summary>
        /// GetToken gets the next token in queue
        /// </summary>
        /// <remarks>
        /// GetToken attempts to the match the next character(s) using the
        /// Regex rules defined in the dictionary. If a match can not be
        /// located, then an Undefined token will be created with an empty
        /// string value. In addition, the token pointer will be incremented
        /// by one so that this token doesn't attempt to get identified again by
        /// GetToken()
        /// </remarks>
        public Token GetToken()
        {
            // If we are at the end of our input string then
            // we return null to signify the end of our input string.
            // While parsing tokens, you will undoubtedly be in a loop.
            // Having your loop check for a null token is a good way to end that
            // loop
            if (_index >= _inputString.Length)
                return null;

            // Iterate through our prepared matches/Tokens dictionary
            foreach (var pair in _regExMatchCollection)
            {
                // Iterate through each prepared match
                foreach (Match match in pair.Value)
                {
                    // If we find a match, update our index pointer and return a new Token object
                    if (match.Index == _index)
                    {
                        _index += match.Length;
                        return new Token(pair.Key, match.Value);
                    }

                    if (match.Index > _index)
                    {
                        break;
                    }
                }
            }
            // If execution got here, then we increment our index pointer
            // and return an Undefined token. 
            _index++;
            return new Token(Tokens.UNDEFINED, string.Empty);
        }

        // Peek() will retrieve a PeekToken object and will allow you to see the next token
        // that GetToken() will retrieve.
        /// <summary>
        /// Returns the next token that GetToken() will return.
        /// </summary>
        /// <seealso cref="Peek(AbstractParser.PeekToken)"/>
        public PeekToken Peek()
        {
            return Peek(new PeekToken(_index, new Token(Tokens.UNDEFINED, string.Empty)));
        }

        // This is an overload for Peek(). By passing in the last PeekToken object
        // received from Peek(), you can peek ahead to the next token, and the token after that, etc...
        /// <summary>
        /// Returns the next token after the Token passed here
        /// </summary>
        /// <param name="peekToken">The PeekToken token returned from a previous Peek() call</param>
        /// <seealso cref="Peek()"/>
        public PeekToken Peek(PeekToken peekToken)
        {
            var oldIndex = _index;

            _index = peekToken.TokenIndex;

            if (_index >= _inputString.Length)
            {
                _index = oldIndex;
                return null;
            }

            foreach (var pair in _tokens)
            {
                var r = new Regex(pair.Value);
                var m = r.Match(_inputString, _index);

                if (!m.Success || m.Index != _index) continue;
                
                _index += m.Length;
                var pt = new PeekToken(_index, new Token(pair.Key, m.Value));
                _index = oldIndex;
                return pt;
            }
            var pt2 = new PeekToken(_index + 1, new Token(Tokens.UNDEFINED, string.Empty));
            _index = oldIndex;
            return pt2;
        }


      
    }
}



