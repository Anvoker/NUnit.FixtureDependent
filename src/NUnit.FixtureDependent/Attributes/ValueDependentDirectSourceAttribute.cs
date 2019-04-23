// ***********************************************************************
// Copyright (c) 2008-2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Reflection;
using NUnit.FixtureDependent.Interfaces;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using static NUnit.FixtureDependent.Internal.ReflectionHelper;

namespace NUnit.FixtureDependent
{
    /// <summary>
    /// <para>
    /// Indicates a source that will provide data for one parameter of a test
    /// method. The source is retrieved from the arguments used to construct the
    /// test fixture.
    /// </para>
    ///
    /// <para>
    /// Accesses the value source directly.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class FixtureDirectValueSourceAttribute : Attribute, IParameterDependentDataSource
    {
        #region Constructors

        /// <summary>
        /// Construct with no arguments. The type of the parameter this is used
        /// on will be used to locate the desired source in the fixture's
        /// arguments.
        /// </summary>
        public FixtureDirectValueSourceAttribute() { }

        #endregion

        #region IParameterDependentDataSource Members

        /// <summary>
        /// Gets an enumeration of data items for use as arguments
        /// for a test method parameter.
        /// </summary>
        /// <param name="parameter">The parameter for which data is needed</param>
        /// <returns>
        /// An enumeration containing individual data items
        /// </returns>
        public IEnumerable GetData(IParameterInfo parameter, Test suite)
        {
            return GetDataSource(parameter, suite);
        }

        #endregion

        #region Helper Methods

        private IEnumerable GetDataSource(IParameterInfo parameter, Test suite)
        {
            var fixtureDataObject = LocateArgumentByType(suite.Arguments, parameter.ParameterType);

            if (fixtureDataObject == null)
            {
                throw new InvalidDataSourceException(
                    $"An argument of type {parameter.ParameterType} could not" +
                    $"be found in the argument list of the fixture's constructor.");
            }

            return fixtureDataObject;
        }

        private static IEnumerable LocateArgumentByType(object[] arguments, Type type)
        {
            foreach (var argument in arguments)
            {
                var argumentType = argument.GetType();
                var elementType = GetCollectionElementType(argumentType);
                if (elementType != null
                    && type.IsAssignableFrom(elementType)
                    && argument is IEnumerable)
                {
                    return (IEnumerable)argument;
                }
            }

            return null;
        }

        #endregion
    }
}