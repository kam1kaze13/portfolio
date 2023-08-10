namespace Calc.SocketServer
{
    using System;

    using Protocol;

    public class CalcProcessor
    {
        /// <summary>
        /// Создает ответ для запроса
        /// </summary>
        /// <param name="request">запрос</param>
        /// <returns>ответ</returns>
        public CalcResponse Process(CalcRequest request)
        {
            long result = 0;

            switch(request.OperationType)
            {
                case OperationType.Add:
                    {
                        result = (long)request.Argument1 + request.Argument2;
                        break;
                    }
                case OperationType.Sub:
                    {
                        result = (long)request.Argument1 - request.Argument2;
                        break;
                    }
                case OperationType.Mult:
                    {
                        result = (long)request.Argument1 * request.Argument2;
                        break;
                    }
            }

            return new CalcResponse { Result = result };
        }
    }
}
