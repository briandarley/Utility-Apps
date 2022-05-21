using System.Collections.Generic;

namespace DeleteMassMailFromInbox.Models
{
    public class PagingEntity<T>
    {
        public int TotalRecords { get; set; }
        public int Index { get; set; }
        public int PageSize { get; set; }
        public List<T> Entities { get; set; }

    }
}
