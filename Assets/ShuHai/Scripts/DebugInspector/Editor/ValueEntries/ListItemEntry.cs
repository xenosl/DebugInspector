using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class ListItemEntry<TList, TItem> : ValueEntry<TList, TItem>
        where TList : IList
    {
        public int Index { get; private set; }

        public override TItem Value { get { return (TItem)Owner[Index]; } set { Owner[Index] = value; } }

        public ListItemEntry(TList owner, int index) : base(owner) { Index = index; }
    }

    public class TListItemEntry<TList, TItem> : ValueEntry<TList, TItem>
        where TList : IList<TItem>
    {
        public int Index { get; private set; }

        public override TItem Value { get { return Owner[Index]; } set { Owner[Index] = value; } }

        public TListItemEntry(TList owner, int index) : base(owner) { Index = index; }
    }

    public static class ListItemEntryFactory
    {
        public static IValueEntry Create<TList>(bool isGeneric, Type itemType, TList owner, int index)
        {
            Ensure.Argument.NotNull(itemType, "itemType");
            Ensure.Argument.NotNull(owner, "owner");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            var entryTypeDef = isGeneric ? typeof(TListItemEntry<,>) : typeof(ListItemEntry<,>);
            try
            {
                var entryType = entryTypeDef.MakeGenericType(typeof(TList), itemType);
                return (IValueEntry)Activator.CreateInstance(entryType, owner, index);
            }
            catch (Exception e)
            {
                var msg = string.Format("Create({0},{1})\n{2}", typeof(TList), itemType, e);
                Debug.LogError(msg);
            }
            return null;
        }
    }
}