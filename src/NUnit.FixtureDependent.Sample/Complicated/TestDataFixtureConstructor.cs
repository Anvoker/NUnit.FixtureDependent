using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.FixtureDependent.Sample.Complicated
{
    /// <summary>
    /// Since TestFixtureSource cannot deal generic type arguments that
    /// can't be inferred from parameters given to the constructor, we use this
    /// class to construct the TestFixtureParameters ourselves, and explicitly
    /// tell NUnit what the type arguments are.
    /// </summary>
    public static class TestDataFixtureConstructor
    {
        public static TestFixtureParameters Construct<T, K, L>(TestData<T, K> data)
        {
            var exposedParams = new ExposedTestFixtureParams()
            {
                Arguments = new object[] { data },
                Properties = new PropertyBag(),
                RunState = RunState.Runnable,
                TypeArgs = new Type[]
                {
                    typeof(T),
                    typeof(K),
                    typeof(L),
                }
            };

            return new TestFixtureParameters(exposedParams);
        }
    }
}
