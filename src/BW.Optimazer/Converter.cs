using BW.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Optimization
{
    static class Converter
    {
        static public List<PolisElement> TriadToPolis(List<Triad> optimazeTriadList)
        {
            var statment = new Stack<Triad>();
            var polisElements = new List<PolisElement>();

            foreach (var triad in optimazeTriadList)
            {
                statment.Push(triad);
                if (triad.Op.Type == PolisElementType.Assigment)
                {
                    var polis = ConvertStatmentToPolis(statment);
                    statment.Clear();

                    polisElements.AddRange(polis);
                }
            }

            return polisElements;
        }

        static private List<PolisElement> ConvertStatmentToPolis(Stack<Triad> statment)
        {
            var polis = new List<PolisElement>();
            var ass_triad = statment.Pop();
            polis.AddRange(ass_triad.ConvertToPolisList());

            while (true)
            {
                var index = polis.FindIndex(x => x.Type == PolisElementType.DirectGo);

                if (index == -1)
                {
                    break;
                }
                polis.RemoveAt(index);

                var triad = statment.Pop();
                polis.InsertRange(index, triad.ConvertToPolisList());
            }

            return polis;
        }

        static public List<Triad> PolisToTriad(List<PolisElement> polis)
        {
            var stack = new Stack<PolisElement>();
            var triadList = new List<Triad>();
            foreach (var elem in polis)
            {
                if (elem.Type != PolisElementType.ArethmOp && elem.Type != PolisElementType.Assigment && elem.Type != PolisElementType.LogicOp)
                {
                    stack.Push(elem);
                }
                else
                {
                    var op = elem;
                    var right = stack.Pop();
                    var left = stack.Pop();
                    triadList.Add(new Triad(left, right, op));

                    if (op.Type != PolisElementType.Assigment)
                    {
                        stack.Push(new PolisElement(PolisElementType.DirectGo, (triadList.Count - 1).ToString()));
                    }
                }
            }

            return triadList;
        }
    }
}
