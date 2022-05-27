using DemoCourseBot.Models;
using DemoCourseBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoCourseBot.Bots
{
    public class GreetingBot : ActivityHandler
    {

        private readonly BotStateService _botStateService;

        public GreetingBot(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            //var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    // await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                    await GetName(turnContext,cancellationToken);
                }
            }
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            //userstate
            UserProfile userProfile = await _botStateService.userProfileAccessor.GetAsync(turnContext, () => new UserProfile());

            //conversational
            UserConversationData userConversationData = await _botStateService.userConversationAccessor.GetAsync(turnContext, () => new UserConversationData());

            //Logic
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hello! {userProfile.Name}. What's Up!"), cancellationToken);
            }

            //conversational

            else
            {
                if (userConversationData.PromptedForUserName)
                {
                    userProfile.Name = turnContext.Activity.Text?.Trim();
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hey {userProfile.Name}. I will remember your name now :)"), cancellationToken);
                    userConversationData.PromptedForUserName = false;
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Whats ur name?"), cancellationToken);
                    userConversationData.PromptedForUserName = true;
                }
            }

            //Saving any  state changes occurs here
            await _botStateService.userProfileAccessor.SetAsync(turnContext, userProfile);
            await _botStateService.userConversationAccessor.SetAsync(turnContext, userConversationData);

            await _botStateService.UserState.SaveChangesAsync(turnContext);
            await _botStateService.ConversationState.SaveChangesAsync(turnContext);

        }
    }
}
