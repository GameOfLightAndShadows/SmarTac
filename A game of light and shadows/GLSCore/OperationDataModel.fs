module GLSCore.OperationDataModel

open GLSCore.CharacterInformation
open GLSCore.GameElement
open GLSCore.GameItemsModel

type StoreOperation = 
    | Purchase
    | Sell

type TransactionOperation = 
    | RemovingFromBill
    | AddingToBill

type StoreTransaction = {
    StoreStock : ItemStack array 
    Bill       : int<usd>
}
with 
    static member Empty = { StoreStock = [| |]; Bill = 0<usd> }

type TeamInformation = {
    Inventory : Inventory 
    Members  : GameCharacter list
}
with 
    static member Initial = { Inventory = Inventory.InitialInventory; Members = [] }