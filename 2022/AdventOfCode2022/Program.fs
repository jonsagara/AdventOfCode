namespace AdventOfCode2022

open Microsoft.Extensions.DependencyInjection
open Serilog

module Program =

    [<EntryPoint>]
    let main argv =

        // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
        //   logger configured in `UseSerilog()` in HostBuilderHelper, once configuration and dependency-injection
        //   have both been set up successfully.
        Log.Logger <- LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
            try
                //
                // Set up the generic host and DI.
                //

                let host = HostBuilderHelper.buildHost argv
                use serviceScope = host.Services.CreateScope()
                let services = serviceScope.ServiceProvider


                //
                // Run the daily challenges.
                //

                //Day1.run()
                //Day2.run()
                //Day3.run()
                //Day4.run()
                //Day5.run()
                Day6.run()

                0
            with
            | ex -> 
                Log.Error(ex, "Unhandled exception in main")
                1
        finally
            Log.CloseAndFlush()