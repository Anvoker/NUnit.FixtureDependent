﻿// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.FixtureDependent.Internal.Builders
{
    /// <summary>
    /// Creates test cases by using all of the parameter data sources in
    /// parallel.
    /// </summary>
    public class SequentialDependentStrategy : ICombiningStrategy
    {
        /// <summary>
        /// Gets or sets an option that defines whether the strategy should
        /// stop as soon as the first source ends.
        /// </summary>
        public bool StopAtShortestSource { get; set; }

        public SequentialDependentStrategy(bool stopAtShortestSource)
        {
            StopAtShortestSource = stopAtShortestSource;
        }

        /// <summary>
        /// Gets the test cases generated by the CombiningStrategy.
        /// </summary>
        /// <returns>The test cases.</returns>
        public IEnumerable<ITestCaseData> GetTestCases(IEnumerable[] sources)
        {
            var testCases = new List<ITestCaseData>();

            var enumerators = new IEnumerator[sources.Length];

            for (int i = 0; i < sources.Length; i++)
            {
                enumerators[i] = sources[i].GetEnumerator();
            }

            bool reachedEndOfOneEnumerator  = false;

            while(true)
            {
                object[] testdata = new object[sources.Length];
                bool reachedEndOfAllEnumerators = true;

                for (int i = 0; i < sources.Length; i++)
                {
                    if (enumerators[i].MoveNext())
                    {
                        testdata[i] = enumerators[i].Current;
                        reachedEndOfAllEnumerators = false;
                    }
                    else
                    {
                        if (StopAtShortestSource)
                        {
                            reachedEndOfOneEnumerator = true;
                            break;
                        }
                        else
                        {
                            testdata[i] = null;
                        }
                    }
                }

                if (StopAtShortestSource && !reachedEndOfOneEnumerator)
                {
                    var parms = new TestCaseParameters(testdata);
                    testCases.Add(parms);
                }
                else
                {
                    if (reachedEndOfAllEnumerators)
                    {
                        break;
                    }

                    var parms = new TestCaseParameters(testdata);
                    testCases.Add(parms);
                }
            }

            return testCases;
        }
    }
}