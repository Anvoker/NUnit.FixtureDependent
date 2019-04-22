using System;
using NUnit.Framework.Interfaces;

namespace NUnit.FixtureDependent
{
    /// <summary>
    /// Has the sole purpose of easily allowing us to create an instance of
    /// <see cref="ITestFixtureData"/> and set its properties.
    /// </summary>
    public class ExposedTestFixtureParams : ITestFixtureData
    {
        public Type[] TypeArgs { get; set; }

        public string TestName { get; set; }

        public RunState RunState { get; set; }

        public object[] Arguments { get; set; }

        public IPropertyBag Properties { get; set; }
    }
}
