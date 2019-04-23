# NUnit.FixtureDependent
Extends NUnit to permit generic test data residing in a test fixture to be passed to its test methods, allowing **generic parametrized test cases** to be generated **from that test fixture data**. 

This decouples test methods from test data, allows combining strategies to be used on data that was fed from ``TestFixtureSource`` (which is normally impossible) and still lets the test cases display properly in the test runner GUI.

# How it works
The ``Arguments`` property of a test fixture is populated with data by NUnit when a fixture is given its constructor arguments via an attribute like ``TestFixtureSource``.

Normally, no other attributes access that property. But they **could**.

**NUnit.FixtureDependent** defines two attributes called ``FixtureValueSource`` and ``FixtureDirectValueSource`` that exploit this.

These attributes can access the ``Arguments`` property, retrieve values and use them to parametrize generic test methods and generate test cases.

## FixtureDirectValueSource

Used to annotate **parameters**. Takes no arguments.

Attempts to find an array in the fixture's ``Arguments`` property whose type is assignable to the **parameter's type**. If found, it is used as the value source for that parameter.

```csharp
[TestFixtureSource(typeof(TestDataSource), nameof(TestDataSource.GetArgs))]
public class GenericTestFixture<T, K>
{
    public GenericTestFixture(T[] t, K[] k) { }

    [Test, SequentialDependent]
    public void TestMethod(
        [FixtureDirectValueSource()] T a,
        [FixtureDirectValueSource()] K b)
    {
        Assert.Pass($"{a} | {b}");
    }
}
```

Results in the following test cases being generated:

```
GenericTestFixture<Int32,String>(System.Int32[],System.String[]) (3)
    TestMethod (3)
        TestMethod(-90,"c")
        TestMethod(100,"b")
        TestMethod(25,"a")
GenericTestFixture<Single,Boolean>(System.Single[],System.Boolean[]) (3)
    TestMethod (3)
        TestMethod(0.01f,True)
        TestMethod(33.0f,True)
        TestMethod(float.Positivelnfinity,False)
```

## FixtureValueSource

Used to annotate **parameters**. Takes a **type** and a **name**.

The **type** is expected to be an arbitrary **data class** that can be found in the fixture's ``Arguments`` property. 

The **name** is expected to be the name of a **member** that exists in that **data class**. The **member** needs to be an **array** whose **element type** is assignable to the **parameter's type**.

Attempts to find an object of the specified **type** in the fixture's ``Arguments`` property and then access one if its members with the specified **name**. If found and its return matches the **parameter's type**, it is used as value source for that parameter.

```csharp
public class TestData<T, K>
{
    public T[] tParams;
    public ICollection<K>[] kCollectionParams;
}

[TestFixtureSource(typeof(TestDataSource), nameof(TestDataSource.GetArgs))]
public class GenericTestFixture<T, K>
{
    public GenericTestFixture(TestData<T, K> testData) { }

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
```

Results in the following test cases being generated:

```
GenericTestFixture<Int32,String> (NUnit.FixtureDependent.Sample.Simple.TestData 2[System.Int32,System.String)) (3)
    TestMethod (3)
        TestMethod(-90,System.Collections.Generic.List 1[System.String])
        TestMethod(100,System.Collections.Generic.List"1[System.String])
        TestMethod(25,System.Collections.Generic.List’ 1[System.String])
GenericTestFixture<Single,Boolean> (NUnit.FixtureDependent.Sample.Simple.TestData 2[System.Single,System.Boolean]) (3)
    TestMethod (3)
        TestMethod(0.0f,System.Collections.Generic.List’ 1[System.Boolean])
        TestMethod(33.0f,System.Collections.Generic. List’ 1[System.Boolean])
        TestMethod(float.NaN,System.Collections.Generic.List’ 1[System.Boolean])
```

# How do I use it?
**Download the [nuget package](https://www.nuget.org/packages/NUnit.FixtureDependent/).**

**Check out ``src/NUnit.FixtureDependent.Sample``. The code, summary tags and comments will help you understand much better how to use it.**

- ``src/NUnit.FixtureDependent.Sample/DirectSimple`` showcases how to use ``FixtureDirectValueSource`` and fluent type argument setting in the source.
- ``src/NUnit.FixtureDependent.Sample/Simple`` showcases how to use ``FixtureValueSource`` and fluent type argument setting in the source.
- ``src/NUnit.FixtureDependent.Sample/Complicated`` showcases how to use ``FixtureValueSource`` with a more explicit boilerplate heavy setup. The explicit steps can help fine tune the data setup process in some cases.

**Steps for using ``FixtureDirectValueSource``:**
1. Define a static class to hold test data. Use ``ExposedTestFixtureParams`` and its ``SetTypeArgs`` method to manually specify type arguments because NUnit can't infer them.
2. Define a generic test fixture with a constructor that takes arguments matching the type and order of those defined in the source.
3. Annotate generic test methods you want to parametrize with ``FixtureDirectValueSource``
4. Annotate tests that use ``FixtureDirectValueSource`` with any NUnit.FixtureDependent combining strategy attribute such as ``SequentialDependent``.

**Steps for using ``FixtureValueSource``:**
1. Define a test data class with generic arrays as members.
2. Define a static class to hold test data. Use ``ExposedTestFixtureParams`` and its ``SetTypeArgs`` method to manually specify type arguments because NUnit can't infer them.
3. Define a generic test fixture with a constructor that takes arguments matching the type and order of those defined in the source.
4. Annotate generic test methods you want to parametrize with ``FixtureValueSource`` specifying the test data class's type as the first argument and the name of the member you want to access as the second argument.
5. Annotate tests that use ``FixtureValueSource`` with any NUnit.FixtureDependent combining strategy attribute such as ``SequentialDependent``.

Note: Forgetting to set the combining strategy will result in a vanilla NUnit combining strategy being used by default which *cannot* deal with NUnit.FixtureDependent's source attributes.

# Why does this exist?

NUnit has four ways of allowing generic tests:
1. **TestCase.** Specifying the data inline for each method via ``TestCase``.
2. **TestCaseSource**. Specifying the data separately and making it available through a static method, property or field that returns ``IEnumerable``, then reference it in a ``TestCaseSource`` attribute.
3. **TestFixture**. Creating a generic test fixture and using  the ``TestFixture`` attribute to specify data inline.
4. **TestFixtureSource**. Creating a generic test fixture and using the ``TestFixtureSource`` attribute, specifying a source that returns a ``TestFixtureData`` instance with arguments compatible with the constructor of the generic test fixture.

### Downsides with existing approaches

| Approach               | Data shareable across methods | Data sheareable across fixtures | Can use combining strategies | Generates individual test cases | Attribute Number   |
| ---------------------- | ----------------------------- | ------------------------------- | ---------------------------- | ------------------------------- | ------------------ |
| TestCase               | NO                            | NO                              | NO                           | **YES**                         | =NCases x NMethods |
| TestCaseSource         | **YES**                       | **YES**                         | NO                           | **YES**                         | =NCases x NMethods |
| TestFixture            | **YES**                       | NO                              | NO                           | NO                              | **=NCases**        |
| TestFixtureCase        | **YES**                       | **YES**                         | NO                           | NO                              | **=NCases**        |
| NUnit.FixtureDependent | **YES**                       | **YES**                         | **YES**                      | **YES**                         | **=NParameters**   |

### Why do these things matter?

- **Data shareable across methods**. The more methods you have using the same data the more duplication happens and the harder it is to maintain if you can't share data across methods.
- **Data shareable across fixtures**. The more fixtures you have using the same data the more duplication happens and the harder it is to maintain if you can't share data across fixtures.
- **No combining strategies**. Without being able to mix and match individual parameters from a test set, we are forced to either give up testing something more thoroughly, write the combinations manually or write multiple sources for each case that do the combining before the data is fed to the attribute. All of this takes time to write and ends up being more code to maintain.
- **No individual test cases**. If you have a sequence of parameters you want to test and you can't immediately see the specific value that made a test method fail, this makes the unit tests less usable. ``TestFixture`` and ``TestFixtureCase`` are both designed to provide single fixture-wide variables that are treated as "globals" from the perspective of the test methods. They are not designed to take sequences of data and build test cases, but NUnit.FixtureDependent is.
- **Attribute Number**. It can get somewhat more difficult to maintain code that has a lot of attributes, and definitely more unpleasant to write.

# Should I use this?
This kind of pattern can definitely be used in bad ways. The root of the problem is that attribute based test discovery and execution obscures the relationship between the tests and data. 

The type system cannot assist the developer in any way as Reflection is used to obtain the data. There are several ways to mess up setting up a ``FixtureValueSource`` and its relationship with its data is not immediately apparent, but this is sort of the case with all Source attributes.

Another consideration is whether fixture parameters should even be permissible as test method parameters. NUnit *implictly* takes the stance that fixture parameters are atomic units of data that should not iterated over as if they were separable into smaller units. You can do it *anyway* with ``Assert.Multiple`` but that makes tests less usable without seeing individual test cases. ``Assert.Multiple`` should be used to check multiple claims about the same data, not the same claim about multiple data.

Sometimes, you just need to be able to test implementations using multiple data sets of varying types. When methods end up using largely the same data but with some variation in the parameter signature and/or the need for combining strategies, you don't have any remotely perfect solution on hand. But NUnit.FixtureDependent can offer a solution with different, arguably better tradeoffs.

- If you don't have a use case like that I would recommend you **NOT** use NUnit.FixtureDependent and use the ``TestFixtureSource`` approach for generic test fixtures.
- If you have a use case that sounds *somewhat* like that, I would recommend you try to see if you can redesign your tests first so that you don't need NUnit.FixtureDependent.
- If you have a use case that sounds much like that, try out NUnit.FixtureDependent.
