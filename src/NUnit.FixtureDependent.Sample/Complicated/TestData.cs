namespace NUnit.FixtureDependent.Sample.Complicated
{
    /// <summary>
    /// Package our data into a data class for easy passing.
    /// </summary>
    public class TestData<T, K>
    {
        public T[] keys;
        public K[] values;
    }
}