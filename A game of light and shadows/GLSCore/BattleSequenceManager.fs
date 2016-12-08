module GLSManager.BattleSequenceManager

open Akka 
open Akka.Actor
open Akka.FSharp

open GLSCore.HelperFunctions
open GLSCore.GameItemsModel
open GLSCore.GameElement
open GLSCore.GameMap
open GLSCore.CharacterInformation
open GLSCore.OperationDataModel

open GLSManager.Protocol
open GLSManager.BrainManager
open GLSManager.CommandManager
open GLSManager.ExperienceSystemManager


type BattleSequenceState = {
    PlayerTeamParty : HumanCharacter array
    BrainTeamParty  : BrainCharacter array
    AliveAvatars    : IGameCharacter list 
    Board           : GameBoard
    Phase           : BattlePhaseState
    MatchState      : MatchState
    CommandManager  : IActorRef option 
    ExperienceManager : IActorRef option
    BrainManager    : IActorRef option
}

with 
    member x.updateBoardState (b: GameBoard) = 
        { x with Board = b }

    member x.updateBrainTeamParty (team: BrainCharacter array) = 
        { x with BrainTeamParty = team }

    member x.updatePlayerParty (team: HumanCharacter array) = 
        { x with PlayerTeamParty = team }

    member x.updateMatchState (state: MatchState) = 
        { x with MatchState = state }

    static member Initial = {
        PlayerTeamParty = [| |]
        BrainTeamParty = [| |]
        AliveAvatars = []
        Board = Map.empty
        Phase = BattlePhaseState.Initial
        MatchState = MatchState.Initial
        CommandManager = None 
        ExperienceManager = None 
        BrainManager = None 
    }

let inventorySystemProcess (mailbox: Actor<BattleSequenceManagerProtocol>) =
    // Initialisation of the battle system 
    mailbox.Self <! LoadCommandManager 
    mailbox.Self <! LoadExperienceManager 
    mailbox.Self <! LoadBrainManager
    mailbox.Self <! CreateGameMap

    let rec loop (state: BattleSequenceState) = actor {
        let! message = mailbox.Receive() 
        match message with
        | UpdateBattlePhase (turn, phase) ->    
            return! loop { state with Phase = { state.Phase with NumberOfTurns = turn; Current = phase} }       
        | LoadCommandManager -> 
            return! loop { state with CommandManager = Some commandManagerRef }
        | LoadExperienceManager -> 
            return! loop { state with ExperienceManager = Some experienceSystem }
        | LoadBrainManager -> 
            return! loop state // BrainManager needs to be implemented as a manager !!!
        | CreateGameMap -> 
            let size = { Width = 100; Height = 100 }
            let gameboard = 
                [ for top in 0 .. size.Height - 1 do
                    for left in 0 .. size.Width - 1 do 
                            let pos = { Top = top; Left = left }
                            let cell = 
                                let value = randomizer.NextDouble()             
                                if value >= 0.0 && value < 0.15 then CollectibleTreasure(Health(HealthPotion))
                                else if value >= 0.15 && value < 0.25 then Empty 
                                else if value >= 0.25 && value < 0.5 then HiddenTrap(ReduceMoney)
                                else if value >= 0.5 && value < 0.65 then HiddenTrap(ReduceLifePoints)
                                else if value >= 0.65 && value < 0.90 then Enemy ( HumanCharacter.InitialGameCharacter ) // The found enemy doesn't matter now for the training purposes !!!
                                else CollectibleTreasure(Currency(30.00<usd>)) // The amount of money doesn't matter for the training purposes 
                            yield pos, cell]
                |> Map.ofList  
            
            return! loop { state with Board = gameboard }
        
    }
    loop BattleSequenceState.Initial