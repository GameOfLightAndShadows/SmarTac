module GLSManager.StateServer

open GLSManager.GlobalGLSState
open GLSManager.Protocol
open Akka
open Akka.FSharp
open Akka.Logger.NLog


let stateServerActor 
    (initialState: GlobalGameState)
//    (gameManager: Actor.ICanTell)
    (mailbox: Actor<'a>) = 


    let handleGlobalState (initialState: GlobalGameState) (mailbox : Actor<'a>) =

      let rec imp lastState =
        actor {
          let! msg = mailbox.Receive()
          match msg with 
          | UpdateStoryline story -> () 
          | UpdateBoard           -> () 
          | UpdateMenu            -> ()
          | UpdateBattleSequence  -> ()
          | UpdateWeaponStore     -> ()
          | UpdateItemStore       -> ()

          
          gameManager <! ""
          return! imp newState
        }

      imp initialState


      static member StartServer() = 
//        (gameManager: Actor.ICanTell)
        (mailbox: Actor<'a>) = 
            let initialState = GlobalGameState.Initial
            let handling = handleGlobalState initialState mailbbox
            handling