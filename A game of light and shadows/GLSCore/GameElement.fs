module GLSCore.GameElement

open System

[<Measure>] type abscissa
[<Measure>] type ordinate

type CharacterState = 
    | Alive 
    | Dead

type BattleSequencePhase= 
    | Move 
    | ``Attack or Defend``
    | Rotate 
    | EndTurn
    | CheckInventory 


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

// Direction
type PlayerDirection = 
    | South
    | North 
    | East 
    | West
with 
    member x.areDirectionOpposite (dir: PlayerDirection) = 
        match x,dir with 
        | South, North  
        | North, South  
        | East, West  
        | West, East -> true 
        | _,_ -> false

// Actions 
type Act = 
    | MeleeAttack 
    | ClassAttack 
    | Left
    | Right 
    | Straight 
    | Back 
    | RaiseDefense
    | Rotate 
    | EndTurn 

// Position 
type Pos = {
    Top: int
    Left:int
}



// Board Cells 
type MovementCost = 
    | Minimum
    | Moderate
    | ``No cost because is impassible``

type TileType = 
    | Normal of MovementCost
    | Difficult of MovementCost
    | ``Road block`` of MovementCost

type GameCell = 
    | Tile of TileType
    | Treasure of RandomTreasure

// AI Character

type Character = { Position: Pos; Direction: PlayerDirection; Cell: GameCell }

// Game Board 

type GameBoard = {
    GameMap: Map<Pos, GameCell>
}
with 
    static member InitialBoard() = 
        { GameMap = Map.empty }

// Game State  

type GameState = { Board: GameBoard ; Characters: Character list; Score: int }

// Game Board Size 
type MapSize = { Width: int; Height: int }
