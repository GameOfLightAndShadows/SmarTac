module GLSManager.ItemStoreManager

open GLSCore.InventoryItems
open Akka 
open Akka.Actor

let computeTotalAmount (items: GameItem list) = 
    items |> List.fold (fun acc x -> acc + x.Price) 0.00<usd>

let filter condition initialvalue list= 
    List.fold (fun statevar element -> if condition statevar then statevar else element) initialvalue list

type StoreTransaction = {
    StoreStock : GameItem list 
    Bill       : decimal<usd>
}
with 
    member x.addItem item = 
        { x with StoreStock = x.StoreStock |> List.append [item]; Bill = x.Bill + item.Price }

    member x.addItems items = 
        let totalPriceAddition = List.fold(fun acc x -> acc + x.Price) 0.00<usd> items
        { x with StoreStock = x.StoreStock |> List.append (items |> Array.toList); Bill = x.Bill + (items |> computeTotalAmount) }

    member x.removeItem item = 
        { x with StoreStock = x.StoreStock |> List.filter(fun i -> i = item) ; Bill = x.Bill - item.Price }

    member x.removeItems items = 
        let stock = List.fold (fun list element -> if items |> List.contains element then list else element) x.StoreStock items
        { x with StoreStock = stock; Bill = x.Bill - (items |> computeTotalAmount) }

type ItemStoreState = {
    TeamPartyRef : IActorRef
    PlayerParty  : PartyCharacter array 
    SelectedCharacter : PartyCharacter option
    BagTotalWeight : float<kg> 
    PlayerTotalCurrency : float<usd> 
    StoreStock   : ( GameItem * ItemCategory) list 
    Transaction  : StoreTransaction
}

type ItemStoreProtocol = 
    | PurchaseMode 
    | SellMode 
    | ConfirmPurchase 
    | ConfirmSell 
    | ConfirmSingleAdditionToTransaction of GameItem 
    | ConfirmMultipleAdditionToTransaction of GameItem array 
    | ConfirmSingleRemovalFromTransaction of GameItem 
    | ConfirmMultipleRemovalFromTransaction of GameItem array
    | SendItemsToInventory of GameItem array
    | SendItemsToSelectedCharacter of GameItem array
    | UpdateGameItemQunatity of GameItem 
    | RemoveItemFromStock of GameItem
    | ShowHowMuchShouldBeBoughtInstead of GameItem
    | DisableStoreStock
    | IncreasePlayerTotalMoney of StoreTransaction
    | DecreasePlayerTotalMoney of StoreTransaction 
