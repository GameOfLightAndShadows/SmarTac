module GLSCore.GameMap

open GLSCore.GameElement
open GLSCore.HelperFunctions
open System

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

    member x.moveCharac (character: Object) (newPos: Pos) = () //later.

    member x.updateBoardState (board: GameBoard) = 
        () // Will be implemented later.
    
        
