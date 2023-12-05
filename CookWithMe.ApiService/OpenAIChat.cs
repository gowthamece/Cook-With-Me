using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using CookWithMe.ApiService.enums;
using Microsoft.Extensions.Options;

namespace CookWithMe.ApiService
{
    public class OpenAIChat: IHostedService
    {
        private readonly IHostApplicationLifetime lifetime;
        private readonly OpenAIClient openAIClient;
        private readonly IOptions<Settings> settings;

        public OpenAIChat(IHostApplicationLifetime lifetime, OpenAIClient openAIClient, IOptions<Settings> settings)
        {
            this.lifetime = lifetime;
            this.openAIClient = openAIClient;
            this.settings = settings;
        }
        public async Task GetRecepie()
        {
           
            //Response<Completions> completionsResponse = client.GetCompletions(completionsOptions);
            //string completion = completionsResponse.Value.Choices[0].Text;
            //Console.WriteLine($"Chatbot: {completion}");
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Settings s = settings.Value;

            bool goodbye = false;
            ChatCompletionsOptions completionsOptions = new()
            {
                MaxTokens = s.MaxTokens,
                Temperature = s.Temperature,
                FrequencyPenalty = s.FrequencyPenalty,
                PresencePenalty = s.PresencePenalty,
                Messages =
            {
                new(ChatRole.System, s.SystemPrompt)
            }
            };
            string? input = await Console.In.ReadLineAsync();


            completionsOptions.Messages.Add(new(ChatRole.User, input));

            var completions = await openAIClient.GetChatCompletionsAsync(completionsOptions, cancellationToken);

            if (completions.Value.Choices.Count == 0)
            {
                await Console.Out.WriteLineAsync("I'm sorry, I don't know how to respond to that.");

            }

            foreach (ChatChoice choice in completions.Value.Choices)
            {
                string content = choice.Message.Content;



                completionsOptions.Messages.Add(new(ChatRole.Assistant, content));
            }


            lifetime.StopApplication();
        }


        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
