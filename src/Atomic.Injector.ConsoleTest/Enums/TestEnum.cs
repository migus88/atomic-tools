using System.Collections.Generic;
using System.Collections.ObjectModel;
using Atomic.Toolbox.Tools.Core.Attributes;
using Atomic.Toolbox.Tools.Core.Enums;

namespace Atomic.Injector.ConsoleTest.Enums
{
    [ExtendedEnum(Mode = EnumExtensionMode.All)]
    public enum TestEnum
    {
        One,
        Two,
        Three,
        Four
    }

    public class TestEnumEx1
    {
        private readonly Dictionary<string, int> _valuesByName = new Dictionary<string, int>
        {
            [Names.One] = Values.One,
            [Names.Two] = Values.Two,
            [Names.Three] = Values.Three,
            [Names.Four] = Values.Four,
        };

        public readonly ReadOnlyDictionary<string, int> ValuesByName;
        
        
        private readonly Dictionary<int, string> _namesByValue = new Dictionary<int, string>
        {
            [Values.One] = Names.One,
            [Values.Two] = Names.Two,
            [Values.Three] = Names.Three,
            [Values.Four] = Names.Four,
        };

        public readonly ReadOnlyDictionary<int, string> NamesByValue;
        
        
        private readonly Dictionary<TestEnum, string> _namesByEnum = new Dictionary<TestEnum, string>
        {
            [TestEnum.One] = Names.One,
            [TestEnum.Two] = Names.Two,
            [TestEnum.Three] = Names.Three,
            [TestEnum.Four] = Names.Four,
        };

        public readonly ReadOnlyDictionary<TestEnum, string> NamesByEnum;
        
        
        private readonly Dictionary<string, TestEnum> _enumsByName = new Dictionary<string, TestEnum>
        {
            [Names.One] = TestEnum.One,
            [Names.Two] = TestEnum.Two,
            [Names.Three] = TestEnum.Three,
            [Names.Four] = TestEnum.Four,
        };

        public readonly ReadOnlyDictionary<string, TestEnum> EnumsByName;
        
        
        private readonly Dictionary<int, TestEnum> _enumsByValue = new Dictionary<int, TestEnum>
        {
            [Values.One] = TestEnum.One,
            [Values.Two] = TestEnum.Two,
            [Values.Three] = TestEnum.Three,
            [Values.Four] = TestEnum.Four,
        };

        public readonly ReadOnlyDictionary<int, TestEnum> EnumsByValue;
        
        
        private readonly Dictionary<TestEnum, int> _valuesByEnum = new Dictionary<TestEnum, int>
        {
            [TestEnum.One] = Values.One,
            [TestEnum.Two] = Values.Two,
            [TestEnum.Three] = Values.Three,
            [TestEnum.Four] = Values.Four,
        };

        public readonly ReadOnlyDictionary<TestEnum, int> ValuesByEnum;

        public TestEnumEx1()
        {
            ValuesByName = new ReadOnlyDictionary<string, int>(_valuesByName);
            NamesByValue = new ReadOnlyDictionary<int, string>(_namesByValue);
            NamesByEnum = new ReadOnlyDictionary<TestEnum, string>(_namesByEnum);
            EnumsByName = new ReadOnlyDictionary<string, TestEnum>(_enumsByName);
            EnumsByValue = new ReadOnlyDictionary<int, TestEnum>(_enumsByValue);
            ValuesByEnum = new ReadOnlyDictionary<TestEnum, int>(_valuesByEnum);
        }
        
        public static class Values
        {
            public const int One = 0;
            public const int Two = 1;
            public const int Three = 2;
            public const int Four = 3;

            private static readonly int[] _allValues = new int[]
            {
                One, 
                Two, 
                Three, 
                Four
            };

            private static readonly ReadOnlyCollection<int> _readOnlyValues = new ReadOnlyCollection<int>(_allValues);
            public static ReadOnlyCollection<int> All() => _readOnlyValues;
        }
        
        public static class Names
        {
            public const string One = "One";
            public const string Two = "Two";
            public const string Three = "Three";
            public const string Four = "Four";

            private static readonly string[] _allNames = new string[]
            {
                One, 
                Two, 
                Three, 
                Four
            };
            private static readonly ReadOnlyCollection<string> _readOnlyNames = new ReadOnlyCollection<string>(_allNames);

            public static ReadOnlyCollection<string> All() => _readOnlyNames;
        }
    }
}