using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using WEB_253502_TicTacToe.Server.Client.Services.RoomService;
using WEB_253502_TicTacToe.Server.Client.Services.WelcomeService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped(sp => new HubConnectionBuilder()
    .WithUrl(sp.GetRequiredService<NavigationManager>().ToAbsoluteUri("/gamehub"))
.Build());

builder.Services.AddScoped<IWelcomeService, WelcomeService>();
builder.Services.AddScoped<IRoomService, RoomService>();

await builder.Build().RunAsync();
