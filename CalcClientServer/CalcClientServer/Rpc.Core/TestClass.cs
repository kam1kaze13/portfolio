using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpc.Core
{
    public interface ICall
    {
        string Call(string value);
    }


    class CallImpl : ICall
    {
        public string Call(string value)
        {
            char[] charArray = value.ToCharArray();

            Array.Reverse(charArray);

            return new string(charArray);
        }
    }
}
