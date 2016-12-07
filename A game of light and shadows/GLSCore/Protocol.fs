module GLSManager.Protocol

open GLSManager.GlobalGLSState
open GLSCore.GameMap
open GLSCore.CharacterInformation
open GLSCore.GameElement
open GLSCore.GameItemsModel
open GLSCore.GameItemsModel.GameItems
open GLSCore.GameItemsModel.Units
open GLSCore.GameItemsModel.ExccesItems
open GLSCore.GameItemsModel.ConsummableItems
open GLSCore.GameItemsModel.CharacterWearableProtection
open GLSCore.GameItemsModel.Energy
open GLSCore.GameItemsModel.Weapons

(*
For all types defined in protocol
Define an OperationDataModel.fs where they shall be stored
Move the unrelevant content to the protocol definition
*)
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

type EquipmentSystemProtocol = 
    | UpdateWithCharacter of GameCharacter
    | UpdateCharacterWithHelmet of Hat 
    | UpdateCharacterWithArmor of Armor 
    | UpdateCharacterWithWeapon of Weaponry 
    | UpdateCharacterWithGloves of Gauntlets
    | UpdateCharacterWithRing of Ring 
    | UpdateCharacterWithPants of Pants
    | UpdateCharacterWithShield of Shield 
    | MoveBackToInventory of GameItem
    | DropFromEquipment of GameItem

type ItemStoreProtocol = 
    | PurchaseMode 
    | SellMode 
    | ConfirmPurchase of bool
    | ConfirmSell of bool
    | SingleAdditionToTransaction of ItemStack 
    | MultipleAdditionToTransaction of ItemStack array 
    | SingleRemovalFromTransaction of ItemStack 
    | MultipleRemovalFromTransaction of ItemStack array
    | SendItemsToInventory of ItemStack array
    | SendItemsToSelectedCharacter of ItemStack array
    | UpdateGameItemQuantity of ItemStack *  TransactionOperation
    | RemoveItemFromStock of ItemStack
    | ShowHowMuchShouldBeBoughtInstead of ItemStack
    | DisableStoreStock
    | IncreasePlayerTotalMoney of StoreTransaction
    | DecreasePlayerTotalMoney of StoreTransaction 
    | AcceptTeamInformation of TeamInformation


type GlobalStateProtocol =
    | UpdateStoryline       of Storyline
    | UpdateBoard           of GameBoard
    | UpdateMenu            of MenuState
    | UpdateBattleSequence  of BattleSequenceState
    | UpdateWeaponStore     of WeaponStoreState
    | UpdateItemStore       of ItemStoreState


type GameManagerProtocol =
    | UpdateGlobalState
    | DestroyCharacter of GameCharacter
    | UpdateCharacterHealth of GameCharacter
    | UpdateCharacterPosition of GameCharacter
    | UpdateCharacterDirection
    | BroadcastInventoryUpdate of Inventory
    | StopManager

// Define CharacterObserver which will update the BattleSequenceManager once a character performed an action.

type BattleSequenceManagerProtocol =
    | ChangePhase of BattlePhase
    | StopManager

type InventoryManagerProtocol =
    | LookAtInventory
    | ReorderInventory
    | TossItem
    | AddItem
    | SelectItem
    | StopManager

type BattleViewManagerProtocol =
    | TryEvade
    | UseMeleeAttack
    | UseCharacterSpecialMove
    | ApplyTemporaryDefenseUpgrade
    | StopManager

type StoreManagerProtocol =
    | ShowStock
    | BuyItem
    | SellItem
    | StopManager

type WorldMapManagerProtocol =
    | RenderStoryPoint
    | MoveCharacterToStoryPoint
    | Stop

type CommandManagerProtocol =
    | PerformAttackCommandOn
    | PerformMoveCommandWith
    | PerformDefendCommandWith
    | PerformRotateCommandWith
    | PerformEndTurn

type StateServerProtocol =
    | UpdateTeamPartyState
    | UpdatePartyCharacter
    | UpdateGameBoardState
    | UpdateTeamPartyInventory

type ExperienceSystemProtocol =
    | ComputeGain of attacker: GameCharacter * target: GameCharacter * selectedAction: EngageAction

type TeamPartyProtocol = 
    (*From ViewTeamInventory to ShowEveryTeamMember, it's related to the dynamic prototype which would load a UI*)
    | ViewTeamInventory 
    | ModifyActiveParty 
    | ShowActiveParty 
    | ShowCharacterStats 
    | ShowEveryTeamMember
    | ShareTeamInformation
    | RemoveCharacterFromParty of GameCharacter
    | RemoveCharacterFromTeam of GameCharacter
    | AddCharacterToParty of GameCharacter
    | AddCharacterToTeam of GameCharacter 