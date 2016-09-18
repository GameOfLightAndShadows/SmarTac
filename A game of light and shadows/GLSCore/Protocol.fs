module GLSManager.Protocol

open GLSManager.GlobalGLSState

type GameManagerProtocol =  
    | UpdateGlobalState 
    | DestroyCharacter 
    | UpdateCharacterHealth 
    | UpdateCharacterPosition
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

type CommanadManagerProtocol = 
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