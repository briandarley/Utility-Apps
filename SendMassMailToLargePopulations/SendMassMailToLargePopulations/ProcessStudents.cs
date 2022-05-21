using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Core;
using Newtonsoft.Json;

namespace SendMassMailToLargePopulations
{
    public class ProcessStudents
    {
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Program));


        public void DoWork()
        {
            var getPersons = GetCustomGroup();
            SendMassMail(new List<string>() {"bdarley@ad.unc.edu"});
            return;
            //var list = GetDistributionGroupMembers().Where(c=> c.Student).ToList();

            var massPageSize = 5000;
            var totalCount = getPersons.Count();
            
            var massPageCount = (totalCount / massPageSize) + ((totalCount % massPageSize) > 1 ? 1 : 0);

            var auditCount = 0;
            var count = 0;
            for (var massPage = 0; massPage < massPageCount; massPage++)
            {
                var massGroup = getPersons.Skip(massPage * massPageSize).Take(massPageSize).ToList();

                var groupPageSize = 100;
                var groupPageCount = (massGroup.Count / groupPageSize) + ((massGroup.Count % groupPageSize) > 1 ? 1 : 0);

                for (var groupPage = 0; groupPage < groupPageCount; groupPage++)
                {
                    var bccGroup = massGroup.Skip(groupPage * groupPageSize).Take(groupPageSize).ToList();
                    try
                    {
                        SendMassMail(bccGroup.Select(c => c).ToList());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    count++;
                }

                auditCount += massGroup.Count;
                //Wait 1 min before sending next group
                Thread.Sleep(1000 * 60 * 1);
            }
            //var massGroup = list.

            Debug.Assert(auditCount == totalCount);
        }

        private static string fileContents = "";
        private Task SendMassMail(List<string> bccList)
        {
            if (string.IsNullOrEmpty(fileContents))
            {
                fileContents = System.IO.File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "MFA Outage.html"));
            }

            var message = new SmtpMessageModel()
            {
                Body = fileContents,
                From = "no_reply@email.unc.edu",
                IsHtml = true,
                ProcessInlineImages = false,
                Recipients = bccList,
                SendViaBCC = true,
                Subject = "[FORMAL] 2-Step Verification Outage for Office 365"
            };


            var proxy = new ProxyClient.EndPoint(Log) { BaseAddress = "http://its-idmprd-app.ad.unc.edu/services/Messaging.Smtp.API/v1/" };
            Task.WaitAll(Task.Run(async () => await proxy.Post("emails", message)));
            return Task.FromResult(0);

        }


        private List<DistributionGroupMember> GetDistributionGroupMembers()
        {
            using (var cn = new DbContext())
            {
                var list = cn.DistributionGroupMembers.OrderBy(c => c.Uid)
                    .ToList();
                return list;
            }
        }

        private IEnumerable<string> GetCustomGroup()
        {
            var contents = System.IO.File.ReadAllText(@"C:\Projects\Utility Apps\SendMassMailToLargePopulations\SendMassMailToLargePopulations\EmailList.json");
            var items = JsonConvert.DeserializeObject<List<string>>(contents);
            return items;
        }
    }
}
