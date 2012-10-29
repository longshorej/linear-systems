using System;

namespace LinearSystems
{
	class Matrix<T> : SetIdentity<Matrix<T>>, Semiring<Matrix<T>> where T : Semiring<T>, new()
	{
		public static Matrix<T> operator +(Matrix<T> m1, Matrix<T> m2)
		{
			return m1.getSum(m2);
		}

        public static Matrix<T> operator *(Matrix<T> m1, Matrix<T> m2)
        {
            return m1.getProduct(m2);
        }

        public Matrix<T> getEmptyElement()
        {
            return new Matrix<T>();
        }

        public Matrix<T> getIdentityElement()
        {
            if (numberOfRows != numberOfColumns)
            {
                throw new UndefinedException();
            }
            else if (numberOfRows < 1)
            {
                return new Matrix<T>();
            }
            else
            {
                T identityElement = this.getElement(1, 1).getIdentityElement();
                T emptyElement = this.getElement(1, 1).getEmptyElement();

                T[,] matrixData = new T[numberOfRows, numberOfColumns];

                for (int i = 0; i < numberOfRows; i++)
                {
                    for (int j = 0; j < numberOfColumns; j++)
                    {
                        if (i == j)
                        {
                            matrixData[i, j] = identityElement;
                        }
                        else
                        {
                            matrixData[i, j] = emptyElement;
                        }
                    }
                }

                Matrix<T> newMatrix = new Matrix<T>(matrixData);


                return newMatrix;
            }
        }


        public Matrix(T[,] mdata)
		{
            data = mdata;
            numberOfRows = data.GetLength(0);
            numberOfColumns = data.GetLength(1);
		}

		public Matrix()
		{
			this.numberOfRows = 0;
			this.numberOfColumns = 0;
			this.data = new T[0,0];
		}

		public T getElement(int rowNumber, int columnNumber)
		{
			return this.data[rowNumber - 1,columnNumber - 1];
		}

        /**
         * Mutator methods that sets an element of the matrix.
         * For use from within Matrix / inheriting classes
         */
        protected void setElement(int rowNumber, int columnNumber, T e)
        {
            this.data[rowNumber - 1, columnNumber - 1] = e;
        }
        
        /**
         * Returns the number of rows in the matrix
         */
		public int getNumberOfRows()
		{
			return this.numberOfRows;
		}

        /**
         * Returns the number of columns in the matrix.
         */
		public int getNumberOfColumns()
		{
			return this.numberOfColumns;
		}

        /**
         * Returns a Matrix with given rows swapped.
         */
		public Matrix<T> getSwappedRows(int rowOne, int rowTwo)
		{
            T[,] swappedMatrixData = new T[numberOfRows, numberOfColumns];

            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfColumns; j++)
                {
                    if(i == rowOne)
                    {
                        swappedMatrixData[rowTwo - 1, j - 1] = data[i - 1, j - 1];
                    }
                    else if(i == rowTwo)
                    {
                        swappedMatrixData[rowOne - 1, j - 1] = data[i - 1, j - 1];
                    }
                    else
                    {
                        swappedMatrixData[i - 1, j - 1] = data[i - 1, j - 1];
                    }
                }
            }

            Matrix<T> swappedMatrix = new Matrix<T>(swappedMatrixData);
            return swappedMatrix;
		}

        /**
         * Returns a Matrix with given columns swapped.
         */
        public Matrix<T> getSwappedColumns(int columnOne, int columnTwo)
        {
            T[,] swappedMatrixData = new T[numberOfRows, numberOfColumns];

            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfColumns; j++)
                {
                    if (j == columnOne)
                    {
                        swappedMatrixData[i - 1, columnTwo - 1] = data[i - 1, j - 1];
                    }
                    else if (j == columnTwo)
                    {
                        swappedMatrixData[i - 1, columnOne - 1] = data[i - 1, j - 1];
                    }
                    else
                    {
                        swappedMatrixData[i - 1, j - 1] = data[i - 1, j - 1];
                    }
                }
            }

            Matrix<T> swappedMatrix = new Matrix<T>(swappedMatrixData);
            return swappedMatrix;
        }

        /**
         * Determines whether the elements of this
         * matrix are rings or not. If not, they
         * must be treated as semirings.
         * 
         * @return True if the elements are rings, false if not.
         */
        public Boolean elementsAreRings()
        {
            return (typeof(T).GetInterface("Ring`1") != null);
        }

        /**
         * Returns the conincal form of the matrix (Smith Normal Form)
         * Only defined if the matrix's elements are rings
         */
        public Matrix<T> getConincalForm()
        {
            if (!elementsAreRings())
            {
                throw new UndefinedException("Conincal Form is only defined for matrices of rings.");
            }

            // Cast data to rings so we can work with division
            Ring<T>[,] ringData = new Ring<T>[numberOfRows, numberOfColumns];
            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    ringData[i, j] = (Ring<T>) data[i, j];
                }
            }

            int currentRowNumber = 0; // Holds the row we are currently working with
            int currentColumnNumber = 0; // Holds the column we are currently working with

            while (currentRowNumber < numberOfRows && currentColumnNumber < numberOfColumns)
            {


                // Divide the entire top row by itself to make it equal to identity element
                // aka multiply it by 1/itself (elementary operation of scalar multiplication)

                Ring<T> currentPosition = ringData[currentRowNumber, currentColumnNumber];

                // If the element is not empty, scale the row by a constant to make it equal to 1
                // TODO: figure out how to generalize this -- what operation can be applied to make it
                // the identity element
                if (!currentPosition.Equals((Ring<T>)currentPosition.getEmptyElement()))
                {
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        ringData[currentRowNumber, i] = (Ring<T>)ringData[currentRowNumber, i].getQuotient((T)currentPosition);
                    }
                }

                // Scale the other rows
                for (int i = currentRowNumber + 1; i < numberOfRows; i++)
                {
                    Ring<T> rowScaler = (Ring<T>)ringData[i, currentColumnNumber];
                    for (int j = 0; j < numberOfColumns; j++)
                    {
                        Ring<T> valueToSubtract = (Ring<T>)ringData[currentRowNumber, j].getProduct( (T) rowScaler);


                   //     Console.Write((Ring<T>)ringData[i, j] + " - " + valueToSubtract);



                        ringData[i, j] = (Ring<T>)ringData[i, j].getDifference((T)valueToSubtract);

                     //   Console.WriteLine(" = " + ringData[i, j]);
                    }
                }








                currentRowNumber++;
                currentColumnNumber++;
            }

            // We are now in row-echelon form...move to reduced..

            

            currentRowNumber--;
            currentColumnNumber--;

            while (currentRowNumber > 0 && currentColumnNumber > 0)
            {
                for (int i = 0; i < currentRowNumber; i++)
                {
                    // scale by the value at this row, colhold col
                    Ring<T> rowScaler = (Ring<T>)ringData[i, currentColumnNumber];

                //    Console.WriteLine("rowScaler = " + rowScaler);

                    for (int j = 0; j < numberOfColumns; j++)
                    {

                    //    Console.WriteLine("i = " + i + " j = " + j);



                        Ring<T> valueToSubtract = (Ring<T>)ringData[currentRowNumber, j].getProduct((T)rowScaler);

                       // Console.WriteLine("value to sub = " + valueToSubtract + " and ring = " + ringData[i,j]);
                        ringData[i, j] = (Ring<T>)ringData[i, j].getDifference((T)valueToSubtract);


                    }
                }

                currentRowNumber--;
                currentColumnNumber--;
            }



            // Save data into new matrix
            T[,] semiringData = new T[numberOfRows, numberOfColumns];

            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    semiringData[i, j] = (T) ringData[i, j];
                }
            }

            Matrix<T> canMatrix = new Matrix<T>(semiringData);
            return canMatrix;
        }

        public Matrix<T> getInverse()
        {
            if (numberOfColumns != numberOfRows)
            {
                throw new UndefinedException();
            }

            Matrix<T> augmented = getAugmented(this.getIdentityElement());
            Matrix<T> solved = augmented.getConincalForm();

            // look at the right half

            T[,] matrixData = new T[numberOfRows, numberOfColumns];

            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    matrixData[i, j] = solved.data[i, j + numberOfColumns];
                }
            }

            return new Matrix<T>(matrixData);
        }

        public Matrix<T> getAugmented(Matrix<T> aug)
        {
            if (numberOfRows != aug.numberOfRows)
            {
                throw new UndefinedException();
            }

            T[,] newData = new T[numberOfRows, numberOfColumns + aug.numberOfColumns];

            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    newData[i, j] = data[i, j];
                }
            }


            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < aug.numberOfColumns; j++)
                {
                    newData[i, j + numberOfRows] = aug.data[i, j];
                }

            }

            return new Matrix<T>(newData);


        }

        /**
         * Returns a Matrix which is the product of this matrix
         * and the supplied Matrix.
         * 
         * Basic O(N^3) implementation
         */
        public Matrix<T> getProduct(Matrix<T> matrixRHS)
		{
            if (this.numberOfColumns != matrixRHS.numberOfRows) throw new UndefinedException();


            T[,] emptyData = new T[this.numberOfRows, matrixRHS.numberOfColumns];

            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < matrixRHS.numberOfColumns; j++)
                {
                    emptyData[i, j] = new T();
                }
            }

            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < matrixRHS.numberOfColumns; j++)
                {
                    for (int k = 0; k < numberOfColumns; k++)
                    {
                      //  Console.WriteLine("Multiplying " + this.data[i, k] + " by " + matrixRHS.data[k, j]);

                        T valueToAdd = this.data[i, k].getProduct(matrixRHS.data[k, j]);


                        emptyData[i, j] = emptyData[i, j].getSum(valueToAdd);
                    }
                }
            }


            return new Matrix<T>(emptyData);
		}

        /**
         * Returns a Matrix which is the sum of this matrix
         * and the supplied Matrix
         */
		public Matrix<T> getSum(Matrix<T> matrixRHS)
		{
            if (this.numberOfRows != matrixRHS.numberOfRows || this.numberOfColumns != matrixRHS.numberOfColumns) throw new UndefinedException();

            T[,] emptyData = new T[this.numberOfRows, matrixRHS.numberOfColumns];
            Matrix<T> summedRHS = new Matrix<T>(emptyData);

			for (int i = 1; i <= this.numberOfRows; i++)
			{
				for (int j = 1; j <= this.numberOfColumns; j++)
				{
					summedRHS.setElement(i, j, this.getElement(i, j).getSum(matrixRHS.getElement(i, j)));
				}
			}

			return summedRHS;
		}

        /**
         * Returns this matrix in string representation.
         */
		public override string ToString()
		{
            // notes: this code fails with big matrices. it was written to get something working really fast
            // todo: rewrite

			string stringRepresentation = "";

			// first, lets figure out the largest element size
			int largestElementSize = 0;
			for (int i = 1; i <= numberOfRows; i++)
			{
				for (int j = 1; j <= numberOfColumns; j++)
				{
					T thisElement = this.getElement(i, j);
					string elementStringRepresentation = thisElement.ToString();
					string[] linesInElement = elementStringRepresentation.Split('\n');
					foreach (string aLine in linesInElement)
					{
						if (largestElementSize < aLine.Length)
						{
							largestElementSize = aLine.Length;
						}
					}
				}
			}
			//System.Console.WriteLine("largestElementSize = " + largestElementSize);
			//System.Console.WriteLine("The Element = " + largestElement);

			for (int i = 1; i <= numberOfRows; i++)
			{
				Boolean[] printedCommas = new Boolean[numberOfColumns];

				if (i > 1)
				{
					stringRepresentation += "\n[";

					string[] stringRepresentationLinesSoFar = stringRepresentation.Split('\n');
					int stringRepresentationLineLength = stringRepresentationLinesSoFar[0].Length - 2;

					for (int asfjf = 0; asfjf < stringRepresentationLineLength; asfjf++)
					{
						stringRepresentation += " ";
					}

					stringRepresentation += "]\n";
				}

				stringRepresentation += "[ ";

				int maxNumberOfLines = 0;

				for (int j = 1; j <= numberOfColumns; j++)
				{
					string elementStringRepresentation = this.getElement(i, j).ToString();

					if (elementStringRepresentation.Split('\n').Length > maxNumberOfLines)
					{
						maxNumberOfLines = elementStringRepresentation.Split('\n').Length;
					}
				}

				for (int k = 0; k < maxNumberOfLines; k++)
				{
					Boolean printedNewLine = false;

					for (int j = 1; j <= numberOfColumns; j++)
					{
						string[] elementLines = this.getElement(i, j).ToString().Split('\n');

						if (elementLines.Length > k && elementLines[k].Trim() != "")
						{
							if (!printedNewLine && k > 0)
							{
								stringRepresentation += " ]\n[ ";
								printedNewLine = true;
							}

							if (j > 1)
							{
								if (!printedCommas[j - 1])
								{
									stringRepresentation += " , ";
									printedCommas[j - 1] = true;
								}
								else
								{
									stringRepresentation += "   ";
								}
							}

							int elementSizeDifferent = largestElementSize - elementLines[k].Length;
							double halfTheSizeDifference = elementSizeDifferent / 2;

							int numberOfSpacesBefore = (int)Math.Floor(halfTheSizeDifference);
							int numberOfSpacesAfter = elementSizeDifferent - numberOfSpacesBefore;

							for (int a = 0; a < numberOfSpacesBefore; a++)
							{
								stringRepresentation += " ";
							}

							stringRepresentation += elementLines[k];


							for (int a = 0; a < numberOfSpacesAfter; a++)
							{
								stringRepresentation += " ";
							}
						}
					}
				}

				stringRepresentation += " ]";
			}

			if (stringRepresentation.Trim() == "")
			{
				return "--\n[]\n--";
			}
			else
			{
				string longestLine = "";

				foreach(string line in stringRepresentation.Split('\n'))
				{
					if (line.Length > longestLine.Length)
					{
						longestLine = line;
					}
				}

				string fix = "";
				for (int i = 0; i < longestLine.Length; i++)
				{
					fix += "-";
				}

				return fix + "\n" + stringRepresentation + "\n" + fix;
			}
		}

        protected int numberOfRows;
        protected int numberOfColumns;
		protected T[,] data;
	}

}
