using Microsoft.Extensions.DependencyInjection;
using System.CommandLine.Parsing;
using VinylEye.Cli.Extensions;

var services = new ServiceCollection();

services.AddCommands();
services.AddCommandParser();

var serviceProvider = services.BuildServiceProvider();
var parser = serviceProvider.GetRequiredService<Parser>();

return await parser.InvokeAsync(args);