module GLSCore.GameMap

open GLSCore.GameElement
open GLSCore.GameItemsModel
open GLSCore.CharacterInformation
open GLSCore.HelperFunctions
open System

type GameCell = 
    | Empty 
    | CollectibleTreasure of Treasure 
    | Character of IGameCharacter 
    | HiddenTrap of Trap
with 
    override x.ToString() = 
        match x with 
        | Empty                 -> "Empty Cell"
        | CollectibleTreasure s -> sprintf "Treasure %O" s
        | Character               e -> sprintf "Enemy %O" e
        | HiddenTrap          h -> sprintf "Trap %O" h

// Game Board Size 
type MapSize = { Width: int; Height: int }
// Game Board 
type GameBoard =  Map<Position, GameCell>
// Game State  
type GameState = { Board: GameBoard ; Character: BrainCharacter; Score: int }

[<Literal>] 
let MaxWidth = 100

[<Literal>] 
let MaxHeight = 100

let generateGameboard = 
    let size = { Width = MaxWidth; Height = MaxHeight }
    let randomizer = Random () 
    [ for top in 0 .. size.Height - 1 do
        for left in 0 .. size.Width - 1 do 
                let pos = { Top = top; Left = left }
                let cell = 
                    let value = randomizer.NextDouble()             
                    if value >= 0.0 && value < 0.15 then CollectibleTreasure(Health(HealthPotion))
                    else if value >= 0.15 && value < 0.25 then Empty 
                    else if value >= 0.25 && value < 0.5 then HiddenTrap(ReduceMoney)
                    else if value >= 0.5 && value < 0.65 then HiddenTrap(ReduceLifePoints)
                    else if value >= 0.65 && value < 0.90 then Character ( HumanCharacter.InitialGameCharacter ) // The found enemy doesn't matter now for the training purposes !!!
                    else CollectibleTreasure(Currency(30.00<usd>)) // The amount of money doesn't matter for the training purposes 
                yield pos, cell]
    |> Map.ofList  

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

let applyDecision (mapSize: MapSize) (action: Act) (charac: BrainCharacter) = 
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
    | _ -> charac

let updateGameBoard (board: GameBoard) (character: BrainCharacter) = 
    board 
        |> Map.filter(fun position _ -> position <> character.Position)  

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
    
        
