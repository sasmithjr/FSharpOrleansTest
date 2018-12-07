namespace Adder.Tests

open Adder.Grains
open Adder.Interfaces
open FSharp.Control.Tasks
open Swensen.Unquote
open System
open Xunit

[<Collection(FixtureNames.ClusterCollectionName)>]
type GrainUnitTests(fixture: Fixtures.ClusterFixture) =
    let client = fixture.Cluster.Client

    [<Fact>]
    let ``AdderGrain adds`` () = task {
        let grain = client.GetGrain<IAdder>(1L)

        let! result = grain.Add(3, 4)

        test <@ 7 = result @>
    }
