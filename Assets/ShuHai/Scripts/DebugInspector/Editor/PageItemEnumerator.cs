using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class PageItemEnumerator<T>
    {
        public bool IsValid { get { return CurrentItemIndex >= 0; } }

        public PageItemEnumerator()
        {
            CurrentItemIndex = -1;
            ItemCountPerPage = Settings.DefaultItemCountPerPage;
        }

        #region Item Info

        public int EnumeratedItemCount { get; protected set; }

        public int CurrentItemIndex { get; protected set; }
        public int MaxItemIndex { get { return Settings.MaxEnumerateCount - 1; } }
        public int ItemIndexInPage { get { return CurrentItemIndex >= 0 ? CurrentItemIndex % ItemCountPerPage : -1; } }

        public int ItemCountPerPage
        {
            get { return itemCountPerPage; }
            set { itemCountPerPage = Mathf.Clamp(value, Settings.MinItemCountPerPage, Settings.MaxItemCountPerPage); }
        }

        public T CurrentItem
        {
            get
            {
                if (!itemAvailable)
                    throw new InvalidOperationException("Current item is not available.");
                return currentItem;
            }
        }

        protected T currentItem;

        #endregion Item Info

        #region Page Info

        public int CurrentPageIndex { get { return GetPageIndex(CurrentItemIndex); } }
        public int MaxPageIndex { get { return GetPageIndex(MaxItemIndex); } }

        public int TargetPageIndex
        {
            get { return targetPageIndex; }
            set { targetPageIndex = Mathf.Clamp(value, 0, MaxPageIndex); }
        }

        private int itemCountPerPage;
        private int targetPageIndex;

        private int GetPageIndex(int itemIndex) { return itemIndex >= 0 ? itemIndex / ItemCountPerPage : -1; }

        #endregion Page Info

        #region Enumeration Control

        public bool CanMoveNext { get; protected set; }

        public virtual void Reset(IEnumerable enumerable, int targetPageIndex, int itemCountPerPage)
        {
            Ensure.Argument.NotNull(enumerable, "enumerable");
            //if (enumerable.GetType().IsGenericType)
            //    Ensure.Argument("enumerable").Satisfy(enumerable is IEnumerable<T>, "Invalid item type.");

            TargetPageIndex = targetPageIndex;
            ItemCountPerPage = itemCountPerPage;

            CurrentItemIndex = -1;
            currentItem = default(T);

            EnumeratedItemCount = 0;
            enumerator = enumerable.GetEnumerator();
            itemAvailable = false;
            CanMoveNext = true;

            // Move enumerator pointer to the start of target page.
            int indexBeforeStart = TargetPageIndex * ItemCountPerPage - 1;
            while (CurrentItemIndex < indexBeforeStart)
            {
                if (!PerformMoveNext())
                    break;
            }
        }

        public bool MoveNext()
        {
            if (!CanMoveNext)
                return false;
            return PerformMoveNext();
        }

        protected bool itemAvailable;

        protected IEnumerator enumerator;

        protected bool PerformMoveNext()
        {
            ClearCurrentErrorRecords();

            var logHandler = HandleErrorLogs ? errorRecordsLogHandler : DebugEx.UnityLogHandler;
            using (new LogHandlerScope(logHandler))
            {
                if (CatchException)
                {
                    try
                    {
                        itemAvailable = PerformMoveNextImpl();
                    }
                    catch (Exception e)
                    {
                        CurrentException = e;
                    }
                }
                else
                {
                    itemAvailable = PerformMoveNextImpl();
                }

                if (itemAvailable)
                    EnumeratedItemCount++;
            }

            return itemAvailable;
        }

        protected virtual bool PerformMoveNextImpl()
        {
            bool result;
            var enumeratorOfT = enumerator as IEnumerator<T>;
            if (enumeratorOfT != null)
            {
                result = enumeratorOfT.MoveNext();
                currentItem = result ? enumeratorOfT.Current : default(T);
            }
            else
            {
                result = enumerator.MoveNext();
                currentItem = result ? (T)enumerator.Current : default(T);
            }

            CurrentItemIndex = result ? CurrentItemIndex + 1 : -1;

            int nextMovePageIndex = GetPageIndex(CurrentItemIndex + 1);
            CanMoveNext = result && nextMovePageIndex <= TargetPageIndex;

            return result;
        }

        #endregion Enumeration Control

        #region Error Handling

        public bool HandleErrorLogs = true;
        public bool CatchException = true;

        public IEnumerable<LogInfo> CurrentErrorLogs { get { return errorRecordsLogHandler.Records; } }

        public Exception CurrentException { get; protected set; }

        public ValueAccessResult CreateCurrentItemGetResult()
        {
            return new ValueAccessResult(CurrentErrorLogs, CurrentException);
        }

        private readonly ErrorRecordsLogHandler errorRecordsLogHandler = new ErrorRecordsLogHandler();

        private void ClearCurrentErrorRecords()
        {
            errorRecordsLogHandler.ClearRecords();
            CurrentException = null;
        }

        #endregion Error Handling
    }
}