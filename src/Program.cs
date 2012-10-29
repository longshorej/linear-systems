using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearSystems
{
    class Program
    {
        static void Main(string[] args)
        {
        //    Program.TestTRN();
           Program.testInverse();
        //    Program.testMultiplication();
        }

        static void testMultiplication()
        {
            TruncatableRealNumber[,] md1 = {
                                                   {0, -1, 2},

                                                   {4,11,2}
                                               };
            TruncatableRealNumber[,] md2 = {
                                                   {3,-1},
                                                   {1,2},
                                                   {6,1}
                                               };

            Matrix<TruncatableRealNumber> m1 = new Matrix<TruncatableRealNumber>(md1);
            Matrix<TruncatableRealNumber> m2 = new Matrix<TruncatableRealNumber>(md2);


            Console.WriteLine(m1);
            Console.WriteLine(" * ");
            Console.WriteLine(m2);
            Console.WriteLine(" = ");
            Console.WriteLine((m1 * m2));


        }

        static void testInverse()
        {
            Console.WriteLine("Calculating...");




            Rational[,] dataRational = {
                                     { .04  , 200 , 22},
                                     { -4.42  , .0006 , 1254248.241 },
                                     { -.7542 , 232264  , .514  },
                                 };
            Matrix<Rational> mRational = new Matrix<Rational>(dataRational);
            Matrix<Rational> mRationalInverse = mRational.getInverse();

            Matrix<Rational> mRationalIdentity = mRational * mRationalInverse;


            Console.WriteLine("Calculation 1 Done...");







            RationalNativeReal[,] dataRatRl = {
                                     { .04  , 200 , 22},
                                     { -4.42  , .0006 , 1254248.241 },
                                     { -.7542 , 232264  , .514  },
                                 };
            Matrix<RationalNativeReal> mRatRl = new Matrix<RationalNativeReal>(dataRatRl);
            Matrix<RationalNativeReal> mRatRlInverse = mRatRl.getInverse();
            Matrix<RationalNativeReal> mRatRlIdentity = mRatRl * mRatRlInverse;


            Console.WriteLine("Calculation 2 Done...");





            TruncatableRealNumber[,] dataTruncReal = {
                                     { .04  , 200 , 22},
                                     { -4.42  , .0006 , 1254248.241 },
                                     { -.7542 , 232264  , .514  },
                                 };
            Matrix<TruncatableRealNumber> mTruncReal = new Matrix<TruncatableRealNumber>(dataTruncReal);
            Matrix<TruncatableRealNumber> mTruncRealInverse = mTruncReal.getInverse();
            Matrix<TruncatableRealNumber> mTruncRealIdentity = mTruncReal * mTruncRealInverse;



            Console.WriteLine("Calculation 3 Done...");




            NativeReal[,] dataReal ={
                                     { .04  , 200 , 22},
                                     { -4.42  , .0006 , 1254248.241 },
                                     { -.7542 , 232264  , .514  },
                                 };
            Matrix<NativeReal> mReal = new Matrix<NativeReal>(dataReal);
            Matrix<NativeReal> mRealInverse = mReal.getInverse();
            Matrix<NativeReal> mRealIdentity = mReal * mRealInverse;

            Console.WriteLine("Calculation 4 Done...");



            Console.WriteLine("Rational(Of LinkedList)");
            Console.WriteLine("\nMatrix  : \n" + mRational);
            Console.WriteLine("\nInverse : \n" + mRationalInverse);
            Console.WriteLine("\nIdentity: \n" + mRationalIdentity);


            Console.WriteLine("Rational(Of NativeType)");
            Console.WriteLine("\nMatrix  : \n" + mRatRl);
            Console.WriteLine("\nInverse : \n" + mRatRlInverse);
            Console.WriteLine("\nIdentity: \n" + mRatRlIdentity);


            Console.WriteLine("LinkedList");
            Console.WriteLine("\nMatrix  : \n" + mTruncReal);
            Console.WriteLine("\nInverse : \n" + mTruncRealInverse);
            Console.WriteLine("\nIdentity: \n" + mTruncRealIdentity);


            Console.WriteLine("NativeType");
            Console.WriteLine("\nMatrix  : \n" + mReal);
            Console.WriteLine("\nInverse : \n" + mRealInverse);
            Console.WriteLine("\nIdentity: \n" + mRealIdentity);

        }

        static void TestTRN()
        {
            TruncatableRealNumber n1 = .5;
            TruncatableRealNumber n2 = .75;

            Console.WriteLine(n1 + n2);

            int timesToTest = 0;

            Random r = new Random();


            for (int i = 0; i < timesToTest; i++)
            {



                double a = r.NextDouble() * 10;
                double b = r.NextDouble() * 10;

                TruncatableRealNumber r1 = a;
                TruncatableRealNumber r2 = b;

                Console.WriteLine(" -----TruncatableRealNumber----- ");

                Console.WriteLine(" r1 = " + r1);
                Console.WriteLine(" r2 = " + r2);

                Console.WriteLine(" r1 + r2 = " + (r1 + r2));
                Console.WriteLine(" r2 + r1 = " + (r2 + r1));
                Console.WriteLine(" r1 - r2 = " + (r1 - r2));
                Console.WriteLine(" r2 - r1= " + (r2 - r1));
                Console.WriteLine(" r1 * r2 = " + (r1 * r2));
                Console.WriteLine(" r2 * r1 = " + (r2 * r1));
                Console.WriteLine(" r1 / r2 = " + (r1 / r2));
                Console.WriteLine(" r2 / r1 = " + (r2 / r1));

                Console.WriteLine(" -----  Built          In  ----- ");

                Console.WriteLine(" r1 = " + a);
                Console.WriteLine(" r2 = " + b);

                Console.WriteLine(" r1 + r2 = " + (a + b));
                Console.WriteLine(" r2 + r1 = " + (b + a));
                Console.WriteLine(" r1 - r2 = " + (a - b));
                Console.WriteLine(" r2 - r1= " + (b - a));
                Console.WriteLine(" r1 * r2 = " + (a * b));
                Console.WriteLine(" r2 * r1 = " + (b * a));
                Console.WriteLine(" r1 / r2 = " + (a / b));
                Console.WriteLine(" r2 / r1 = " + (b / a));

            }
        }


    }
}
