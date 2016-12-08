module GLSManager.BattleSequenceManager

open Akka 
open Akka.FSharp

open GLSCore.HelperFunctions
open GLSCore.GameElement
open GLSCore.GameMap
open GLSCore.CharacterInformation
open GLSCore.OperationDataModel

open GLSManager.Protocol

type BattleSequenceState = {
    ActivePhase     : BattlePhase
    PlayerTeamParty : HumanCharacter array
    BrainTeamParty  : BrainCharacter array
    Board           : GameBoard
    MatchState      : MatchState
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
        ActivePhase = Move 
        PlayerTeamParty = [| |]
        BrainTeamParty = [| |]
        Board = Map.empty
        MatchState = MatchState.Initial
    }

type BattleSystemState = {
    HumanParty : HumanCharacter list 
    BrainParty : BrainCharacter list 
    AliveAvatars : IGameCharacter list
    Map        : GameBoard
    BattlePhase : int * BattleSequencePhase
}