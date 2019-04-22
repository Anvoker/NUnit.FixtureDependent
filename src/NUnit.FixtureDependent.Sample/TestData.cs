using System.Collections.Generic;

namespace NUnit.FixtureDependent.Sample
{
    /// <summary>
    /// Package our data into a data class for easy passing.
    /// </summary>
    public class TestData<T, K>
    {
        // Variables we want to serve as parameters for test methods must be
        // arrays whose element type matches that of the parameter.
        public T[] tParams;
        public ICollection<K>[] kCollectionParams;

        // Other variables can be passed and accessed in the Test Fixture, but
        // they won't work when given to ValueSourceDependent
        public string otherParam;
    }
}