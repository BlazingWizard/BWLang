using BW.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Optimization
{
    public class Optimazer
    {
        private List<PolisElement> _polisForOptimaze;

        private bool _canOptimaze = true;

        private Dictionary<string, double?> _varTable = new Dictionary<string, double?>();

        public Optimazer(List<PolisElement> polis)
        {
            _polisForOptimaze = polis;
            foreach (var elem in polis){
                if (elem.Type == PolisElementType.NegativeGo || elem.Type == PolisElementType.DirectGo)
                {
                    _canOptimaze = false;
                    break;
                }
            }
        }

        public List<PolisElement> Optimaze()
        {
            if (!_canOptimaze)
            {
                return _polisForOptimaze;
            }

            var triadList = Converter.PolisToTriad(_polisForOptimaze);
            var optimazeTriadList = TriadOptimization(triadList);
            var polis = Converter.TriadToPolis(optimazeTriadList);

            return polis;
        }

        private List<Triad> TriadOptimization(List<Triad> triadList)
        {
            for(var i = 0; i < triadList.Count; i++)
            {
                var triad = triadList[i];

                if (triad.Op.Type == PolisElementType.ArethmOp)
                {
                    triadList[i] = CalculateTriad(triad, triadList);
                }

                if (triad.Op.Type == PolisElementType.Assigment)
                {
                    triadList[i] = ProcessAssigments(triad, triadList);
                }
            }

            var optimazeTriadList = new List<Triad>();
            foreach(var triad in triadList) {
                if (triad.Op.Value != "C")
                {
                    optimazeTriadList.Add(triad);
                }
            }

            return optimazeTriadList;
        }

        private Triad ProcessAssigments(Triad triad, List<Triad> triads)
        {
            var varName = triad.Left.Value;

            TriadElement operand = triad.Right;
            double? varValue = GetValue(operand);
            if (triad.Right.Type == PolisElementType.DirectGo)
            {
                operand = triads[int.Parse(triad.Right.Value)].Left;
                varValue = GetValue(operand);

                triad.Right.Type = PolisElementType.Double;
                triad.Right.Value = varValue.ToString();
            }


            if (!_varTable.ContainsKey(varName))
            {
                _varTable.Add(varName, varValue);
            }
            else
            {
                _varTable[varName] = varValue;
            }

            return triad;
        }

        private Triad CalculateTriad(Triad triad, List<Triad> triads)
        {
            TriadElement leftOperand = triad.Left;
            if (triad.Left.Type == PolisElementType.DirectGo)
            {
                leftOperand = triads[int.Parse(triad.Left.Value)].Left;  
            }

            TriadElement rightOperand = triad.Right;
            if (triad.Right.Type == PolisElementType.DirectGo)
            {
                rightOperand = triads[int.Parse(triad.Right.Value)].Left;
            }

            TriadElement op = triad.Op;
            if (op.Type != PolisElementType.ArethmOp)
            {
                throw new NotImplementedException();
            }

            Func<double?, double?, double?> func;
            switch (triad.Op.Value)
            {
                case "+":
                    func = (a, b) => a + b;
                    break;
                case "-":
                    func = (a, b) => a - b;
                    break;
                case "*":
                    func = (a, b) => a * b;
                    break;
                case "/":
                    func = (a, b) => a / b;
                    break;
                default:
                    throw new NotImplementedException();
            }


            var left = GetValue(leftOperand);
            var right = GetValue(rightOperand);

            if ( left != null && right != null)
            {
                var newTriad = new Triad()
                {
                    Op = new TriadElement(PolisElementType.DirectGo, "C"),
                    Left = new TriadElement(PolisElementType.Double, func(left, right).ToString()),
                    Right = new TriadElement(PolisElementType.Double, "0")
                };

                return newTriad;
            }
            else
            {
                return triad;
            }
        }

        private double? GetValue(TriadElement element)
        {
            if (element.Type == PolisElementType.Var)
            {
                var varName = element.Value;
                return _varTable.ContainsKey(varName) ? _varTable[varName] : null;
            }

            if (element.Type == PolisElementType.Double)
            {
                return double.Parse(element.Value);
            }


            return null;
        }
    }
}
