using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DeleteMassMailFromInbox.Models;
using Newtonsoft.Json;

namespace DeleteMassMailFromInbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var prgm = new Program();


            var massMailItemRequest = prgm.GetMassMailCampaignDetail(763);
            var campaign = massMailItemRequest.Result;

            var mailRecipientsRequest = prgm.GetMassMailRecipients(763);
            mailRecipientsRequest.Wait();
            var recipients = mailRecipientsRequest.Result.ToList();

            var request = prgm.DeleteMassMailCampaignMessages(campaign, recipients);

            request.Wait();
            Console.ReadLine();
        }

        private async Task DeleteMassMailCampaignMessages2(CampaignModel campaign, List<CampaignRecipient> recipients)
        {//@email.unc.edu
            var allowedDomains = new List<string> { "@unc.edu", "@med.unc.edu", "@live.unc.edu", "@med.unc.edu" };
            var disallowedDomains = new List<string> { "@kenan-flagler.unc.edu" };
            //@kenan-flagler.unc.edu
            var client = new HttpClient();
            //Parallel.ForEach(recipients, async recipient =>
            //{
            var doWork = false;
            var lastUser = "abohmer@live.unc.edu";
            foreach (var recipient in recipients)
            {

                if (recipient.Mail == lastUser)
                {
                    doWork = true;
                }

                if (!doWork)
                {
                    continue;
                }

                if (recipient.Mail == lastUser)
                {
                    continue;

                }
                if (disallowedDomains.Any(c => recipient.Mail.EndsWith(c)))
                {
                    continue;

                }

                var criteria = new
                {
                    UserPrincipalName = recipient,
                    Subject = campaign.Priority == "Informational" ? $"[INFORMATIONAL] {campaign.Subject}" : $"[Formal Message] {campaign.Subject}",
                    SentDateFrom = recipient.CreateDate.AddMinutes(-1),
                    SentDateThru = recipient.CreateDate.AddMinutes(5),
                    From = "no_reply@email.unc.edu"

                };
                var serialized = criteria.ToQueryParams();
                var request = await client.GetAsync($"http://its-idmprd-app.ad.unc.edu/services/microsoft-graph.api/v1/Users/{recipient.Mail}/messages?{serialized}");
                //no_reply@email.unc.edu
                var content = await request.Content.ReadAsStringAsync();
                if (!request.IsSuccessStatusCode)
                {

                }

                var list = JsonConvert.DeserializeObject<List<UserMessageModel>>(content);

                if (list.Any())
                {
                    var messageId = list.Single().Id;
                    request = await client.DeleteAsync($"http://its-idmprd-app.ad.unc.edu/services/microsoft-graph.api/v1/Users/{recipient.Mail}/messages/{messageId}");
                    if (!request.IsSuccessStatusCode)
                    {

                    }
                    else
                    {
                        Console.WriteLine($"Successfully removed message from user {recipient.Mail}");
                    }
                }
                else
                {
                    Console.WriteLine($"Mail not present on user {recipient.Mail}");
                }

            }
            //});




        }

        private async Task<List<CampaignRecipient>> DeleteMassMailCampaignMessages(CampaignModel campaign, List<CampaignRecipient> recipients)
        {//@email.unc.edu
            var allowedDomains = new List<string> { "@unc.edu", "@med.unc.edu", "@live.unc.edu", "@med.unc.edu" };
            var disallowedDomains = new List<string> { "@kenan-flagler.unc.edu", "@cs.unc.edu", "@unch.unc.edu", "@duke.edu", "@unchealth.unc.edu", "@gmail.com", "@dentistry.unc.edu" };
            //@kenan-flagler.unc.edu
            var client = new HttpClient();
            var errorList = new List<CampaignRecipient>();
            //new ParallelOptions { MaxDegreeOfParallelism = 4 },
            Parallel.ForEach(recipients, async recipient =>
            {

                try
                {
                    if (disallowedDomains.Any(c => recipient.Mail.EndsWith(c)))
                    {
                        return;

                    }

                    var criteria = new
                    {
                        UserPrincipalName = recipient,
                        Subject = campaign.Priority == "Informational" ? $"[INFORMATIONAL] {campaign.Subject}" : $"[Formal Message] {campaign.Subject}",
                        SentDateFrom = recipient.CreateDate.AddMinutes(-1),
                        SentDateThru = recipient.CreateDate.AddMinutes(5),
                        From = "no_reply@email.unc.edu"

                    };
                    var serialized = criteria.ToQueryParams();
                    var request = await client.GetAsync($"http://its-idmprd-app.ad.unc.edu/services/microsoft-graph.api/v1/Users/{recipient.Mail}/messages?{serialized}");
                    //no_reply@email.unc.edu
                    var content = await request.Content.ReadAsStringAsync();
                    if (!request.IsSuccessStatusCode)
                    {
                        //request = await client.GetAsync($"http://its-idmprd-app.ad.unc.edu/services/microsoft-graph.api/v1/Users/{recipient.Mail}/messages?{serialized}");
                        Console.WriteLine($"Error with {recipient.Mail}");
                        errorList.Add(recipient);
                        return;
                    }

                    var list = JsonConvert.DeserializeObject<List<UserMessageModel>>(content);

                    if (list.Any())
                    {
                        var messageId = list.Single().Id;
                        request = await client.DeleteAsync($"http://its-idmprd-app.ad.unc.edu/services/microsoft-graph.api/v1/Users/{recipient.Mail}/messages/{messageId}");
                        if (!request.IsSuccessStatusCode)
                        {
                            errorList.Add(recipient);
                            Console.WriteLine($"Error with {recipient.Mail}");
                        }
                        else
                        {
                            Console.WriteLine($"Successfully removed message from user {recipient.Mail}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Mail not present on user {recipient.Mail}");
                    }
                }
                catch (Exception ex)
                {
                    errorList.Add(recipient);
                    //Console.WriteLine(ex);
                    Console.WriteLine($"Error with {recipient.Mail}");
                    
                }
            
            

            
                

            
            });


            return errorList;

        }

        private async Task<CampaignModel> GetMassMailCampaignDetail(int campaignId)
        {
            var client = new HttpClient();


            var request = await client.GetAsync($"http://its-idmprd-app.ad.unc.edu/Services/MassMail/Dal.MassMail.API/v1/Campaigns/{campaignId}");
            var content = await request.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<CampaignModel>(content);
            return response;
        }

        async Task<List<CampaignRecipient>> GetMassMailRecipients(int campaignId)
        {
            var client = new HttpClient();


            var request = await client.GetAsync($"http://its-idmprd-app.ad.unc.edu/Services/MassMail/Dal.MassMail.API/v1/CampaignRecipients/{campaignId}");
            var content = await request.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<CampaignRecipient>>(content);

            return list;
        }

    }
}
