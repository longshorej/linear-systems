namespace LinearSystems
{
	interface Semiring<T> : SetIdentity<T>
	{
		T getProduct(T rhs);
		T getSum(T rhs);
	}
}