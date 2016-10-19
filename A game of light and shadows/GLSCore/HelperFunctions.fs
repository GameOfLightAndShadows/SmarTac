module GLSCore.HelperFunctions
open System 

//open Akka
//open Akka.FSharp
//
//let system = System.create "system" <| Configuration.load ()

let inline (%%%) (x:int) (y:int) = 
    if x>= 0 then x % y else y + (x%y)

let powerOfN (value:int) (nExponential: int) = 
    Math.Pow (value |> float, nExponential |> float)
