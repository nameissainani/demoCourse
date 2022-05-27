using DemoCourseBot.Bots;
using DemoCourseBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCourseBot.Services
{
    public class BotStateService
    {

        //state property

            //userstate
        public UserState UserState { get; }
        //conversation
        public ConversationState ConversationState { get; }

       




        //id

        public static string userProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";
        public static string userConversationId { get; } = $"{nameof(BotStateService)}.UserConversation";

        //dialog id
        public static string dialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";




        //accessors

        public IStatePropertyAccessor<UserProfile> userProfileAccessor { get; set; }
        public IStatePropertyAccessor<UserConversationData> userConversationAccessor { get; set; }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

        //public BotStateService(UserState userState)
        //{

        //    UserState = userState ?? throw new ArgumentNullException(nameof(userState));

        //    InitializeAccessors();
        //}

        public BotStateService(ConversationState conversationState, UserState userState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

            InitializeAccessors();
        }

        public void InitializeAccessors()
        {

            //intilize the userstate and conversational
            userProfileAccessor = UserState.CreateProperty<UserProfile>(userProfileId);
            userConversationAccessor = ConversationState.CreateProperty<UserConversationData>(userConversationId);

            //for dialog this one

            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(dialogStateId);

        }
    }
}
