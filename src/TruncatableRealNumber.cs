using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace LinearSystems
{
    class TruncatableRealNumber : Field<TruncatableRealNumber>, Ring<TruncatableRealNumber>
    {
        public static implicit operator TruncatableRealNumber(double v)
        {
            return new TruncatableRealNumber(v);
        }

        public static TruncatableRealNumber operator -(TruncatableRealNumber t) {
            return t.getOpposite();
        }

        public static TruncatableRealNumber  operator * (TruncatableRealNumber t1, TruncatableRealNumber t2) {
            return t1.getProduct(t2);
        }

        public static TruncatableRealNumber operator / (TruncatableRealNumber t1, TruncatableRealNumber t2) {
            return t1.getQuotient(t2);
        }

        public static TruncatableRealNumber operator + (TruncatableRealNumber t1, TruncatableRealNumber t2)
        {
            return t1.getSum(t2);
        }

        public static TruncatableRealNumber operator -(TruncatableRealNumber t1, TruncatableRealNumber t2)
        {
            return t1.getDifference(t2);
        }


        public static bool isNumeric(string someString)
        {
            // The easiest way was to express with two regexp calls
            return (Regex.IsMatch(someString, "^(-)?(\\.)([0-9])+$") || Regex.IsMatch(someString, "^(-)?[0-9]+((\\.)[0-9]+)?$"));
        }

        private TruncatableRealNumber(LinkedList<long> leftDigits, LinkedList<long> rightDigits) : this(leftDigits, rightDigits, false) { }

        private TruncatableRealNumber(LinkedList<long> leftDigits, LinkedList<long> rightDigits, bool isNegative) : this(leftDigits, rightDigits, isNegative, false) { }

        private TruncatableRealNumber(LinkedList<long> leftDigits, LinkedList<long> rightDigits, bool isNegative, bool isTruncated)
        {
            digitsToTheLeft = new LinkedList<long>(leftDigits);
            digitsToTheRight = new LinkedList<long>(rightDigits);
            truncated = isTruncated;
            negative = isNegative;
            normalize();
        }

        public TruncatableRealNumber(TruncatableRealNumber copy)
        {
            digitsToTheLeft = new LinkedList<long>(copy.digitsToTheLeft);
            digitsToTheRight = new LinkedList<long>(copy.digitsToTheRight);
            truncated = copy.truncated;
            negative = copy.negative;
            normalize();
        }

        public TruncatableRealNumber(string ns)
        {
            if (!TruncatableRealNumber.isNumeric(ns))
            {
                throw new NonNumericDataException();
            }

            if (ns.Length > 0 && ns.Substring(0, 1) == "-")
            {
                negative = true;
                ns = ns.Substring(1);
            }

            digitsToTheLeft = new LinkedList<long>();
            digitsToTheRight = new LinkedList<long>();
            truncated = false;
            
            string[] numberStringParts = ns.Split('.');
            string digitsToTheLeftString = numberStringParts[0];
            string digitsToTheRightString = numberStringParts.Length > 1 ? numberStringParts[1] : "";

            // Throw in digits to the left
            for (int i = 0; i < digitsToTheLeftString.Length; i++)
            {
                digitsToTheLeft.AddFirst(long.Parse(digitsToTheLeftString.Substring(i, 1)));
            }

            // Throw in digits to the right
            for (int i = 0; i < digitsToTheRightString.Length; i++)
            {
                digitsToTheRight.AddLast(long.Parse(digitsToTheRightString.Substring(i, 1)));
            }

            normalize();
        }

        public TruncatableRealNumber(double value) : this( value.ToString() )
        {

        }

        public TruncatableRealNumber() : this(0)
        {

        }

        public bool isTruncated()
        {
            return truncated;
        }

        public bool isNegative()
        {
            return negative;
        }

		public bool hasFractionalPart()
		{
			return digitsToTheRight.Count > 0;
		}


        public TruncatableRealNumber getEmptyElement()
        {
            return new TruncatableRealNumber(0);
        }

        public TruncatableRealNumber getIdentityElement()
        {
            return new TruncatableRealNumber(1);
        }

        /**
         * Conceptually equivalent of casting a double
         * to int. Drops the fractional part
         */
        public TruncatableRealNumber getRepresentationWithDroppedFractionalPart()
        {
            TruncatableRealNumber copy = new TruncatableRealNumber(this);
            copy.digitsToTheRight = new LinkedList<long>();
            copy.normalize();
            return copy;
        }


        public static TruncatableRealNumber getApproximation(TruncatableRealNumber lhs, TruncatableRealNumber rhs)
        {
            return TruncatableRealNumber.getApproximation(lhs, rhs, new TruncatableRealNumber(1), new TruncatableRealNumber(lhs));
        }
        /**
         * Returns the biggest number s such that rhs * cg <= lhs
         * Returns 0 if rhs > lhs
         * Returns lhs if rhs = 1
         * 
         * Preconditions: LHS, RHS have no fractional parts. They will be ignored.
         */
        public static TruncatableRealNumber getApproximation(TruncatableRealNumber lhs, TruncatableRealNumber rhs, TruncatableRealNumber lowParameter, TruncatableRealNumber highParameter)
        {
            if (rhs > lhs) {
                return 0;
            }
            else if(rhs == 1) {
                return lhs;
            }

            TruncatableRealNumber low = lowParameter = lowParameter.getRepresentationWithDroppedFractionalPart();
            TruncatableRealNumber high = highParameter = highParameter.getRepresentationWithDroppedFractionalPart();
            TruncatableRealNumber currentGuessForQuotient = ((low + high) * 0.5).getRepresentationWithDroppedFractionalPart();
            TruncatableRealNumber currentEvaluationOfQuotient = currentGuessForQuotient * rhs;

            if (currentEvaluationOfQuotient < lhs) {
                TruncatableRealNumber sum = currentEvaluationOfQuotient + rhs;

                if(sum > lhs) {
                    // Bounds Case: We have the right answer.
                    return currentGuessForQuotient;
                }
                else if (sum == lhs) {
                    // Bounds Case: The next number is the right answer.
                    return currentGuessForQuotient + 1;
                }
                else {
                    // Not on a bounds case -- recurse
                    return getApproximation(lhs, rhs, currentGuessForQuotient, high);
                }
            }
            else if(currentEvaluationOfQuotient == lhs ) {
                return currentGuessForQuotient;
            }
            else {
                TruncatableRealNumber difference = currentEvaluationOfQuotient - rhs;

                if (difference <= lhs) {
                    // Bounds Case: The previous number is the right answer
                    return currentGuessForQuotient - 1;
                }
                else {
                    // Not on a bounds case -- recurse
                    return getApproximation(lhs, rhs, low, currentGuessForQuotient - 1);
                }
            }
        }


        public TruncatableRealNumber getQuotient(TruncatableRealNumber rhs) {
			if(rhs == 0) {
				throw new UndefinedException();
			}
            else if(this == rhs) {
                return 1;
            }
            else if(rhs == 1) {
				return new TruncatableRealNumber(this);
			}
            else if(this == 0) {
				return 0;
			}
            else if (hasFractionalPart() || rhs.hasFractionalPart()) {
                // Scale out until there is no more fractional part, then re-call
                TruncatableRealNumber scaledLHS = this.getShiftedLeft();
                TruncatableRealNumber scaledRHS = rhs.getShiftedLeft();

                while (scaledLHS.hasFractionalPart() || scaledRHS.hasFractionalPart())
                {
                    scaledLHS = scaledLHS.getShiftedLeft();
                    scaledRHS = scaledRHS.getShiftedLeft();
                }

                return scaledLHS / scaledRHS;
            } 
			else {
				// RHS does not have a decimal part
				LinkedList<long> newLeftDigits = new LinkedList<long>();
				LinkedList<long> newRightDigits = new LinkedList<long>();

                // Loop Control + Sign Calculation
                bool doneDividing = false;
                bool newIsTruncated = false;
                bool newIsNegative = isNegative() ^ rhs.isNegative();

                // Setup LHS/RHS
				TruncatableRealNumber lhs = getAbs();
				rhs = rhs.getAbs();

				int iterationCounter = 0;
				while(!doneDividing) {
                    TruncatableRealNumber currentGuess = getApproximation(lhs, rhs);

                    if ( iterationCounter < 1 ) {
                        // The first iteration will calculate the entire whole-number portion
						foreach( long digit in currentGuess.digitsToTheLeft ) {
							newLeftDigits.AddLast(digit);
						}				
					}
					else {
						foreach(long digit in currentGuess.digitsToTheLeft) {
							newRightDigits.AddLast(digit);
						}
					}

                    lhs -= (currentGuess * rhs);

                    if (lhs == 0) {
                        doneDividing = true;
                    }
                    else {
                        lhs = lhs.getShiftedLeft();
                        if (iterationCounter > MAX_DIVISION_PRECISION) {
                            doneDividing = true;
                            newIsTruncated = true;
                        }
                    }

					iterationCounter++;
				}

				TruncatableRealNumber newTRN = new TruncatableRealNumber(newLeftDigits, newRightDigits, newIsNegative, newIsTruncated);
                return newTRN;		
			}
        }


        /**
         * Returns a TRN with the digits shifted to
         * the right. That is, the first digit to
         * the left is made the first digit to the
         * right and removed. If none exists, a 0
         * is inserted at the beginning of right.
         * This has the effect of dividing by the
         * base.
         */
		public TruncatableRealNumber getShiftedRight() {
			TruncatableRealNumber copy = new TruncatableRealNumber(this);

			if(digitsToTheLeft.Count > 0 ) {
				copy.digitsToTheRight.AddFirst( digitsToTheLeft.First.Value );
				copy.digitsToTheLeft.RemoveFirst();
			}
			else {
				copy.digitsToTheRight.AddFirst( 0 );
			}

			copy.normalize();
			return copy;
		}

        /**
         * Returns a TRN with the digits shifted to
         * the left. That is, the first digit to the right
         * is removed and made the first digit to the left,
         * or a 0 is added if no digits to the right exist.
         * This has the effect of multiplying by numberBase
         */
		public TruncatableRealNumber getShiftedLeft() {
			TruncatableRealNumber copy = new TruncatableRealNumber(this);
			if( !copy.hasFractionalPart() ) {
				copy.digitsToTheLeft.AddFirst(0);
				copy.normalize();
				return copy;
			}
			else {
				// Move the first digitsToTheRight to the first digitsToTheLeft			
				copy.digitsToTheLeft.AddFirst ( digitsToTheRight.First.Value );
				copy.digitsToTheRight.RemoveFirst();
				copy.normalize();
				return copy;
			}
		}

        public TruncatableRealNumber getProduct(TruncatableRealNumber r)
        {
            // Algorithm is good, structure sucks.
            // TODO: cleanup by using nodes instead
            // of foreaches

            if (r == 0 || this == 0) {
                return 0;
            }
            else if (r == 1) {
                return new TruncatableRealNumber(this);
            }
            else if (this == 1) {
                return new TruncatableRealNumber(r);
            }
            else {

                TruncatableRealNumber rationalToUse;
                TruncatableRealNumber otherRational;

                if ((r.digitsToTheLeft.Count + r.digitsToTheRight.Count) > (digitsToTheLeft.Count + digitsToTheRight.Count))
                {
                    rationalToUse = r;
                    otherRational = this;
                }
                else
                {
                    rationalToUse = this;
                    otherRational = r;
                }

                LinkedList<long> digitsToUse = new LinkedList<long>();
                LinkedList<long> otherDigits = new LinkedList<long>();

                foreach (long digit in rationalToUse.digitsToTheLeft)
                {
                    digitsToUse.AddLast(digit);
                }

                foreach (long digit in rationalToUse.digitsToTheRight)
                {
                    digitsToUse.AddFirst(digit);
                }

                foreach (long digit in otherRational.digitsToTheLeft)
                {
                    otherDigits.AddLast(digit);
                }

                foreach (long digit in otherRational.digitsToTheRight)
                {
                    otherDigits.AddFirst(digit);
                }

                long[] newDigits = new long[digitsToUse.Count];

                int counter = 0;
                int bottomDigitCounter = 0;
                foreach (long bottomDigit in otherDigits)
                {
                    counter = bottomDigitCounter;
                    foreach (long topDigit in digitsToUse)
                    {
                        if (counter >= newDigits.Length - 1)
                        {
                            int i = 0;
                            long[] resizedDigits = new long[newDigits.Length + 1];
                            foreach (long digit in newDigits)
                            {
                                resizedDigits[i] = digit;
                                i++;
                                newDigits = resizedDigits;
                            }
                        }
                        newDigits[counter] += bottomDigit * topDigit;
                        counter++;
                    }
                    bottomDigitCounter++;
                }

                LinkedList<long> allNewDigits = new LinkedList<long>();
                foreach (long digit in newDigits)
                {
                    allNewDigits.AddLast(digit);
                }

                TruncatableRealNumber normalized = new TruncatableRealNumber("0");
                normalized.digitsToTheLeft = allNewDigits;
                normalized.normalize();

                LinkedList<long> normalizedDigits = new LinkedList<long>();

                foreach (long digit in normalized.digitsToTheLeft)
                {
                    normalizedDigits.AddFirst(digit);
                }

                foreach (long digit in normalized.digitsToTheRight)
                {
                    normalizedDigits.AddLast(digit);
                }

                LinkedList<long> newLeftDigits = new LinkedList<long>();
                LinkedList<long> newRightDigits = new LinkedList<long>();

                int maxLeftSize = normalizedDigits.Count - (rationalToUse.digitsToTheRight.Count + otherRational.digitsToTheRight.Count);
                foreach (long digit in normalizedDigits)
                {
                    if (newLeftDigits.Count < maxLeftSize)
                    {
                        newLeftDigits.AddFirst(digit);
                    }
                    else
                    {
                        newRightDigits.AddLast(digit);
                    }
                }

                return new TruncatableRealNumber(newLeftDigits, newRightDigits, rationalToUse.isNegative() ^ otherRational.isNegative());
            }
        }

        public TruncatableRealNumber getSum(TruncatableRealNumber rhs) {
            if (rhs == 0) {
				return new TruncatableRealNumber(this);
			}
            else if (this == 0) {
				return new TruncatableRealNumber(rhs);
			}
			else if(isNegative()) {
                return rhs - -this;
			}
			else if(rhs.isNegative()) {
                return this - -rhs;
			}
			else {

				// Summing two positive numbers
                LinkedList<long> newLeftDigits = new LinkedList<long>();
                LinkedList<long> newRightDigits = new LinkedList<long>();

                // Pad digits so that they match up
                int mostLeftDigits = rhs.digitsToTheLeft.Count > digitsToTheLeft.Count ? rhs.digitsToTheLeft.Count : digitsToTheLeft.Count;
                int mostRightDigits = rhs.digitsToTheRight.Count > digitsToTheRight.Count ? rhs.digitsToTheRight.Count : digitsToTheRight.Count;

                LinkedList<long> lhsLeftDigits = getPaddedDigitsToTheLeft(mostLeftDigits);
                LinkedList<long> lhsRightDigits = getPaddedDigitsToTheRight(mostRightDigits);

                LinkedList<long> rhsLeftDigits = rhs.getPaddedDigitsToTheLeft(mostLeftDigits);
                LinkedList<long> rhsRightDigits = rhs.getPaddedDigitsToTheRight(mostRightDigits);

                // Sum the nodes
                LinkedListNode<long> currentLHSNode;
                LinkedListNode<long> currentRHSNode;

                // Build Left Nodes
                currentLHSNode = lhsLeftDigits.First;
                currentRHSNode = rhsLeftDigits.First;
                for (int i = 0 ; i < mostLeftDigits; i++) {
                    newLeftDigits.AddLast(currentLHSNode.Value + currentRHSNode.Value);
                    currentLHSNode = currentLHSNode.Next;
                    currentRHSNode = currentRHSNode.Next;
                }

                // Build Right Nodes
                currentLHSNode = lhsRightDigits.First;
                currentRHSNode = rhsRightDigits.First;
                for (int i = 0; i < mostRightDigits; i++) {
                    long addedValue = currentLHSNode.Value + currentRHSNode.Value;
                  //  Console.WriteLine("Adding " + addedValue);
                    newRightDigits.AddLast(addedValue);
                    currentLHSNode = currentLHSNode.Next;
                    currentRHSNode = currentRHSNode.Next;
                }

                return new TruncatableRealNumber(newLeftDigits, newRightDigits, false);
			}
        }
		
		public TruncatableRealNumber getDifference(TruncatableRealNumber rhs) {
            if(rhs == this) {
                return 0;
            }
            else if (rhs == 0) {
				return new TruncatableRealNumber(this);
			}
            else if (this == 0) {
                return -rhs;
			}
			else if(isNegative()) {
                return -(getOpposite() + rhs);
			}
			else if(rhs.isNegative()) {
                return this + -rhs;
			}
			else if(rhs > this) {
                return -(rhs - this);
			}
			else {
				// Subtracting two positive numbers, where this > other, and neither are 0, nor equal
                LinkedList<long> newLeftDigits = new LinkedList<long>();
                LinkedList<long> newRightDigits = new LinkedList<long>();

                // Pad digits so that they match up
                int mostLeftDigits = rhs.digitsToTheLeft.Count > digitsToTheLeft.Count ? rhs.digitsToTheLeft.Count : digitsToTheLeft.Count;
                int mostRightDigits = rhs.digitsToTheRight.Count > digitsToTheRight.Count ? rhs.digitsToTheRight.Count : digitsToTheRight.Count;
                LinkedList<long> lhsLeftDigits = getPaddedDigitsToTheLeft(mostLeftDigits);
                LinkedList<long> lhsRightDigits = getPaddedDigitsToTheRight(mostRightDigits);
                LinkedList<long> rhsLeftDigits = rhs.getPaddedDigitsToTheLeft(mostLeftDigits);
                LinkedList<long> rhsRightDigits = rhs.getPaddedDigitsToTheRight(mostRightDigits);

                LinkedListNode<long> currentRHSNode = null;
                LinkedListNode<long> currentLHSNode = null;

                bool onRight;
                bool moreNodes;
                if (lhsRightDigits.Count > 0) {
                    currentRHSNode = rhsRightDigits.Last;
                    currentLHSNode = lhsRightDigits.Last;
                    onRight = true;
                    moreNodes = true;
                }
                else if (lhsLeftDigits.Count > 0) {
                    currentRHSNode = rhsLeftDigits.First;
                    currentLHSNode = lhsLeftDigits.First;
                    onRight = false;
                    moreNodes = true;
                }
                else {
                    moreNodes = false;
                    onRight = false;
                }

                bool borrowedLastIteration = false;
                while (moreNodes) {
                    long digitDifference = currentLHSNode.Value - currentRHSNode.Value;


                    if (borrowedLastIteration) {
                        digitDifference--;
                        borrowedLastIteration = false;
                    }

                    if (digitDifference < 0) {
                        digitDifference += TruncatableRealNumber.BASE;
                        borrowedLastIteration = true;
                    }

                    if (onRight) {
                        newRightDigits.AddFirst(digitDifference);

                        if(currentLHSNode.Previous != null) {
                            currentLHSNode = currentLHSNode.Previous;
                            currentRHSNode = currentRHSNode.Previous;
                        }
                        else if( lhsLeftDigits.Count > 0 ) {
                            currentLHSNode = lhsLeftDigits.First;
                            currentRHSNode = rhsLeftDigits.First;
                            onRight = false;
                        }
                        else {
                            moreNodes = false;
                        }
                    }
                    else {
                        newLeftDigits.AddLast(digitDifference);

                        if (currentLHSNode.Next != null) {
                            currentLHSNode = currentLHSNode.Next;
							currentRHSNode = currentRHSNode.Next;
                        }
                        else {
                            moreNodes = false;
                        }
                    }
                }

                return new TruncatableRealNumber(newLeftDigits, newRightDigits, false);
			}
		}

		public TruncatableRealNumber getOpposite()
		{
			TruncatableRealNumber copy = new TruncatableRealNumber(this);
			copy.negative = !copy.negative;
			copy.normalize();
			return copy;
		}

        public TruncatableRealNumber getAbs()
        {
			TruncatableRealNumber copy = new TruncatableRealNumber(this);
			copy.negative = false;
			return copy;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) {
                return false;
            }

            TruncatableRealNumber t = obj as TruncatableRealNumber;
            if ((System.Object)t == null) {
                return false;
            }

            return this == t;
        }

        public static bool operator < (TruncatableRealNumber one, TruncatableRealNumber two)
        {
            if (one.negative)
            {
                if (!two.negative)
                {
                    return true;
                }
                else
                {
                    // Both Negative

                    // Define in terms of inverse -- avoid duplicate conceptual code.
                    TruncatableRealNumber inverseOne = new TruncatableRealNumber(one);
                    inverseOne.negative = false;

                    TruncatableRealNumber inverseTwo = new TruncatableRealNumber(two);
                    inverseTwo.negative = false;

                    return inverseOne > inverseTwo;                    
                }
            }
            else
            {
                if (two.negative)
                {
                    return false;
                }
                else
                {
                    // Both Positive or 0
                    if (one == two)
                    {
                        return false;
                    }
                    else
                    {
                        if (one.digitsToTheLeft.Count > two.digitsToTheLeft.Count)
                        {
                            return false;
                        }
                        else if (one.digitsToTheLeft.Count < two.digitsToTheLeft.Count)
                        {
                            return true;
                        }
                        else
                        {

                                // Equal number of left digits...compare starting at left most
                                
                                /*
                                    // If either right count is 0, then we can return right here
                                    // Umm lol? what was i thinking?
                                    if (one.digitsToTheRight.Count < 1 && two.digitsToTheRight.Count > 0 )
                                    {
                                        return true;
                                    }
                                    else if (two.digitsToTheRight.Count < 1 && one.digitsToTheRight.Count > 0)
                                    {
                                        return false;
                                    }
                                */



                                LinkedListNode<long> currentOneNode = null;
                                LinkedListNode<long> currentTwoNode = null;
                                bool moreNodes = false;
                                bool onRight = false;

                                if (one.digitsToTheLeft.Last != null)
                                {
                                    onRight = false;
                                    moreNodes = true;
                                    currentOneNode = one.digitsToTheLeft.Last;
                                    currentTwoNode = two.digitsToTheLeft.Last;
                                }
                                else if (one.digitsToTheRight.First != null)
                                {
                                    onRight = true;
                                    moreNodes = true;
                                    currentOneNode = one.digitsToTheRight.First;
                                    currentTwoNode = two.digitsToTheRight.First;
                                }
                                else
                                {
                                    moreNodes = false;
                                    currentOneNode = null;
                                    currentTwoNode = null;
                                }

                                while (moreNodes)
                                {

                                //    Console.WriteLine("one = " + currentOneNode.Value + " two = " + currentTwoNode.Value);

                                    if (currentOneNode.Value < currentTwoNode.Value)
                                    {

                                        return true;
                                    }
                                    else if (currentOneNode.Value > currentTwoNode.Value)
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        if (!onRight)
                                        {
                                            if (currentOneNode.Previous != null)
                                            {
                                                // Since list size is equal, we can assume here..

                                                currentOneNode = currentOneNode.Previous;
                                                currentTwoNode = currentTwoNode.Previous;
                                            }
                                            else
                                            {
                                                currentOneNode = one.digitsToTheRight.First;
                                                currentTwoNode = two.digitsToTheRight.First;

                                                if (currentOneNode == null && currentTwoNode == null)
                                                {
                                                    // equal
                                                    return false;
                                                }
                                                else if (currentOneNode == null)
                                                {
                                                    return true;
                                                }
                                                else if (currentTwoNode == null)
                                                {
                                                    return false;
                                                }
                                                

                                                onRight = true;
                                            }
                                        }
                                        else
                                        {
                                            currentOneNode = currentOneNode.Next;
                                            currentTwoNode = currentTwoNode.Next;

                                            if (currentOneNode == null && currentTwoNode != null)
                                            {
                                                return true;
                                            }
                                            else if (currentOneNode != null && currentTwoNode == null)
                                            {
                                                return false;
                                            }
                                            else if (currentOneNode == null && currentTwoNode == null)
                                            {
                                                moreNodes = false;
                                            }
                                            else
                                            {

                                                // Continue
                                            }
                                        }
                                    }
                                }


                                // Totally equal digits

                                return false;
                        }

                    }
                }
            }
        }

        public static bool operator >=(TruncatableRealNumber one, TruncatableRealNumber two)
        {
            return (one > two) || (one == two);
        }

        public static bool operator <=(TruncatableRealNumber one, TruncatableRealNumber two)
        {
            return (one < two) || (one == two);
        }

        public static bool operator >(TruncatableRealNumber one, TruncatableRealNumber two)
        {
            return (!(one < two)) && (!(one == two));
        }

        public static bool operator == (TruncatableRealNumber one, TruncatableRealNumber two)
        {
            if (System.Object.ReferenceEquals(one, two)) {
                // Both null or identical instance
                return true;
            }
            else if (((object)one == null) || ((object)two == null)) {
                // One null
                return false;
            }
            else {
                if (one.negative != two.negative) {
                    return false;
                }
                else
                {
                    TruncatableRealNumber oneTen = one;
                    TruncatableRealNumber twoTen = two;

                    // Make sure digitsToTheLeft and digitsToTheRight are identical
                    if (oneTen.digitsToTheLeft.Count != twoTen.digitsToTheLeft.Count)
                    {
                        return false;
                    }
                    else if (oneTen.digitsToTheRight.Count != twoTen.digitsToTheRight.Count)
                    {
                        return false;
                    }
                    else
                    {
                        bool onRight = false;

                        bool moreNodes = false;
                        LinkedListNode<long> currentOneNode = null;
                        LinkedListNode<long> currentTwoNode = null;

                        if(oneTen.digitsToTheLeft.Count > 0)
                        {
                            moreNodes = true;
                            onRight = false;
                            currentOneNode = oneTen.digitsToTheLeft.First;
                            currentTwoNode = twoTen.digitsToTheLeft.First;
                        }
                        else if(oneTen.digitsToTheRight.Count > 0)
                        {
                            moreNodes = true;
                            onRight = true;
                            currentOneNode = oneTen.digitsToTheRight.First;
                            currentTwoNode = twoTen.digitsToTheRight.First;
                        }
                        else
                        {
                            moreNodes = false;
                            currentOneNode = null;
                            currentTwoNode = null;
                        }

                        while (moreNodes)
                        {
                            if (currentOneNode.Value != currentTwoNode.Value)
                            {
                                return false;
                            }
                            else
                            {
                                if (currentOneNode.Next == null)
                                {
                                    if (!onRight)
                                    {
                                        currentOneNode = oneTen.digitsToTheRight.First;
                                        currentTwoNode = twoTen.digitsToTheRight.First;
                                        onRight = true;

                                        if (currentOneNode == null)
                                        {
                                            moreNodes = false;
                                        }
                                    }
                                    else
                                    {
                                        moreNodes = false;
                                    }
                                }
                                else
                                {
                                    currentOneNode = currentOneNode.Next;
                                    currentTwoNode = currentTwoNode.Next;
                                }
                            }
                        }

                        return true;
                    }
                }

            }
        }

        public static bool operator != (TruncatableRealNumber one, TruncatableRealNumber two)
        {
            return !(one == two);
        }

        private void normalize()
        {
            // Move digits >= numberBase to the digit to the left

            bool workingWithRight;
            LinkedListNode<long> currentDigit;
            if(digitsToTheRight.Count > 0) {
                currentDigit = digitsToTheRight.Last;
                workingWithRight = true;
            }
            else {
                currentDigit = digitsToTheLeft.First;
                workingWithRight = false;
            }

            long numberToAdd = 0;
            while (currentDigit != null) {
                currentDigit.Value += numberToAdd;
                numberToAdd = 0;

                while (currentDigit.Value >= TruncatableRealNumber.BASE)
                {
                    numberToAdd++;
                    currentDigit.Value -= TruncatableRealNumber.BASE;
                }

                if (workingWithRight) {
                    if (currentDigit.Previous != null) {
                        currentDigit = currentDigit.Previous;
                    }
                    else {
                        currentDigit = digitsToTheLeft.First;
                        workingWithRight = false;

                        if (currentDigit == null && numberToAdd > 0) {
                            digitsToTheLeft.AddLast(0);
                            currentDigit = digitsToTheLeft.Last;
                        }
                    }
                }
                else {
                    if (currentDigit.Next == null && numberToAdd > 0) {
                        while ( numberToAdd != 0 ) {
                            digitsToTheLeft.AddLast(numberToAdd);

                            if(numberToAdd >= TruncatableRealNumber.BASE) {
                                numberToAdd -= TruncatableRealNumber.BASE;
                            }
                            else {
                                numberToAdd = 0;
                            }
                        }
                    }
                    currentDigit = currentDigit.Next;
                }
            }

            // Remove 0s from both sides of the number

            while (digitsToTheRight.Last != null && digitsToTheRight.Last.Value == 0) {
                digitsToTheRight.RemoveLast();
            }

            while (digitsToTheLeft.Last != null && digitsToTheLeft.Last.Value == 0) {
                digitsToTheLeft.RemoveLast();
            }
           
            if (digitsToTheLeft.Count < 1 && digitsToTheRight.Count < 1) {

                // After removing 0s, if there are no digits left,
                // then we want to make sure we are not negative.

                negative = false;
            }
        }

        public void printDigits() {
            Console.WriteLine("BGN Printing Digits for " + GetHashCode());

            if (negative) {
                Console.WriteLine("Negative.");
            }

            if(truncated) {
                Console.WriteLine("Truncated.");
            }

            Console.WriteLine("BGN Left");

            foreach (long digit in digitsToTheLeft) {
                Console.WriteLine(digit);
            }

            Console.WriteLine("END Left");
            Console.WriteLine("BGN Right");

            foreach (long digit in digitsToTheRight) {
                Console.WriteLine(digit);
            }

            Console.WriteLine("END Right");
            Console.WriteLine("END Printing Digits for " + GetHashCode());
        }

        public override string ToString() {
            string stringRepresentation = "";

            if( digitsToTheLeft.Count > 0 ) {
                foreach (long digit in digitsToTheLeft) {
                    stringRepresentation = digit + stringRepresentation;
                }
            }
            else {
                stringRepresentation += "0";
            }

            if (digitsToTheRight.Count > 0) {
                stringRepresentation += ".";
                foreach (long digit in digitsToTheRight) {
                    stringRepresentation += digit;
                }
            }

            if (negative) {
                stringRepresentation = "-" + stringRepresentation;
            }

            if (truncated) {
                stringRepresentation = "~" + stringRepresentation;
            }

            return stringRepresentation;
        }

        /**
         * Helper Function
         * 
         * Returns digits to the left
         * padded to specified length
         * by adding zeroes to the end of the list.
         * 
         * If given length is less than current
         * length, a copy of digitsToTheLeft is
         * returned.
         */
        private LinkedList<long> getPaddedDigitsToTheLeft(int length)
        {
            LinkedList<long> digitsCopy = new LinkedList<long>(digitsToTheLeft);

            while (digitsCopy.Count < length) {
                digitsCopy.AddLast(0);
            }

            return digitsCopy;
        }

        /**
         * Helper Function
         * 
         * Returns digits to the right
         * padded to specified length
         * by adding zeroes to the end of the list.
         * 
         * If given length is less than current
         * length, a copy of digitsToTheRight is
         * returned.
         */
        private LinkedList<long> getPaddedDigitsToTheRight(int length) {
            LinkedList<long> digitsCopy = new LinkedList<long>(digitsToTheRight);

            while (digitsCopy.Count < length) {
                digitsCopy.AddLast(0);
            }

            return digitsCopy;
        }


        private LinkedList<long> digitsToTheLeft; // Digits to the left of the radix point
        private LinkedList<long> digitsToTheRight; // Digits to the right of the radix point
        private bool negative;
        private bool truncated;
        public const int MAX_DIVISION_PRECISION = 10;
        public const ushort BASE = 10;
    }
}