using System.Collections.Generic;

namespace SendMassMailToLargePopulations
{
    public class SmtpMessageModel
    {
        public bool IsHtml { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public List<string> Recipients { get; set; }
        public bool ProcessInlineImages { get; set; }
        public bool SendViaBCC { get; set; }
    }
}
