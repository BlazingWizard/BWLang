namespace BW.Common
{
    public class Token
    {
        public TerminalWords Lexemma { get; private set;}
        public string Value { get; private set; }

        public Token(TerminalWords lexemma, string value)
        {
            Lexemma = lexemma;
            Value = value;
        }


        public override string ToString()
        {
            return $"{Lexemma} : {Value}";
        }
    }
}
