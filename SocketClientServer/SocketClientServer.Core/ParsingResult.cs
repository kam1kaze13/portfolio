using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientServer.Core
{
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
