namespace Calc.Protocol
{
    using System.Collections.Generic;

    /// <summary>
    /// Общий класс для представления результатов парсинга данных
    /// </summary>
    /// <typeparam name="TParsedObject">тип распарсенных объектов</typeparam>
    public class ParsingResult<TParsedObject>
    {
        public ParsingResult()
        {
            this.ParsedObjects = new List<TParsedObject>();
        }

        /// <summary>
        /// Размер обработанных данных
        /// </summary>
        public int ProcessedBytes { get; set; }

        /// <summary>
        /// Считать ли сессию закрытой
        /// </summary>
        public bool IsClosed { get; set; }

        public List<TParsedObject> ParsedObjects { get; set; }
    }
}
