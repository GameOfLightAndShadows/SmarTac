#load "HelperFunctions.fs"
#load "GameElement.fs"
open GLSCore.GameElement
#load "BrainManager.fs"
open GLSManager.BrainManager
open System

let size = { Width = 40; Height = 20 }
let player = { Position = { Top = 10; Left = 20 }; Direction = North }
let moveRange = 4 
let rng = Random ()

let gameboard = 
    [ for top in 0 .. size.Height - 1 do
        for left in 0 .. size.Width - 1 do 
                let pos = { Top = top; Left = left }
                let cell = 
                    let value = randomizer.NextDouble()             
                    if value >= 0.0 && value < 0.15 then CollectibleTreasure(HealthPotion)
                    else if value >= 0.15 && value < 0.25 then Empty 
                    else if value >= 0.25 && value < 0.5 then HiddenTrap(ReduceMoney)
                    else if value >= 0.5 && value < 0.65 then HiddenTrap(ReduceLifePoints)
                    else if value >= 0.65 && value < 0.90 then Enemy ( { Position = { Top = top; Left = left}; Direction = South })
                    else CollectibleTreasure(Currency)
                yield pos, cell]
    |> Map.ofList  

let score = 0

let initial = { Board = gameboard; Character = player; Score = score }    

let simulate (decide:Brain -> CurrentGameState -> Act) iters runs =

    let rec loop (state:GameState,brain:Brain,iter:int) =

        let currentState = visibleState moveRange size state.Board state.Character 
        let decision = PrimitiveBrain.decide brain currentState
            
        let move =  
            let value = randomizer.NextDouble()                        
            if value >= 0.0 && value < 0.25 then Up
            else if value >= 0.25 && value < 0.50 then Right
            else if value >= 0.50 && value < 0.75 then Down
            else Left

        let encounter = randomMove()  
             
        // world update
        let player = state.Character |> applyDecision size decision
        let board = updateGameBoard state.Board player
        let gain = computeScoreGain state.Board player encounter
        let score = state.Score + gain

        // learning
        let nextState = visibleState moveRange size board player
        let experience = {
            State = currentState;
            Action = decision;
            Reward = gain |> float;
            NextState = nextState; }
        let brain = learn brain experience

        let updated = { Board = board; Character = player; Score = score }
        if iter < iters
        then loop (updated,brain,iter+1)
        else score

    [ for run in 1 .. runs -> loop (initial,Map.empty,0) ]

printfn "Random decision"
let random = simulate (fun _ _ -> PrimitiveBrain.randomMove ()) 500 20
printfn "average: %.0f" (random |> Seq.averageBy float)
printfn "Crude brain"
let crudeBrain = simulate PrimitiveBrain.decide 500 20
printfn "average: %.0f" (crudeBrain |> Seq.averageBy float)
