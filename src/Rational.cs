using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    class Rational : Field<Rational>, Ring<Rational>
    {
        public static implicit operator Rational(TruncatableRealNumber v)
        {
            return new Rational(v);
        }

        public static implicit operator Rational(double v)
        {
            return new Rational(new TruncatableRealNumber(v));
        }

        public Rational(Rational c) {
            numerator = new TruncatableRealNumber(c.numerator);
            denominator = new TruncatableRealNumber(c.denominator);

            if (denominator == new TruncatableRealNumber(0)) {
                throw new UndefinedException("Divide By 0");
            }

			normalize();
        }

        public Rational() : this(new TruncatableRealNumber(0), new TruncatableRealNumber(1) ) { }

        public Rational(TruncatableRealNumber n, TruncatableRealNumber d) {
            numerator = new TruncatableRealNumber(n);
            denominator = new TruncatableRealNumber(d);

            normalize();
        }

        public Rational(TruncatableRealNumber n) : this(n, 1) {

        }

        public Rational getIdentityElement() {
            return new Rational(1);
        }

        public Rational getEmptyElement() {
            return new Rational(0);
        }

        public Rational getSum(Rational rhs) {
            TruncatableRealNumber summedNumerator;
            TruncatableRealNumber denominatorToUse;

            if (denominator == rhs.denominator)
            {
                denominatorToUse = new TruncatableRealNumber(denominator);
                summedNumerator = numerator.getSum(rhs.numerator);
            }
            else
            {
                TruncatableRealNumber scaledLHSNumerator = numerator.getProduct(rhs.denominator);
                TruncatableRealNumber scaledRHSNumerator = rhs.numerator.getProduct(denominator);
                summedNumerator = scaledLHSNumerator.getSum(scaledRHSNumerator);
                denominatorToUse = denominator.getProduct(rhs.denominator);
            }

            return new Rational(summedNumerator, denominatorToUse);
        }

        public Rational getDifference(Rational rhs) {
            TruncatableRealNumber differencedNumerator;
            TruncatableRealNumber denominatorToUse;

            if (denominator == rhs.denominator) {
                denominatorToUse = new TruncatableRealNumber(denominator);
                differencedNumerator = numerator.getDifference(rhs.numerator);
            }
            else {
                TruncatableRealNumber scaledLHSNumerator = numerator.getProduct(rhs.denominator);
                TruncatableRealNumber scaledRHSNumerator = rhs.numerator.getProduct(denominator);
                differencedNumerator = scaledLHSNumerator.getDifference(scaledRHSNumerator);
                denominatorToUse = denominator.getProduct(rhs.denominator);
            }

            return new Rational(differencedNumerator, denominatorToUse);
        }

        public Rational getProduct(Rational rhs) {
            TruncatableRealNumber newNumerator = numerator.getProduct(rhs.numerator);
            TruncatableRealNumber newDenominator = denominator.getProduct(rhs.denominator);

            return new Rational(newNumerator, newDenominator);
        }

        public Rational getQuotient(Rational rhs) {
            return getProduct(rhs.getReciprocal());
        }

        public bool isNegative() {
            if (numerator == new TruncatableRealNumber(0)) {
                return false;
            }
            else {
                return numerator.isNegative() ^ denominator.isNegative();
            }
        }

        public Rational getReciprocal() {
            return new Rational(new TruncatableRealNumber(denominator), new TruncatableRealNumber(numerator));
        }

        private void normalize() {
		   	if (numerator == denominator) {
                numerator = 1;
                denominator = 1;
            }
            else if (numerator.getAbs() == 0) {
                numerator = 0;
                denominator = 1;
            }

			while( denominator.hasFractionalPart() || numerator.hasFractionalPart() ) {
                // Remove fractions in numerator/denominator by shifting both right
				denominator = denominator.getShiftedLeft();
				numerator = numerator.getShiftedLeft();
			}
           
            /*
			if( numerator != 0 && denominator != 0)
			{
				TruncatableRealNumber gcd = LSMath.gcd(numerator, denominator);
				if( gcd != zero) {
					numerator /= gcd;
					denominator = gcd;
				}
			}*/
        }

        public override string ToString() {
            return (numerator / denominator).ToString();

            String stringRepresentation;
            if (denominator.getAbs() == 1) {
                stringRepresentation = numerator.ToString();
            }
            else {
                stringRepresentation = numerator.getAbs().ToString() + "/" + denominator.getAbs().ToString();
            }

            return isNegative() ? "-" + stringRepresentation : stringRepresentation;
        }

        private TruncatableRealNumber numerator;
        private TruncatableRealNumber denominator;
    }
}
