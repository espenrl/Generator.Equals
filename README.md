[![Nuget](https://img.shields.io/nuget/v/Generator.Equals)](https://www.nuget.org/packages/Generator.Equals/)
# Generator.Equals
A source code generator for automatically implementing IEquatable&lt;T&gt; using only attributes.

----------------
## Requirements

In order to use this library, you must:
* Use a target framework that supports .NET Standard >= 2.0 (eg. .NET Core 3.1 or .NET 5.0);
* Set your project's C# ```LangVersion``` property to 8.0 or higher.

## Usage

The below sample shows how to use Generator.Equals to override the default equality implementation for a C# record, enhancing it with the ability to determine the equality between the array contents of the record.

```c#
using Generator.Equals;

[Equatable]
partial record MyRecord(
    [property: OrderedEquality] string[] Fruits
);

class Program
{
    static void Main(string[] args)
    {
        var record1 = new MyRecord(new[] {"banana", "apple"});
        var record2 = new MyRecord(new[] {"banana", "apple"});

        Console.WriteLine(record1 == record2);
    }
}
```
Not using records? Generator.Equals also support classes.

```c#
using Generator.Equals;

[Equatable]
partial class MyClass
{
    [OrderedEquality] 
    public string[] Fruits { get; set; }
}
```

## Supported Comparers

Below is a list of all supported comparers. Would you like something else added? Let me know by raising an issue or sending a PR!

### Default

This is the comparer that's used when a property has no attributes indicating otherwise. The generated code will use 
```EqualityComparer<T>.Default``` for both equals and hashing operation.


### IgnoreEquality

```c#
[IgnoreEquality] 
public string Name { get; set; }
```

As the name implies, the property is ignored during Equals and GetHashCode calls!


### OrderedEquality

```c#
[OrderedEquality] 
public string[] Fruits { get; set; } // Fruits have to be in the same order for the array to be considered equal.
```

This equality comparer will compare properties as a sequence instead of a reference. This works just like ```Enumerable.SequenceEqual```, which assumes both lists are of the same size and same sort.

Bear in mind that the property has to implement IEnumerable<T> and the that the items themselves implement equality (you can use Generator.Equals in the items too!).

### UnorderedEquality

```c#
[UnorderedEquality] 
public string[] Fruits { get; set; } // Does not care about the order of the fruits!

[UnorderedEquality] 
public IDictionary<string, object> Properties { get; set; } // Works with dictionaries too!
```

This equality comparer will compare properties as an unordered sequence instead of a reference. This works just like ```Enumerable.SequenceEqual```, but it does not care about the order as long as the all values (including the repetitions) are present.

As with OrderedEquality, bear in mind that the property (or key and values if using a dictionary) has to implement IEnumerable<T> and the that the items themselves implement equality (you can use Generator.Equals in the items too!).

### SetEquality

```c#
[SetEquality] 
public HashSet<string> Fruits { get; set; } // Fruits can be in any order and it can be repeated
```

This equality comparer will do a set comparison, using ```SetEquals``` whenever the underlying collection implements `ISet<T>`, otherwise falling back to  manually comparing both collections, which can be expensive for large collections.

Hashing always returns 0 for this type of equality,
### ReferenceEquality

```c#
[ReferenceEquality] 
public string Name { get; set; } // Will only return true if strings are the same reference (eg. when used with string.Intern)
```

This will ignore whatever equality is implemented for a particular object and compare references instead.

### CustomEquality

```c#
class LengthEqualityComparer : IEqualityComparer<string>
{
    public static readonly LengthEqualityComparer Default = new();

    public bool Equals(string? x, string? y) => x?.Length == y?.Length;

    public int GetHashCode(string obj) => obj.Length.GetHashCode();
}

class NameEqualityComparer 
{
    public static readonly IEqualityComparer<string> Default = new SomeCustomComparer();
}


[CustomEquality(typeof(LengthEqualityComparer))] 
public string Name1 { get; set; } // Will use LengthEqualityComparer to compare the values of Name1.

[CustomEquality(typeof(NameEqualityComparer))] 
public string Name2 { get; set; } // Will use NameEqualityComparer.Default to compare values of Name2.

[CustomEquality(typeof(StringComparer), nameof(StringComparer.OrdinalIgnoreCase))] 
public string Name2 { get; set; } // Will use StringComparer.OrdinalIgnoreCase to compare values of Name2.
```

This attribute allows you to specify a custom comparer for a particular property. For it to work, the type passed as an
argument to CustomEqualityAttribute should fulfill AT LEAST one of the following:

* Have a static field/property named Default returning a valid IEqualityComparer<T> instance for the target type;
* Have a static field/property with the same name passed to the CustomComparerAttribute returning a valid IEqualityComparer<T> instance for the target type;
* Implement IEqualityComparer<T> and expose a parameterless constructor.

## Explicit Mode
The generator allows you to explicitly specify which properties are used to generate the `IEquatable`.  

To do this, set the `Explicit` property of `EquatableAttribute` to `true` and specify the required properties using `DefaultEqualityAttribute` or other attributes.
```cs
using Generator.Equals;

[Equatable(Explicit = true)]
partial class MyClass
{
    // Only this property will be used for equality!
    [DefaultEquality] 
    public string Name { get; set; } = "Konstantin"; 
    
    public string Description { get; set; } = "";
}
```