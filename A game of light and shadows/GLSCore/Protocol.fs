module GLSManager.Protocol

open GLSManager.GlobalGLSState
open GLSCore.GameMap
open GLSCore.CharacterInformation
open GLSCore.GameElement
open GLSCore.PartyCharacter

type GlobalStateProtocol = 
    | UpdateStoryline       of Storyline 
    | UpdateBoard           of GameBoard 
    | UpdateMenu            of MenuState
    | UpdateBattleSequence  of BattleSequenceState
    | UpdateWeaponStore     of WeaponStoreState
    | UpdateItemStore       of ItemStoreState


type GameManagerProtocol =  
    | UpdateGlobalState 
    | DestroyCharacter of PartyCharacter
    | UpdateCharacterHealth of PartyCharacter
    | UpdateCharacterPosition of PartyCharacter
    | UpdateCharacterDirection 
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