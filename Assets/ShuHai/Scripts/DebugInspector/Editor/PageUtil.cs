namespace ShuHai.DebugInspector.Editor
{
    public static class PageUtil
    {
        public static int PageCount(int totalItemCount, int itemCountPerPage)
        {
            int count = totalItemCount / itemCountPerPage;
            if (totalItemCount % itemCountPerPage > 0)
                count++;
            return count;
        }

        public static int ItemCountInPage(int totalItemCount, int itemCountPerPage, int pageIndex)
        {
            int pageCount = PageCount(totalItemCount, itemCountPerPage);
            if (pageCount == 0)
                return 0;

            if (pageIndex == pageCount - 1)
            {
                int count = totalItemCount % itemCountPerPage;
                return count == 0 ? itemCountPerPage : count;
            }

            if (pageIndex >= pageCount)
                return 0;

            return itemCountPerPage;
        }

        public static int FirstItemIndexOfPage(int itemCountPerPage, int pageIndex)
        {
            return pageIndex * itemCountPerPage;
        }

        public static int LastItemIndexOfPage(int totalItemCount, int itemCountPerPage, int pageIndex)
        {
            return FirstItemIndexOfPage(itemCountPerPage, pageIndex) +
                ItemCountInPage(totalItemCount, itemCountPerPage, pageIndex) - 1;
        }
    }
}