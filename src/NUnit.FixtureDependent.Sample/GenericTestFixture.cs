using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.FixtureDependent.Sample
{
    /// <summary>
    /// A generic test fixture that receives its parameters and type arguments
    /// from <see cref="TestDataProvider"/> by having <see cref="TestDataFixtureConstructor"/>
    /// instantiate a <see cref="TestFixtureParameters"/> instance that allows
    /// us to successfully populate the test fixture with generic data.
    /// </summary>
    [TestFixtureSource(typeof(TestDataProvider),
        nameof(TestDataProvider.GetArgs))]
    public class GenericTestFixture<T, K>
    {
        /// <summary>
        /// <para>
        /// A private field to save the data fed by the constructor.
        /// </para>
        ///
        /// <para>
        /// This field by itself would be insufficient for creating parametrized
        /// generic tests as any data not specified in the argument list of the
        /// method will not generate an individual test case and will not appear
        /// in any test runner GUI, hence the need for NUnit.FixtureDependent
        /// to begin with. This field is optional because
        /// <see cref="ValueDependentSourceAttribute"/> gets its data from the
        /// <see cref="Test.Arguments"/> of the Test Fixture itself. That data
        /// gets saved into the test object when the fixture is first constructed.
        /// </para>
        /// </summary>
        private readonly TestData<T, K> testData;

        /// <summary>
        /// Mandatory constructor. The argument list of the constructor must be
        /// compatible in type order and number to the data provided by the
        /// source. If it is incompatible the test will be invisible and NUnit
        /// will issue no error.
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
        /// the test fixture. In this case <see cref="ValueDependentSourceAttribute"/>
        /// tries to find a variable of the type <see cref="TestData{T, K}"/>
        /// or a type compatible to it. After it finds the variable, it attempts
        /// to access the <see cref="TestData{T, K}.tParams"/> and
        /// <see cref="TestData{T, K}.kCollectionParams"/> members to get the
        /// data.
        /// </para>
        ///
        /// <para>
        /// When using this style of method, it is obligatory to specify a
        /// combination strategy attribute from the <see cref="NUnit.FixtureDependent"/>
        /// family. Such as <see cref="SequentialDependentAttribute"/>.
        /// Specifying no combination strategy will make NUnit default to
        /// <see cref="SequentialAttribute"/> which is incapable of processing
        /// parameters annotated with <see cref="ValueDependentSourceAttribute"/>.
        /// </para>
        /// </summary>
        [Test, SequentialDependent]
        public void TestMethod(
            [ValueDependentSource(typeof(TestData<,>),
                nameof(TestData<T, K>.tParams))]
            T a,
            [ValueDependentSource(typeof(TestData<,>),
                nameof(TestData<T, K>.kCollectionParams))]
            ICollection<K> b)
        {
            Assert.Pass(
                a.ToString() + " | " +
                b.ToString() + " | " +
                testData.otherParam);
        }
    }
}
