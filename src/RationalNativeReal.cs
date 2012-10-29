using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    class RationalNativeReal : Field<RationalNativeReal>, Ring<RationalNativeReal>
    {
        public static implicit operator RationalNativeReal(NativeReal v)
        {
            return new RationalNativeReal(v);
        }

        public static implicit operator RationalNativeReal(double v)
        {
            return new RationalNativeReal(new NativeReal(v));
        }

        public RationalNativeReal(RationalNativeReal c)
        {
            numerator = new NativeReal(c.numerator);
            denominator = new NativeReal(c.denominator);

            if (denominator == new NativeReal(0))
            {
                throw new UndefinedException("Divide By 0");
            }

            normalize();
        }

        public RationalNativeReal()
            : this(new NativeReal(0), new NativeReal(1))
        {

        }

        public RationalNativeReal(NativeReal n, NativeReal d)
        {
            numerator = new NativeReal(n);
            denominator = new NativeReal(d);

            normalize();
        }

        public RationalNativeReal(NativeReal n)
            : this(n, new NativeReal(1))
        {

        }

        public RationalNativeReal getIdentityElement()
        {
            return new RationalNativeReal(new NativeReal(1), new NativeReal(1));
        }

        public RationalNativeReal getEmptyElement()
        {
            return new RationalNativeReal(new NativeReal(0), new NativeReal(1));
        }

        public RationalNativeReal getSum(RationalNativeReal rhs)
        {
            NativeReal summedNumerator;
            NativeReal denominatorToUse;

            if (denominator == rhs.denominator)
            {
                denominatorToUse = new NativeReal(denominator);
                summedNumerator = numerator.getSum(rhs.numerator);
            }
            else
            {
                NativeReal scaledLHSNumerator = numerator.getProduct(rhs.denominator);
                NativeReal scaledRHSNumerator = rhs.numerator.getProduct(denominator);
                summedNumerator = scaledLHSNumerator.getSum(scaledRHSNumerator);
                denominatorToUse = denominator.getProduct(rhs.denominator);
            }

            return new RationalNativeReal(summedNumerator, denominatorToUse);
        }

        public RationalNativeReal getDifference(RationalNativeReal rhs)
        {
            NativeReal differencedNumerator;
            NativeReal denominatorToUse;

            if (denominator == rhs.denominator)
            {
                denominatorToUse = new NativeReal(denominator);
                differencedNumerator = numerator.getDifference(rhs.numerator);
            }
            else
            {
                NativeReal scaledLHSNumerator = numerator.getProduct(rhs.denominator);
                NativeReal scaledRHSNumerator = rhs.numerator.getProduct(denominator);
                differencedNumerator = scaledLHSNumerator.getDifference(scaledRHSNumerator);
                denominatorToUse = denominator.getProduct(rhs.denominator);
            }

            return new RationalNativeReal(differencedNumerator, denominatorToUse);
        }

        public RationalNativeReal getProduct(RationalNativeReal rhs)
        {
            NativeReal newNumerator = numerator.getProduct(rhs.numerator);
            NativeReal newDenominator = denominator.getProduct(rhs.denominator);

            return new RationalNativeReal(newNumerator, newDenominator);
        }

        public RationalNativeReal getQuotient(RationalNativeReal rhs)
        {
            return getProduct(rhs.getReciprocal());
        }

        public bool isNegative()
        {
            if (numerator == new NativeReal(0))
            {
                return false;
            }
            else
            {
                return numerator.isNegative() ^ denominator.isNegative();
            }
        }

        public RationalNativeReal getReciprocal()
        {
            return new RationalNativeReal(new NativeReal(denominator), new NativeReal(numerator));
        }

        private void normalize()
        {
            NativeReal zero = new NativeReal(0);
            if (numerator == denominator)
            {
                numerator = new NativeReal(1);
                denominator = new NativeReal(1);
            }
            else if (numerator.getAbs() == zero)
            {
                numerator = new NativeReal(0);
                denominator = new NativeReal(1);
            }

            while (denominator.hasFractionalPart() || numerator.hasFractionalPart())
            {

                denominator = denominator.getShiftedRight();
                numerator = numerator.getShiftedRight();

            }

            
            if (numerator != zero && denominator != zero)
            {
                NativeReal gcd = LSMath.gcd(numerator, denominator);
                if (gcd != zero)
                {
                    numerator = numerator.getQuotient(gcd);
                    denominator = denominator.getQuotient(gcd);
                }
            }
        }

        public override string ToString()
        {

            if (denominator == new NativeReal(1))
            {
                return numerator.ToString();
            }
            else
            {
                String stringRepresentation = numerator.getAbs().ToString() + "/" + denominator.getAbs().ToString();

                if (isNegative())
                {
                    stringRepresentation = "-" + stringRepresentation;
                }

                return stringRepresentation;
            }
        }

        private NativeReal numerator;
        private NativeReal denominator;
    }
}
