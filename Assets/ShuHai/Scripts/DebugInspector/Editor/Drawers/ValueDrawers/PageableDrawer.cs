using System;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public abstract class PageableDrawer<TOwner, TValue> : ValueDrawer<TOwner, TValue>
    {
        #region Parameters

        public event Action<int> ItemCountPerPageChanged;
        public event Action<int> CurrentPageIndexChanged;

        public int ItemCountPerPage
        {
            get { return itemCountPerPage; }
            set
            {
                value = Mathf.Clamp(value, Settings.MinItemCountPerPage, Settings.MaxItemCountPerPage);
                PropertyUtil.SetValue(ref itemCountPerPage, value, OnItemCountPerPageChanged, ItemCountPerPageChanged);
            }
        }

        public int CurrentPageIndex
        {
            get { return currentPageIndex; }
            set
            {
                if (value == currentPageIndex)
                    return;

                value = BeforeCurrentPageIndexChange(value);
                int oldValue = currentPageIndex;
                currentPageIndex = value;
                AfterCurrentPageIndexChange(oldValue);

                CurrentPageIndexChanged.NPInvoke(oldValue);
            }
        }

        protected virtual void OnItemCountPerPageChanged(int oldCount) { }

        protected virtual int BeforeCurrentPageIndexChange(int newIndex)
        {
            return Mathf.Clamp(newIndex, 0, PageCount - 1);
        }

        protected virtual void AfterCurrentPageIndexChange(int oldIndex) { }

        private int itemCountPerPage = Settings.ItemCountPerPage;
        private int currentPageIndex;

        #endregion Parameters

        public abstract int ItemCount { get; }

        public int PageCount { get { return PageUtil.PageCount(ItemCount, ItemCountPerPage); } }

        public int ItemCountInCurrentPage { get { return ItemCountInPage(CurrentPageIndex); } }

        public int FirstItemIndexOfCurrentPage { get { return FirstItemIndexOfPage(CurrentPageIndex); } }

        public int LastItemIndexOfCurrentPage { get { return FirstItemIndexOfCurrentPage + ItemCountInCurrentPage; } }

        /// <summary>
        ///     Get a value indicating whether the specific index is a valid page index for current instance.
        /// </summary>
        /// <param name="pageIndex">The page index to check.</param>
        /// <remarks>
        ///     Note that this method always returns <see langword="false" /> if <see cref="PageCount" /> is 0.
        /// </remarks>
        public bool IsValidPage(int pageIndex) { return PageCount != 0 && pageIndex >= 0 && pageIndex < PageCount; }

        public int ItemCountInPage(int pageIndex)
        {
            return PageUtil.ItemCountInPage(ItemCount, ItemCountPerPage, pageIndex);
        }

        public int FirstItemIndexOfPage(int pageIndex)
        {
            return PageUtil.FirstItemIndexOfPage(ItemCountPerPage, pageIndex);
        }

        public int LastItemIndexOfPage(int pageIndex)
        {
            return PageUtil.LastItemIndexOfPage(ItemCount, ItemCountPerPage, pageIndex);
        }

        public bool IsItemInCurrentPage(int itemIndex)
        {
            return itemIndex >= FirstItemIndexOfCurrentPage && itemIndex <= LastItemIndexOfCurrentPage;
        }

        public bool IsLastPage(int pageIndex) { return PageCount != 0 && pageIndex == PageCount - 1; }
    }
}