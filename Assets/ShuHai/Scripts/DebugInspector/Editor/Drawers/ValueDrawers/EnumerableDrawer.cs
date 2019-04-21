using System;
using System.Collections;
using System.Collections.Generic;
using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class EnumerableDrawer<TOwner, TCollection> : PageableDrawer<TOwner, TCollection>
        where TCollection : IEnumerable
    {
        private readonly EnumerableDrawerImpl<TOwner, TCollection> impl;

        protected EnumerableDrawer()
        {
            impl = new EnumerableDrawerImpl<TOwner, TCollection>(this)
            {
                ItemTypeGetter = i => GetItemType(enumeratedItems, i),
                ValueEntryCreator = (i, c) => CreateItemValueEntry(enumeratedItems, i, TypedDrawingValue)
            };
        }

        #region Items

        public override int ItemCount { get { return enumeratedItemCount; } }

        //public virtual bool TooManyItemsToEnumerate
        //{
        //    get { return ItemCount >= Settings.DefaultMaxItemCountToEnumerate; }
        //}

        protected static Type GetItemType<TItem>(ItemList<TItem> list, int index)
        {
            if (!Index.IsValid(index, list.Count))
                return typeof(TItem);
            var item = list[index].Item;
            return item != null ? item.GetType() : typeof(TItem);
        }

        protected static IValueEntry CreateItemValueEntry<TItem>(
            ItemList<TItem> list, int itemIndex, TCollection collection)
        {
            if (!Index.IsValid(itemIndex, list.Count))
                return null;
            return list[itemIndex].CreateValueEntry(collection);
        }

        protected class ItemList<TItem> : IList<ItemInfo<TItem>>
        {
            public int Count { get { return list.Count; } }

            public ItemInfo<TItem> this[int index] { get { return list[index]; } set { list[index] = value; } }

            public ItemList() { list = new List<ItemInfo<TItem>>(); }
            public ItemList(int capacity) { list = new List<ItemInfo<TItem>>(capacity); }
            public ItemList(IEnumerable<ItemInfo<TItem>> collection) { list = new List<ItemInfo<TItem>>(collection); }

            public void Add(ItemInfo<TItem> item) { list.Add(item); }
            public void Insert(int index, ItemInfo<TItem> item) { list.Insert(index, item); }

            public void RemoveAt(int index) { list.RemoveAt(index); }
            public bool Remove(ItemInfo<TItem> item) { return list.Remove(item); }
            public void Clear() { list.Clear(); }

            public int IndexOf(ItemInfo<TItem> item) { return list.IndexOf(item); }
            public bool Contains(ItemInfo<TItem> item) { return list.Contains(item); }

            public void CopyTo(ItemInfo<TItem>[] array, int arrayIndex) { list.CopyTo(array, arrayIndex); }
            public ItemInfo<TItem>[] ToArray() { return list.ToArray(); }

            private readonly List<ItemInfo<TItem>> list;

            #region Enumerator

            public IEnumerator<ItemInfo<TItem>> GetEnumerator() { return list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

            #endregion

            #region Explicite Implementations

            bool ICollection<ItemInfo<TItem>>.IsReadOnly { get { return collection.IsReadOnly; } }

            private ICollection<ItemInfo<TItem>> collection { get { return list; } }

            #endregion
        }

        protected struct ItemInfo<TItem>
        {
            public readonly TItem Item;

            public readonly ValueAccessResult GetResult;

            public ItemInfo(TItem item, ValueAccessResult getResult)
            {
                Item = item;
                GetResult = getResult;
            }

            public IValueEntry<TCollection, TItem> CreateValueEntry(TCollection owner)
            {
                return new FixedValueEntry<TCollection, TItem>(owner, Item, GetResult);
            }
        }

        #region Enumeration

        private int enumeratedItemCount { get { return enumeratedItems.Count; } }
        private readonly ItemList<object> enumeratedItems = new ItemList<object>();

        private readonly ItemEnumerator<object> itemEnumerator = new ItemEnumerator<object>();

        protected virtual void UpdateItems(bool continueFromLastUpdate, int stopPageIndex)
        {
            int stopCount = (stopPageIndex + 1) * ItemCountPerPage;
            UpdateItems(enumeratedItems, TypedDrawingValue, itemEnumerator, continueFromLastUpdate, stopCount);
        }

        protected static void UpdateItems<TItem>(ItemList<TItem> items,
            TCollection collection, ItemEnumerator<TItem> enumerator, bool continueFromLastUpdate, int stopCount)
        {
            if (collection == null)
            {
                items.Clear();
                return;
            }

            if (!continueFromLastUpdate || enumerator == null)
            {
                enumerator.Reset(collection);
                items.Clear();
            }

            while (items.Count < stopCount && enumerator.MoveNext())
                items.Add(new ItemInfo<TItem>(enumerator.CurrentItem, enumerator.CurrentGetResult));
        }

        protected class ItemEnumerator<TItem>
        {
            public bool IsDone { get; private set; }

            public TItem CurrentItem { get; private set; }

            public ValueAccessResult CurrentGetResult { get; private set; }

            public void Reset(IEnumerable collection)
            {
                CurrentItem = default(TItem);
                CurrentGetResult = default(ValueAccessResult);

                var typedCollection = collection as IEnumerable<TItem>;
                if (typedCollection != null)
                    typedEnumerator = typedCollection.GetEnumerator();
                else
                    enumerator = collection.GetEnumerator();

                IsDone = false;
            }

            public bool MoveNext()
            {
                var h = ErrorRecordsLogHandler.Default;
                h.ClearRecords();
                Exception exception = null;

                using (new LogHandlerScope(h))
                {
                    try
                    {
                        if (typedEnumerator != null)
                        {
                            IsDone = !typedEnumerator.MoveNext();
                            CurrentItem = typedEnumerator.Current;
                        }
                        else
                        {
                            IsDone = !enumerator.MoveNext();
                            CurrentItem = (TItem)enumerator.Current;
                        }
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }
                CurrentGetResult = new ValueAccessResult(h.Records, exception);

                return !IsDone;
            }

            private IEnumerator enumerator;
            private IEnumerator<TItem> typedEnumerator;
        }

        #endregion Enumeration

        #endregion Items

        #region Update

        protected bool childrenNeedUpdate = true;

        protected override void SelfUpdate()
        {
            base.SelfUpdate();
            UpdateItems(false, CurrentPageIndex);
        }

        protected override void ChildrenUpdate()
        {
            if (!childrenAvailable)
                return;
            if (!childrenNeedUpdate)
                return;
            ChildrenUpdateImpl();
            childrenNeedUpdate = false;
        }

        protected override void ChildrenUpdateImpl() { impl.ChildrenUpdate(enumeratedItemCount); }

        #endregion Update

        #region GUI

        protected override void GUIImpl(GUIStyle style, params GUILayoutOption[] options)
        {
            base.GUIImpl(style ?? DIGUIStyles.CollectionBox, options);
        }

        protected override void FieldGUI()
        {
            NameGUI(false);
            GUILayout.FlexibleSpace();
            PageSelectGUI();
            ValueErrorGUI();
        }

        protected virtual void PageSelectGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUIEx.MinWidthLabelField(GetItemCountText());

                if (DIGUI.PageFlipButton(DIGUIContents.PrevButton))
                    CurrentPageIndex--;

                int pageCount = PageCount;
                int pageNo = pageCount == 0 ? 0 : CurrentPageIndex + 1;
                pageNo = EditorGUIEx.MinWidthDelayedIntField(pageNo);
                CurrentPageIndex = pageCount == 0 ? 0 : pageNo - 1;
                EditorGUIEx.MinWidthLabelField("/" + PageCount);

                if (DIGUI.PageFlipButton(DIGUIContents.NextButton) && ItemCountInCurrentPage == ItemCountPerPage)
                    CurrentPageIndex++;
            }
        }

        protected virtual string GetItemCountText() { return GetItemCountText(ItemCount, true); }

        protected static string GetItemCountText(int count, bool counted)
        {
            switch (count)
            {
                case 0:
                    return "Empty";
                case 1:
                    return "1 Item";
                default:
                    var text = count + " Items";
                    if (counted)
                        text += " Counted";
                    return text;
            }
        }

        #endregion GUI

        #region Page Flip

        protected override int BeforeCurrentPageIndexChange(int newPageIndex)
        {
            if (PageCount == 0)
                return 0;

            if (newPageIndex < 0)
                newPageIndex = 0;

            if (newPageIndex - CurrentPageIndex > 0) // Next Page
            {
                int startItemIndex = newPageIndex * ItemCountPerPage;
                if (startItemIndex > enumeratedItemCount - 1 && !itemEnumerator.IsDone)
                    UpdateItems(true, newPageIndex);

                int lastPageIndex = PageCount - 1;
                if (newPageIndex > lastPageIndex)
                    newPageIndex = lastPageIndex;
            }
            childrenNeedUpdate = true;

            return newPageIndex;
        }

        protected override void AfterCurrentPageIndexChange(int oldIndex)
        {
            base.AfterCurrentPageIndexChange(oldIndex);
            ChildrenUpdate();
        }

        #endregion Page Flip
    }

    public class EnumerableDrawer<TOwner, TCollection, TItem> : EnumerableDrawer<TOwner, TCollection>
        where TCollection : IEnumerable<TItem>
    {
        private readonly EnumerableDrawerImpl<TOwner, TCollection> impl;

        protected EnumerableDrawer()
        {
            impl = new EnumerableDrawerImpl<TOwner, TCollection>(this)
            {
                ItemTypeGetter = i => GetItemType(enumeratedItems, i),
                ValueEntryCreator = (i, c) => CreateItemValueEntry(enumeratedItems, i, TypedDrawingValue)
            };
        }

        #region Items

        public override int ItemCount { get { return enumeratedItems.Count; } }

        private int enumeratedItemCount { get { return enumeratedItems.Count; } }
        private readonly ItemList<TItem> enumeratedItems = new ItemList<TItem>();

        private readonly ItemEnumerator<TItem> itemEnumerator = new ItemEnumerator<TItem>();

        protected override void UpdateItems(bool continueFromLastUpdate, int stopPageIndex)
        {
            int stopCount = (stopPageIndex + 1) * ItemCountPerPage;
            UpdateItems(enumeratedItems, TypedDrawingValue, itemEnumerator, continueFromLastUpdate, stopCount);
        }

        #endregion Items

        protected override void ChildrenUpdateImpl() { impl.ChildrenUpdate(enumeratedItemCount); }
    }

    internal class EnumerableDrawerImpl<TOwner, TCollection>
    {
        public readonly PageableDrawer<TOwner, TCollection> This;

        public Func<int, Type> ItemTypeGetter;
        public Func<int, TCollection, IValueEntry> ValueEntryCreator;

        public EnumerableDrawerImpl(PageableDrawer<TOwner, TCollection> @this) { This = @this; }

        public void ChildrenUpdate(int itemCount)
        {
            int pageIndex = This.CurrentPageIndex, countPerPage = This.ItemCountPerPage;
            int countInPage = PageUtil.ItemCountInPage(itemCount, countPerPage, pageIndex);

            // Remove redundant child drawers.
            while (This.ChildCount > countInPage)
                This.LastChild.Parent = null;

            if (countInPage == 0)
                return;

            int startIndex = PageUtil.FirstItemIndexOfPage(countPerPage, pageIndex);
            int stopIndex = PageUtil.LastItemIndexOfPage(itemCount, countPerPage, pageIndex);
            for (int index = startIndex; index <= stopIndex; ++index)
            {
                if (index >= itemCount)
                    continue;

                var itemType = ItemTypeGetter(index);
                var childType = ValueDrawerTypes.GetOrBuild(typeof(TCollection), itemType);

                int childIndex = index - startIndex;
                var child = This.GetChild(childIndex) as ValueDrawer;
                if (child != null)
                {
                    if (child.GetType() != childType)
                    {
                        child = (ValueDrawer)Drawer.Create(childType);
                        This.ReplaceChild(childIndex, child);
                    }
                    else
                    {
                        //if (child.DrawingValue )
                    }
                }
                else
                {
                    child = (ValueDrawer)Drawer.Create(childType);
                    child.ChangeParent(This, childIndex);
                }
                child.Name = index.ToString();
                child.ValueEntry = ValueEntryCreator(index, This.TypedDrawingValue);
            }
        }
    }
}