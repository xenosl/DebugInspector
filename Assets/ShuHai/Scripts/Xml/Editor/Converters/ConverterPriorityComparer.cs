using System;
using System.Collections.Generic;

namespace ShuHai.Xml
{
    internal struct ConverterPriorityComparer : IComparer<XConverter>, IDisposable
    {
        public readonly Type ConvertType;

        #region Disposable

        public ConverterPriorityComparer(Type convertType)
        {
            ConvertType = convertType;
            disposed = false;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
        }

        private bool disposed;

        #endregion

        #region Compare

        public int Compare(XConverter l, XConverter r)
        {
            if (l == null && r == null)
                return 0;
            if (l == null) // && r != null
                return -1;
            if (r == null) // && l != null
                return 1;

            if (ConvertType != null)
            {
                int? lp = l.GetPriority(ConvertType), rp = r.GetPriority(ConvertType);
                if (lp.HasValue && rp.HasValue)
                    return lp.Value.CompareTo(rp.Value);
                if (lp.HasValue) // && !rp.HasValue
                    return 1;
                if (rp.HasValue) // && !lp.HasValue
                    return -1;
            }

            return 0;
        }

        #endregion
    }
}