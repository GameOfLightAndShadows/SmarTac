module GameConsoleApp

open System
open System.Threading 
open GLSCore.GameElement
open GLSManager.BrainManager

module Program = 
    
    [<EntryPoint>]
    let main argv =
        // Init world 
        let size = { Width = 64; Height = 64 }
        let characterMoveRange = 4
        let character = { Position = { Top = 10; Left = 10 }; Direction = East;  } 
        let randomizer = Random () 

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
        let gameState = { Board = gameboard; Character = character; Score = score }    

        let rec loop (state:GameState,brain:Brain) =

            let currentState = visibleState characterMoveRange size state.Board state.Character 
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
            let gain = computeScoreGain state.Board player decision
            let score = state.Score + gain

            // learning
            let nextState = visibleState characterMoveRange size board player
            let experience = {
                State = currentState;
                Action = decision;
                Reward = gain |> float;
                NextState = nextState; }
            let brain = learn brain experience

            let updated = { Board = board; Character = player; Score = score }
            
            Thread.Sleep 20
            let result = sprintf "Action : %O - Reward : %f - Score: %d " experience.Action experience.Reward score
            Console.Write(result)
            Console.ReadLine() |> ignore
            loop (updated,brain)

        let _ = loop (gameState,Map.empty)

        Console.ReadLine() |> ignore

        0