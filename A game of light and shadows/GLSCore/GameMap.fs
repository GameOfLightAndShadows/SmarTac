module GLSCore.GameMap

open GLSCore.GameElement
open GLSCore.CharacterInformation
open GLSCore.HelperFunctions
open System

type GameCell = 
    | Empty 
    | CollectibleTreasure of Treasure 
    | Enemy of GameCharacter 
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
type GameBoard =  Map<Position, GameCell>
// Game State  
type GameState = { Board: GameBoard ; Character: GameCharacter; Score: int }

    let onboard (size: MapSize) (pos: Position) = {   
        Top = pos.Top %%% size.Height
        Left = pos.Left %%% size.Width    
    }

    let changeDirection (act: Act) (dir: PlayerDirection) = 
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
        (pos: Position)  = 
        match direction with 
        | North -> { pos with Top = (pos.Top - 1) %%% size.Height }
        | South -> { pos with Top = (pos.Top + 1) %%% size.Height }
        | East  -> { pos with Left = (pos.Left + 1) %%% size.Width }
        | West  -> { pos with Left = (pos.Left - 1) %%% size.Width }

    let applyDecision (mapSize: MapSize) (action: Act) (charac: GameCharacter) = 
        Console.WriteLine (action.ToString())
        match action with 
        | Up 
        | Down 
        | Left
        | Right -> 
            let newDirection = charac.CurrentDirection |> changeDirection action
            { charac with 
                CurrentPosition = charac.CurrentPosition |> moveTo mapSize newDirection
                CurrentDirection = newDirection }   
        | _ -> charac

    let computeScoreGain (board: GameBoard) (charac: GameCharacter) (action: Act) = 
        let pos = charac.CurrentPosition
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
                    match ct with //TODO: COMPLETE MATCHING
                    | HealthPotion(_) ->  HEALTHPOTIONSCORE
                    | Currency(_)     ->  CURRENCYSCORE
            | None -> 0

        scoreGain 

    let updateGameBoard (board: GameBoard) (character: GameCharacter) = 
        board |> Map.filter(fun position _ -> position <> character.CurrentPosition)


         

type Point = 
            {x:int;y:int}   
             static member (-) (a :Point , b :Point) = {x=a.x-b.x ; y=a.y-b.y}
             static member (+) (a :Point , b :Point) = {x=a.x+b.x ; y=a.y+b.y}
             static member (*) (a :Point , b :int) = {x=a.x*b ; y=a.y*b}             
             static member (/) (a :Point , b :int) = {x=a.x/b ; y=a.y/b} 

type MapPoint =             
    {point:Point;value:int}                                     
    member this.Distance mp = sqrt (powerOfN(this.point.x+mp.x) 2 + powerOfN (this.point.y+mp.y) 2) //abs(this.x-mp.x)+abs(this.y-mp.y) //Calculate distance to other map point

type Map =  //Simple construct to hold the 2D map data
    {width:int; height:int; map:int list} //Width & Height of map and map data in 1D array
    member this.GetElement x y = {point = {x=x;y=y}; value=this.map.[x % this.height + y * this.width]} //function to wrap 1D array into 2D array to retrive map point
    member this.GetElementP p = {point = p; value=this.map.[p.x % this.height + p.y * this.width]} //function to wrap 1D array into 2D array to retrive map point
    member this.GetNeighboursOf p =  //return list of map points that surround current map point
        [   for y in p.y-1..p.y+1 do
                for x in p.x-1..p.x+1 do
                    if ((y<>p.y || x <>p.x) && y>=0 && x>=0 && x<this.width && y<this.height) //bounds checking
                    then yield this.GetElement  x y]

type GameBoardState(size: MapSize) = 
    
    member private x.size = size 

    member x.moveCharac (character: Object) (newPos: Position) = () //later.

    member x.updateBoardState (board: GameBoard) = 
        () // Will be implemented later.
    
        
