module GLSCore.Pathfinding

open System
open GLSCore.GameMap

let rec remove l predicate =    //Remove element where predicate
    match l with
    | [] -> []
    | hd::tl -> if predicate(hd) then
                    (remove tl predicate)
                 else
                     hd::(remove tl predicate)

//The A* Algorithm
let rec aStar value g h neighbours goal start (openNodes: 'a list) (closedNodes: 'a list) =
    let f x:float = (g x)+(h x) //f will be the value we sort open nodes buy.
    let isShorter nodeA nodeB = nodeA = nodeB && f nodeA < f nodeB 
            
    let rec checkNeighbours neighbours openNodeAcc = 
        match neighbours with
        | [] -> openNodeAcc
        | currentNode::rest ->
            let likeCurrent = fun n -> (value n) = (value currentNode) //vale of n == value of current
            let containsCurrent = List.exists likeCurrent              //list contains likeCurrent
            let checkNeighbours = checkNeighbours rest 

            if openNodeAcc |> List.exists (isShorter currentNode) then //The current node is a shorter path than than one we already have.
                let shorterPath = remove openNodeAcc likeCurrent //So remove the old one...
                checkNeighbours  (currentNode::shorterPath)   //...and arry on with the new one.
            elif not(containsCurrent closedNodes) && not(containsCurrent openNodeAcc) then //The current node has not been queried
                checkNeighbours (currentNode::openNodeAcc) //So add it to the open set
            else checkNeighbours openNodeAcc // else carry on

    let nodes = neighbours openNodes.Head //The next set of nodes to work on
    
    let pathToGoal = nodes |> List.tryFind (fun x -> (value x) = goal) 
    if pathToGoal.IsSome then pathToGoal //Found the goal!
    else
        let nextSet = 
            checkNeighbours nodes openNodes.Tail
            |> List.sortBy f //sort open set by node.f
        if nextSet.Length > 0 then //Carry on pathfinding
            aStar value g h neighbours goal start nextSet (nextSet.Head::closedNodes)
        else None //if there are no open nodes pathing has failed


type PathingNode =  
        {mapPoint:MapPoint; h:float; g:float; parent:PathingNode option} //g = cost of path so far, h = estimated cost to goal, parent = tile we came here from                
//returns a pathnode based on a given map point
let pointToPathNode parent goal node = {mapPoint=node; h=node.Distance goal; g=(parent.g+1.0); parent=Some(parent)} 

//A 2D tile specific version of the A* algorithm
let pathFind (map:Map) (goal:MapPoint) = aStar (fun n-> n.mapPoint) (fun n-> n.g) (fun n-> n.h) (fun n-> (map.GetNeighboursOf n.mapPoint.point) |> List.filter(fun n-> n.value =0) |> List.map (pointToPathNode n goal.point)) goal

let rec readPath (path:PathingNode) (list:Point list) =
    match path.parent.IsNone with
    | true -> list
    | false -> readPath  path.parent.Value (path.parent.Value.mapPoint.point::list)
