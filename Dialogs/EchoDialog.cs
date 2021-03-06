﻿using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Linq;
using Cafex.LiveAssist.Bot;
using System.Timers;
using Microsoft.Bot.Builder.ConnectorEx;
using SimpleEchoBot.Dialogs;

#region General Code
//namespace Microsoft.Bot.Sample.SimpleEchoBot
//{
//    [Serializable]
//    public class EchoDialog : IDialog<object>
//    {
//        protected int count = 1;
//        static string TRANSFER_MESSAGE = "transfer to ";
//        public string customerName;
//        public string email;
//        public string phone;
//        public string complaint;
//        public string language;
//        static string host = "https://api.microsofttranslator.com";
//        static string path = "/V2/Http.svc/Translate";

//        // NOTE: Replace this example key with a valid subscription key.
//        static string key = "830fda84bdce4810a78cc508745a2f9e";

//        // Live Assist custom channel data.
//        public class LiveAssistChannelData
//        {
//            [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
//            public string Type { get; set; }

//            [JsonProperty("skill", NullValueHandling = NullValueHandling.Ignore)]
//            public string Skill { get; set; }
//        }

//        public async Task StartAsync(IDialogContext context)
//        {
//            context.Wait(MessageReceivedAsync);
//        }

//        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
//        {
//            var message = await argument;

//            if (message.ChannelId == "directline")
//            {
//                var laChannelData = message.GetChannelData<LiveAssistChannelData>();

//                switch (laChannelData.Type)
//                {
//                    case "visitorContextData":
//                        //process context data if required. This is the first message received so say hello.
//                        //await context.PostAsync("Hi, I am an echo bot and will repeat everything you said.");
//                        string Welcomemessage = "Glad to talk to you. Welcome to iBot - your Virtual Wasl Property Consultant.";
//                        await context.PostAsync(Welcomemessage);

//                        PromptDialog.Text(
//                        context: context,
//                        resume: ResumeLanguageOptions,
//                        prompt: $@"Which language you want to prefer?{Environment.NewLine} 1. English {Environment.NewLine} 2. Arabic",
//                        retry: "Sorry, I don't understand that.");

//                        break;

//                    case "systemMessage":
//                        //react to system messages if required
//                        break;

//                    case "transferFailed":
//                        //react to transfer failures if required
//                        break;

//                    case "otherAgentMessage":
//                        //react to messages from a supervisor if required
//                        break;

//                    case "visitorMessage":
//                        // Check for transfer message

//                        if (message.Text.StartsWith(TRANSFER_MESSAGE))
//                        {
//                            var reply = context.MakeMessage();
//                            var transferTo = message.Text.Substring(TRANSFER_MESSAGE.Length);

//                            reply.ChannelData = new LiveAssistChannelData()
//                            {
//                                Type = "transfer",
//                                Skill = "BotEscalation"
//                            };

//                            await context.PostAsync(reply);
//                        }
//                        else if (message.Text.StartsWith("hi") || message.Text.StartsWith("hello") || message.Text.StartsWith("Hi"))
//                        {
//                            if (customerName == null)
//                            {
//                                string Welcomemessage2 = "Glad to talk to you. Welcome to iBot - your Virtual Wasl Property Consultant.";
//                                await context.PostAsync(Welcomemessage2);

//                                PromptDialog.Text(
//                                context: context,
//                                resume: ResumeLanguageOptions,
//                                prompt: $@"Which language you want to prefer?{Environment.NewLine} 1. English {Environment.NewLine} 2. Arabic",
//                                retry: "Sorry, I don't understand that.");
//                            }
//                            else
//                            {
//                                string message23 = "Tell me " + customerName + ". How i can help you?";
//                                await context.PostAsync(message23);
//                                context.Wait(MessageReceivedAsync);
//                            }
//                        }
//                        else if (message.Text.Contains("issue") || message.Text.Contains("problem"))
//                        {
//                            PromptDialog.Text(
//                               context: context,
//                               resume: CustomerRepeatChecking,
//                               prompt: "May i know your mobile number for verification purpose?",
//                               retry: "Sorry, I don't understand that.");
//                        }
//                        else if (message.Text.Contains("sell") || message.Text.Contains("buy") || message.Text.Contains("property"))
//                        {
//                            PromptDialog.Text(
//                               context: context,
//                               resume: ResumeLanguageOptions,
//                               prompt: $@"Which language you want to prefer?{Environment.NewLine} 1. English {Environment.NewLine} 2. Arabic",
//                               retry: "Sorry, I don't understand that.");
//                        }
//                        else
//                        {
//                            await context.PostAsync("You said: " + message.Text);
//                        }
//                        break;

//                    default:
//                        await context.PostAsync("This is not a Live Assist message " + laChannelData.Type);
//                        break;
//                }
//            }

//            else if (message.Text == "reset")
//            {
//                PromptDialog.Confirm(
//                    context,
//                    AfterResetAsync,
//                    "Are you sure you want to reset the count?",
//                    "Didn't get that!",
//                    promptStyle: PromptStyle.Auto);
//            }
//            else
//            {
//                await context.PostAsync($"{this.count++}: You said {message.Text}");
//                context.Wait(MessageReceivedAsync);
//            }
//        }
//        public async Task CustomerRepeatChecking(IDialogContext context, IAwaitable<string> argument)
//        {
//            if(phone!=null)
//            {

//                     PromptDialog.Text(
//           context: context,
//           resume: RepeatFinal,
//           prompt: "Thank you for that, May i know your compliant ? ",
//           retry: "Sorry, I don't understand that.");
//            }
//            else
//            {
//                PromptDialog.Text(
//          context: context,
//          resume: ServiceMessageReceivedAsyncService,
//          prompt: $@"Which category you want to prefer?. {Environment.NewLine} 1. New Lease Enquiry {Environment.NewLine} 2. Customer Support",
//          retry: "Sorry, I don't understand that.");
//            }

//        }
//        public async Task RepeatFinal(IDialogContext context,IAwaitable<string> argument)
//        {
//            string response = await argument;
//            complaint = response;

//            await context.PostAsync($@"Thank you for your interest, your request has been logged. Our customer service team will get back to you shortly.
//                                    {Environment.NewLine}Your service request  summary:
//                                    {Environment.NewLine}Complaint Title: {complaint},
//                                    {Environment.NewLine}Customer Name: {customerName},
//                                    {Environment.NewLine}Phone Number: {phone},
//                                    {Environment.NewLine}Email: {email}");

//            PromptDialog.Text(
//        context: context,
//        resume: AnythingElseHandler,
//        prompt: "Is there anything else that I could help?",
//        retry: "Sorry, I don't understand that.");

//        }
//        private async Task<string> Translation(string text)
//        {
//            HttpClient client = new HttpClient();
//            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
//            string uri = host + path + "?from=ar-ae&to=en-us&text=" + System.Net.WebUtility.UrlEncode(text);

//            HttpResponseMessage response = await client.GetAsync(uri);

//            string result = await response.Content.ReadAsStringAsync();
//            var content = XElement.Parse(result).Value;
//            return content;
//        }

//        public async Task ResumeLanguageOptions(IDialogContext context, IAwaitable<string> argument)
//        {
//            var result = await argument;
//            if(result.Contains("English"))
//            {
//                PromptDialog.Text(
//         context: context,
//         resume: ServiceMessageReceivedAsyncService,
//         prompt: $@"Which category you want to prefer?. {Environment.NewLine} 1. New Lease Enquiry {Environment.NewLine} 2. Customer Support",
//         retry: "Sorry, I don't understand that.");
//            }
//          else
//            {
//                PromptDialog.Text(
//        context: context,
//        resume: ServiceMessageArabic,
//        prompt: "ما هي الفئة التي تريد ان تفضلها ؟. جديد الإيجار الاستفسار   دعم العملاء",
//        retry: "Sorry, I don't understand that.");
//            }
//        }
//        public async Task ServiceMessageArabic(IDialogContext context, IAwaitable<string> result)
//        {
//            string transText = await Translation(result.ToString());

//            if (transText.Contains("New Lease Enquiry") || transText.Contains("Lease") || transText.Contains("lease"))
//            {
//                PromptDialog.Text(
//                context: context,
//                resume: NameCategoryArabic,
//                prompt: "هل لي ان اعرف اسمك من فضلك ؟",
//                retry: "Sorry, I don't understand that.");
//            }
//            else if (transText.Contains("Customer Support") || transText.Contains("customer support") || transText.Contains("support") || transText.Contains("service"))
//            {
//                PromptDialog.Text(
//           context: context,
//           resume: CustomerArabic,
//           prompt: "هل لي ان اعرف اسمك من فضلك ؟",
//           retry: "Sorry, I don't understand that.");
//                //Name
//            }
//            else
//            {
//                await context.PostAsync("Transfering to agent...");
//                var reply = context.MakeMessage();
//                //var transferTo = message.Text.Substring(TRANSFER_MESSAGE.Length);

//                reply.ChannelData = new LiveAssistChannelData()
//                {
//                    Type = "transfer",
//                    Skill = "BotEscalation"
//                };

//                await context.PostAsync(reply);
//            }
//        }

//        public async Task NameCategoryArabic(IDialogContext context, IAwaitable<string> result)
//        {
//            await context.PostAsync("هل تبحث عن السكنية/التجارية ؟");
//        }
//        public async Task CustomerArabic(IDialogContext context, IAwaitable<string> result)
//        {
//            PromptDialog.Text(
//                context: context,
//                resume: ComplaintArabic,
//                prompt: "هل لي برقم هاتفك النقال ؟",
//                retry: "Sorry, I don't understand that.");

//            //Number
//        }
//        public async Task ComplaintArabic(IDialogContext context, IAwaitable<string> result)
//        {
//            PromptDialog.Text(
//                context: context,
//                resume: FinalCaseArabic,
//                prompt: "ما هي شكواك ؟",
//                retry: "Sorry, I don't understand that.");

//            //Complaint
//        }
//        public async Task FinalCaseArabic(IDialogContext context, IAwaitable<string> result)
//        {
//            await context.PostAsync("أشكركم علي التفاصيل الخاصة بك. سيقوم وكيل خدمه العملاء لدينا بالرد عليك في غضون 24 ساعة.");

//            //Complaint
//        }
//        public async Task ServiceMessageReceivedAsyncService(IDialogContext context, IAwaitable<string> result)
//        {
//            var userFeedback = await result;

//            if (userFeedback.Contains("New Lease Enquiry") || userFeedback.Contains("Lease") || userFeedback.Contains("lease"))
//            {
//                PromptDialog.Text(
//                context: context,
//                resume: NameCategory,
//                prompt: "May i know your Name please?",
//                retry: "Sorry, I don't understand that.");
//            }
//            else if (userFeedback.Contains("Customer Support") || userFeedback.Contains("customer support") || userFeedback.Contains("support") || userFeedback.Contains("service") || userFeedback.Contains("Support") || userFeedback.Contains("Service"))
//            {
//                PromptDialog.Text(
//           context: context,
//           resume: Customer,
//           prompt: "May i know your name please?",
//           retry: "Sorry, I don't understand that.");
//            }
//        }
//        public async Task Customer(IDialogContext context, IAwaitable<string> result)
//        {
//            string response = await result;
//            customerName = response;

//            PromptDialog.Text(
//                context: context,
//                resume: CustomerApartment,
//                prompt: "May I have your Mobile Number?",
//                retry: "Sorry, I don't understand that.");
//        }
//        public async Task CustomerApartment(IDialogContext context, IAwaitable<string> result)
//        {
//            string response = await result;
//            phone = response;


//            PromptDialog.Text(
//                         context: context,
//                         resume: CustomerApartChecking,
//                         prompt: "Alright, I belive you live in Apt. 901 at Barsha 1 Tower? Am i right ?",
//                         retry: "Sorry, I don't understand that.");
//        }


//        public async Task CustomerApartChecking(IDialogContext context, IAwaitable<string> result)
//        {
//            string res = await result;
//            if(res.Contains("Yes") || res.StartsWith("Y") || res.StartsWith("y") || res.Contains("yes"))
//            {
//                PromptDialog.Text(
//               context: context,
//               resume: CustomerEmail,
//               prompt: "What is your complaint/suggestion?",
//               retry: "Sorry, I don't understand that.");
//            }
//            else
//            {
//                PromptDialog.Text(
//              context: context,
//              resume: CustomerAfterAPRT,
//              prompt: " Im sorry, I got you wrong, May i know your apartment details please?",
//              retry: "Sorry, I don't understand that.");
//            }

//        }
//        public async Task CustomerAfterAPRT(IDialogContext context, IAwaitable<string> result)
//        {
//            string response = await result;
//            phone = response;

//            PromptDialog.Text(
//                context: context,
//                resume: CustomerEmail,
//                prompt: "Thank you for that and I have updated my information, May i know your compliant?",
//                retry: "Sorry, I don't understand that.");
//        }
//        //public async Task CustomerMobileNumber(IDialogContext context, IAwaitable<string> result)
//        //{
//        //    string response = await result;
//        //    phone = response;

//        //    PromptDialog.Text(
//        //        context: context,
//        //        resume: CustomerEmail,
//        //        prompt: "What is your complaint/suggestion?",
//        //        retry: "Sorry, I don't understand that.");
//        //}
//        public async Task CustomerEmail(IDialogContext context, IAwaitable<string> result)
//        {
//            string response = await result;
//            complaint = response;

//            PromptDialog.Text(
//               context: context,
//               resume: FinalResultHandler,
//               prompt: "Sorry to hear that, I am going to register a complaint right away, for that I need your email ID as well",
//               retry: "Sorry, I don't understand that.");
//        }
//        public virtual async Task FinalResultHandler(IDialogContext context, IAwaitable<string> argument)
//        {
//            string response = await argument;
//            email = response;

//            await context.PostAsync($@"Thank you for your interest, your request has been logged. Our customer service team will get back to you shortly.
//                                    {Environment.NewLine}Your service request  summary:
//                                    {Environment.NewLine}Complaint Title: {complaint},
//                                    {Environment.NewLine}Customer Name: {customerName},
//                                    {Environment.NewLine}Phone Number: {phone},
//                                    {Environment.NewLine}Email: {email}");

//            PromptDialog.Text(
//          context: context,
//          resume: AnythingElseHandler,
//          prompt: "Is there anything else that I could help?",
//          retry: "Sorry, I don't understand that.");

//        }
//        public async Task AnythingElseHandler(IDialogContext context, IAwaitable<string> argument)
//        {


//            var answer = await argument;
//            if (answer.Contains("Yes") || answer.StartsWith("y") || answer.StartsWith("Y") || answer.StartsWith("yes"))
//            {
//                await GeneralGreeting(context, null);
//            }
//            else
//            {
//                string message = $"Thanks for using I Bot. Hope you have a great day!";
//                await context.PostAsync(message);

//                //var survey = context.MakeMessage();

//                //var attachment = GetSurveyCard();
//                //survey.Attachments.Add(attachment);

//                //await context.PostAsync(survey);

//                context.Done<string>("conversation ended.");
//            }
//        }
//        public virtual async Task GeneralGreeting(IDialogContext context, IAwaitable<string> argument)
//        {
//            string message = $"Great! What else that can I help you?";
//            await context.PostAsync(message);
//            context.Wait(MessageReceivedAsync);
//        }
//        public async Task NameCategory(IDialogContext context, IAwaitable<string> result)
//        {
//            //PromptDialog.Choice(context, ServiceMessageReceivedAsyncHomeH,
//            //          new List<string>()
//            //          {
//            //                "Residential",
//            //                "Commercial"
//            //          },
//            //          "Are you looking for Residence/Commercial?");

//            PromptDialog.Text(
//           context: context,
//           resume: ServiceMessageReceivedAsyncHomeH,
//           prompt: "Are you looking for Resedence/Commercial?",
//           retry: "Sorry, I don't understand that.");

//        }
//        public async Task ServiceMessageReceivedAsyncHomeH(IDialogContext context, IAwaitable<string> result)
//        {
//            var userFeedback = await result;

//           // if (userFeedback.Contains("Residential") || userFeedback.Contains("Commercial") || userFeedback.Contains("residence") || userFeedback.Contains("residential") || userFeedback.Contains("commercial"))
//            //{
//                PromptDialog.Text(
//               context: context,
//               resume: PropertyCity,
//               prompt: "Great. I can show you active homes If you tell me a little bit, Which part of UAE are you looking in?",
//               retry: "Sorry, I don't understand that.");
//            //}
//        }
//        public async Task PropertyCity(IDialogContext context, IAwaitable<string> result)
//        {
//            PromptDialog.Text(
//                context: context,
//                resume: PropertyBedrooms,
//                prompt: "That is a great market. There are currently 306 listings on the market in that area. To narrow it down a bit, what price do you require?",
//                retry: "Sorry, I don't understand that.");
//        }
//        public async Task PropertyBedrooms(IDialogContext context, IAwaitable<string> result)
//        {
//            //PromptDialog.Choice(context, ResumePropertyOptionsR,
//            //        new List<string>()
//            //        {
//            //            "Single Family",
//            //            "Studio",
//            //            "1 Bed Room",
//            //            "2 Bed Room",
//            //            "3 Bed Room",
//            //            "4 Bed Room",
//            //            "5 Bed Room"
//            //        },
//            //        "There are 54 available. Which type are you interested in?");

//            PromptDialog.Text(
//           context: context,
//           resume: ResumePropertyOptionsR,
//           prompt: $@"There are 54 available. Which type are you interested in? {Environment.NewLine} Single Family {Environment.NewLine} Studio",
//           retry: "Sorry, I don't understand that.");

//        }
//        public virtual async Task ResumePropertyOptionsR(IDialogContext context, IAwaitable<string> argument)
//        {
//            var selection = await argument;
//            string result = selection;

//            string message = "Great there are 25  " + result + " homes/properties that meet your needs. You can swipe to see each home/property.";
//            await context.PostAsync(message);

//            var reply = context.MakeMessage();

//            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
//            reply.Attachments = GetCardsAttachments();

//            await context.PostAsync(reply);

//            //context.Wait(this.MessageReceived);
//            //PromptDialog.Confirm(
//            //     context: context,
//            //     resume: CustomerLeadCreation,
//            //     prompt: "Would you like to get updates of new listings like these?",
//            //     retry: "Sorry, I don't understand that.");

//            PromptDialog.Text(
//          context: context,
//          resume: CustomerLeadCreation,
//          prompt: "Would you like to get updates of new listings like these ? ",
//          retry: "Sorry, I don't understand that.");
//        }
//        public async Task CustomerLeadCreation(IDialogContext context, IAwaitable<string> result)
//        {
//            var answer = await result;
//            if (answer.Contains("y") || answer.Contains("Yes") || answer.StartsWith("Y") || answer.StartsWith("Y"))
//            {
//                PromptDialog.Text(
//               context: context,
//               resume: CustomerLead,
//               prompt: "May I have your email id? ",
//               retry: "Sorry, I don't understand that.");
//            }
//            else
//            {
//                string message = $"Thanks for using I Bot. Hope you have a great day!";
//                await context.PostAsync(message);
//            }
//        }
//        public async Task CustomerLead(IDialogContext context, IAwaitable<string> result)
//        {
//            string response = await result;
//            email = response;

//            await context.PostAsync("Thank you for your interest. Our property consultant will get back to you shortly.");

//            //PromptDialog.Confirm(
//            //     context: context,
//            //     resume: AnythingElseHandler,
//            //     prompt: "Is there anything else that I could help?",
//            //     retry: "Sorry, I don't understand that.");
//            //CRMConnection.CreateLeadReg(customerName, email);

//            PromptDialog.Text(
//              context: context,
//              resume: AnythingElseHandler,
//              prompt: "Is there anything else that I could help?",
//              retry: "Sorry, I don't understand that.");
//        }
//        private static IList<Attachment> GetCardsAttachments()
//        {
//            return new List<Attachment>()
//            {
//                GetHeroCard(
//                    "Wasl Properties",
//                    "AED 950000",
//                    "Wasl Properties Group is a property development and management company based in Dubai, United Arab Emirates.",
//                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/1.jpg"),
//                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
//                GetHeroCard(
//                     "Wasl Properties",
//                    "AED 250000",
//                    "Wasl Properties is a leading real estate master developer based in Dubai. Aligned to the leadership’s vision and overall development plans.",
//                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/2.jpg"),
//                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
//                GetHeroCard(
//                     "Wasl Properties",
//                    "AED 670000",
//                    "Wasl Properties is a major contributor to realizing the vision of Dubai. A dynamic and forward-thinking organistion.",
//                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/3.jpg"),
//                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
//                GetHeroCard(
//                     "Wasl Properties",
//                    "AED 450009",
//                    "Wasl Properties is committed to creating and managing renowned developments that provide distinctive and enriching lifestyles.",
//                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/4.jpg"),
//                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
//                  GetHeroCard(
//                     "Wasl Properties",
//                    "AED 450009",
//                    "Riverside is part of Marasi Business Bay - an exciting new development by Wasl Properties, in the heart of Business Bay.",
//                    new CardImage(url: "https://dubaipropertieschatbot.azurewebsites.net/5.jpg"),
//                    new CardAction(ActionTypes.OpenUrl, "Read more", value: "https://www.waslproperties.com/en")),
//            };
//        }

//        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
//        {
//            var heroCard = new HeroCard
//            {
//                Title = title,
//                Subtitle = subtitle,
//                Text = text,
//                Images = new List<CardImage>() { cardImage },
//                Buttons = new List<CardAction>() { cardAction },
//            };

//            return heroCard.ToAttachment();
//        }

//        private static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
//        {
//            var heroCard = new ThumbnailCard
//            {
//                Title = title,
//                Subtitle = subtitle,
//                Text = text,
//                Images = new List<CardImage>() { cardImage },
//                Buttons = new List<CardAction>() { cardAction },
//            };

//            return heroCard.ToAttachment();
//        }
//        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
//        {
//            var confirm = await argument;
//            if (confirm)
//            {
//                this.count = 1;
//                await context.PostAsync("Reset count.");
//            }
//            else
//            {
//                await context.PostAsync("Did not reset count.");
//            }
//            context.Wait(MessageReceivedAsync);
//        }

//    }
//}

#endregion


#region Live Assist Without Live Assist Bot and Without Previous Chat History


//namespace Microsoft.Bot.Sample.SimpleEchoBot
//{
//    [Serializable]
//    public class EchoDialog : IDialog<object>
//    {
//        protected int count = 1;
//        private static Sdk sdk;
//        private static ChatContext chatContext;
//        private static string conversationRef;
//        private static Timer timer;

//        public async Task StartAsync(IDialogContext context)
//        {
//            sdk = sdk ?? new Sdk(new SdkConfiguration()
//            {
//                AccountNumber = "50283501"
//            });
//            context.Wait(MessageReceivedAsync);
//        }
//        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
//        {
//            var activity = await argument as Activity;

//            if (chatContext != null)
//            {
//                // As chatContext is not null we already have an escalated chat.
//                // Post the incoming message line to the escalated chat
//                await sdk.PostLine(activity.Text, chatContext);
//            }
//            else if (activity.Text == "reset")
//            {
//                PromptDialog.Confirm(
//                    context,
//                    AfterResetAsync,
//                    "Are you sure you want to reset the count?",
//                    "Didn't get that!",
//                    promptStyle: PromptStyle.Auto);
//            }
//            else if (activity.Text.Contains("help"))
//            {
//                // "help" within the message is our escalation trigger.
//                await context.PostAsync("Escalating to agent");
//                await Escalate(activity); // Implemented in next step.
//            }
//            else
//            {
//                await context.PostAsync($"{this.count++}: You said {activity.Text}");
//                context.Wait(MessageReceivedAsync);
//            }
//        }

//        private async Task Escalate(Activity activity)
//        {
//            // This is our reference to the upstream conversation
//            conversationRef = JsonConvert.SerializeObject(activity.ToConversationReference());

//            var chatSpec = new ChatSpec()
//            {
//                // Set Agent skill to target
//                Skill = "BotEscalation",
//                VisitorName = activity.From.Name
//            };

//            // Start timer to poll for Live Assist chat events
//            if (timer == null)
//            {
//                timer = timer ?? new Timer(5000);
//                // OnTimedEvent is implemented in the next step
//                timer.Elapsed += (sender, e) => OnTimedEvent(sender, e);
//                timer.Start();
//            }

//            // Request a chat via the Sdk    
//            chatContext = await sdk.RequestChat(chatSpec);
//        }

//        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
//        {
//            var confirm = await argument;
//            if (confirm)
//            {
//                this.count = 1;
//                await context.PostAsync("Reset count.");
//            }
//            else
//            {
//                await context.PostAsync("Did not reset count.");
//            }
//            context.Wait(MessageReceivedAsync);
//        }

//        async void OnTimedEvent(Object source, ElapsedEventArgs eea)
//        {
//            if (chatContext != null)
//            {
//                // Create an upstream reply
//                var reply = JsonConvert.DeserializeObject<ConversationReference>(conversationRef)
//                    .GetPostToBotMessage().CreateReply();

//                // Create upstream connection on which to send reply 
//                var client = new ConnectorClient(new Uri(reply.ServiceUrl));

//                // Poll Live Assist for events
//                var chatInfo = await sdk.Poll(chatContext);

//                if (chatInfo != null)
//                {
//                    // ChatInfo.ChatEvents will contain events since last call to poll.
//                    if (chatInfo.ChatEvents != null && chatInfo.ChatEvents.Count > 0)
//                    {
//                        foreach (ChatEvent e in chatInfo.ChatEvents)
//                        {
//                            switch (e.Type)
//                            {
//                                // type is either "state" or "line".
//                                case "line":
//                                    // Source is either: "system", "agent" or "visitor"
//                                    if (e.Source.Equals("system"))
//                                    {
//                                        reply.From.Name = "system";
//                                    }
//                                    else if (e.Source.Equals("agent"))
//                                    {
//                                        reply.From.Name = chatInfo.AgentName;

//                                    }
//                                    else
//                                    {
//                                        break;
//                                    }

//                                    reply.Type = "message";
//                                    reply.Text = e.Text;
//                                    client.Conversations.ReplyToActivity(reply);
//                                    break;

//                                case "state":
//                                    // State changes
//                                    // Valid values: "waiting", "chatting", "ended"
//                                    if (chatInfo.State.Equals("ended"))
//                                    {
//                                        chatContext = null;
//                                    }
//                                    break;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//    }
//}

#endregion


#region Live Assist Without live Assist Bot and With Previous CHat History
namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        private static Sdk sdk;
        private static ChatContext chatContext;
        private static string conversationRef;
        private static Timer timer;
        private static List<TranscriptLine> transcript;

        public async Task StartAsync(IDialogContext context)
        {
            sdk = sdk ?? new Sdk(new SdkConfiguration()
            {
                AccountNumber = "50283501",  // Live assist account number.
                ContextDataHost = "service.eu1.liveassistfor365.com" // Host name of the context data service.
            });
            transcript = new List<TranscriptLine>();

            context.Wait(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> argument)
        {
            var activity = await argument as Activity;
            if (chatContext != null)
            {
                // As chatContext is not null we already have an escalated chat.
                // Post the incoming message line to the escalated chat
                await sdk.PostLine(activity.Text, chatContext);
            }
            else if (activity.Text.Contains("help"))
            {
                transcript.Add(new TranscriptLine()
                {
                    IsBot = false,
                    Timestamp = DateTime.Now,
                    SrcName = activity.From.Name,
                    Line = activity.Text
                });
                // "help" within the message is our escalation trigger.
                await context.PostAsync("Escalating to agent");
                await Escalate(activity); // Implemented in next step.
            }
            else
            {
                var message = $"You said {activity.Text}";

                transcript.Add(new TranscriptLine()
                {
                    IsBot = false,
                    Timestamp = DateTime.Now,
                    SrcName = activity.From.Name,
                    Line = activity.Text
                });

                transcript.Add(new TranscriptLine()
                {
                    IsBot = true,
                    Timestamp = DateTime.Now,
                    SrcName = "EscalationBot",
                    Line = message
                });

                await context.PostAsync(message);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task Escalate(Activity activity)
        {
            // This is our reference to the upstream conversation
            conversationRef = JsonConvert.SerializeObject(activity.ToConversationReference());

            var chatSpec = new ChatSpec()
            {
                // Set Agent skill to target
                Skill = "BotEscalation",
                Transcript = transcript,
                VisitorName = activity.From.Name,
                ContextData = CreateJwt,
            };

            // Start timer to poll for Live Assist chat events
            if (timer == null)
            {
                timer = timer ?? new Timer(5000);
                // OnTimedEvent is implemented in the next step
                timer.Elapsed += (sender, e) => OnTimedEvent(sender, e);
                timer.Start();
            }

            // Request a chat via the Sdk    
            chatContext = await sdk.RequestChat(chatSpec);
        }

        public static String CreateJwt(string contextId)
        {
            var contextData = new ContextData()
            {
                customer = new Customer()
                {
                    firstName = new AssertedString()
                    {
                        value = "VenkataSambasivaRao"
                    },
                    lastName = new AssertedString()
                    {
                        value = "Kesanam"
                    }
                }
            };

            return Jwt.Create(contextId, contextData);
        }

        async void OnTimedEvent(Object source, ElapsedEventArgs eea)
        {
            if (chatContext != null)
            {
                // Create an upstream reply
                var reply = JsonConvert.DeserializeObject<ConversationReference>(conversationRef)
                    .GetPostToBotMessage().CreateReply();

                // Create upstream connection on which to send reply 
                var client = new ConnectorClient(new Uri(reply.ServiceUrl));

                // Poll Live Assist for events
                var chatInfo = await sdk.Poll(chatContext);

                if (chatInfo != null)
                {
                    // ChatInfo.ChatEvents will contain events since last call to poll.
                    if (chatInfo.ChatEvents != null && chatInfo.ChatEvents.Count > 0)
                    {
                        foreach (ChatEvent e in chatInfo.ChatEvents)
                        {
                            switch (e.Type)
                            {
                                // type is either "state" or "line".
                                case "line":
                                    // Source is either: "system", "agent" or "visitor"
                                    if (e.Source.Equals("system"))
                                    {
                                        reply.From.Name = "system";
                                    }
                                    else if (e.Source.Equals("agent"))
                                    {
                                        reply.From.Name = chatInfo.AgentName;

                                    }
                                    else
                                    {
                                        break;
                                    }

                                    reply.Type = "message";
                                    reply.Text = e.Text;
                                    client.Conversations.ReplyToActivity(reply);
                                    break;

                                case "state":
                                    // State changes
                                    // Valid values: "waiting", "chatting", "ended"
                                    if (chatInfo.State.Equals("ended"))
                                    {
                                        chatContext = null;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

    }
}
#endregion