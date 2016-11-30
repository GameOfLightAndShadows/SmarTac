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
