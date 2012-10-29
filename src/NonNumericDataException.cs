using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    class NonNumericDataException : Exception
    {
        public NonNumericDataException() : base("Non Numeric Data Entered") { }
    }
}
