using NUnit.Framework.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

namespace NUnit.FixtureDependent.Sample.DirectSimple
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
                yield return new ExposedTestFixtureParams(
                    new int[] { 25, 100, -90, },
                    new string[] { "a", "b", "c" })
                    .SetTypeArgs(typeof(int), typeof(string));


                yield return new ExposedTestFixtureParams(
                    new float[] { 33.0f, float.PositiveInfinity, 0.01f, },
                    new bool[] { true, false, true })
                    .SetTypeArgs(typeof(float), typeof(bool));
            }
        }
    }
}