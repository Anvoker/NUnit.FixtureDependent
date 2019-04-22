using NUnit.Framework.Interfaces;
using NUnit.Framework;

namespace NUnit.FixtureDependent.Sample
{
    /// <summary>
    /// <para>
    /// We use this static class to call upon <see cref="TestDataFixtureConstructor"/>
    /// to construct our individual <see cref="ITestFixtureData"/> which are needed
    /// for instantiating our <see cref="GenericTestFixture{T, K}"/>.
    /// </para>
    ///
    /// <para>
    /// <see cref="GetArgs"/> can then successfully construct our fixture when
    /// given as the argument to a <see cref="TestFixtureSourceAttribute"/>
    /// </para>
    /// </summary>
    public static class TestDataProvider
    {
        public static ITestFixtureData[] GetArgs => new ITestFixtureData[]
        {
            TestDataFixtureConstructor.Construct(TestDataSource.DataCaseIntString),
            TestDataFixtureConstructor.Construct(TestDataSource.DataCaseFloatBool),
        };
    }
}