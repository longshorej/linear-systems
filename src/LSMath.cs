namespace LinearSystems {
	class LSMath {
		public static TruncatableRealNumber lcm(TruncatableRealNumber one, TruncatableRealNumber two) {
			return one.getAbs().getProduct(two.getAbs()).getQuotient( LSMath.gcd(one, two) );
		}

		/**
		 * Returns the greatest common divider of
		 * two TRNs, ignoring signs (Unsigned GCD).
		 * Uses euclid's algorithm
		 *
		 * @return GCD
		 */
		public static TruncatableRealNumber gcd(TruncatableRealNumber one, TruncatableRealNumber two) {
			TruncatableRealNumber zero = new TruncatableRealNumber(0);
			TruncatableRealNumber u = new TruncatableRealNumber(one).getAbs();
			TruncatableRealNumber v = new TruncatableRealNumber(two).getAbs();
			TruncatableRealNumber temp = null;
			
	//		System.Console.WriteLine("GCD( " + one + " , " + two + " ) ");
            do
            {
                if (u < v)
                {
                    temp = u;
                    u = v;
                    v = temp;
                }

                u = u.getDifference(v);

            }
            while (u != zero);

			return v;

		}

        public static NativeReal gcd(NativeReal one, NativeReal two)
        {
            NativeReal zero = 0;            

            NativeReal u = new NativeReal(one).getAbs();
            NativeReal v = new NativeReal(two).getAbs();




            NativeReal temp = null;

            //		System.Console.WriteLine("GCD( " + one + " , " + two + " ) ");
            do
            {
                if (u < v)
                {
                    temp = u;
                    u = v;
                    v = temp;
                }

                u = u.getDifference(v);

                //System.Console.WriteLine("one = " + one + " two = " + two + " u = " + u);
            }
            while (u != zero);

            return v;
        }
	}
}
