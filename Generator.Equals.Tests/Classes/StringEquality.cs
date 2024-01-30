using FluentAssertions;

namespace Generator.Equals.Tests.Classes;

public partial class StringEquality
{
    public class EqualsTests : EqualityTestCase
    {
        public override object Factory1() =>
            new Sample
            {
                StringOrdinal = "",
                StringOrdinalIgnoreCase = ""
            };

        public override object Factory2() =>
            new Sample
            {
                StringOrdinal = "",
                StringOrdinalIgnoreCase = ""
            };

        public override bool EqualsOperator(object value1, object value2) => (Sample)value1 == (Sample)value2;
        public override bool NotEqualsOperator(object value1, object value2) => (Sample)value1 != (Sample)value2;
    }

    public class NotEqualsTest : EqualityTestCase
    {
        public override bool Expected => false;

        public override object Factory1() =>
            new Sample
            {
                StringOrdinal = "",
                StringOrdinalIgnoreCase = ""
            };

        public override object Factory2() =>
            new Sample
            {
                StringOrdinal = "",
                StringOrdinalIgnoreCase = ""
            };

        public override bool EqualsOperator(object value1, object value2) => (Sample)value1 == (Sample)value2;
        public override bool NotEqualsOperator(object value1, object value2) => (Sample)value1 != (Sample)value2;

        public override void TestHashCode()
        {
            var value1 = Factory1();
            var value2 = Factory2();
            var result = value1.GetHashCode() == value2.GetHashCode();
            result.Should().BeTrue();
        }
    }
}