namespace Adder.Grains

[<AutoOpen>]
module Grains =

    open Adder.Interfaces
    open Orleans
    open System.Threading.Tasks

    type AdderGrain () =
        inherit Grain ()
        interface Grains.IAdder with
            member __.Add(x, y): Task<int> =
                x + y |> Task.FromResult
