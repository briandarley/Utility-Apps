using System;

namespace DeleteMassMailFromInbox.Models
{
    public class CampaignRecipient
    {
        public int CampaignId { get; set; }
        public string Uid { get; set; }
        public int Pid { get; set; }
        public string Mail { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
