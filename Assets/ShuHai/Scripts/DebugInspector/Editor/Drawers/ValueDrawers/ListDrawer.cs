using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class ListDrawer<TOwner, TList> : CollectionDrawer<TOwner, TList>
        where TList : IList
    {
        private readonly ListDrawerImpl<TOwner, TList> impl;

        protected ListDrawer()
        {
            impl = new ListDrawerImpl<TOwner, TList>(this)
            {
                DefaultItemType = typeof(object),
                ItemTypeGetter = new ValueAccesHelper<TList, int, Type>(GetItemType),
                ChildValueEntryCreator = CreateChildValueEntry
            };
        }

        private static Type GetItemType(TList list, int index)
        {
            var item = list[index];
            return item != null ? item.GetType() : typeof(object);
        }

        private static IValueEntry CreateChildValueEntry(TList list, Type itemType, int itemIndex)
        {
            return ListItemEntryFactory.Create(false, itemType, list, itemIndex);
        }

        protected override void ChildrenUpdateImpl() { impl.ChildrenUpdate(TypedDrawingValue); }

        protected override int BeforeCurrentPageIndexChange(int newIndex)
        {
            int index = impl.BeforeCurrentPageIndexChange(newIndex);
            childrenNeedUpdate = true;
            return index;
        }
    }

    public class ListDrawer<TOwner, TList, TItem> : CollectionDrawer<TOwner, TList, TItem>
        where TList : IList<TItem>
    {
        private readonly ListDrawerImpl<TOwner, TList> impl;

        protected ListDrawer()
        {
            impl = new ListDrawerImpl<TOwner, TList>(this)
            {
                DefaultItemType = typeof(TItem),
                ItemTypeGetter = new ValueAccesHelper<TList, int, Type>(GetItemType),
                ChildValueEntryCreator = CreateChildValueEntry
            };
        }

        private static Type GetItemType(TList list, int index) { return typeof(TItem); }

        private static IValueEntry CreateChildValueEntry(TList list, Type itemType, int itemIndex)
        {
            return ListItemEntryFactory.Create(true, itemType, list, itemIndex);
        }

        protected override void ChildrenUpdateImpl() { impl.ChildrenUpdate(TypedDrawingValue); }

        protected override int BeforeCurrentPageIndexChange(int newIndex)
        {
            int index = impl.BeforeCurrentPageIndexChange(newIndex);
            childrenNeedUpdate = true;
            return index;
        }
    }

    internal class ListDrawerImpl<TOwner, TList>
    {
        public readonly PageableDrawer<TOwner, TList> This;

        public ListDrawerImpl(PageableDrawer<TOwner, TList> @this) { This = @this; }

        public Type DefaultItemType;
        public ValueAccesHelper<TList, int, Type> ItemTypeGetter;
        public Func<TList, Type, int, IValueEntry> ChildValueEntryCreator;

        public void UpdateChildCount()
        {
            // Remove redundant child drawers.
            while (This.ChildCount > This.ItemCountInCurrentPage)
                This.LastChild.Parent = null;
        }

        public void ChildrenUpdate(TList list)
        {
            int countInPage = This.ItemCountInCurrentPage;
            // Remove redundant child drawers.
            while (This.ChildCount > countInPage)
                This.LastChild.Parent = null;

            int firstItemIndex = This.FirstItemIndexOfCurrentPage;
            for (int i = 0; i < countInPage; ++i)
            {
                int itemIndex = firstItemIndex + i;
                Type itemType;
                ItemTypeGetter.GetValue(list, itemIndex, out itemType, DefaultItemType);
                var childType = ValueDrawerTypes.GetOrBuild(typeof(TList), itemType);

                var child = This.GetChild(i) as ValueDrawer;
                if (child != null)
                {
                    if (child.GetType() != childType)
                    {
                        child = (ValueDrawer)Drawer.Create(childType);
                        This.ReplaceChild(i, child);
                    }
                }
                else
                {
                    child = (ValueDrawer)Drawer.Create(childType);
                    child.ChangeParent(This, i);
                }
                child.Name = itemIndex.ToString();
                child.ValueEntry = ChildValueEntryCreator(list, itemType, itemIndex);
            }
        }

        public int BeforeCurrentPageIndexChange(int newIndex) { return Mathf.Clamp(newIndex, 0, This.PageCount - 1); }
    }
}