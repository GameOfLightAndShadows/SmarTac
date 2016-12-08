module GLSManager.BattleSequenceManager

open Akka 
open Akka.FSharp

open GLSCore.HelperFunctions
open GLSCore.GameElement
open GLSCore.GameMap
open GLSCore.CharacterInformation

open GLSManager.Protocol

type BattleSystemState = {
    HumanParty : GameCharacter list 
    BrainParty : GameCharacter list 
    Map        : GameBoard
    BattlePhase : int * BattleSequencePhase
}