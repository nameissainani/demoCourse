using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using DemoCourseBot.Services;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder;

namespace DemoCourseBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {

        // this is only for the state conversation and   usersate services
        private readonly BotStateService _botStateService;

        // this is for the connection for lusis  as well as the qna
        private readonly BotServices _botServices;


        // with only with conversation and userstate

        //public MainDialog(BotStateService botStateService) : base(nameof(MainDialog))
        //{
        //    _botStateService = botStateService ?? throw new ArgumentNullException(nameof(BotStateService));
        //    InitializeWaterfallDialog();
        //}

        // this constructor with the connecction with lusi and qna

        public MainDialog(BotStateService botStateService, BotServices botservices) : base(nameof(MainDialog))
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(BotStateService));
            _botServices = botservices ?? throw new ArgumentNullException(nameof(BotServices));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepASync
            };

            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.GreetingDialog", _botStateService));
           
            AddDialog(new BugReportDialog($"{nameof(MainDialog)}.BugReportDialog", _botStateService));
            AddDialog(new BugTypeDialog($"{nameof(MainDialog)}.QueryBugDialog", _botStateService, _botServices));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.MainFlow", waterfallSteps));

            InitialDialogId = $"{nameof(MainDialog)}.MainFlow";
        }



        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            // this code is only for the basic checking of regex condition for checking hi condition


            //    // as of now i am doing with regex condition


            //    if(Regex.Match(stepContext.Context.Activity.Text.ToLower() ,"hi").Success)
            // //   if (stepContext.Context.Activity.Text == "hi")
            //    {
            //        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.GreetingDialog", null, cancellationToken);
            //    }
            //    else
            //    {
            //        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.BugDialog");
            //    }

            //}


            try
            {
                // first i am chekcing with dispatch to check whether it is luis or qna maker
                var recongizerresult = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);



                // now should tell us which is top intent
                var topintent = recongizerresult.GetTopScoringIntent();
                switch (topintent.intent)
                {
                    case "Greetings":

                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.GreetingDialog", cancellationToken: cancellationToken);

                    case "BugReport":

                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.BugReportDialog",null, cancellationToken);

                    case "QueryBug":

                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.QueryBugDialog", null,cancellationToken);
                    case "None":

                        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"sorry i cant found the issue"), cancellationToken);

                        break;



                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

           

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepASync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}