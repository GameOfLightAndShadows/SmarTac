module GLSCore.GameElement

open System
open GLSCore.HelperFunctions

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


// Actions 
type Act = 
    | MeleeAttack 
    | ClassAttack 
    | Left
    | Right 
    | Up 
    | Down
    | RaiseDefense
    | EndTurn 

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

    member x.switchOppositeDirection () = 
        match x with 
        | South -> North 
        | North -> South 
        | East -> West 
        | West -> East 

// Board Cells 
type MovementCost = 
    | Minimum
    | Moderate
    | ``No cost because is impassible``

type TileType = 
    | Normal of MovementCost
    | Difficult of MovementCost
    | ``Road block`` of MovementCost

type Treasure = 
    | HealthPotion  
    | Currency      

type Trap = 
    | ReduceLifePoints
    | ReduceMoney     

type GameCell = 
    | Empty 
    | CollectibleTreasure of Treasure 
    | Enemy
    | HiddenTrap of Trap
    
// Game Board Size 
type MapSize = { Width: int; Height: int }

// Position 
type Pos = { Top: int; Left:int }

// AI Character

type Character = { Position: Pos; Direction: PlayerDirection }

// Game Board 

type GameBoard = {
    GameMap: Map<Pos, GameCell>
}
with 
    static member InitialBoard() = 
        { GameMap = Map.empty }

// Game State  

type GameState = { Board: GameBoard ; Characters: Character list; Score: int }

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
        
    let onboard (size: MapSize) (pos: Pos) = {   
        Top = pos.Top %%% size.Height
        Left = pos.Left %%% size.Width    
    }

    let changeDirection (act: Act) (dir: PlayerDirection) = 
        match act with 
        | Up 
        | Down
        | Right 
        | Left ->  dir.switchOppositeDirection()
        | _ -> dir 

    let moveTo 
        (size: MapSize) 
        (direction: PlayerDirection)
        (pos: Pos)  = 
        match direction with 
        | North -> { pos with Top = (pos.Top - 1) %%% size.Height }
        | South -> { pos with Top = (pos.Top + 1) %%% size.Height }
        | East  -> { pos with Left = (pos.Left + 1) %%% size.Width }
        | West  -> { pos with Left = (pos.Left - 1) %%% size.Width }

    let applyMoveDecision (mapSize: MapSize) (action: Act) (charac: Character) = 
        let newDirection = charac.Direction |> changeDirection action 
        { Position = charac.Position |> moveTo mapSize newDirection
          Direction = newDirection }

    let computeScoreGain (board: GameBoard) (charac: Character) = 
        let pos = charac.Position
        match board.GameMap.TryFind pos with 
        | Some cell -> 
            match cell with
            | Empty 
            | Enemy -> 0
            | HiddenTrap t -> 
                match t with 
                | ReduceLifePoints -> REDUCELIFEPOINTSCORE
                | ReduceMoney      -> REDUCECURRENCYSCORE
            | CollectibleTreasure ct -> 
                match ct with 
                | HealthPotion ->  HEALTHPOTIONSCORE
                | Currency     ->  CURRENCYSCORE
        | None -> 0

    let updateGameBoard (board: GameBoard) (character: Character) = 
        board.GameMap |> Map.filter(fun position _ -> position <> character.Position)
            


        



