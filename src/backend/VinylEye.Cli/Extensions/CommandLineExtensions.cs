using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using VinylEye.Cli.Commands;

namespace VinylEye.Cli.Extensions;

public static class CommandLineExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        var commandType = typeof(Command);

        var commands = typeof(PerspectiveCorrectImageCommand).Assembly
            .GetExportedTypes()
            .Where(x => commandType.IsAssignableFrom(x) && x is { IsAbstract: false, IsInterface: false });


        foreach (var command in commands)
        {
            services.AddScoped(commandType, command);
        }

        return services;
    }

    public static IServiceCollection AddCommandParser(this IServiceCollection services)
    {

        services.AddSingleton<Parser>(provider =>
        {
            var commandLineBuilder = new CommandLineBuilder();

            foreach (var command in provider.GetServices<Command>())
            {
                commandLineBuilder.Command.AddCommand(command);
            }

            return commandLineBuilder.Build();
        });
               
            
        return services;
    }
}