using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    interface Field<T> : Semiring<T>
    {
        T getDifference(T rhs);
        T getQuotient(T rhs);
    }
}
