module GLSCore.CoreHelpers

open Akka
open Akka.FSharp

let system = System.create "system" <| Configuration.load ()
