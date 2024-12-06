using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using WEB_253502_TicTacToe.Server.Client.Services.RoomService;
using WEB_253502_TicTacToe.Server.Client.Services.WelcomeService;
using WEB_253502_TicTacToe.Server.Components;
using WEB_253502_TicTacToe.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();

builder.Services.AddScoped(sp => new HubConnectionBuilder()
    .WithUrl(sp.GetRequiredService<NavigationManager>().ToAbsoluteUri("/gamehub"))
    .Build());

builder.Services.AddScoped<IWelcomeService, WelcomeService>();
builder.Services.AddScoped<IRoomService, RoomService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHub<GameHub>("/gamehub");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WEB_253502_TicTacToe.Server.Client._Imports).Assembly);

app.Run();
