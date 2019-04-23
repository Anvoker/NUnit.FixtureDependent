using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.FixtureDependent
{
    /// <summary>
    /// Has the sole purpose of easily allowing us to create an instance of
    /// <see cref="ITestFixtureData"/> where we can manually set the type
    /// arguments.
    /// </summary>
    public class ExposedTestFixtureParams : ITestFixtureData
    {
        public Type[] TypeArgs { get; set; }

        public string TestName { get; set; }

        public RunState RunState { get; set; }

        public object[] Arguments { get; set; }

        public IPropertyBag Properties { get; set; }

        public ExposedTestFixtureParams(object data)
        {
            Arguments = new object[] { data };
            Properties = new PropertyBag();
            RunState = RunState.Runnable;
        }

        public ExposedTestFixtureParams(params object[] data)
        {
            Arguments = data;
            Properties = new PropertyBag();
            RunState = RunState.Runnable;
        }

        public ExposedTestFixtureParams SetTypeArgs(params Type[] types)
        {
            TypeArgs = types;
            return this;
        }
    }
}
