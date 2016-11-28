module GLSManager.ItemStoreManager

open GLSCore.InventoryItems
open Akka 
open Akka.Actor

type StoreTransaction = {
    StoreStock : GameItem list 
    Bill       : decimal<usd>
}

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
    | SendItemsToInventory of GameItem array
    | SendItemsToSelectedCharacter of GameItem array
    | UpdateGameItemQunatity of GameItem 
    | RemoveItemFromStock of GameItem
    | ShowHowMuchShouldBeBoughtInstead of GameItem
    | DisableStoreStock
    | IncreasePlayerTotalMoney of StoreTransaction
    | DecreasePlayerTotalMoney of StoreTransaction 
