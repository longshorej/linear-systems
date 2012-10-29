using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    interface SetIdentity<T>
    {
        T getIdentityElement();
        T getEmptyElement();
    }
}
