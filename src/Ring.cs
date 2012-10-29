using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    // For my purposes, ring = field
    interface Ring<T> : Semiring<T>
    {
        T getDifference(T rhs);
        T getQuotient(T rhs);
    }
}
