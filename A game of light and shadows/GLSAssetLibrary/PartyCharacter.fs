module GLSAsset.PartyCharacter

open GLSAsset.CharacterInformation
open System

(* - After Move, player will be able to choose any action
   - After Attack, player won't be able to use move or defend
   - After Defend, player won't be able to use move or atttack 
   - After Rotate, player won't be able to move 
   - After EndTurn, battle sequence will move to the next character
   - CheckInventory can be use at any time during the character's turn 
*)
type BattleSequenceAction = 
    | Move 
    | Attack 
    | Defend 
    | Rotate 
    | EndTurn
    | CheckInventory 

[<AbstractClass>]
type CharacterBase(job: CharacterJob) =

    abstract member TeamParty: Object array

    abstract member ActionPoints: int

    abstract member Direction: PlayerDirection

    abstract member CurrentCoordinates: int * int

    abstract member MoveRange: int * int

    abstract member CanDoExtraDamage: unit -> bool

    abstract member Action: BattleSequenceAction -> BattleSequenceAction

    member x.Job = job

    member x.Stats = job.Stats

[<AbstractClass>] // Abstract for now.
type PartyCharacter(job: CharacterJob) = 
    inherit CharacterBase(job)