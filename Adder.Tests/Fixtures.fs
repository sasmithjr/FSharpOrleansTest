namespace Adder.Tests

module FixtureNames =
    [<Literal>]
    let ClusterCollectionName = "Cluster collection"

module rec Fixtures =
    open Adder.Interfaces
    open Adder.Grains
    open Orleans
    open Orleans.Hosting
    open Orleans.Runtime.Configuration
    open Orleans.TestingHost
    open Orleans.TestingHost.Utils
    open System
    open Xunit

    type SiloConfigurator () =
        interface ISiloBuilderConfigurator with
            member __.Configure (hostBuilder: ISiloHostBuilder) =
                hostBuilder
                    .AddMemoryGrainStorageAsDefault()
                    .ConfigureApplicationParts(fun parts ->
                        parts
                            .AddApplicationPart((typeof<AdderGrain>).Assembly).WithReferences()
                            .WithCodeGeneration()
                            |> ignore
                    )
                    |> ignore

    type ClientConfigurator () =
        interface IClientBuilderConfigurator with
            member __.Configure (config, clientBuilder) =
                clientBuilder
                    .ConfigureApplicationParts(fun parts ->
                        parts
                            .AddApplicationPart((typeof<IAdder>).Assembly)
                            .WithCodeGeneration()
                            |> ignore
                    )
                    |> ignore

    type ClusterFixture () =
        let builder = TestClusterBuilder()
        do builder.AddSiloBuilderConfigurator<SiloConfigurator>()
        do builder.AddClientBuilderConfigurator<ClientConfigurator>()

        let cluster = builder.Build()

        do cluster.Deploy()

        member __.Cluster = cluster

        interface IDisposable with
            member this.Dispose () =
                cluster.StopAllSilos()

    [<CollectionDefinition(FixtureNames.ClusterCollectionName)>]
    type ClusterCollection () =
        interface ICollectionFixture<ClusterFixture>
