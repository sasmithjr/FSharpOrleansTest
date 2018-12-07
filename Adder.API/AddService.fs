namespace Adder.API

open Adder.Interfaces
open FSharp.Control.Tasks.V2
open Giraffe
open Microsoft.AspNetCore.Http
open Orleans

module AddService =
    [<CLIMutable>]
    type AddOperands =
        { X: int
          Y: int }

    type ResultResponse<'ReturnType> =
        { Result: 'ReturnType }

    let addHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! operands = ctx.BindModelAsync<AddOperands>()
            let result: ResultResponse<int> = { Result = operands.X + operands.Y }
            return! json result next ctx
        }

    let actorAddHandler (next: HttpFunc) (ctx: HttpContext) =
        task {
            let! operands = ctx.BindModelAsync<AddOperands>()
            let client = ctx.GetService<IClusterClient>()
            let grain = client.GetGrain<IAdder>(1L)
            let! resultValue = grain.Add(operands.X, operands.Y)
            let result: ResultResponse<int> = { Result = resultValue }
            return! json result next ctx
        }