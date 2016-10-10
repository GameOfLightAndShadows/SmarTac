module GLSCore.ChoiceSelector

open GLSCore.CharacterInformation
open GLSCore.PartyCharacter
open GLSCore.CharacterAction
open GLSCore.GameMap

open System
open System.Collections

//Later game manager will receive message which will
// update the GlobalGameState containing the board for something such as moving a character towards its target

type IBattleActionSequence = 
    abstract member canMoveSelf: PartyCharacter -> GameBoard -> bool 

    abstract member isFacingTarget: PartyCharacter -> PartyCharacter -> bool 

    abstract member isCharacterInRange: PartyCharacter -> PartyCharacter -> GameBoard -> bool

    abstract member createCommandSequence: unit -> Action list

    abstract member generateSequenceScore: unit -> int

    abstract member createQuickestPathTowardsTarget: unit -> GameCell list

    abstract member moveTowardsTarget: PartyCharacter -> PartyCharacter -> GameBoard -> unit 

    abstract member faceTarget: PartyCharacter -> PartyCharacter -> unit


type CommandSequence = {
    Caller      : PartyCharacter 
    Target      : PartyCharacter option
    Sequence    : (unit -> unit) list
}
with 
    interface IBattleActionSequence with
        member x.canMoveSelf 
            (caller: PartyCharacter) 
            (board: GameBoard) = 
            
            // Check whether with a character move range and the board's actual state 
            // if it's actually possible to move in any possible direction
            true

        member x.isFacingTarget
            (caller: PartyCharacter)
            (target: PartyCharacter) = 
            caller.CharacterDirection.areDirectionOpposite (target.CharacterDirection)

        member x.isCharacterInRange     
            (caller: PartyCharacter)
            (target: PartyCharacter) 
            (board: GameBoard) = 

            // Check the position of caller and target in the board 
            // Check ranges of both caller and target and validate if they overlapped
            true

        member x.createCommandSequence() = []

        member x.generateSequenceScore() = 0 

        member x.createQuickestPathTowardsTarget() = []

        member x.faceTarget
            (caller: PartyCharacter)
            (target: PartyCharacter) = 
            // Check in which direction to rotate the character to face target 
            // update the caller's direction 
            ()

        member x.moveTowardsTarget
            (caller: PartyCharacter)
            (target: PartyCharacter)
            (board: GameBoard) = 
            
            // Call createQuickestPathTowardsTarget to get path 

            // Move caller towards the target

            ()

type OffensiveStrategy = 
    | HarmClosestTarget of CommandSequence
    | HarmMostDamageableTarget of CommandSequence 

type DefensiveStrategy = 
    | MoveTowardsHealer of CommandSequence
    | MoveAwayFromDanger of CommandSequence

type ChoiceSelector() = class 
    end 