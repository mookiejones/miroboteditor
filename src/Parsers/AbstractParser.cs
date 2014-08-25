namespace miRobotEditor.Parsers
{
    public abstract class AbstractParser
    {
        // This defines the PeekToken object

        public string Parse(Token value)
        {
            return string.Empty;
        }

        /// <summary>
        ///     A PeekToken object class
        /// </summary>
        /// <remarks>
        ///     A PeekToken is a special pointer object that can be used to Peek() several
        ///     tokens ahead in the GetToken() queue.
        /// </remarks>
        public class PeekToken
        {
            public PeekToken(int index, Token value)
            {
                TokenIndex = index;
                TokenPeek = value;
            }

            public int TokenIndex { get; set; }

            public Token TokenPeek { get; set; }
        }

        // This defines the Token object
        /// <summary>
        ///     a Token object class
        /// </summary>
        /// <remarks>
        ///     A Token object holds the token and token value.
        /// </remarks>
        public class Token
        {
            public Token(E6Parser.Tokens name, string value)
            {
                TokenName = name;
                TokenValue = value;
            }

            public E6Parser.Tokens TokenName { get; set; }

            public string TokenValue { get; set; }
        }
    }
}