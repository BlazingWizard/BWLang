using BW.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Optimization
{
    class Triad
    {
        public TriadElement Left;

        public TriadElement Right;

        public TriadElement Op;

        public Triad()
        {

        }

        public Triad(PolisElement left, PolisElement right, PolisElement op)
        {
            this.Left = new TriadElement(left);
            this.Right = new TriadElement(right);
            this.Op = new TriadElement(op);
        }

        public override string ToString()
        {
            return $"{Op} ({Left},{Right})";
        }

        public List<PolisElement> ConvertToPolisList()
        {
            var polis = new List<PolisElement>();
            polis.Add(this.Left.ConvertToPolisElemnt());
            polis.Add(this.Right.ConvertToPolisElemnt());
            polis.Add(this.Op.ConvertToPolisElemnt());
            return polis;
        }
    }
}
