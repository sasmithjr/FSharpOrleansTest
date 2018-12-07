namespace Adder.API

open Adder.Grains
open Adder.Interfaces
open Giraffe
open Orleans
open Orleans.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open System

module Routes =
    let routes =
        GET >=> choose
                  [ routeCi "/add" >=> AddService.addHandler
                    routeCi "/actorAdd" >=> AddService.actorAddHandler ]

module Orleans =
    [<Literal>]
    let ServiceName = "fsharp-orleans-test"

    let createClient (serviceProvider: IServiceProvider) =
        let client =
            ClientBuilder()
                .UseLocalhostClustering(serviceId = ServiceName)
                .ConfigureApplicationParts(fun parts ->
                    parts
                        .AddApplicationPart((typeof<IAdder>).Assembly)
                        .WithCodeGeneration()
                        |> ignore )
                .Build()

        client.Connect().Wait()

        client

type Startup() =

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    member __.ConfigureServices(services: IServiceCollection) =
        services.AddSingleton<IClusterClient>(Orleans.createClient) |> ignore
        services.AddGiraffe() |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member __.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseGiraffe Routes.routes
