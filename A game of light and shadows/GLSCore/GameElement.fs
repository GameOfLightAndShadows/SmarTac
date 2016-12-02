module GLSCore.GameElement

open System
open GLSCore.HelperFunctions
open GLSCore.GameItemsModel
open GLSCore.GameItemsModel.GameItems
open GLSCore.GameItemsModel.Units
open GLSCore.GameItemsModel.ExccesItems
open GLSCore.GameItemsModel.ConsummableItems
open GLSCore.GameItemsModel.CharacterWearableProtection
open GLSCore.GameItemsModel.Energy
open GLSCore.GameItemsModel.Weapons

open Akka.Actor

type BattleSequencePhase = 
    | Move 
    | ``Attack or Defend``
    | Rotate 
    | EndTurn
    | CheckInventory 

type MoveRange = {
    Horizontal : int<abscissa>
    Vertical : int<ordinate>
}

// Actions 
type Act = 
    | Up
    | Down 
    | Left 
    | Right
    | MeleeAttack 
    | SpecialMove 
    | RaiseDefense
    | EndTurn 
with 
    override x.ToString() = 
        match x with 
        | MeleeAttack   -> "Melee attack"
        | SpecialMove   -> "Special Move"
        | Left          -> "Going left"
        | Right         -> "Going right"
        | Up            -> "Going  up"
        | Down          -> "Going down"
        | RaiseDefense  -> "Temporally raised defense"
        | EndTurn       -> "Turn completed"

// Board Cells 
type MovementCost = 
    | Minimum
    | Moderate
    | ``No cost because is impassible``

type TileType = 
    | Normal of MovementCost
    | Difficult of MovementCost
    | ``Road block`` of MovementCost
    | TreasureChest of MovementCost

type Treasure = 
    | Health of ConsumableItem
    | Mana of ConsumableItem 
    | MagicFeather of ConsumableItem
    | RareStaff of Staff
    | RareBlade of Blade
    | Currency of float<usd>
with 
    member x.Description = 
        match x with 
        | Currency c -> sprintf "You have found %O !" c
        | Health hp -> hp.Name
        | Mana mp -> mp.Name
        | MagicFeather pf -> pf.Name 
        | RareStaff rs -> rs.Name 
        | RareBlade rb -> rb.Name

type Trap = 
    | ReduceLifePoints
    | ReduceMoney     
with 
    override x.ToString() = 
        match x with 
        | ReduceLifePoints -> "Reduce life points"
        | ReduceMoney      -> "Reduce money"

// Based on Machine Learning Projects for .NET developpers
[<AutoOpen>]
module GameLogic = 
    [<Literal>]
    let HEALTHPOTIONSCORE = 50 

    [<Literal>]
    let CURRENCYSCORE = 100 

    [<Literal>]
    let REDUCELIFEPOINTSCORE = -125 

    [<Literal>]
    let REDUCECURRENCYSCORE = -75 

    [<Literal>]
    let MELEEATTACKSCORE = 75

    [<Literal>]
    let RAISEDEFENSESCORE = 25

    [<Literal>]
    let SPECIALMOVESCORE = 150 
        