namespace miRobotEditor.Core.Parsers
{

    public abstract class AbstractParser
    {
       // This defines the PeekToken object
        /// <summary>
        /// A PeekToken object class
        /// </summary>
        /// <remarks>
        /// A PeekToken is a special pointer object that can be used to Peek() several
        /// tokens ahead in the GetToken() queue.
        /// </remarks>
        public class PeekToken
        {
            public int TokenIndex { get; set; }

            public Token TokenPeek { get; set; }

            public PeekToken(int index, Token value)
            {
                TokenIndex = index;
                TokenPeek = value;
            }
        }

        public string Parse(Token value)
        {
            return string.Empty;
        }
        // This defines the Token object
        /// <summary>
        /// a Token object class
        /// </summary>
        /// <remarks>
        /// A Token object holds the token and token value.
        /// </remarks>
        public class Token
        {
            public E6Parser.Tokens TokenName { get; set; }

            public string TokenValue { get; set; }

            public Token(E6Parser.Tokens name, string value)
            {
                TokenName = name;
                TokenValue = value;
            }
        }
    }
}
