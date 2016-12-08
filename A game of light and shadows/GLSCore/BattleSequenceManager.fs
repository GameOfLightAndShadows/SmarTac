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
    PlayerTeamParty : HumanCharacter array
    BrainTeamParty  : BrainCharacter array
    AliveAvatars    : IGameCharacter list 
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
        PlayerTeamParty = [| |]
        BrainTeamParty = [| |]
        AliveAvatars = []
        Board = Map.empty
        MatchState = MatchState.Initial
    }