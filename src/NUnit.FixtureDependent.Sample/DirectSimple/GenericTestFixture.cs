using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.FixtureDependent.Sample.DirectSimple
{
    /// <summary>
    /// A generic test fixture that receives its parameters and type arguments
    /// from <see cref="TestDataSource"/> via <see cref="TestFixtureSourceAttribute"/>.
    /// </summary>
    [TestFixtureSource(typeof(TestDataSource), nameof(TestDataSource.GetArgs))]
    public class GenericTestFixture<T, K>
    {
        /// <summary>
        /// Mandatory constructor. The argument list of the constructor must be
        /// compatible in type, order, and number to the data provided by the
        /// source.
        /// </summary>
        public GenericTestFixture(T[] t, K[] k) { }

        /// <summary>
        /// <para>
        /// A generic parametrized test method that gets its parameter data from
        /// the test fixture itself.
        /// </para>
        ///
        /// <para>
        /// The parameters get their data by accessing the arguments passed to
        /// the test fixture. In this case <see cref="FixtureDirectValueSourceAttribute"/>
        /// tries to find a variable of type <typeparamref name="T"/> and
        /// <typeparamref name="K"/> respectively. After it finds the variable,
        /// it attempts retrieves the values and builds the test cases with them.
        /// </para>
        ///
        /// <para>
        /// It is obligatory to specify a combination strategy attribute from
        /// the <see cref="NUnit.FixtureDependent"/> family.
        /// </para>
        /// </summary>
        [Test, SequentialDependent]
        public void TestMethod(
            [FixtureDirectValueSource()] T a,
            [FixtureDirectValueSource()] K b)
        {
            Assert.Pass($"{a} | {b}");
        }
    }
}
