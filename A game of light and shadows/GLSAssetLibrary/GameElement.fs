module GLSCore.GameElement

open System

[<Measure>] type abscissa
[<Measure>] type ordinate

type CharacterState = 
    | Alive 
    | Dead

type PlayerDirection = 
    | South
    | North 
    | East 
    | West

type BattleSequencePhase= 
    | Move 
    | ``Attack or Defend``
    | Rotate 
    | EndTurn
    | CheckInventory 

type CharacPos = {
    Top: int
    Left:int
}

type MoveRange = {
    Horizontal : int<abscissa>
    Vertical : int<ordinate>
}

type RandomTreasure = 
    | Weapon 
    | Potion
    | Currency

type LifePoints ={
    Current : float 
    Max     : float
}
with 
    member x.isDead = x.Current < 0.0

    member x.capHealth () =
        if x.Current > x.Max then { Current = x.Max; Max = x.Max } else x

    member x.raiseHealth (lifePoint:int) = 
        let raisedHealth = { x with Current = x.Current + (lifePoint |> float) }.capHealth()
        raisedHealth

    member x.takeHit (hitPoint: int) = 
        let reducedLife = { x with Current = x.Current - (hitPoint |> float) }
        reducedLife

