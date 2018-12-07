namespace Adder.Interfaces

[<AutoOpen>]
module Grains =

    open System.Threading.Tasks

    type IAdder =
        inherit Orleans.IGrainWithIntegerKey
        abstract member Add: int * int -> Task<int>
