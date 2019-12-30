namespace BW.Common
{
    public class Token
    {
        public TerminalWords Lexemma { get; set;}
        public string Value { get; set; }

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
