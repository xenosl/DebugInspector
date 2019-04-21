using System.Collections;
using System.Collections.Generic;

namespace ShuHai.DebugInspector.Editor
{
    public class CollectionDrawer<TOwner, TCollection> : EnumerableDrawer<TOwner, TCollection>
        where TCollection : ICollection
    {
        protected CollectionDrawer() { itemCountGetter = new ValueAccesHelper<int>(() => TypedDrawingValue.Count); }

        #region Item Count

        public override int ItemCount { get { return itemCount; } }

        private int itemCount;

        protected ValueAccessResult? itemCountGetResult;

        protected readonly ValueAccesHelper<int> itemCountGetter;

        protected virtual void UpdateItemCount()
        {
            var collection = TypedDrawingValue;
            itemCountGetResult = typeof(TCollection).IsValueType || collection != null
                ? itemCountGetter.GetValue(out itemCount) : (ValueAccessResult?)null;
        }

        protected override string GetItemCountText() { return GetItemCountText(ItemCount, false); }

        #endregion Item Count

        protected override void SelfUpdate()
        {
            base.SelfUpdate();
            UpdateItemCount();
        }
    }

    public class CollectionDrawer<TOwner, TCollection, TItem> : EnumerableDrawer<TOwner, TCollection, TItem>
        where TCollection : ICollection<TItem>
    {
        protected CollectionDrawer() { itemCountGetter = new ValueAccesHelper<int>(() => TypedDrawingValue.Count); }

        #region Item Count

        public override int ItemCount { get { return itemCount; } }

        private int itemCount;

        protected ValueAccessResult? itemCountGetResult;

        protected readonly ValueAccesHelper<int> itemCountGetter;

        protected virtual void UpdateItemCount()
        {
            var collection = TypedDrawingValue;
            itemCountGetResult = typeof(TCollection).IsValueType || collection != null
                ? itemCountGetter.GetValue(out itemCount) : (ValueAccessResult?)null;
        }

        protected override string GetItemCountText() { return GetItemCountText(ItemCount, false); }

        #endregion Item Count

        protected override void SelfUpdate()
        {
            base.SelfUpdate();
            UpdateItemCount();
        }
    }
}