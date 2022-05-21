using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Core;

namespace SendMassMailToLargePopulations
{
    public class ProcessEmployees
    {
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Program));


        public void DoWork()
        {
            SendMassMail(new List<string>() {"bdarley@ad.unc.edu", "kate_hash@unc.edu" });
            return;
            var list = GetDistributionGroupMembers().Where(c => c.Employee).ToList();

            var massPageSize = 5000;
            var totalCount = list.Count;
            var massPageCount = (list.Count / massPageSize) + ((list.Count % massPageSize) > 1 ? 1 : 0);

            var auditCount = 0;
            var count = 0;
            for (var massPage = 0; massPage < massPageCount; massPage++)
            {
                var massGroup = list.Skip(massPage * massPageSize).Take(massPageSize).ToList();

                var groupPageSize = 100;
                var groupPageCount = (massGroup.Count / groupPageSize) + ((massGroup.Count % groupPageSize) > 1 ? 1 : 0);

                for (var groupPage = 0; groupPage < groupPageCount; groupPage++)
                {
                    var bccGroup = massGroup.Skip(groupPage * groupPageSize).Take(groupPageSize).ToList();
                    try
                    {
                        SendMassMail(bccGroup.Select(c => c.Uid + "@ad.unc.edu").ToList());
                        if (count == 0)
                        {
                            SendMassMail(new List<string> { "bdarley@ad.unc.edu" });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    count++;
                }

                auditCount += massGroup.Count;
                //Wait one hour before sending next group
                Thread.Sleep(1000 * 60 * 30);
            }
            //var massGroup = list.

            Debug.Assert(auditCount == totalCount);
        }

        private static string fileContents = "";
        private Task SendMassMail(List<string> bccList)
        {
            if (string.IsNullOrEmpty(fileContents))
            {
                fileContents = System.IO.File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "Fall 2018 Back to Campus Note.html"));
            }

            var message = new SmtpMessageModel()
            {
                Body = fileContents,
                From = "no_reply@email.unc.edu",
                IsHtml = true,
                ProcessInlineImages = false,
                Recipients = bccList,
                SendViaBCC = true,
                Subject = "[FORMAL NOTICE] 2-Step Verification Protects Our Campus"
            };


            var proxy = new ProxyClient.EndPoint(Log) { BaseAddress = "http://its-idm-tst-web.adtest.unc.edu/services/Messaging.Smtp.API/v1/" };
            Task.WaitAll(Task.Run(async () => await proxy.Post("emails", message)));
            return Task.FromResult(0);

        }


        private List<DistributionGroupMember> GetDistributionGroupMembers()
        {
            using (var cn = new DbContext())
            {
                var list = cn.DistributionGroupMembers.OrderBy(c => c.Uid)
                    .Where(c => c.Employee)
                    .ToList();
                return list;
            }
        }


    }
}
