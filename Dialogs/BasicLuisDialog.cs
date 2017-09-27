using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Sample.LuisBot
{
  
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisAPIKey"])))
        {
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            Random rand = new Random();
            int pick = rand.Next(1, 10);
            if (pick <= 5)
                await context.PostAsync("Well hello there.");
            else await context.PostAsync("Hi. Welcome.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("You are welcome.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Goodbye")]
        public async Task Goodbye(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Bye. See you next time.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("HowAreYou")]
        public async Task HowAreYou(IDialogContext context, LuisResult result)
        {
            Random rand = new Random();
            int pick = rand.Next(1, 10);
            if (pick <= 5)
                await context.PostAsync("I am fine. Thank you.");
            else await context.PostAsync("Very well, thanks.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Softelligence.info")]
        public async Task SoftellInfo(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Softelligence is a leading provider of business software services that combines insightful domain knowledge from key industries with powerful engineering services and genuine customer orientation to deliver innovative custom software solutions. Softelligence stands out as a „provider of choice” for top 10 global players within Financial Services, Retail, Logistics and Information Technology.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Softelligence.location")]
        public async Task SoftellLocation(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Romania. Address: Nova Building, Strada Gara Herastrau 2-4, Bucuresti 020334");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Softelligence.customers")]
        public async Task SoftellCustomers(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Softelligence offers software solutions for industry leaders such as Allianz, Aon, ING, Reiffeisen, Mercedes Benz Financial, Honeywell, Henkel, or DB Schenker, as part of their digital journey. ");
            context.Wait(MessageReceived);
        }
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string responseString = string.Empty;

            var query = result;
            var builder = new UriBuilder($"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/538f47ef-7d40-4108-ae53-f14be0a58d44/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", "10022448e00340129eb24a360a779a4f");
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }

            JObject r = JObject.Parse(responseString);
            string cheat = (string)r.SelectToken("answers[0].answer");
            if (cheat != "No good match found in the KB")
            {
                await context.PostAsync(cheat);
            }
            else
            {
                await context.PostAsync("I don't have an answer for that.");
            }
            context.Wait(MessageReceived);
        }
    }
}
