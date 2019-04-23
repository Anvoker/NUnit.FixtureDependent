using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.FixtureDependent.Sample.Simple
{
    /// <summary>
    /// A generic test fixture that receives its parameters and type arguments
    /// from <see cref="TestDataSource"/> via <see cref="TestFixtureSourceAttribute"/>.
    /// </summary>
    [TestFixtureSource(typeof(TestDataSource), nameof(TestDataSource.GetArgs))]
    public class GenericTestFixture<T, K>
    {
        /// <summary>
        /// A private field to save the data fed by the constructor. This is
        /// only needed to hold data we don't access with
        /// <see cref="FixtureValueSourceAttribute"/>.
        /// </summary>
        private readonly TestData<T, K> testData;

        /// <summary>
        /// Mandatory constructor. The argument list of the constructor must be
        /// compatible in type, order, and number to the data provided by the
        /// source.
        /// </summary>
        public GenericTestFixture(TestData<T, K> testData)
        {
            this.testData = testData;
        }

        /// <summary>
        /// <para>
        /// A generic parametrized test method that gets its parameter data from
        /// the test fixture itself.
        /// </para>
        ///
        /// <para>
        /// The parameters get their data by accessing the arguments passed to
        /// the test fixture. In this case <see cref="FixtureValueSourceAttribute"/>
        /// tries to find an argument that's an instance of <see cref="TestData{T, K}"/>.
        /// After it finds the variable, it attempts retrieves the values
        /// by accessing <see cref="TestData{T, K}.tParams"/> and
        /// <see cref="TestData{T, K}.kCollectionParams"/> respectively and
        /// build the test cases.
        /// </para>
        ///
        /// <para>
        /// It is obligatory to specify a combination strategy attribute from
        /// the <see cref="NUnit.FixtureDependent"/> family.
        /// </para>
        /// </summary>
        [Test, SequentialDependent]
        public void TestMethod(
            [FixtureValueSource(typeof(TestData<,>),
                nameof(TestData<T, K>.tParams))]
            T a,
            [FixtureValueSource(typeof(TestData<,>),
                nameof(TestData<T, K>.kCollectionParams))]
            ICollection<K> b)
        {
            Assert.Pass(
                a.ToString() + " | " +
                b.ToString() + " | ");
        }
    }
}
