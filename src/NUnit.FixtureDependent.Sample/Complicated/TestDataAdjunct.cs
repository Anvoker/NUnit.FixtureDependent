using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework;

namespace NUnit.FixtureDependent.Sample.Complicated
{
    /// <summary>
    /// <para>
    /// We use this static class to call upon <see cref="TestDataFixtureConstructor"/>
    /// to construct our individual <see cref="ITestFixtureData"/> which are needed
    /// for instantiating our <see cref="GenericTestFixture{T, K, L}"/> because
    /// its constructor parameters are insufficient for NUnit to infer the
    /// generic type arguments of the fixture.
    /// </para>
    ///
    /// <para>
    /// <see cref="GetArgs"/> can then successfully construct our fixture when
    /// given as the argument to a <see cref="TestFixtureSourceAttribute"/>
    /// </para>
    /// </summary>
    public static class TestDataAdjunct
    {
        public static ITestFixtureData[] GetArgs => new ITestFixtureData[]
        {
            TestDataFixtureConstructor
                .Construct<int, string, Dictionary<int, string>>(
                    TestDataSource.DataCaseIntString),

            TestDataFixtureConstructor
                .Construct<float, bool, ConcurrentDictionary<float, bool>>(
                    TestDataSource.DataCaseFloatBool),
        };
    }
}