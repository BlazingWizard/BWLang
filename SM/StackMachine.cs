using BW.Common;
using BW.Common.DataStruct;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BW.SM
{
    public class StackMachine
    {
        private List<PolisElement> _polis;

        private Dictionary<string, List<PolisElement>> _functionTable;

        private Stack<PolisElement> _programmPerformStack;

        private VarTable _varTable;

        private Dictionary<string, Func<object>> _availibleTypes;

        private Dictionary<string, Dictionary<string, Func<double[], object>>> _availibleMethods;

        public StackMachine(List<PolisElement> polis, Dictionary<string, List<PolisElement>> functionTable)
        {
            _polis = polis;
            _functionTable = functionTable;

            _varTable = new VarTable();
            _programmPerformStack = new Stack<PolisElement>();

            _availibleTypes = new Dictionary<string, Func<object>>();
            _availibleTypes.Add("list", () => new BWList<double>());
            _availibleTypes.Add("hashset", () => new BWHashSet<double>());
        }

        public void PerformProgram()
        {
            for (int i = 0; i < _polis.Count; i++)
            {
                var currentItem = _polis[i];

                switch (currentItem.Type)
                {
                    case PolisElementType.Var:
                        if (IsFunction(currentItem))
                        {
                            StartFunction(currentItem);
                        }
                        else
                        {
                            _programmPerformStack.Push(currentItem);
                        }
                        break;
                    case PolisElementType.FunctionArg:
                    case PolisElementType.Double:
                    case PolisElementType.OpNew:
                        _programmPerformStack.Push(currentItem);
                        break;
                    case PolisElementType.Assigment:
                        PerformAssigment();
                        break;
                    case PolisElementType.ArethmOp:
                        PerformArethmStm(currentItem);
                        break;
                    case PolisElementType.LogicOp:
                        PerformLogicStm(currentItem);
                        break;
                    case PolisElementType.DirectGo:
                        PerformDirectGo(ref i);
                        break;
                    case PolisElementType.NegativeGo:
                        PerformNegativeGo(ref i);
                        break;
                    case PolisElementType.MethodCall:
                        PerformMethodCall();
                        break;
                    case PolisElementType.Print:
                        _varTable.Print();
                        break;
                }
            }
        }

        private void StartFunction(PolisElement currentItem)
        {
            var peformInNewThread = currentItem.Value.Contains(":");

            if (peformInNewThread)
            {
                var tmp = currentItem.Value.Split(':');
                var functionName = tmp[0];
                var sectionName = tmp[1];

                Task.Run(() => PerformFunction(functionName));
            }
            else
            {
                var functionName = currentItem.Value;
                PerformFunction(functionName);
            }

        }

        private void PerformFunction(string functionName)
        {
            var argValue = GetValue(_programmPerformStack.Pop());

            var funcPolis = _functionTable[functionName];
            funcPolis.Find(x => x.Type == PolisElementType.FunctionArg).Value = argValue.ToString();

            var smFunctionPerform = new StackMachine(funcPolis, _functionTable);
            smFunctionPerform.PerformProgram();
            var functionResult = smFunctionPerform.GetResult();

            if (functionResult != null)
            {
                _programmPerformStack.Push(new PolisElement(PolisElementType.Double, functionResult.ToString()));
            }
        }

        private object GetResult()
        {
            object result;
            try
            {
                result = _varTable["result"];
            }
            catch
            {
                result = null;
            }
            return result;
        }

        private bool IsFunction(PolisElement currentItem)
        {
            var tokenValue = currentItem.Value.Contains(":") ? currentItem.Value.Split(':')[0] : currentItem.Value;
            return _functionTable.ContainsKey(tokenValue);
        }

        private void PerformMethodCall()
        {
            var value = GetValue(_programmPerformStack.Pop());
            var methodName = _programmPerformStack.Pop();
            var variable = _programmPerformStack.Pop();

            var obj = _varTable[variable.Value];

            if (obj is BWList<double>)
            {
                var list = (BWList<double>)obj;
                switch (methodName.Value)
                {
                    case "add":
                        list.Add(value);
                        break;
                    case "delete":
                        list.Delete((int)value);
                        break;
                    case "get":
                        var res = list.Get((int)value);
                        _programmPerformStack.Push(new PolisElement(PolisElementType.Double, res.ToString()));
                        break;
                    default:
                        throw new RuntimeException("Method not found!");
                }
                return;
            }

            if (obj is BWHashSet<double>)
            {
                var hashSet = (BWHashSet<double>)obj;
                switch (methodName.Value)
                {
                    case "add":
                        hashSet.Add(value);
                        break;
                    case "contains":
                        var res = hashSet.Contains(value);
                        _programmPerformStack.Push(new PolisElement(PolisElementType.Bool, res.ToString()));
                        break;
                    default:
                        throw new RuntimeException("Method not found!");
                }
                return;
            }

            throw new RuntimeException("Not a object");
        }

        private void PerformNegativeGo(ref int currenElementID)
        {
            var boolRes = bool.Parse(_programmPerformStack.Pop().Value);

            if (!boolRes)
            {
                var nextPolisElement = _polis[currenElementID + 1];
                var address = int.Parse(nextPolisElement.Value);
                currenElementID = address - 1;
            }
        }

        private void PerformDirectGo(ref int currenElementID)
        {
            var nextPolisElement = _polis[currenElementID + 1];
            var address = int.Parse(nextPolisElement.Value);
            currenElementID = address - 1;
        }

        private void PerformLogicStm(PolisElement currentItem)
        {
            Func<double, double, bool> func;
            switch (currentItem.Value)
            {
                case "<":
                    func = (a, b) => a < b;
                    break;
                case ">":
                    func = (a, b) => a > b;
                    break;
                case "==":
                    func = (a, b) => a == b;
                    break;
                case "!=":
                    func = (a, b) => a != b;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var second = _programmPerformStack.Pop();
            var secondValue = GetValue(second);

            var first = _programmPerformStack.Pop();
            var firstValue = GetValue(first);

            var result = func(firstValue, secondValue);
            _programmPerformStack.Push(new PolisElement(PolisElementType.Bool, result.ToString()));
        }

        private void PerformArethmStm(PolisElement currentItem)
        {
            Func<double, double, double> func;
            switch (currentItem.Value)
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

            var second = _programmPerformStack.Pop();
            var secondValue = GetValue(second);

            var first = _programmPerformStack.Pop();
            var firstValue = GetValue(first);

            var result = func(firstValue, secondValue);
            _programmPerformStack.Push(new PolisElement(PolisElementType.Double, result.ToString()));
        }

        private double GetValue(PolisElement element)
        {
            if (element.Type == PolisElementType.Var)
            {
                var varName = element.Value;
                return (double)_varTable[varName];
            }

            if (element.Type == PolisElementType.Double || element.Type == PolisElementType.FunctionArg)
            {
                return double.Parse(element.Value);
            }

            throw new NotImplementedException();
        }

        private void PerformAssigment()
        {
            var topElement = _programmPerformStack.Pop();


            if (topElement.Type == PolisElementType.OpNew)
            {
                var typeName = _programmPerformStack.Pop().Value;
                if (_availibleTypes.ContainsKey(typeName))
                {
                    var var = _programmPerformStack.Pop();
                    _varTable[$"{typeName} {var.Value}"] = _availibleTypes[typeName]();
                }
                else
                {
                    throw new RuntimeException("Unsoported type");
                }
            }
            else
            {
                var var = _programmPerformStack.Pop();
                _varTable["double " + var.Value] = GetValue(topElement);
            }
        }
    }
}