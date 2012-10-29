namespace LinearSystems
{
    class Vector<T> : Matrix<T> where T : Semiring<T>, new()
	{
		public Vector(T[] data, int direction)
		{
            if (direction == VECTOR_TYPE_ROW_VECTOR)
            {
                T[,] newData = new T[1, data.Length];

                int counter = 0;
                foreach (T element in data)
                {
                    newData[0, counter] = element;
                    counter++;
                }

                base.data = newData;
                base.numberOfRows = 1;
                base.numberOfColumns = data.Length;
            }
            else
            {
                T[,] newData = new T[data.Length, 1];

                int counter = 0;
                foreach (T element in data)
                {
                    newData[counter, 0] = element;
                    counter++;
                }

                base.data = newData;
                base.numberOfRows = data.Length;
                base.numberOfColumns = 1;
            }

			this.direction = direction;
		}

        public Vector(T[] data)
        {
            T[,] newData = new T[1, data.Length];

            int counter = 0;
            foreach (T element in data)
            {
                newData[0, counter] = element;
                counter++;
            }

            base.data = newData;
            base.numberOfRows = 1;
            base.numberOfColumns = data.Length;
            this.direction = VECTOR_TYPE_ROW_VECTOR;
        }

        public Vector(T[,] data)
        {
            int numberOfRows = data.GetLength(0);
            int numberOfColumns = data.GetLength(1);

            if(numberOfRows > 1 && numberOfColumns > 1)
            {
                throw new UndefinedException();
            }
            else if(numberOfRows == 1 || numberOfColumns == 1)
            {
                base.data = data;
                base.numberOfRows = numberOfRows;
                base.numberOfColumns = numberOfColumns;
            }
            else if(numberOfColumns == 0 && numberOfRows == 0)
            {
                // Do nothing -- empty constructor was called
            }
            else
            {
                throw new UndefinedException();
            }
        }

		public int getDirection() {
			return direction;
		}

		private int direction;
		public const ushort VECTOR_TYPE_COLUMN_VECTOR = 1;
		public const ushort VECTOR_TYPE_ROW_VECTOR = 2;
	}
}
