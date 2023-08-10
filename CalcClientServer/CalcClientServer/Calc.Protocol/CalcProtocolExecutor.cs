namespace Calc.Protocol
{   
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Инкапсулирует работу с бинарными данными из сокета
    /// </summary>
    public class CalcProtocolExecutor : IProtocolExecutor<CalcRequest, CalcResponse>
    {
        private const string ErrorMessage = "Can't parse the expression";
        private static readonly Regex RequestRegex = new Regex(@"Calculate (-?\d{1,10})(\+|-|\*)(-?\d{1,10})", RegexOptions.Compiled);
        private static readonly Regex ResponseRegex = new Regex(@"The result of expression is (-?\d{1,20})", RegexOptions.Compiled);

        /// <summary>
        /// Распознает запросы из бинарных данных
        /// </summary>
        /// <param name="data">буфер</param>
        /// <param name="length">размер данных</param>
        /// <returns>распознанный результат</returns>
        public ParsingResult<CalcRequest> ParseInput(byte[] data, int length)
        {
            // TODO: реализовать
            var result = new ParsingResult<CalcRequest>();
            int start = 0;
            for (int i = 0; i < length; i++)
            {
                // ищем ";", которые сигнализируют об окончании запроса

                // если две ";" подряд - означают конец сеанса

                // получаем строку запроса
                // Encoding.ASCII.GetString

                // распознаем запрос с помощью RequestRegex
                // и добавляем распарсенный объект запроса в результат

                var element = Encoding.ASCII.GetString(data,i,1);

                if (element == ";")
                {
                    if (Encoding.ASCII.GetString(data, start, 1) == ";")
                    {
                        result.IsClosed = true;
                        break;
                    }

                    var arg1 = BitConverter.ToInt32(data, start);
                    var opType = OperationExtension.ParseProtocolString(Encoding.ASCII.GetString(data, start+4, 1));
                    var arg2 = BitConverter.ToInt32(data, start+5);

                    result.ParsedObjects.Add(new CalcRequest { Argument1 = arg1, Argument2 = arg2, OperationType = opType });

                    start = i+1;
                }

            }

            // не забываем поставить нужное количество в result.ProcessedBytes
            result.ProcessedBytes = start;
           
            return result;
        }

        /// <summary>
        /// Распознает ответы сервера из бинарных данных
        /// </summary>
        /// <param name="data">буфер</param>
        /// <param name="length">размер данных</param>
        /// <returns>распознанный результат</returns>
        public ParsingResult<CalcResponse> ParseOutput(byte[] data, int length)
        {
            // TODO: реализовать
            var result = new ParsingResult<CalcResponse>();
            int start = 0;
            for (int i = 0; i < data.Length; i++)
            {
                // ищем ";", которые сигнализируют об окончании запроса

                // если две ";" подряд - означают конец сеанса

                // получаем строку ответа
                // Encoding.ASCII.GetString

                // распознаем запрос с помощью RequestRegex
                // и добавляем распарсенный объект ответа в результат

                var element = Encoding.ASCII.GetString(data, i, 1);

                if (element == ";")
                {
                    if (Encoding.ASCII.GetString(data, start, 1) == ";")
                    {
                        result.IsClosed = true;
                        break;
                    }

                    var outputResult = BitConverter.ToInt64(data, start);

                    result.ParsedObjects.Add(new CalcResponse { Result = outputResult});

                    start = i+1;
                }

            }

            // не забываем поставить нужное количество в result.ProcessedBytes
            result.ProcessedBytes = start;

            return result;
        }

        /// <summary>
        /// Конструирует бинарные данные для запроса
        /// </summary>
        /// <param name="request">запрос</param>
        /// <returns>данные</returns>
        public byte[] CreateRequest(CalcRequest request)
        {
            // TODO: реализовать
            var arg1 = BitConverter.GetBytes(request.Argument1);
            var operation = Encoding.ASCII.GetBytes(OperationExtension.ToProtocolString(request.OperationType));
            var arg2 = BitConverter.GetBytes(request.Argument2);
            var end = Encoding.ASCII.GetBytes(";");


            return arg1.Concat(operation).Concat(arg2).Concat(end).ToArray();
        }

        /// <summary>
        /// Конструирует бинарные данные для ответа
        /// </summary>
        /// <param name="response">ответ</param>
        /// <returns>данные</returns>
        public byte[] CreateResponse(CalcResponse response)
        {
            // TODO: реализовать

            var res = BitConverter.GetBytes(response.Result);
            var end = Encoding.ASCII.GetBytes(";");

            return res.Concat(end).ToArray();
        }

        /// <summary>
        /// Конструирует бинарные данные для ответа об ошибке
        /// </summary>
        public byte[] CreateErrorMessage()
        {
            // TODO: реализовать
            var msg = Encoding.ASCII.GetBytes(ErrorMessage);
            var end = Encoding.ASCII.GetBytes(";");
            return msg.Concat(end).ToArray();
        }
    }
}
