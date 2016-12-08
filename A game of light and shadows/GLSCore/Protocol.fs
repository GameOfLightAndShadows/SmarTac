module GLSManager.Protocol

open GLSManager.GlobalGLSState
open GLSCore.GameMap
open GLSCore.CharacterInformation
open GLSCore.GameElement
open GLSCore.GameItemsModel
open GLSCore.OperationDataModel

type InventorySystemManagerProtocol =
      | AddSingleItem of ItemStack
      | AddItems of ItemStack array
      | RemoveSingleItem of ItemStack
      | MoveExcessToInventory
      | BindItemToEquipment of GameItem
      | ReleaseItemFromEquipment of GameItem

type EquipmentSystemProtocol = 
    | UpdateWithCharacter of HumanCharacter
    | UpdateCharacterWithHelmet of Hat 
    | UpdateCharacterWithArmor of Armor 
    | UpdateCharacterWithWeapon of Weaponry 
    | UpdateCharacterWithGloves of Gauntlets
    | UpdateCharacterWithRing of Ring 
    | UpdateCharacterWithPants of Pants
    | UpdateCharacterWithShield of Shield 
    | UpdateCharacterWithLoot of ConsumableItem
    | MoveBackHelmetToInventory 
    | MoveBackArmorToInventory  
    | MoveBackWeaponToInventory 
    | MoveBackGlovesToInventory 
    | MoveBackRingToInventory   
    | MoveBackPantsToInventory  
    | MoveBackShieldToInventory     
    | MoveBackLootToInventory

type StoreProtocol = 
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
//    | UpdateBattleSequence  of BattleSequenceState
    | UpdateWeaponStore     of WeaponStoreState
    | UpdateItemStore       of ItemStoreState

type GameManagerProtocol =
    | UpdateGlobalState
    | DestroyCharacter of HumanCharacter
    | UpdateCharacterHealth of HumanCharacter
    | UpdateCharacterPosition of HumanCharacter
    | UpdateCharacterDirection
    | BroadcastInventoryUpdate of Inventory
    | StopManager

type BattleSequenceManagerProtocol =
    | UpdateBattlePhase of turn: int * actualPhase : BattleSequencePhase
    | LoadCommandManager
    | LoadExperienceManager 
    | LoadBrainManager
    | CreateGameMap
    | UpdateCharacterPosition of HumanCharacter 
    | RemoveGameCharacterFromBattle of HumanCharacter
    | MoveToNextActiveCharacter 
    | ExecuteHumanTurn of HumanCharacter
    | ExecuteBrainTurn of HumanCharacter 
    | CharacterDied of IGameCharacter
    | DisposeRessources

//type BattleViewManagerProtocol =
//    | TryEvade
//    | UseMeleeAttack
//    | UseCharacterSpecialMove
//    | ApplyTemporaryDefenseUpgrade
//    | StopManager

type WorldMapManagerProtocol =
    | RenderStoryPoint
    | MoveCharacterToStoryPoint
    | Stop

type CommandManagerProtocol =
    | PerformAttackCommandOn of BrainCharacter
    | PerformMoveCommandWith of HumanCharacter 
    | PerformDefendCommandWith of HumanCharacter 
    | PerformRotateCommandWith of HumanCharacter 
    | PerformEndTurn of IGameCharacter 

type StateServerProtocol =
    | UpdateTeamPartyState
    | UpdatePartyCharacter
    | UpdateGameBoardState
    | UpdateTeamPartyInventory

type ExperienceSystemProtocol =
    | ComputeGain of attacker: HumanCharacter * target: BrainCharacter * selectedAction: EngageAction

type TeamPartyProtocol = 
    (*From ViewTeamInventory to ShowEveryTeamMember, it's related to the dynamic prototype which would load a UI*)
    | ViewTeamInventory 
    | ModifyActiveParty 
    | ShowActiveParty 
    | ShowCharacterStats 
    | ShowEveryTeamMember
    | ShareTeamInformation
    | RemoveCharacterFromParty of HumanCharacter
    | RemoveCharacterFromTeam of HumanCharacter
    | AddCharacterToParty of HumanCharacter
    | AddCharacterToTeam of HumanCharacter 