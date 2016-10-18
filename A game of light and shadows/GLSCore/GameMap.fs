module GLSCore.GameMap

open GLSCore.GameElement
open System


type GameBoardState(size: MapSize) = 
    
    member private x.size = size 

    member x.moveCharac (character: Object) (newPos: Pos) = () //later.

    member x.updateBoardState (board: GameBoard) = 
        () // Will be implemented later.
    
        
