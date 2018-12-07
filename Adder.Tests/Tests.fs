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

    [<Theory>]
    [<InlineData(0, 0)>]
    [<InlineData(0, 1)>]
    [<InlineData(3, 4)>]
    [<InlineData(-1, -2)>]
    let ``AdderGrain adds`` (x, y) = task {
        let grain = client.GetGrain<IAdder>(1L)

        let! result = grain.Add(x, y)
        let expected = x + y

        test <@ expected = result @>
    }
