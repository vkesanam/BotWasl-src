using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;
        static string TRANSFER_MESSAGE = "transfer to ";
        public string customerName;
        public string email;
        public string phone;
        public string complaint;
        public string language;
        // Live Assist custom channel data.
        public class LiveAssistChannelData
        {
            [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty("skill", NullValueHandling = NullValueHandling.Ignore)]
            public string Skill { get; set; }
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.ChannelId == "directline")
            {
                var laChannelData = message.GetChannelData<LiveAssistChannelData>();

                switch (laChannelData.Type)
                {
                    case "visitorContextData":
                        //process context data if required. This is the first message received so say hello.
                        //await context.PostAsync("Hi, I am an echo bot and will repeat everything you said.");
                        string Welcomemessage = "Glad to talk to you. Welcome to iBot - your Virtual Wasl Property Consultant.";
                        await context.PostAsync(Welcomemessage);

                        PromptDialog.Text(
                        context: context,
                        resume: ResumeLanguageOptions,
                        prompt: $@"Which language you want to prefer?{Environment.NewLine} 1. English {Environment.NewLine} 2. Arabic",
                        retry: "Sorry, I don't understand that.");

                        break;

                    case "systemMessage":
                        //react to system messages if required
                        break;

                    case "transferFailed":
                        //react to transfer failures if required
                        break;

                    case "otherAgentMessage":
                        //react to messages from a supervisor if required
                        break;

                    case "visitorMessage":
                        // Check for transfer message

                        if (message.Text.StartsWith(TRANSFER_MESSAGE))
                        {
                            var reply = context.MakeMessage();
                            var transferTo = message.Text.Substring(TRANSFER_MESSAGE.Length);

                            reply.ChannelData = new LiveAssistChannelData()
                            {
                                Type = "transfer",
                                Skill = "BotEscalation"
                            };

                            await context.PostAsync(reply);
                        }
                        else
                        {
                            await context.PostAsync("You said: " + message.Text);
                        }
                        break;

                    default:
                        await context.PostAsync("This is not a Live Assist message " + laChannelData.Type);
                        break;
                }
            }

            else if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }
        public async Task ResumeLanguageOptions(IDialogContext context, IAwaitable<string> argument)
        {
            PromptDialog.Text(
           context: context,
           resume: ServiceMessageReceivedAsyncService,
           prompt: $@"Which category you want to prefer?. {Environment.NewLine} 1. New Lease Enquiry {Environment.NewLine} 2. Customer Support",
           retry: "Sorry, I don't understand that.");
        }
        public async Task ServiceMessageReceivedAsyncService(IDialogContext context, IAwaitable<string> result)
        {
            var userFeedback = await result;

            if (userFeedback.Contains("New Lease Enquiry"))
            {
                PromptDialog.Text(
                context: context,
                resume: NameCategory,
                prompt: "May i know your Name please?",
                retry: "Sorry, I don't understand that.");
            }
            else if (userFeedback.Contains("Customer Support"))
            {
                PromptDialog.Text(
           context: context,
           resume: Customer,
           prompt: "May i know your name please?",
           retry: "Sorry, I don't understand that.");
            }
        }
        public async Task Customer(IDialogContext context, IAwaitable<string> result)
        {
            string response = await result;
            customerName = response;

            PromptDialog.Text(
                context: context,
                resume: CustomerMobileNumber,
                prompt: "What is your complaint/suggestion? ",
                retry: "Sorry, I don't understand that.");
        }
        public async Task CustomerMobileNumber(IDialogContext context, IAwaitable<string> result)
        {
            string response = await result;
            complaint = response;

            PromptDialog.Text(
                context: context,
                resume: CustomerEmail,
                prompt: "May I have your Mobile Number? ",
                retry: "Sorry, I don't understand that.");
        }
        public async Task CustomerEmail(IDialogContext context, IAwaitable<string> result)
        {
            string response = await result;
            phone = response;

            PromptDialog.Text(
               context: context,
               resume: FinalResultHandler,
               prompt: "May I have your Email ID? ",
               retry: "Sorry, I don't understand that.");
        }
        public virtual async Task FinalResultHandler(IDialogContext context, IAwaitable<string> argument)
        {
            string response = await argument;
            email = response;

            await context.PostAsync($@"Thank you for your interest, your request has been logged. Our customer service team will get back to you shortly.
                                    {Environment.NewLine}Your service request  summary:
                                    {Environment.NewLine}Complaint Title: {complaint},
                                    {Environment.NewLine}Customer Name: {customerName},
                                    {Environment.NewLine}Phone Number: {phone},
                                    {Environment.NewLine}Email: {email}");

            PromptDialog.Text(
          context: context,
          resume: AnythingElseHandler,
          prompt: "Is there anything else that I could help?",
          retry: "Sorry, I don't understand that.");

        }
        public async Task AnythingElseHandler(IDialogContext context, IAwaitable<string> argument)
        {


            var answer = await argument;
            if (answer.Contains("Yes") || answer.StartsWith("y"))
            {
                await GeneralGreeting(context, null);
            }
            else
            {
                string message = $"Thanks for using I Bot. Hope you have a great day!";
                await context.PostAsync(message);

                //var survey = context.MakeMessage();

                //var attachment = GetSurveyCard();
                //survey.Attachments.Add(attachment);

                //await context.PostAsync(survey);

                context.Done<string>("conversation ended.");
            }
        }
        public virtual async Task GeneralGreeting(IDialogContext context, IAwaitable<string> argument)
        {
            string message = $"Great! What else that can I help you?";
            await context.PostAsync(message);
            context.Wait(MessageReceivedAsync);
        }
        public async Task NameCategory(IDialogContext context, IAwaitable<string> result)
        {
            //PromptDialog.Choice(context, ServiceMessageReceivedAsyncHomeH,
            //          new List<string>()
            //          {
            //                "Residential",
            //                "Commercial"
            //          },
            //          "Are you looking for Residence/Commercial?");

            PromptDialog.Text(
           context: context,
           resume: ServiceMessageReceivedAsyncHomeH,
           prompt: "Are you looking for Resedence/Commercial?",
           retry: "Sorry, I don't understand that.");

        }
        public async Task ServiceMessageReceivedAsyncHomeH(IDialogContext context, IAwaitable<string> result)
        {
            var userFeedback = await result;

            if (userFeedback.Contains("Residential") || userFeedback.Contains("Commercial"))
            {
                PromptDialog.Text(
               context: context,
               resume: PropertyCity,
               prompt: "Great. I can show you active homes If you tell me a little bit, Which part of UAE are you looking in?",
               retry: "Sorry, I don't understand that.");
            }
        }
        public async Task PropertyCity(IDialogContext context, IAwaitable<string> result)
        {
            PromptDialog.Text(
                context: context,
                resume: PropertyBedrooms,
                prompt: "That is a great market. There are currently 306 listings on the market in that area. To narrow it down a bit, what price do you require?",
                retry: "Sorry, I don't understand that.");
        }
        public async Task PropertyBedrooms(IDialogContext context, IAwaitable<string> result)
        {
            //PromptDialog.Choice(context, ResumePropertyOptionsR,
            //        new List<string>()
            //        {
            //            "Single Family",
            //            "Studio",
            //            "1 Bed Room",
            //            "2 Bed Room",
            //            "3 Bed Room",
            //            "4 Bed Room",
            //            "5 Bed Room"
            //        },
            //        "There are 54 available. Which type are you interested in?");

            PromptDialog.Text(
           context: context,
           resume: ResumePropertyOptionsR,
           prompt: $@"There are 54 available. Which type are you interested in? {Environment.NewLine} Single Family {Environment.NewLine} Studio",
           retry: "Sorry, I don't understand that.");

        }
        public virtual async Task ResumePropertyOptionsR(IDialogContext context, IAwaitable<string> argument)
        {
            var selection = await argument;
            string result = selection;

            string message = "Great there are 25  " + result + " homes/properties that meet your needs. You can swipe to see each home/property.";
            await context.PostAsync(message);

            var reply = context.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetCardsAttachments();

            await context.PostAsync(reply);

            //context.Wait(this.MessageReceived);
            //PromptDialog.Confirm(
            //     context: context,
            //     resume: CustomerLeadCreation,
            //     prompt: "Would you like to get updates of new listings like these?",
            //     retry: "Sorry, I don't understand that.");

            PromptDialog.Text(
          context: context,
          resume: CustomerLeadCreation,
          prompt: "Would you like to get updates of new listings like these ? ",
          retry: "Sorry, I don't understand that.");
        }
        public async Task CustomerLeadCreation(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            if (answer.Contains("y") || answer.Contains("Yes"))
            {
                PromptDialog.Text(
               context: context,
               resume: CustomerLead,
               prompt: "May I have your email id? ",
               retry: "Sorry, I don't understand that.");
            }
            else
            {
                string message = $"Thanks for using I Bot. Hope you have a great day!";
                await context.PostAsync(message);
            }
        }
        public async Task CustomerLead(IDialogContext context, IAwaitable<string> result)
        {
            string response = await result;
            email = response;

            await context.PostAsync("Thank you for your interest. Our property consultant will get back to you shortly.");

            //PromptDialog.Confirm(
            //     context: context,
            //     resume: AnythingElseHandler,
            //     prompt: "Is there anything else that I could help?",
            //     retry: "Sorry, I don't understand that.");
            //CRMConnection.CreateLeadReg(customerName, email);

            PromptDialog.Text(
              context: context,
              resume: AnythingElseHandler,
              prompt: "Is there anything else that I could help?",
              retry: "Sorry, I don't understand that.");
        }
        private static IList<Attachment> GetCardsAttachments()
        {
            return new List<Attachment>()
            {
                GetHeroCard(
                    "Wasl Properties",
                    "AED 950000",
                    "Wasl Properties Group is a property development and management company based in Dubai, United Arab Emirates.",
                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/1.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
                GetHeroCard(
                     "Wasl Properties",
                    "AED 250000",
                    "Wasl Properties is a leading real estate master developer based in Dubai. Aligned to the leadership’s vision and overall development plans.",
                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/2.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
                GetHeroCard(
                     "Wasl Properties",
                    "AED 670000",
                    "Wasl Properties is a major contributor to realizing the vision of Dubai. A dynamic and forward-thinking organistion.",
                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/3.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
                GetHeroCard(
                     "Wasl Properties",
                    "AED 450009",
                    "Wasl Properties is committed to creating and managing renowned developments that provide distinctive and enriching lifestyles.",
                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/4.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
                  GetHeroCard(
                     "Wasl Properties",
                    "AED 450009",
                    "Riverside is part of Marasi Business Bay - an exciting new development by Wasl Properties, in the heart of Business Bay.",
                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/5.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
            };
        }

        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        private static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }
        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}