module GLSManager.StateServer

open GLSManager.GlobalGLSState
open GLSManager.Protocol
open Akka
open Akka.FSharp
open Akka.Logger.NLog

open GLSCore.CoreHelpers

let processGlobalState
    (mailbox: Actor<_>)  =
    let internalState = GlobalGameState.Initial
    let rec loop (state: GlobalGameState) = actor { 
        let! message = mailbox.Receive ()
        let internalState = 
            match message with 
            | UpdateStoryline story ->  
                state.updateStoryline story
            | UpdateBoard  board ->
                state.BattleSequence.updateBoardState board |> state.updateBattleSequence
            | UpdateMenu  menu        -> 
                state.updateMenuState menu
            | UpdateBattleSequence bs -> 
                state.updateBattleSequence bs
            | UpdateWeaponStore ws  ->
                state.updateWeaponStore ws 
            | UpdateItemStore is -> 
                state.updateItemStore is      
        return! loop internalState 
    }
    loop internalState

let stateServerRef = spawn system "State-Server" <| processGlobalState 
// Must confirm, but looks like the Akka actors have to spawn locally. 
// Will implement a supervisor of actors with the responsability of spawning every one of them 

