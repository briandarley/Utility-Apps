using System;
using System.Collections.Generic;
using System.Text;

namespace DeleteMassMailFromInbox.Models
{
    public class CampaignModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Author { get; set; }

        public string Priority { get; set; }
    }
}
