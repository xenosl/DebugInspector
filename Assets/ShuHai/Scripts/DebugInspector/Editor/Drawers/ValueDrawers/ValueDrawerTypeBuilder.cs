using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class ValueDrawerTypeBuilder
    {
        #region Instances

        /// <summary>
        ///     The <see cref="ValueDrawerTypeBuilder" /> instance for <see cref="ValueDrawer{TOwner, TValue}" />.
        /// </summary>
        public static ValueDrawerTypeBuilder Root { get; private set; }

        /// <summary>
        ///     Number of <see cref="ValueDrawerTypeBuilder" /> instances.
        /// </summary>
        public static int InstanceCount { get { return instances.Count; } }

        /// <summary>
        ///     Enumeration of all <see cref="ValueDrawerTypeBuilder" /> instances.
        /// </summary>
        public static IEnumerable<ValueDrawerTypeBuilder> Instances { get { return instances.Values; } }

        /// <summary>
        ///     Get instance of <see cref="ValueDrawerTypeBuilder" /> by the drawer type definition the instance builds from.
        /// </summary>
        /// <param name="drawerTypeDef">
        ///     The generic type definition of <see cref="ValueDrawer{TOwner, TValue}" /> that the result instance builds from.
        /// </param>
        public static ValueDrawerTypeBuilder Get(Type drawerTypeDef) { return instances.GetValue(drawerTypeDef); }

        private static Dictionary<Type, ValueDrawerTypeBuilder> instances;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            instances = ValueDrawerTypes.Definitions.ToDictionary(t => t, t => new ValueDrawerTypeBuilder(t));
            Root = Get(ValueDrawerTypes.RootDefinition);
        }

        #endregion Instances

        public readonly Type DrawerTypeDef;

        public Type Build(Type ownerType, Type valueType)
        {
            Ensure.Argument.NotNull(ownerType, "ownerType");
            Ensure.Argument.NotNull(valueType, "valueType");
            return BuildImpl(ownerType, valueType);
        }

        public bool TryBuild(Type ownerType, Type valueType, out Type drawerType)
        {
            Ensure.Argument.NotNull(ownerType, "ownerType");
            Ensure.Argument.NotNull(valueType, "valueType");

            bool succeed = true;
            try
            {
                drawerType = BuildImpl(ownerType, valueType);
            }
            catch (Exception)
            {
                drawerType = null;
                succeed = false;
            }
            return succeed;
        }

        private Type BuildImpl(Type ownerType, Type valueType)
        {
            CheckUserArgIfRootIsSpecificType(ownerType, "ownerType", GenericArgOwnerIndex);
            CheckUserArgIfRootIsSpecificType(valueType, "valueType", GenericArgValueIndex);

            Type[] specificArgs;
            if (!TryExtractSpecificGenericArgs(ownerType, valueType, out specificArgs))
            {
                var msg = string.Format(
                    "Unable to extract generic arguments from '{0}' and '{1}' for {2}",
                    ownerType, valueType, DrawerTypeDef);
                throw new TypeLoadException(msg);
            }

            for (int i = 0; i < genericArgs.Count; ++i)
            {
                Type sa = specificArgs[i], ga = genericArgs[i];
                if (!GenericParameterConstraints.Match(sa, ga))
                    throw new ArgumentException(string.Format("Generic argument '{0}' not match with '{1}'.", sa, ga));
            }

            return DrawerTypeDef.MakeGenericType(specificArgs);
        }

        private void CheckUserArgIfRootIsSpecificType(Type userArg, string argName, int rootArgIndex)
        {
            var rootArg = rootGenericArgs[rootArgIndex];
            if (rootArg.ContainsGenericParameters)
                return;

            if (!rootArg.IsAssignableFrom(userArg))
            {
                var msg = string.Format("Generic argument '{0}' not match with '{1}'.", userArg, rootArg);
                throw new ArgumentException(msg, argName);
            }
        }

        private ValueDrawerTypeBuilder(Type typeDef)
        {
            Ensure.Argument.NotNull(typeDef, "typeDef");
            Ensure.Argument.Satisfy(typeDef.IsGenericTypeDefinition, "typeDef", "Generic type definition expected.");
            Ensure.Argument.Is<ValueDrawer>(typeDef, "typeDef");

            DrawerTypeDef = typeDef;

            InitGenericArgs();
            InitArgExtractionNodes();
        }

        #region Specific Argument Extraction

        public bool TryExtractSpecificGenericArgs(Type ownerType, Type valueType, out Type[] specificArgs)
        {
            specificArgs = ArrayUtil.Empty<Type>();

            Type ownerArg = genericArgs.Owner, valueArg = genericArgs.Value;
            bool ownerExtractable = ownerArg.ContainsGenericParameters,
                valueExtractable = valueArg.ContainsGenericParameters;
            if (!ownerExtractable && !valueExtractable)
                return false;

            var argInfos = new List<Tuple<Type, int>>();
            if (ownerExtractable)
            {
                Ensure.Argument.NotNull(ownerType, "ownerType");

                Tuple<Type, int>[] resultArgs;
                if (!TryExtractSpecificGenericArgs(ownerType, GenericArgOwnerIndex, out resultArgs))
                    return false;
                argInfos.AddRange(resultArgs);
            }

            if (valueExtractable)
            {
                Ensure.Argument.NotNull(valueType, "valueType");

                Tuple<Type, int>[] resultArgs;
                if (!TryExtractSpecificGenericArgs(valueType, GenericArgValueIndex, out resultArgs))
                    return false;
                argInfos.AddRange(resultArgs);
            }

            var argInfoOrdered = argInfos.Distinct().OrderBy(i => i.Item2).ToArray();

            int invalidCount = argInfoOrdered.Where((info, index) => info.Item2 != index).Count();
            specificArgs = argInfoOrdered.Select(i => i.Item1).ToArray();
            return invalidCount == 0 && specificArgs.Length == genericArgs.Count;
        }

        public bool TryExtractSpecificGenericArgs(
            Type userArg, int rootArgIndex, out Tuple<Type, int>[] specificArgs)
        {
            Ensure.Argument.NotNull(userArg, "userArg");
            Ensure.Argument.IsValidIndex(rootArgIndex, "rootArgIndex", rootGenericArgInfos.Length);

            bool succeed = true;
            try
            {
                specificArgs = ExtractSpecificGenericArgs(userArg, rootArgIndex);
            }
            catch (Exception)
            {
                specificArgs = ArrayUtil.Empty<Tuple<Type, int>>();
                succeed = false;
            }
            return succeed;
        }

        /// <summary>
        ///     Extract specific types of the generic argument list from a given type base on a generic parameter of root
        ///     drawer type.
        /// </summary>
        /// <param name="userArg"> From where the specific types of generic arguments are extracted. </param>
        /// <param name="rootArgIndex"> Index of the generic parameter in root drawer type. </param>
        /// <returns>
        ///     Tuple pairs that contain the extracted specific  types of generic arguments and its corresponding index.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="userArg" /> is <see langword="null" />
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="userArg" /> contains any generic parameter that is not replaced by specific type.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="rootArgIndex" /> is less than 0, or is equal or greater than number of root generic parameters.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Root generic parameter at <paramref name="rootArgIndex" /> is already replaced by specific type.
        /// </exception>
        public Tuple<Type, int>[] ExtractSpecificGenericArgs(Type userArg, int rootArgIndex)
        {
            Ensure.Argument.NotNull(userArg, "userArg");
            Ensure.Argument.Satisfy(!userArg.ContainsGenericParameters,
                "userArg", "Specific generic argument type expected.");
            Ensure.Argument.IsValidIndex(rootArgIndex, "rootArgIndex", rootGenericArgInfos.Length);

            var rootArgInfo = rootGenericArgInfos[rootArgIndex];
            var rootArg = rootArgInfo.ArgType;
            if (!rootArg.ContainsGenericParameters)
            {
                var msg = string.Format(
                    "Unable to extract specific arguments based on root argument '{0}' which does not contain any generic parameter.",
                    rootArg);
                throw new InvalidOperationException(msg);
            }

            var argInfos = new List<Tuple<Type, int>>();
            if (rootArgInfo.IsSelfArg)
                argInfos.Add(Tuple.Create(userArg, rootArgInfo.IndexInSelf));

            var extractionNode = argExtractionNodes[rootArgIndex];
            if (extractionNode != null)
                argInfos.AddRange(extractionNode.ExtractTargetArgs(userArg));

            return argInfos.Distinct().ToArray();
        }

        #region Generic Arguments

        private const int GenericArgOwnerIndex = 0;
        private const int GenericArgValueIndex = 1;

        private GenericArguments rootGenericArgs { get { return genericArgs.Roots; } }
        private ValueDrawerGenericArguments genericArgs;

        private GenericArgInfo[] rootGenericArgInfos;

        private void InitGenericArgs()
        {
            genericArgs = new ValueDrawerGenericArguments(DrawerTypeDef);

            int rootArgCount = rootGenericArgs.Count;
            rootGenericArgInfos = new GenericArgInfo[rootArgCount];
            for (int i = 0; i < rootArgCount; ++i)
            {
                var arg = rootGenericArgs[i];
                rootGenericArgInfos[i] = new GenericArgInfo(arg, genericArgs.IndexOf(arg));
            }
        }

        private class GenericArgInfo
        {
            public readonly Type ArgType;

            public readonly int IndexInSelf = -1;

            public bool IsSelfArg { get { return IndexInSelf >= 0; } }

            public GenericArgInfo(Type argType, int indexInSelf)
            {
                ArgType = argType;
                IndexInSelf = indexInSelf;
            }
        }

        #endregion Generic Arguments

        #region Extraction Nodes

        private ArgExtractionNode[] argExtractionNodes;

        private void InitArgExtractionNodes()
        {
            argExtractionNodes = rootGenericArgs.Select(CreateArgExtractionNode).ToArray();
        }

        private ArgExtractionNode CreateArgExtractionNode(Type arg)
        {
            if (!arg.ContainsGenericParameters)
                return null;

            if (arg.IsGenericType)
            {
                return new ArgExtractionNode(arg, null, genericArgs);
            }
            else if (arg.IsGenericParameter)
            {
                var constraints = arg.GetGenericParameterConstraints();
                if (constraints.Length > 0)
                    return new ArgExtractionNode(constraints[0], null, genericArgs);
                else
                    return new ArgExtractionNode(arg, null, genericArgs);
            }
            return null;
        }

        private sealed class ArgExtractionNode : IEquatable<ArgExtractionNode>
        {
            /// <summary>
            ///     The generic argument this node represents.
            /// </summary>
            public readonly Type Arg;

            /// <summary>
            ///     Index of <see cref="Arg" /> in the generic argument list of target type.
            /// </summary>
            public readonly int IndexInTargetArgs;

            /// <summary>
            ///     Parent node of this node.
            /// </summary>
            public readonly ArgExtractionNode Parent;

            /// <summary>
            ///     Indicates whether current node or any children of the current node contains any target generic
            ///     arguments. That is <see cref="IndexInTargetArgs" /> refers to some target argument.
            /// </summary>
            public bool ContainsTargetArg { get; private set; }

            public ArgExtractionNode(Type arg, ArgExtractionNode parent, GenericArguments targetArgs)
            {
                Ensure.Argument.NotNull(arg, "arg");
                //Ensure.Argument("arg").Satisfy(arg.ContainsGenericParameters, "Any generic parameter expected.");

                Arg = arg;
                Parent = parent;

                IndexInTargetArgs = targetArgs.IndexOf(Arg);
                if (IndexInTargetArgs >= 0)
                {
                    var inst = this;
                    while (inst != null)
                    {
                        inst.ContainsTargetArg = true;
                        inst = inst.Parent;
                    }
                }
                else
                {
                    children = arg.GetGenericArguments()
                        .Select(a => new ArgExtractionNode(a, this, targetArgs))
                        .ToArray();
                }
            }

            #region Children

            public int ChildCount { get { return children.Length; } }
            public IEnumerable<ArgExtractionNode> Children { get { return children; } }

            public ArgExtractionNode GetChild(int index) { return children[index]; }

            private ArgExtractionNode[] children;

            #endregion Children

            #region Extract

            public Tuple<Type, int>[] ExtractTargetArgs(Type toExtract)
            {
                Ensure.Argument.NotNull(toExtract, "toExtract");

                var typeToIndex = new Dictionary<Type, int>();
                ExtractTargetArgs(toExtract, typeToIndex);
                return typeToIndex.Select(kvp => Tuple.Create(kvp.Key, kvp.Value)).ToArray();
            }

            private void ExtractTargetArgs(Type toExtract, Dictionary<Type, int> typeToIndex)
            {
                if (IndexInTargetArgs < 0) // this.Arg is not one of the target arguments.
                {
                    var args = toExtract.GetGenericArguments();
                    if (args.Length == ChildCount)
                    {
                        ExtractTargetArgsInChildren(args, typeToIndex);
                    }
                    else
                    {
                        var toExtractInterfaces = toExtract.GetInterfaces();
                        foreach (var i in toExtractInterfaces)
                        {
                            if (!i.IsGenericType)
                                continue;

                            var iargs = i.GetGenericArguments();
                            if (iargs.Length == ChildCount)
                            {
                                ExtractTargetArgsInChildren(iargs, typeToIndex);
                                break;
                            }
                        }
                    }
                }
                else // IndexInTargetArgs >= 0, this.Arg is one of the target arguments.
                {
                    typeToIndex.Add(toExtract, IndexInTargetArgs);
                }
            }

            private void ExtractTargetArgsInChildren(Type[] toExtractArgs, Dictionary<Type, int> typeToIndex)
            {
                for (int i = 0; i < ChildCount; ++i)
                    GetChild(i).ExtractTargetArgs(toExtractArgs[i], typeToIndex);
            }

            #endregion Extract

            #region Equality

            public static bool operator ==(ArgExtractionNode l, ArgExtractionNode r) { return Equals(l, r); }
            public static bool operator !=(ArgExtractionNode l, ArgExtractionNode r) { return !Equals(l, r); }

            public bool Equals(ArgExtractionNode other)
            {
                return other != null
                    && Arg == other.Arg
                    && IndexInTargetArgs == other.IndexInTargetArgs
                    && Parent == other.Parent;
            }

            public override bool Equals(object obj) { return Equals(obj as ArgExtractionNode); }

            public override int GetHashCode() { return HashCode.Calculate(Arg, IndexInTargetArgs, Parent); }

            #endregion Equality

            #region ToString

            public string ToString(bool includeChildren, bool fullname = false)
            {
                var builder = AppendNode(new StringBuilder(), this, fullname);

                if (includeChildren)
                {
                    foreach (var child in Children)
                        AppendChild(builder, child, fullname, 1);
                }

                return builder.ToString();
            }

            public override string ToString() { return ToString(false, false); }

            private static StringBuilder AppendNode(StringBuilder builder, ArgExtractionNode node, bool fullname)
            {
                return builder.Append(node.GetType().GetName(fullname))
                    .AppendFormat("({0}@{1})", node.Arg.GetName(fullname), node.IndexInTargetArgs);
            }

            private StringBuilder AppendChild(StringBuilder builder, ArgExtractionNode node, bool fullname, byte depth)
            {
                builder.AppendLine().AppendIndent(depth, "  ");
                AppendNode(builder, node, fullname);
                foreach (var child in node.Children)
                    AppendChild(builder, child, fullname, (byte)(depth + 1));
                return builder;
            }

            #endregion ToString
        }

        #endregion Extraction Nodes

        #endregion Specific Argument Extraction

        #region ToString

        public string ToString(bool fullname)
        {
            return string.Format("{0}({1})", GetType().GetName(fullname), DrawerTypeDef.GetName(fullname));
        }

        public override string ToString() { return ToString(false); }

        #endregion ToString
    }
}