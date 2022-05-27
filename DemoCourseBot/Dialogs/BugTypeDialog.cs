using DemoCourseBot.Helpers;
using DemoCourseBot.Services;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoCourseBot.Dialogs
{
    public class BugTypeDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        private readonly BotServices _botServices;

        public BugTypeDialog(string dialogId, BotStateService botStateService, BotServices botServices) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(BotStateService));
            _botServices = botServices ?? throw new ArgumentNullException(nameof(BotServices));
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
            AddDialog(new WaterfallDialog($"{nameof(BugTypeDialog)}.MainFlow", waterfallSteps));
            //AddDialog(new TextPrompt($"{nameof(BugTypeDialog)}.Name"));


            //set the main dialog

            InitialDialogId = $"{nameof(BugTypeDialog)}.MainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //luis code

            var result = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
            var luisResult = result.Properties["luisResult"] as LuisResult;
            var entities = luisResult.Entities;

            foreach (var entity in entities)
            {
                if (Common.BugTypes.Any(s => s.Equals(entity.Entity, StringComparison.OrdinalIgnoreCase)))
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($" yes it is bug type"));
                }

                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($" no it is bug type"));
                }

            }


                return await stepContext.NextAsync(null, cancellationToken);
            }





            private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        
    }
}

   
