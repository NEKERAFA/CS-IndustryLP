namespace IndustryLP.Entities
{
    public struct Tuple<T>
    {
        public T First { get; private set; }
        public T Second { get; private set; }

        public Tuple(T first, T second)
        {
            First = first;
            Second = second;
        }
    }
}
