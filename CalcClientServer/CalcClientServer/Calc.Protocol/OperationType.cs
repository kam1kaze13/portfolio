namespace Calc.Protocol
{
    using System;

    public enum OperationType
    {
        Add,
        Sub,
        Mult,
    }

    /// <summary>
    /// Для преобразования OperationType в строку и обратно
    /// </summary>
    public static class OperationExtension
    {
        public static string ToProtocolString(this OperationType operationType)
        {
            switch (operationType)
            {
                case OperationType.Add:
                    return "+";
                case OperationType.Sub:
                    return "-";
                case OperationType.Mult:
                    return "*";
                default:
                    throw new ArgumentException("Bad operation", "operationType");
            }
        }

        public static OperationType ParseProtocolString(string str)
        {
            switch (str)
            {
                case "+":
                    return OperationType.Add;
                case "-":
                    return OperationType.Sub;
                case "*":
                    return OperationType.Mult;
                default:
                    throw new ArgumentException("Bad operation", "str");
            }
        }
    }
}
