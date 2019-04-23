namespace NUnit.FixtureDependent.Sample.Complicated
{
    /// <summary>
    /// Define all our data in a statically available location.
    /// </summary>
    public static class TestDataSource
    {
        public static TestData<int, string> DataCaseIntString = new TestData<int, string>
        {
            keys = new int[]
            {
                25,
                100,
                -90,
            },

            values = new string[]
            {
                "aaaa",
                "b",
                "c"
            },
        };

        public static TestData<float, bool> DataCaseFloatBool = new TestData<float, bool>
        {
            keys = new float[]
            {
                32.0f,
                float.NaN,
                -900.0f,
            },

            values = new bool[]
            {
                false,
                false,
                true,
            },
        };
    }
}