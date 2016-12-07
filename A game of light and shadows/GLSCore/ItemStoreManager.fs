module GLSManager.ItemStoreManager

open GLSCore.GameItemsModel
open GLSCore.CharacterInformation
open GLSCore.HelperFunctions
open GLSManager.Protocol

open Akka.Actor
open Akka.FSharp

//Will be used to determine if the merchand can show the stock
//to the player based on the total currency and the cheapest item
let canShowStoreStock
    (totalCurrency: int<usd>) 
    (stock: ItemStack list) =
    let lowestPrice = 
        stock 
        |> List.fold(fun lowest is -> if lowest > is.Item.Price then is.Item.Price else lowest ) 10000<usd> 
    lowestPrice < totalCurrency


let storeBill (op: StoreOperation) (is: ItemStack) = 
    is.Item.Price * is.Count * if op = Purchase then -1 else 1

let updateTransactionWithItem
    (op: StoreOperation)
    (is: ItemStack)
    (st: StoreTransaction) =     
    { st with StoreStock = [|is|] |> Array.append st.StoreStock; Bill = st.Bill + storeBill op is }

let updateTransactionWithItems 
    (op: StoreOperation)
    (isList: ItemStack array)
    (st: StoreTransaction) = 
    let mutable storeTransaction = st 

    for i in isList do 
        storeTransaction <- updateTransactionWithItem op i storeTransaction
    storeTransaction

let transactionWeight st = 
    st |> List.fold(fun acc itemStack -> acc + itemStack.Item.Weight) 0.00<kg>

type ItemStoreState = {
//    TeamPartyRef : IActorRef
//    PlayerParty  : GameCharacter array 
//    SelectedCharacter : GameCharacter option
    BagTotalWeight : float<kg> 
    PlayerTotalCurrency : float<usd> 
    StoreStock   : ItemStack array 
    Transaction  : StoreTransaction
    Operation    : StoreOperation option
}
with 
    static member Initial = {
        BagTotalWeight = 0.00<kg>
        PlayerTotalCurrency = 0.00<usd>
        StoreStock = [| |]
        Transaction = StoreTransaction.Empty
        Operation = None // We don't know every time the player enters what operation has to be done
    }


let itemStoreManager (mailbox: Actor<ItemStoreProtocol>) =
    let rec handleProtocol (state: ItemStoreState) = actor { 
        let! message = mailbox.Receive()
        
        match message with 
        | PurchaseMode -> return! handleProtocol { state with Operation = Some Purchase }
        
        | SellMode     -> return! handleProtocol { state with Operation = Some Sell }
        
        | ConfirmPurchase isConfirmed
        
        | ConfirmSell isConfirmed -> 
            if not isConfirmed then return! handleProtocol { state with Transaction = StoreTransaction.Empty }
            else return! handleProtocol state 
        
        | SingleAdditionToTransaction is -> 
            mailbox.Self <! UpdateGameItemQuantity (is, AddingToBill)
            return! handleProtocol { state with Transaction = updateTransactionWithItem Purchase is state.Transaction }
       
        | MultipleAdditionToTransaction isList -> 
            return! handleProtocol { state with Transaction = updateTransactionWithItems Purchase isList state.Transaction }
        
        | SingleRemovalFromTransaction is -> 
            mailbox.Self <! UpdateGameItemQuantity (is, RemovingFromBill)
            return! handleProtocol { state with Transaction = { state.Transaction with StoreStock = state.Transaction.StoreStock |> Array.filter(fun x -> x <> is) } }
        
        | MultipleRemovalFromTransaction isList -> 
            let list = state.Transaction.StoreStock
            let exclusionList = isList 
            let mutable remainingItemStacks = list 
            
            for exItem in exclusionList do 
                remainingItemStacks <- remainingItemStacks |> Array.filter(fun item -> item <> exItem)
              
            return! handleProtocol { state with Transaction = { state.Transaction with StoreStock =remainingItemStacks; } }

        | UpdateGameItemQuantity (is,op) -> 
            let oFindItemStack = state.StoreStock |> Array.tryFind(fun stack -> stack = is)
            match oFindItemStack with
            | None -> return! handleProtocol { state with StoreStock = state.StoreStock |> Array.append [|is|] }
            | Some itemStack -> 
                let qty = 
                    if op = RemovingFromBill 
                        then is.Count 
                    else is.Count * -1
                let index = state.StoreStock |> Array.findIndex(fun itemS -> itemS = itemStack) 
                let stock = state.StoreStock
                stock.[index] <- { itemStack with Count = itemStack.Count + qty }
                return! handleProtocol { state with StoreStock = stock }
    } handleProtocol ItemStoreState.Initial

let shopSystem = spawn system "Shop System" <| itemStoreManager

        