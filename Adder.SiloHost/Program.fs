// Learn more about F# at http://fsharp.org

open Adder.Grains
open Adder.Interfaces
open FSharp.Control.Tasks.V2
open Orleans
open Orleans.Hosting
open Orleans.Runtime.Configuration
open System

[<Literal>]
let ServiceName = "fsharp-orleans-test"

// Example of setup found at:
//   https://github.com/dotnet/orleans/blob/d7d35989bd370ff896556a5018a78f3faef9c0fb/Samples/2.1/FSharp.NetCore/src/FSharp.NetCore.SiloHost/Program.fs
let buildSiloHost () =
    (new SiloHostBuilder())
        .UseLocalhostClustering()
        .ConfigureApplicationParts(fun parts ->
            // Need the .WithReferences() in order to add the Interfaces
            // Interfaces must also be added so the client getting grains by interface type will work
            parts
                .AddApplicationPart((typeof<AdderGrain>).Assembly).WithReferences()
                .WithCodeGeneration()
                |> ignore )
        .Build()

[<EntryPoint>]
let main argv =
    let t = task {
        let host = buildSiloHost ()
        do! host.StartAsync()

        // Press a key to stop the host
        Console.Read() |> ignore

        do! host.StopAsync()
    }

    printf "Silo running..."
    t.Wait()

    0 // return an integer exit code
