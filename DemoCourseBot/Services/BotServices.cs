using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCourseBot.Services
{
    public class BotServices
    {



        public BotServices(IConfiguration configuration)
        {
            Dispatch = new LuisRecognizer(new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
                $"https://{configuration["LuisAPIHostName"]}.api.cognitive.microsoft.com"),
                new LuisPredictionOptions { IncludeAllIntents = true, IncludeInstanceData = true });

        }

    //    SampleQnA = new QnAMaker(new QnAMakerEndpoint
    //        {
    //            KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
    //            EndpointKey = configuration["QnAEndpointKey"],
    //            Host = configuration["QnAEndpointHostName"]
    //});

        // public QnAMaker SampleQnA { get; private set; }

        public LuisRecognizer Dispatch { get; set; }
    }
}
