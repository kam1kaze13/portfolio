namespace Calc.SocketClient
{
    using Protocol;

    /// <summary>
    /// Представляет операцию, происходящую на сервере
    /// </summary>
    public class Operation
    {
        public int Argument1 { get; set; }

        public int Argument2 { get; set; }

        public OperationType OperationType { get; set; }

        public Operation(int arg1, OperationType op, int arg2)
        {
            this.Argument1 = arg1;
            this.Argument2 = arg2;
            this.OperationType = op;
        }

        public static Operation Add(int arg1, int arg2)
        {
            return new Operation(arg1, OperationType.Add, arg2);
        }

        public static Operation Sub(int arg1, int arg2)
        {
            return new Operation(arg1, OperationType.Sub, arg2);
        }

        public static Operation Mult(int arg1, int arg2)
        {
            return new Operation(arg1, OperationType.Mult, arg2);
        }
    }
}
