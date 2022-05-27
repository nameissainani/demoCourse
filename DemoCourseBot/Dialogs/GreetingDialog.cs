using DemoCourseBot.Models;
using DemoCourseBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoCourseBot.Dialogs
{
    public class GreetingDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        //private readonly BotServices _botServices;

        public GreetingDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(BotStateService));
           // _botServices = botServices ?? throw new ArgumentNullException(nameof(BotServices));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {

            // create waterfall
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            //add dialogs
            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.MainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.Name"));


            //set the main dialog

            InitialDialogId = $"{nameof(GreetingDialog)}.MainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //luis code
            //var result = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
            //var luisResult = result.Properties["luisResult"] as LuisResult;
            //var entities = luisResult.Entities;

            //foreach (var entity in entities)
            //{
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{entity.Entity}"));
            //}

            //
            UserProfile userProfile = await _botStateService.userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                stepContext.Context.Activity.Text = "Your name please?";
                return await stepContext.PromptAsync(
                    $"{nameof(GreetingDialog)}.Name",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Your name please?")
                    });
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {

                //set the name 
                userProfile.Name = (String)stepContext.Result;
                await _botStateService.userProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }


           // stepContext.Context.Activity.Text = $"Hello, {userProfile.Name}. How can I help you?";

            //save anny state changes during this turn.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hello, {userProfile.Name}. How can I help you?"));
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
