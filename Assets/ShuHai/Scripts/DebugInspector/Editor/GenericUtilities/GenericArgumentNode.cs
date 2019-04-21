using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShuHai.DebugInspector.Editor
{
    public class GenericArgumentNode
    {
        public readonly Type ArgumentType;

        public readonly GenericArgumentNode Parent;

        public GenericArgumentNode(Type argumentType, GenericArgumentNode parent = null)
        {
            Ensure.Argument.NotNull(argumentType, "argumentType");

            ArgumentType = argumentType;
            Parent = parent;

            if (argumentType.IsGenericType)
            {
                children = ArgumentType.GetGenericArguments()
                    .Select(a => new GenericArgumentNode(a, this))
                    .ToArray();
            }
        }

        #region Children

        public int ChildCount { get { return children.Length; } }

        public IEnumerable<GenericArgumentNode> Children { get { return children; } }

        public GenericArgumentNode GetChild(int index) { return children[index]; }

        private GenericArgumentNode[] children;

        #endregion Children

        #region Equality

        public static bool operator ==(GenericArgumentNode l, GenericArgumentNode r) { return Equals(l, r); }
        public static bool operator !=(GenericArgumentNode l, GenericArgumentNode r) { return !Equals(l, r); }

        public bool Equals(GenericArgumentNode other)
        {
            return other != null
                && ArgumentType == other.ArgumentType
                && Parent == other.Parent
                && CompareUtil.ListEquals(children, other.children);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GenericArgumentNode);
        }

        public override int GetHashCode() { return hashCode; }

        private int hashCode;

        private void InitHashCode()
        {
            unchecked
            {
                hashCode = HashCode.Calculate(ArgumentType, Parent);
                hashCode = hashCode * 23 + HashCode.Calculate(children);
            }
        }

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

        public override string ToString()
        {
            return ToString(false, false);
        }

        private static StringBuilder AppendNode(StringBuilder builder, GenericArgumentNode node, bool fullname)
        {
            return builder.Append(node.GetType().GetName(fullname))
                .AppendFormat("({0})", node.ArgumentType.GetName(fullname));
        }

        private StringBuilder AppendChild(StringBuilder builder, GenericArgumentNode node, bool fullname, byte depth)
        {
            builder.AppendLine().AppendIndent(depth, "  ");
            AppendNode(builder, node, fullname);
            foreach (var child in node.Children)
                AppendChild(builder, child, fullname, (byte)(depth + 1));
            return builder;
        }

        #endregion ToString
    }
}