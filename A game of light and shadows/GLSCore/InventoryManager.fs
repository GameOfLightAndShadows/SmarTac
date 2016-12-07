module GLSManager.InventoryManager

open System

open GLSCore.HelperFunctions
open GLSCore.GameItemsModel
open GLSManager.Protocol
open GLSManager.StateServer


open Akka
open Akka.Actor
open Akka.FSharp

type InventorySystemState = {
    Inventory : Inventory
    StateServer : IActorRef
}
with 
    static member InitialState = 
    { Inventory = Inventory.InitialInventory; StateServer =  stateServerRef }

let inventorySystemProcess (mailbox: Actor<InventorySystemManagerProtocol>) =
    let rec loop (state: InventorySystemState) = actor {
        let! message = mailbox.Receive ()
        match message with
        | AddSingleItem item ->
          let inventory = state.Inventory.addItem item
          let state = { state with Inventory = inventory }
          return! loop state

        | AddItems items ->
          let inventory = state.Inventory.addItems items
          let state = { state with Inventory = inventory }
          return! loop state

        | RemoveSingleItem item ->
          let inventory = state.Inventory.dropItem item
          let state = { state with Inventory = inventory }
          return! loop state
    }
    loop InventorySystemState.InitialState

let inventoryManager = spawn system "Inventory System" <| inventorySystemProcess 
