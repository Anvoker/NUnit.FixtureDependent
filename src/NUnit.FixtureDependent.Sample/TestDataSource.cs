using System.Collections.Generic;

namespace NUnit.FixtureDependent.Sample
{
    /// <summary>
    /// Define all our data in a statically available location.
    /// </summary>
    public static class TestDataSource
    {
        public static TestData<int, string> DataCaseIntString = new TestData<int, string>
        {
            tParams = new int[]
            {
                25,
                100,
                -90,
            },

            kCollectionParams = new ICollection<string>[]
            {
                new List<string> { "a", "b", "c" },
                new List<string> { "abc" },
                new List<string> { "ccc" },
            },

            otherParam = "nyaa",
        };

        public static TestData<float, bool> DataCaseFloatBool = new TestData<float, bool>
        {
            tParams = new float[]
            {
                33.0f,
                0.0f,
                float.NaN,
            },

            kCollectionParams = new ICollection<bool>[]
            {
                new List<bool> { true, true, true },
                new List<bool> { false },
                new List<bool> { true },
            },

            otherParam = "meow",
        };
    }
}