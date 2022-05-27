using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using DemoCourseBot.Models;
using DemoCourseBot.Services;
using System.Threading;

namespace DemoCourseBot.Dialogs
{
    public class BugReportDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        public BugReportDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(BotStateService));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                DescriptionStepAsync,
                CallBackTimeAsync,
                PhoneNumberStepAsync,
                BugStepAsync,
                SummaryStepASync
            };

            AddDialog(new WaterfallDialog($"{nameof(BugReportDialog)}.MainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.Description"));
            AddDialog(new DateTimePrompt($"{nameof(BugReportDialog)}.CallBackTime", callBackValidator));
            AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.PhoneNumber", phoneNumberValidator));
            AddDialog(new ChoicePrompt($"{nameof(BugReportDialog)}.BugChoices"));

            InitialDialogId = $"{nameof(BugReportDialog)}.MainFlow";
        }

       
        private async Task<DialogTurnResult> SummaryStepASync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["bug"] = ((FoundChoice)stepContext.Result).Value;

            UserProfile userProfile = await _botStateService.userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            //userprofile only i am adding this ds call all properties

            userProfile.Description = (string)stepContext.Values["description"];
            userProfile.CallBackTime = (DateTime)stepContext.Values["callbackTime"];
            userProfile.PhoneNumber = (string)stepContext.Values["phoneNumber"];
            userProfile.Bug = (string)stepContext.Values["bug"];

            stepContext.Context.Activity.Text = $"Summary : \n{userProfile.Description}  \n{userProfile.CallBackTime}  \n{userProfile.PhoneNumber}  \n{userProfile.Bug}";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Summary : \n{userProfile.Description}  \n{userProfile.CallBackTime}  \n{userProfile.PhoneNumber}  \n{userProfile.Bug}"));

            await _botStateService.userProfileAccessor.SetAsync(stepContext.Context, userProfile);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> BugStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["phoneNumber"] = (string)stepContext.Result;
            stepContext.Context.Activity.Text = $"Plese select a bug type";
            return await stepContext.PromptAsync(
              $"{nameof(BugReportDialog)}.BugChoices",
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Plese select a bug type"),
                   Choices = ChoiceFactory.ToChoices(new List<string> { "Security", "Crash", "Power", "Seroius", "Other" })
               }, cancellationToken
               ); ;
        }

        private async Task<DialogTurnResult> PhoneNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["callbackTime"] = Convert.ToDateTime(((List<DateTimeResolution>)stepContext.Result).FirstOrDefault().Value);

            stepContext.Context.Activity.Text = $"Plese enter in a Phone number to call back";
            return await stepContext.PromptAsync(
              $"{nameof(BugReportDialog)}.PhoneNumber",
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Plese enter in a Phone number to call back"),
                   RetryPrompt = MessageFactory.Text("Please enter a valid phone number")
               }, cancellationToken
               ); ;
        }

        private async Task<DialogTurnResult> CallBackTimeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["description"] = (string)stepContext.Result;
            stepContext.Context.Activity.Text = $"Plese enter in a callback time";
            return await stepContext.PromptAsync(
                $"{nameof(BugReportDialog)}.CallBackTime",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Plese enter in a callback time"),
                    RetryPrompt = MessageFactory.Text("value must be between 9am to 5pm")
                }, cancellationToken
                ); ;
        }

        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Context.Activity.Text = $"Enter description for the report";
            return await stepContext.PromptAsync(
                $"{nameof(BugReportDialog)}.Description",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Enter description for the report")
                }, cancellationToken
                );
        }

        private Task<bool> phoneNumberValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = true;

            return Task.FromResult(valid);
        }

        private Task<bool> callBackValidator(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;

            if (promptContext.Recognized.Succeeded)
            {
                var resolution = promptContext.Recognized.Value.First();
                DateTime selectedDate = Convert.ToDateTime(resolution.Value);
                TimeSpan start = new TimeSpan(9, 0, 0);
                TimeSpan end = new TimeSpan(17, 0, 0);
                if ((selectedDate.TimeOfDay >= start) && (selectedDate.TimeOfDay <= end))
                {
                    valid = true;
                }
            }
            return Task.FromResult(valid);
        }

    }
}
