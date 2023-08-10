namespace Calc.Protocol
{
    using System;

    /// <summary>
    /// Общий класс для специфических ошибок при работе с протоколом
    /// </summary>
    public class ProtocolException : ApplicationException
    {
        public ProtocolException() : base()
        {
        }

        public ProtocolException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Сервер не может распознать запрос
    /// </summary>
    public class BadRequestException : ProtocolException
    {
        public BadRequestException()
            : base("Server can't parse input data")
        {
        }
    }

    /// <summary>
    /// Клиент не может распознать ответ сервера
    /// </summary>
    public class BadResponseException : ProtocolException
    {
        public BadResponseException()
            : base("Bad data from server")
        {
        }
    }
}
