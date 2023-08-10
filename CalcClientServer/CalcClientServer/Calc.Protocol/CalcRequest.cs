namespace Calc.Protocol
{
    /// <summary>
    /// Запрос операции
    /// </summary>
    public class CalcRequest
    {
        public int Argument1 { get; set; }

        public int Argument2 { get; set; }

        public OperationType OperationType { get; set; }
    }
}
