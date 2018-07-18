using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EZData;
using UnityEngine;

public static class Evaluator {
    public enum OpType {
        AND,
        OR,
        NOT,
        EQUAL,
        NOT_EQUAL,
        
        GREATER,
        LESS,
        GEQUAL,
        LEQUAL
    }

    public enum BracketType {
        OPEN,
        CLOSE
    }

    public class Token {
        public String Text;
    }

    public class Operator : Token {
        public Operator(OpType type, int priority, int numArgs) {
            Type = type;
            Priority = priority;
            NumArgs = numArgs;
        }

        public Operator() {}

        public OpType Type;
        public int Priority;
        public int NumArgs;
    }

    public abstract class Valuable : Token {
        public abstract int GetValue();
    }

    public class Contant : Valuable {
        public int Value;

        public override int GetValue() {
            return Value;
        }
    }

    public class Variable : Valuable {
        private Type type;
        private Property property;

        public bool Subscribe(BaseBinding binding, NotifyPropertyChanged handler) {
            var context = binding.GetContext(Text);
            if (context == null) {
                return false;
            }

            var properties = new Dictionary<Type, Property>() {
                {typeof(bool), context.FindProperty<bool>(Text, binding)},
                {typeof(int), context.FindProperty<int>(Text, binding)},
                {typeof(float), context.FindProperty<float>(Text, binding)},
                {typeof(double), context.FindProperty<double>(Text, binding)}
            };
            var pair = properties.FirstOrDefault(x => x.Value != null);
            if (pair.Equals(default(KeyValuePair<Type, Property>))) {
                return false;
            }

            type = pair.Key;
            property = pair.Value;

            //Предотвращеие двойной подписи
            property.OnChange -= handler;
            property.OnChange += handler;

            return true;
        }

        public void Unsubscribe(NotifyPropertyChanged handler) {
            if (property == null) return;
            property.OnChange -= handler;
        }

        public override int GetValue() {
            if (type == typeof (bool)) {
                bool value = ((Property<bool>) property).GetValue();
                return Convert.ToInt32(value);
            } 
            if (type == typeof (int)) {
                int value = ((Property<int>)property).GetValue();
                return value;
            }
            if (type == typeof (float)) {
                float value = ((Property<float>)property).GetValue();
                return (int) value;
            }
            if (type == typeof (double)) {
                double value = ((Property<double>)property).GetValue();
                return (int) value;
            }

            Debug.LogError("Неизвестный тип " + type);
            return 0;
        }
    }

    public class Bracket : Token {
        public BracketType Type;
    }

    private static Dictionary<String, Operator> Operators = new Dictionary<String, Operator>() {
        {"&&", new Operator(OpType.AND, 2, 2)},
        {"||", new Operator(OpType.OR, 1, 2)},
        {"!", new Operator(OpType.NOT, 4, 1)},
        {"==", new Operator(OpType.EQUAL, 3, 2)},
        {">=", new Operator(OpType.GEQUAL, 3, 2)},
        {"<=", new Operator(OpType.LEQUAL, 3, 2)},
        {"!=", new Operator(OpType.NOT_EQUAL, 3, 2)},
        {">", new Operator(OpType.GREATER, 3, 2)},
        {"<", new Operator(OpType.LESS, 3, 2)}
    };

    private static Dictionary<String, BracketType> BracketTypes = new Dictionary<string, BracketType>() {
        {"(", BracketType.OPEN},
        {")", BracketType.CLOSE}
    };

    private static String CutOutOccurence(String str, String pattern) {
        int index = str.IndexOf(pattern);
        return str.Substring(index + pattern.Length);
    }

    public static List<Token> ParseTokens(String expression) {
        var variable = new Regex(@"^\s*([a-zA-Z.]+)");
        var operation = new Regex(@"^\s*(&&|\|\||!=|==|<=|>=|!|<|>)");
        var bracket = new Regex(@"^\s*([\(\)]+)");
        var constant = new Regex(@"^\s*([0-9]*\.?[0-9]+)");

        List<Token> result = new List<Token>();
        String rest = expression;
        Match match = null;
        while (rest.Length > 0) {
            match = variable.Match(rest);
            if (match.Success) {
                var text = match.Groups[0].Value.Trim();
                Token token = new Variable {
                    Text = text
                };
                result.Add(token);
                rest = CutOutOccurence(rest, text);

                continue;
            }

            match = operation.Match(rest);
            if (match.Success) {
                var text = match.Groups[0].Value.Trim();
                if (!Operators.ContainsKey(text)) {
                    UnityEngine.Debug.LogError("Неизвестный оператор " + text);
                    return null;
                }
                var op = Operators[text];
                Token token = new Operator {
                    Text = text,
                    Type = op.Type,
                    NumArgs = op.NumArgs,
                    Priority = op.Priority
                };
                result.Add(token);
                rest = CutOutOccurence(rest, text);

                continue;
            }

            match = bracket.Match(rest);
            if (match.Success) {
                var text = match.Groups[0].Value.Trim();
                if (!BracketTypes.ContainsKey(text)) {
                    UnityEngine.Debug.LogError("Неизвестный символ " + text);
                    return null;
                }
                var type = BracketTypes[text];
                Token token = new Bracket {
                    Text = text,
                    Type = type
                };
                result.Add(token);
                rest = CutOutOccurence(rest, text);
                
                continue;
            }

            match = constant.Match(rest);
            if (match.Success) {
                var text = match.Groups[0].Value.Trim();
                float value = Convert.ToSingle(text);
                Token token = new Contant {
                    Text = text,
                    Value = (int) value
                };
                result.Add(token);
                rest = CutOutOccurence(rest, text);

                continue;
            }

            Debug.LogError("Ошибка распознавания токенов в строке " + rest);
            return null;
        }

        return result;
    } 

    public static List<Token> ToReversePolishNotation(List<Token> tokens) {
        List<Token> result = new List<Token>();
        Stack<Token> stack = new Stack<Token>();

        foreach (var token in tokens) {
            if (token is Valuable) {
                result.Add(token);
            }
            else if (token is Operator) {
                Operator op = token as Operator;
                if (op.Type == OpType.NOT) {
                    //TODO: я считаю что NOT - это функция
                    stack.Push(op);
                    continue;
                }

                Operator topOperator = null;
                if (stack.Count > 0) topOperator = stack.Peek() as Operator;

                if (topOperator != null && topOperator.Priority > op.Priority) {
                    result.Add(stack.Pop());
                }
                stack.Push(op);
            }
            else if (token is Bracket) {
                var bracket = token as Bracket;
                if (bracket.Type == BracketType.OPEN) {
                    stack.Push(bracket);
                } else {
                    bool isOpenBracketFound = false;
                    while (stack.Count > 0) {
                        var top = stack.Pop();
                        if (top is Operator) {
                            result.Add(top);
                        }
                        if (top is Bracket) {
                            var topBracket = top as Bracket;
                            if (topBracket.Type == BracketType.OPEN) {
                                isOpenBracketFound = true;
                                break;
                            }
                        } 
                    }

                    if (!isOpenBracketFound) {
                        UnityEngine.Debug.LogError("Нарушен баланс скобок");
                    }
                }
            }
        }

        while (stack.Count > 0) {
            result.Add(stack.Pop());
        }

        return result;
    }

    public static int EvaluateTokens(List<Token> tokens) {
        var stack = new Stack<int>();

        foreach (var token in tokens) {
            if (token is Valuable) {
                var valuable = token as Valuable;
                int value = valuable.GetValue();
                stack.Push(value);
            }
            if (token is Operator) {
                var op = token as Operator;
                int numArgs = op.NumArgs;

                var values = new List<int>(); 
                for (int i = 0; i < numArgs; ++i) values.Add(stack.Pop());
                values.Reverse();

                bool result = false;
                switch (op.Type) {
                    case OpType.AND:
                        result = (values[0] != 0) && (values[1] != 0);
                        break;
                    case OpType.OR:
                        result = (values[0] != 0) || (values[1] != 0);
                        break;
                    case OpType.NOT:
                        result = values[0] == 0;
                        break;
                    case OpType.EQUAL:
                        result = (values[0] == values[1]);
                        break;
                    case OpType.NOT_EQUAL:
                        result = (values[0] != values[1]);
                        break;
                    case OpType.GEQUAL:
                        result = (values[0] >= values[1]);
                        break;
                    case OpType.LEQUAL:
                        result = (values[0] <= values[1]);
                        break;
                    case OpType.LESS:
                        result = (values[0] < values[1]);
                        break;
                    case OpType.GREATER:
                        result = (values[0] > values[1]);
                        break;
                }

                int intResult = Convert.ToInt32(result);
                stack.Push(intResult);
            }
        }
        return stack.Pop();
    } 
} 