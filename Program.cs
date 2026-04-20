using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using StudyCallAIApi.Components;

namespace StudyCallAIApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Razor Components
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // HttpClient
            var apiBase = builder.Configuration["LocalApi:BaseUrl"]
                ?? throw new Exception("LocalApi:BaseUrl が設定されていません。");

            builder.Services.AddHttpClient("local", client =>
            {
                client.BaseAddress = new Uri(apiBase);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Azure OpenAI Client を DI 登録
            string endpoint = builder.Configuration["AzureOpenAI:Endpoint"];
            string apiKey = builder.Configuration["AzureOpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("AzureOpenAI の設定が appsettings.json にありません。");

            builder.Services.AddSingleton(new AzureOpenAIClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey)
            ));

            // Controller を使えるようにする
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            // Controller のルーティング
            app.MapControllers();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
