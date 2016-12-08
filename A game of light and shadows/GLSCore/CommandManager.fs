module GLSManager.CommandManager

open Akka 
open Akka.FSharp

open GLSCore.HelperFunctions
open GLSCore.GameElement
open GLSCore.CharacterInformation

open GLSManager.Protocol

let processCommand
    (mailbox: Actor<_>) =
    let rec loop () = actor { 
        let! message = mailbox.Receive ()
        match message with 
        | NormalAttack (caller, target) 
        | SpecialAttack (caller, target) -> 
            target.Stats.Health.takeHit (caller.Stats.Strength |> int32)  |> ignore
            // actorRef <! UpdatePlayerHealth target
            return! loop()
        | Defend caller -> caller.Stats.applyTemporaryDefense 10 |> ignore<CharacterStats> // TODO: Cannot have ignore here
            
        | _ -> () 
        return! loop () 
    }
    loop ()

let commandManagerRef = spawn system "State-Server" <| processCommand 

