using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public static class CustomPredicate
    {
        public static Func<string, bool> Or(params Func<string, bool>[] functions)
        {
            return x => functions.Any(f => f(x));
        }

        public static Func<string, bool> And(params Func<string, bool>[] functions)
        {
            return x => functions.All(f => f(x));
        }
    }
}
