using System.Collections.Generic;

namespace BaseNodeExtensions
{
    public static class Extensions
    {
        public static void SortByCreation(this List<BaseNode> nodeList)
        {
            nodeList.Sort(new BaseNodeComparer());
        }
    }

    public class BaseNodeComparer : IComparer<BaseNode>
    {
        public int Compare(BaseNode a, BaseNode b)  
        {
            if(a.creationId < b.creationId)
            {
                return -1;
            }

            if (a.creationId > b.creationId)
            {
                return 1;
            }

            return 0;
        }
    }
}
