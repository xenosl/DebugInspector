using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    /// <summary>
    ///     Converter that converts objects implemented <see cref="ICollection{T}" /> <see langword="interface" />.
    /// </summary>
    public class GenericCollectionConverter : XConverter
    {
        #region Priority

        protected override int? GetPriorityImpl(Type type)
        {
            if (type.IsInterface && IsGenericCollection(type))
                return 1;
            return type.GetInterfaces().Any(IsGenericCollection) ? 1 : (int?)null;
        }

        protected static bool IsGenericCollection(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        #endregion

        protected const string ItemName = "Item";

        protected override void PopulateElementValue(
            XElement element, Type declareType, object obj, XConvertSettings settings) { }

        protected override void PopulateElementChildren(
            XElement element, Type declareType, object obj, XConvertSettings settings)
        {
            if (obj == null)
                return;

            var collectionProxy = CreateCollectionProxy(obj);
            var itemDeclareType = collectionProxy.ItemType;

            foreach (var item in collectionProxy)
            {
                var itemActualType = item != null ? item.GetType() : null;
                var usingItemType = itemActualType ?? itemDeclareType;

                var itemElement = XConvert.ToElement(ItemName, item, settings);
                if (itemDeclareType != usingItemType)
                    itemElement.Add(XConvertUtil.CreateTypeAttribute(usingItemType, settings.AssemblyNameStyle));
                element.Add(itemElement);
            }
        }

        protected override void PopulateObjectImpl(object obj, XElement element, XConvertSettings settings)
        {
            var collectionProxy = CreateCollectionProxy(obj);
            foreach (var itemElement in element.Elements(ItemName))
            {
                var itemType = XConvertUtil.ParseType(itemElement) ?? collectionProxy.ItemType;
                var item = XConvert.ToObject(itemType, itemElement, settings);
                collectionProxy.Add(item);
            }
        }

        #region Collection Proxy

        protected static CollectionProxy CreateCollectionProxy(object underlayingCollection)
        {
            var type = underlayingCollection.GetType();
            var collectionType = IsGenericCollection(type) ? type
                : type.GetInterfaces().FirstOrDefault(IsGenericCollection);
            var itemType = collectionType.GetGenericArguments()[0];
            var proxyType = typeof(CollectionProxy<>).MakeGenericType(itemType);
            return (CollectionProxy)Activator.CreateInstance(proxyType, underlayingCollection);
        }

        protected abstract class CollectionProxy : IEnumerable
        {
            public abstract Type ItemType { get; }

            public abstract void Add(object item);
            public abstract bool Remvoe(object item);

            public abstract IEnumerator GetEnumerator();

            protected CollectionProxy(object underlayingCollection)
            {
                this.underlayingCollection = underlayingCollection;
            }

            protected readonly object underlayingCollection;
        }

        protected class CollectionProxy<T> : CollectionProxy
        {
            public override Type ItemType { get { return typeof(T); } }

            public CollectionProxy(ICollection<T> underlayingCollection) : base(underlayingCollection) { }

            public override void Add(object item) { items.Add((T)item); }
            public override bool Remvoe(object item) { return items.Remove((T)item); }

            public override IEnumerator GetEnumerator() { return items.GetEnumerator(); }

            private ICollection<T> items { get { return (ICollection<T>)underlayingCollection; } }
        }

        #endregion
    }
}