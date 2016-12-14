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
    AliveAvatars    : IGameCharacter array 
    Active          : IGameCharacter option
    ActiveIndex     : int
    Board           : GameBoard
    Phase           : BattlePhaseState
    MatchState      : MatchState
    CommandManager  : IActorRef option 
    ExperienceManager : IActorRef option
    BrainManager    : IActorRef option
} 
with
    static member Initial = {
        PlayerTeamParty = [| |]
        BrainTeamParty = [| |]
        AliveAvatars = [||]
        Active = None 
        Board = Map.empty
        ActiveIndex = 0
        Phase = BattlePhaseState.Initial
        MatchState = MatchState.Initial
        CommandManager = None 
        ExperienceManager = None 
        BrainManager = None 
    }

type IndexUpdateOption = 
    | NextTurn 
    | Death 

let updatedAvatarIndex index (arr: IGameCharacter array) updateOpt = 
    match updateOpt with 
    | NextTurn -> 
        if (index = arr.Length-1) then 0 else index + 1
    | Death ->  
        if index = 0 then 0 else index - 1

let battleSequenceSystem (mailbox: Actor<BattleSequenceManagerProtocol>) =
    // Initialisation of the battle system 
    mailbox.Self <! LoadCommandManager 
    mailbox.Self <! LoadExperienceManager 
    mailbox.Self <! LoadBrainManager
    mailbox.Self <! CreateGameMap
    mailbox.Self <! MoveToNextActiveCharacter

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
            return! loop { state with Board = generateGameboard }
        | UpdateCharacterPosition igc ->
            let map = state.Board
            let pos = igc.position()
            let oldPosition =  map |> Map.tryFindKey(fun key _-> key = (igc.position()))
            match oldPosition with
            | Some p ->     
                let board = 
                    map 
                    |> Map.add p Empty
                    |> Map.add pos (Character igc)
                return! loop { state with Board = board }
            | None -> 
                return! loop state

        | CharacterDied dead -> 
            let remainingAvatars = state.AliveAvatars |> Array.filter(fun char -> char <> dead)
            let index = updatedAvatarIndex state.ActiveIndex state.AliveAvatars Death
            let state = 
                match dead with 
                | :? HumanCharacter as hc -> 
                    let remainingHumans = state.PlayerTeamParty |> Array.filter(fun char -> char <> hc)
                    { state with PlayerTeamParty = remainingHumans }
                | :? BrainCharacter as bc -> 
                    let remainingBrains = state.BrainTeamParty |> Array.filter(fun char -> char <> bc)
                    { state with BrainTeamParty = remainingBrains }
                
                | _ -> state

            let matchState = if state.BrainTeamParty.Length = 0 then PlayerWon
                             elif state.PlayerTeamParty.Length = 0 then BrainWon 
                             else InProcess 

            mailbox.Self <! ValidateIfGameFinished matchState

            let state = { state with AliveAvatars = remainingAvatars; MatchState = matchState; ActiveIndex = index }
            return! loop state    
            
        | MoveToNextActiveCharacter -> 
            let index = updatedAvatarIndex state.ActiveIndex state.AliveAvatars NextTurn
            let active = Array.get state.AliveAvatars index
            match active with 
            | :? HumanCharacter as hc -> 
                state.CommandManager 
                |> Option.iter(fun manager -> manager <! ReceiveActiveHumanCharacter hc )
            | :? BrainCharacter as bc -> 
                state.BrainManager
                |> Option.iter(fun manager -> manager <! ReceiveActiveBrainMember bc)
            | _ -> ()
            return! loop { state with ActiveIndex = index; Active = Some active }

        | ValidateIfGameFinished mState -> 
            match mState with 
            | InProcess -> return! loop state 
            | PlayerWon -> 
                printfn "Player Won"
                return () // The player finished the level 
            | BrainWon -> 
                printfn "Brain won"
                return ()
    }
    loop BattleSequenceState.Initial