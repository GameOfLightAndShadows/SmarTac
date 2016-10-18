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

module CharacterInformation = 
    type CharacterStats = {
        Health          : LifePoints 
        Speed           : float 
        Strength        : float 
        MagicPower      : float option 
        Defense         : int
        Resistance      : int
        MagicResist     : float  
        Evade           : int 
        Luck            : int
    }
    with 
        member x.applyTemporaryDefense (tPoints: int) = 
            { x with Defense = x.Defense + tPoints }

        member x.updateLifePoints (lp: LifePoints) = 
            { x with Health = lp }

    type CharacterRole = 
        | Wizard of MoveRange
        | Knight of MoveRange
        | Fighter of MoveRange
        | MagicSoldier of MoveRange 
        | Sniper  of MoveRange
    with    
        member x.moveRange = 
            match x with 
            | Wizard mr -> mr 
            | Knight mr -> mr 
            | Fighter mr -> mr 
            | MagicSoldier mr -> mr 
            | Sniper  mr -> mr

    type CharacterJob = 
        | Healer        of CharacterRole  * CharacterStats
        | Knight        of CharacterRole  * CharacterStats
        | Berserker     of CharacterRole  * CharacterStats
        | Rider         of CharacterRole  * CharacterStats
        | Paladin       of CharacterRole  * CharacterStats
        | BowAndBlade   of CharacterRole  * CharacterStats
        | Necromancer   of CharacterRole  * CharacterStats
        | Nightblade    of CharacterRole  * CharacterStats
    with 
        member x.Stats = 
            match x with
            | Healer       (_,stats) -> stats
            | Knight       (_,stats) -> stats
            | Berserker    (_,stats) -> stats
            | Rider        (_,stats) -> stats
            | Paladin      (_,stats) -> stats
            | BowAndBlade  (_,stats) -> stats
            | Necromancer  (_,stats) -> stats
            | Nightblade   (_,stats) -> stats

        member x.Role = 
            match x with
            | Healer       (role, _) -> role
            | Knight       (role, _) -> role
            | Berserker    (role, _) -> role
            | Rider        (role, _) -> role
            | Paladin      (role, _) -> role
            | BowAndBlade  (role, _) -> role
            | Necromancer  (role, _) -> role
            | Nightblade   (role, _) -> role


    type CharacterEquipement = {
        Hat         : string 
        Gauntlets   : string 
        Armor       : string 
        Weapon      : string 
        Pants       : string 
        Boots       : string
    }

    type CombatStyle = 
        | DualWielder of int
        | MaceUser of int 
        |  ``Sword and shield`` of int
        | Archer of int 
        | ``Staff wielder`` of int
        | Polyvalent of int
    with 
        member x.actionPoints = 
            match x with 
            | DualWielder ap -> ap 
            | MaceUser ap -> ap
            | Archer ap -> ap 
            | Polyvalent ap -> ap 
            | ``Sword and shield`` ap -> ap 
            | ``Staff wielder`` ap -> ap 

open CharacterInformation 

type MoveAction = 
    | Up 
    | Down 
    | Left 
    | Right 
with 
    override x.ToString() =
        match x with
        | Left          -> "Going left"
        | Right         -> "Going right"
        | Up            -> "Going  up"
        | Down          -> "Going down"

// Actions 
type Act = 
    | Move of MoveAction
    | MeleeAttack 
    | SpecialMove 
    | RaiseDefense
    | EndTurn 
with 
    override x.ToString() = 
        match x with 
        | MeleeAttack   -> "Melee attack"
        | SpecialMove   -> "Special Move"
        | Move m        -> m.ToString()
        | RaiseDefense  -> "Temporally raised defense"
        | EndTurn       -> "Turn completed"
// Position 
type Pos = 
    { Top: int; Left:int }
with 
    override x.ToString() = sprintf "(Top %d, Left %d)" x.Top x.Left

// Direction
type PlayerDirection = 
    | South
    | North 
    | East 
    | West
with 
    override x.ToString() = 
        match x with 
        | South -> "South"
        | North -> "North"
        | East ->  "East"
        | West ->  "West"

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

// Character
type Character = {   
        Position: Pos
        Direction: PlayerDirection
//        Role: CharacterJob
//        Equipment: CharacterEquipement
//        CombatStyle: CombatStyle
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

type Treasure = 
    | HealthPotion  
    | Currency  
with 
    override x.ToString() = 
        match x with 
        | HealthPotion -> "Health potion"
        | Currency     -> "Currency"    

type Trap = 
    | ReduceLifePoints
    | ReduceMoney     
with 
    override x.ToString() = 
        match x with 
        | ReduceLifePoints -> "Reduce life points"
        | ReduceMoney      -> "Reduce money"

type GameCell = 
    | Empty 
    | CollectibleTreasure of Treasure 
    | Enemy of Character 
    | HiddenTrap of Trap
with 
    override x.ToString() = 
        match x with 
        | Empty                 -> "Empty Cell"
        | CollectibleTreasure s -> sprintf "Treasure %O" s
        | Enemy               e -> sprintf "Enemy %O" e
        | HiddenTrap          h -> sprintf "Trap %O" h
// Game Board Size 
type MapSize = { Width: int; Height: int }

// Game Board 
type GameBoard =  Map<Pos, GameCell>

// Game State  
type GameState = { Board: GameBoard ; Character: Character; Score: int }

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
        
    let onboard (size: MapSize) (pos: Pos) = {   
        Top = pos.Top %%% size.Height
        Left = pos.Left %%% size.Width    
    }

    let changeDirection (act: MoveAction) (dir: PlayerDirection) = 
        match act with 
        | Right -> 
            match dir with 
            | North -> East 
            | East -> South 
            | South -> West 
            | West -> North 
             
        | Left ->  
            match dir with 
            | North -> West 
            | West -> South 
            | South -> East 
            | East -> North 
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

//    let encounterEnemyDecision (character: Character) (enemy: Character) (action: Act) = 
//        match action with 
//        | MeleeAttack  -> 
//            enemy.Role.Stats.updateLifePoints <| enemy.Role.Stats.Health.takeHit (character.Role.Stats.Strength |> int32) |> ignore<CharacterStats>      
//        | RaiseDefense ->
//             character.Role.Stats.applyTemporaryDefense 50 |> ignore<CharacterStats>
//        | _ -> 
//            ()
//        character  

    let applyDecision (mapSize: MapSize) (action: MoveAction) (charac: Character) = 
        Console.WriteLine (action.ToString())
        match action with 
        | Up 
        | Down 
        | Left
        | Right -> 
            let newDirection = charac.Direction |> changeDirection action
            { charac with 
                Position = charac.Position |> moveTo mapSize newDirection
                Direction = newDirection }           

    let computeScoreGain (board: GameBoard) (charac: Character) (action: Act) = 
        let pos = charac.Position
        let oCell = board |> Map.tryFind pos 
        let scoreGain = 
            match oCell with 
            | Some cell -> 
                Console.WriteLine (sprintf "Type of cell: %O" cell)
                match cell with
                | Empty -> 
                    0
                | Enemy e -> 
                    match action with 
                    | MeleeAttack  -> MELEEATTACKSCORE 
                    | RaiseDefense -> RAISEDEFENSESCORE
                    | SpecialMove  -> SPECIALMOVESCORE
                    | _ -> 0
                | HiddenTrap t -> 
                    match t with 
                    | ReduceLifePoints -> REDUCELIFEPOINTSCORE
                    | ReduceMoney      -> REDUCECURRENCYSCORE
                | CollectibleTreasure ct -> 
                    match ct with 
                    | HealthPotion ->  HEALTHPOTIONSCORE
                    | Currency     ->  CURRENCYSCORE
            | None -> 0

        scoreGain 

    let updateGameBoard (board: GameBoard) (character: Character) = 
        board |> Map.filter(fun position _ -> position <> character.Position)
         