using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace ShuHai
{
    /// <summary>
    ///     Represents type name information that can are needed to make up the type.
    /// </summary>
    public sealed class TypeName : IEquatable<TypeName>
    {
        public string AssemblyName { get; private set; }

        /// <summary>
        ///     The simple name of the type.
        ///     This is the name written in code without namespace for non-generic types, e.g. String, Int32, TypeName, etc; for
        ///     generic type this is the type definition name without its namespace.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Full name of the type, including its namespace but not its assembly.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        ///     Namespace of the type.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        ///     Assembly-qualified name of the type, which includes <see cref="AssemblyName" /> if exists.
        /// </summary>
        public string AssemblyQualifiedName { get; private set; }

        /// <summary>
        ///     The name used to find the type in single assembly - without generic arguments or array symbols.
        ///     e.g. Decalre name of "System.Collections.Generic.Dictionary`2[System.String,System.Boolean][,]" is
        ///     "System.Collections.Generic.Dictionary`2".
        /// </summary>
        public string DeclareName { get; private set; }

        /// <summary>
        ///     Parent type name of current instance if current instance represents a generic argument type name of certain generic
        ///     type name.
        /// </summary>
        public readonly TypeName GenericParent;

        #region Construct

        private TypeName(string fullyQualifiedName) { Construct(fullyQualifiedName); }

        private TypeName(string fullyQualifiedName, TypeName genericParent)
            : this(fullyQualifiedName)
        {
            GenericParent = genericParent;
        }

        private void Construct(string fullyQualifiedName)
        {
            Ensure.Argument.NotNullOrEmpty(fullyQualifiedName, "fullQualifiedName");

            AssemblyQualifiedName = TrimName(fullyQualifiedName);
            ParseFullNameAndAssemblyNameFromAssemblyQualifiedName();

            int arrayBracketIndex;
            ParseArrayRanksFromFullName(out arrayBracketIndex);

            ParseDeclareNameAndGenericArgumentsFromFullName(arrayBracketIndex);

            ParseNamespaceAndNameFromDeclareName();
        }

        private void ParseFullNameAndAssemblyNameFromAssemblyQualifiedName()
        {
            int delimiterIndex = GetAssemblyDelimiterIndex(AssemblyQualifiedName);
            if (delimiterIndex == 0)
                throw new ArgumentException(string.Format("'{0}' is not a valid type name.", AssemblyQualifiedName));

            if (delimiterIndex > 0)
            {
                FullName = AssemblyQualifiedName.Substring(0, delimiterIndex).Trim();
                AssemblyName = AssemblyQualifiedName
                    .Substring(delimiterIndex + 1, AssemblyQualifiedName.Length - delimiterIndex - 1).Trim();
            }
            else
            {
                FullName = AssemblyQualifiedName;
                AssemblyName = null;
            }
        }

        private void ParseDeclareNameAndGenericArgumentsFromFullName(int arrayBracketIndex)
        {
            DeclareName = arrayBracketIndex >= 0 ? FullName.Substring(0, arrayBracketIndex) : FullName;

            int leftBracketIndex, rightBracketIndex;
            if (FindFirstRootBracketPairIndices(DeclareName, out leftBracketIndex, out rightBracketIndex))
            {
                var argString = DeclareName.Substring(
                    leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();
                DeclareName = DeclareName.Substring(0, leftBracketIndex).Trim();
                ParseGenericArguments(argString);
            }
        }

        private void ParseNamespaceAndNameFromDeclareName()
        {
            var nsArray = DeclareName.Split('.');
            int nsArrayLen = nsArray.Length;

            var nestedName = nsArray[nsArrayLen - 1];
            if (nsArrayLen > 1)
                Namespace = DeclareName.Substring(0, DeclareName.Length - nestedName.Length - 1);

            var nameArray = nestedName.Split('+');
            Name = nameArray[nameArray.Length - 1];
        }

        #endregion

        #region Equality

        public static bool operator ==(TypeName l, TypeName r) { return CompareUtil.OperatorEquals(l, r); }
        public static bool operator !=(TypeName l, TypeName r) { return !CompareUtil.OperatorEquals(l, r); }

        public bool Equals(TypeName other)
        {
            return other != null && AssemblyQualifiedName == other.AssemblyQualifiedName;
        }

        public override bool Equals(object obj) { return Equals(obj as TypeName); }

        public override int GetHashCode() { return AssemblyQualifiedName.GetHashCode(); }

        #endregion

        #region To String

        public override string ToString() { return ToString(null); }

        public string ToString(FormatterAssemblyStyle? assemblyNameStyle) { return ToString(true, assemblyNameStyle); }

        public string ToString(bool withNamespace, FormatterAssemblyStyle? assemblyNameStyle)
        {
            return ToString(new StringBuilder(), withNamespace, assemblyNameStyle).ToString();
        }

        private StringBuilder ToString(StringBuilder builder,
            bool withNamespace, FormatterAssemblyStyle? assemblyNameStyle)
        {
            builder.Append(withNamespace ? DeclareName : Name);
            AppendGenericArguments(builder, withNamespace, assemblyNameStyle);
            AppendArrayRanks(builder);
            AppendAssemblyName(builder, assemblyNameStyle);
            return builder;
        }

        private void AppendGenericArguments(StringBuilder builder,
            bool withNamespace, FormatterAssemblyStyle? assemblyNameStyle)
        {
            if (GenericArgumentCount == 0)
                return;

            builder.Append('[');

            foreach (var arg in GenericArguments)
            {
                bool appendAssemblyName = CanAppendAssemblyName(arg, assemblyNameStyle);
                if (appendAssemblyName)
                    builder.Append('[');
                arg.ToString(builder, withNamespace, assemblyNameStyle);
                if (appendAssemblyName)
                    builder.Append(']');
                builder.Append(',');
            }
            builder.RemoveTail(1); // Rmove last ','

            builder.Append(']');
        }

        private void AppendArrayRanks(StringBuilder builder)
        {
            if (!IsArray)
                return;

            foreach (var rank in arrayRanks)
            {
                builder.Append('[');
                for (int i = 1; i < rank; ++i)
                    builder.Append(',');
                builder.Append(']');
            }
        }

        private void AppendAssemblyName(StringBuilder builder, FormatterAssemblyStyle? assemblyNameStyle)
        {
            if (!CanAppendAssemblyName(this, assemblyNameStyle))
                return;

            builder.Append(',');
            builder.Append(' ');

            var style = assemblyNameStyle.Value;
            switch (style)
            {
                case FormatterAssemblyStyle.Simple:
                    for (int i = 0; i < AssemblyName.Length; ++i)
                    {
                        var c = AssemblyName[i];
                        if (c == ',')
                            break;
                        builder.Append(c);
                    }
                    break;
                case FormatterAssemblyStyle.Full:
                    builder.Append(AssemblyName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("assemblyNameStyle");
            }
        }

        private static bool CanAppendAssemblyName(TypeName typeName, FormatterAssemblyStyle? assemblyNameStyle)
        {
            return !string.IsNullOrEmpty(typeName.AssemblyName) && assemblyNameStyle.HasValue;
        }

        #endregion

        #region Array

        public bool IsArray { get { return arrayRanks.Count > 0; } }

        public int ArrayDeclareCount { get { return arrayRanks.Count; } }

        /// <summary>
        ///     Number of dimensions for each array declaration if current instance is a jagged array.
        /// </summary>
        public IEnumerable<int> ArrayRanks { get { return arrayRanks; } }

        /// <summary>
        ///     Get the number of dimensions array of current type instance.
        /// </summary>
        /// <param name="index">
        ///     Index to locate which array declaration to get. The value greater than 0 is only valid if current instance is a
        ///     jagged array.
        /// </param>
        public int GetArrayRank(int index = 0) { return arrayRanks[index]; }

        private readonly List<int> arrayRanks = new List<int>();

        private bool ParseArrayRanksFromFullName(out int bracketStartIndex)
        {
            bracketStartIndex = -1;

            int depth = 0, maxIndex = FullName.Length - 1, rank = 1;
            for (int i = maxIndex; i >= 0; --i)
            {
                bool nonArrayChar = false;
                var c = FullName[i];
                switch (c)
                {
                    case ']':
                        depth++;
                        break;
                    case '[':
                        depth--;
                        if (depth == 0)
                        {
                            arrayRanks.Add(rank);
                            rank = 1;
                        }
                        break;
                    case ',':
                        rank++;
                        break;
                    case ' ':
                    case '*':
                        break;
                    default: // Characters except "[*,] ".
                        nonArrayChar = true;
                        if (depth == 0 && i != maxIndex)
                            bracketStartIndex = i + 1;
                        break;
                }
                if (nonArrayChar)
                    break;
            }
            arrayRanks.Reverse();

            return bracketStartIndex >= 0;
        }

        #endregion

        #region Generic Arguments

        public bool IsGeneric { get { return GenericArgumentCount > 0; } }

        public int GenericArgumentCount { get { return genericArguments.Count; } }

        public IEnumerable<TypeName> GenericArguments { get { return genericArguments; } }

        public TypeName GetGenericArgument(int index) { return genericArguments[index]; }

        private readonly List<TypeName> genericArguments = new List<TypeName>();

        private void ParseGenericArguments(string argString)
        {
            var delimiterIndices = new List<int>(EnumRootDelimiterIndices(argString));
            if (delimiterIndices.Count > 0)
            {
                int startIndex = 0;
                foreach (var index in delimiterIndices)
                {
                    var nodeStr = argString.Substring(startIndex, index - startIndex);
                    AddGenericArgument(nodeStr);
                    startIndex = index + 1;
                }
                var lastNodeStr = argString.Substring(startIndex, argString.Length - startIndex);
                AddGenericArgument(lastNodeStr);
            }
            else
            {
                AddGenericArgument(argString);
            }
        }

        private void AddGenericArgument(string argString) { genericArguments.Add(new TypeName(argString, this)); }

        #endregion

        #region Instances

        public static TypeName Get(Type type)
        {
            Ensure.Argument.NotNull(type, "type");
            return Get(type.AssemblyQualifiedName);
        }

        public static TypeName Get(string fullyQualifiedName)
        {
            TypeName name;
            if (!instances.TryGetValue(fullyQualifiedName, out name))
            {
                name = new TypeName(fullyQualifiedName);
                instances.Add(fullyQualifiedName, name);
            }
            return name;
        }

        private static readonly Dictionary<string, TypeName> instances = new Dictionary<string, TypeName>();

        #endregion

        #region Utilities

        private static string TrimName(string name)
        {
            name = name.Trim();
            if (name.StartsWith("[") && name.EndsWith("]"))
                name = name.Substring(1, name.Length - 2);
            return name;
        }

        private static int GetAssemblyDelimiterIndex(string fullQualifiedName)
        {
            // We need to get the first comma following all surrounded in brackets because of generic types. e.g.
            // System.Collections.Generic.Dictionary`2[
            //   [System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],
            //   [System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]],
            //     mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
            foreach (var index in EnumRootDelimiterIndices(fullQualifiedName))
                return index;
            return -1;
        }

        private static IEnumerable<int> EnumRootDelimiterIndices(string fullName)
        {
            int depth = 0;
            for (int i = 0; i < fullName.Length; i++)
            {
                char c = fullName[i];
                switch (c)
                {
                    case '[':
                        depth++;
                        break;
                    case ']':
                        depth--;
                        break;
                    case ',':
                        if (depth == 0)
                            yield return i;
                        break;
                }
            }
        }

        private static bool FindFirstRootBracketPairIndices(string fullName, out int leftIndex, out int rightIndex)
        {
            leftIndex = -1;
            rightIndex = -1;

            int depth = 0;
            for (int i = 0; i < fullName.Length; i++)
            {
                char c = fullName[i];
                switch (c)
                {
                    case '[':
                        if (depth == 0)
                            leftIndex = i;
                        depth++;
                        break;
                    case ']':
                        depth--;
                        if (depth == 0)
                            rightIndex = i;
                        break;
                }

                if (leftIndex >= 0 && rightIndex >= 0)
                    break;
            }

            return leftIndex >= 0 && rightIndex >= 0;
        }

        #endregion
    }
}