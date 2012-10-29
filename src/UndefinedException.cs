using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    class UndefinedException : Exception
    {
        public UndefinedException() : base("Undefined") { }
        public UndefinedException(String m) : base(m) { }
    }
}
