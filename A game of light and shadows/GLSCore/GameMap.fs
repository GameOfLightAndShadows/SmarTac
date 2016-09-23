module GLSCore.GameMap

open GLSCore.GameElement
open System

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

type MapSize = {
    Width   : int 
    Height  : int
}

type GameBoard = {
    GameMap: Map<CharacPos, GameCell>
}
with 
    static member InitialBoard() = 
        { GameMap = Map.empty }

type GameBoardState(size: MapSize) = 
    
    member private x.size = size 

    member x.moveCharac (character: Object) (newPos: CharacPos) = () //later.

    member x.updateBoardState (board: GameBoard) = 
        () // Will be implemented later.
    
        
