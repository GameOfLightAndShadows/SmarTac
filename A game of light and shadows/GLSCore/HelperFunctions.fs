module GLSCore.HelperFunctions

open System 
open System.Collections
open System.Collections.Generic
open Akka.FSharp

let system = System.create "system" <| Configuration.load ()

let inline (%%%) (x:int) (y:int) =  if x >= 0 then x % y else y + (x%y)

let powerOfN (value:int) (nExponential: int) = 
    Math.Pow (value |> float, nExponential |> float)

let optionDefaultsToValue opt v = 
    match opt with 
    | Some v -> v 
    | None -> v 

let flipFunction a b = b a 

let removeUnitFromFloat (x: float<_>) =
    float x

let removeUnitFromInt (x: int<_>) =
    int x

let killActor = ()
