using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    class NativeReal : Field<NativeReal>, Ring<NativeReal>
    {
        public static implicit operator NativeReal(double v)
        {
            return new NativeReal(v);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Real return false.
            NativeReal r = obj as NativeReal;
            if ((System.Object)r == null)
            {
                return false;
            }

            return this.data == r.data;
        }


        public static bool operator ==(NativeReal one, NativeReal two)
        {
            if (System.Object.ReferenceEquals(one, two))
            {
                // Both null or identical instance
                return true;
            }
            else if (((object)one == null) || ((object)two == null))
            {
                // One null
                return false;
            }
            else
            {
                return one.data == two.data;
            }
        }

        public static bool operator !=(NativeReal one, NativeReal two)
        {
            return !(one == two);
        }






        public bool Equals(NativeReal r)
        {
            if ((System.Object)r == null)
            {
                return false;
            }

            return this.data == r.data;
        }

        public override int GetHashCode()
        {
            return (int)data;
        }

        public NativeReal(NativeReal r)
        {
            data = r.data;
        }

        public NativeReal(double v)
        {
            data = v;
        }

        public bool isNegative()
        {
            return data < 0;
        }

        public NativeReal getAbs()
        {
            NativeReal abs = new NativeReal(Math.Abs(data));
            return abs;
        }

        public bool hasFractionalPart()
        {
            return data % 1 != 0;
        }

        public NativeReal getShiftedRight()
        {
            return new NativeReal(data / 10);
        }

        public NativeReal getShiftedLeft()
        {
            return new NativeReal(data * 10);
        }

        public NativeReal()
        {
            data = 0;
        }

        public NativeReal getSum(NativeReal s)
        {
            return new NativeReal(s.data + this.data);
        }

        public NativeReal getProduct(NativeReal s)
        {
            return new NativeReal(s.data * this.data);
        }

        public NativeReal getDifference(NativeReal s)
        {
            return new NativeReal(this.data - s.data);
        }

        public NativeReal getQuotient(NativeReal s)
        {
            if (s.data == 0) throw new UndefinedException();

            return new NativeReal(this.data / s.data);
        }

        public NativeReal getEmptyElement()
        {
            return new NativeReal(0);
        }

        public NativeReal getIdentityElement()
        {
            return new NativeReal(1);
        }

        public override String ToString()
        {
            return data.ToString();
        }

        public static bool operator <(NativeReal one, NativeReal two)
        {
            return one.data < two.data;
        }

        public static bool operator >(NativeReal one, NativeReal two)
        {
            return one.data > two.data;
        }



        private double data;
    }
}
