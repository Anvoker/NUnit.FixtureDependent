using NUnit.Framework.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

namespace NUnit.FixtureDependent.Sample.Simple
{
    /// <summary>
    /// Define all our data in a statically available location.
    /// </summary>
    public static class TestDataSource
    {
        /// <summary>
        /// Because <see cref="TestFixtureSourceAttribute"/> has very poor
        /// ability to infer type arguments, we use
        /// <see cref="ExposedTestFixtureParams.SetTypeArgs(System.Type[])"/>
        /// to manually specify them and allow NUnit to build our fixture.
        /// </summary>
        public static IEnumerable GetArgs
        {
            get
            {
                yield return new ExposedTestFixtureParams(new TestData<int, string>
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
                }).SetTypeArgs(typeof(int), typeof(string));


                yield return new ExposedTestFixtureParams(new TestData<float, bool>
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
                }).SetTypeArgs(typeof(float), typeof(bool));
            }
        }
    }
}