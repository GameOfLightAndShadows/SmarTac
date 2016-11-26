module GLSManager.InventoryManager

open System

open GLSCore.GLSCore.InventoryItems
open GLSManager.Protocol

open Akka
open Akka.FSharp


let system = System.create "system" <| Configuration.load ()

type InventorySystemState = {
    Inventory : Inventory
    GameManager : Actor<GameManagerProtocol>
}

let inventorySystemProcess
    (mailbox: Actor<InventorySystemManagerProtocol>) =
    let rec loop (state: InventorySystemState) = actor {
        let! message = mailbox.Receive ()
        match message with
        | AddSingleItem item ->
          let inventory = state.Inventory.addItem item
          let state = { state with Inventory = inventory }
          state.GameManager <<- BroadcastInventoryUpdate(inventory)
          return! loop state
        | AddItems items ->
          let inventory = state.Inventory.addItems items
          let state = { state with Inventory = inventory }
          state.GameManager <<- BroadcastInventoryUpdate(inventory)
          return! loop state
        | RemoveSingleItem item ->
          let inventory = state.Inventory.dropItem item
          let state = { state with Inventory = inventory }
          state.GameManager <<- BroadcastInventoryUpdate(inventory)
          return! loop state
       | MoveExcessToInventory ->
          let inventory = state.Inventory.removeItemFromExcess()
          let state = { state with Inventory = inventory }
          state.GameManager <<- BroadcastInventoryUpdate(inventory)
          return! loop state
    }
    loop ()

let commandManagerRef = spawn system "Inventory System" <| inventorySystemProcess 
