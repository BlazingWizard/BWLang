using BW.Common;

namespace BW.Optimization
{
    class TriadElement
    {
        public PolisElementType Type;
        public string Value;

        public TriadElement(PolisElement polisElement)
        {
            this.Type = polisElement.Type;
            this.Value = polisElement.Value;
        }

        public TriadElement()
        {

        }

        public TriadElement(PolisElementType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public PolisElement ConvertToPolisElemnt()
        {
            return new PolisElement(this.Type, this.Value);
        }

        public override string ToString()
        {
            return Type == PolisElementType.DirectGo ? $"^{Value}" : Value;
        }
    }
}
