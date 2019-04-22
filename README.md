# NUnit.FixtureDependent
Extends NUnit to permit generic test data residing in a test fixture to be passed to its test methods, allowing **generic parametrized test cases** to be generated **from that test fixture data**. 

This decouples test methods from test data, allows combining strategies to be used on data that was fed from ``TestFixtureSource`` (which is normally impossible) and still lets the test cases display properly in the test runner GUI.

# How it works
**NUnit.FixtureDependent** defines an attribute called ``ValueSourceDependent`` that can access the ``Arguments`` property of a Test Fixture, retrieve a variable, generally a data class, and then access a member of that data class to populate test cases. 

The member has to be an array. The data class is initially fed to the Test Fixture via its constructor and the ``TestFixtureSource`` attribute.

# How do I use it?
Check out ``src/NUnit.FixtureDependent.Sample``. The code, summary tags and comments will help you understand much better than reading the readme.

The "short" of it is that you need to:
1. Define a test data class with generic arrays as members.
2. Define a static class to hold instances of the test data class. These instances will act as sources.
3. Define a generic test fixture with a constructor that takes an instance of the data test class as its argument.
4. Define a ``TestFixtureParameters`` factory that lets you specify the type arguments of the test fixture and pass a test data class instance as an argument.
5. Define a static class we'll refer to as provider. Have it call the factory's construct method for each source in part and return an array of ``TestFixtureParameters`` through a property we'll arbitrarily call GetArgs.
6. Annotate the generic test fixture with the ``TestFixtureSource`` attribute using the provider's GetArgs property as the argument to the attribute's constructor.
6. Annotate the generic parametrized test methods in the fixture with a dependent combinatorial strategy attribute: ``CombinatorialDependent``, ``SequentialDependent``, or ``PairwiseDependent``.
7. Annotate the parameters of the generic parametrized test methods with the ``ValueSourceDependent`` attribute, using the type of the test data class as the first argument, and the name of the member you want to access in the test data class as the second argument. 

This will result in each individual element in the specified member's array serving as the data source of that parameter as if you had used ``ValueSource``.

For example if you had a test data class called ``TestData<T>`` that had a ``T[] dataParam`` property and you wanted that property to be used as the source of a parameter you would write it like this:

```csharp
[Test, SequentialDependent]
public void TestMethod([ValueDependentSource(typeof(TestData<>), nameof(TestData<T>.dataParam))] T param)
{
    //...
}
```

# Why does this exist?

NUnit has four ways of allowing generic tests:
1. **TestCase.** Specifying the data inline for each method via ``TestCase``.
2. **TestCaseSource**. Specifying the data separately and making it available through a static method, property or field that returns ``IEnumerable``, then reference it in a ``TestCaseSource`` attribute.
3. **TestFixture**. Creating a generic test fixture and using  the ``TestFixture`` attribute to specify data inline.
4. **TestFixtureSource**. Creating a generic test fixture and using the ``TestFixtureSource`` attribute, specifying a source that returns a ``TestFixtureData`` instance with arguments compatible with the constructor of the generic test fixture.

### Downsides with existing approaches

TestCase approach:
- **Data duplication**. Inline data can lead to duplicating a lot of data over many methods.
- **Too many attributes**. Number of methods multiplied by number of data sets.

TestCaseSource approach:
- **No combining strategies**. ``TestCaseSource`` requires that the source provide _complete rows_ of parameters, which makes it impossible to use combining strategies on the parameters.
- **Too many attributes**. Number of methods multiplied by number of data sets.

TestFixture approach:
- **Data duplication**. Data exists only for that specific test fixture. Somewhat mitigated since it's less likely data will be duplicated across multiple test fixtures.
- **No combining strategies**. Cannot be used because the data is available to the test methods through accessing variables in the parent test fixture instead of through parameters.
- **No individual test cases**. Because the data isn't fed to the test methods through parameters individual test cases cannot be generated.

TestFixtureCase approach:
- **No individual test cases**. For the same reasons as before.
- **No combining strategies**. For the same reasons as before.

### Why are these downsides

- **Data duplication**. Isn't a problem with small data sets, but it can become unworkable with big data sets.
- **Too many attributes**. Can get really annoying given a sufficiently high number of sources.
- **No combining strategies**. Can artificially inflate the amount of setup or test data required.
- **No individual test cases**. Can't immediately see the specific value that made a test method fail.

**NUnit.FixtureDependent** allows you to mostly eliminate the aforementioned downsides. Data is not duplicated, the number of attributes is equal to the number of parameters, individual test cases are visible, and combining strategies can be used.

# Should I use this?
This kind of pattern can definitely be used in terrible ways and it has a decent amount of boilerplate. It will significantly inflate a codebase that doesn't need this kind of abstraction.

But sometimes, you just need to be able to test an implementation using multiple data sets of varying types and at that point using any of the four approaches explained results in some at least some significant downsides. If you want fine grained data to be easily visible in failed tests then you really want to have individual test cases be generated for each data input. If you have common data most/all test methods in a fixture use but the parameter list varies a little or you want to combine variables, the four approaches can be really tedious, but **NUnit.FixtureDependent** can do well.

If you don't have a use case like that I would recommend you **NOT** use **NUnit.FixtureDependent** and use the ``TestFixtureSource`` approach for generic test fixtures.

If you have a use case that sounds _somewhat_ like that, I would recommend you still **NOT** use **NUnit.FixtureDependent** and try to see if you can redesign your tests first so that you don't need it.
