module GameConsoleApp

open System
open System.Threading 
open GLSCore.GameElement

module Program = 
    
    [<EntryPoint>]
    let main argv =
        // Init world 
        let size = { Width = 64; Height = 64 }

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
        
        // Random Decision making 
        let moveChoices = [| Up; Down; Left; Right |]
        let choices = [| MeleeAttack; SpecialMove; RaiseDefense; |]
        
        let moveDecision() = moveChoices.[randomizer.Next(4)]
        let encounterDecision() = choices.[randomizer.Next(3)]

        // Game Loop 
        let rec loop(state: GameState) = 
            
            let move =  
                let value = randomizer.NextDouble()                        
                if value >= 0.0 && value < 0.25 then Up
                else if value >= 0.25 && value < 0.50 then Right
                else if value >= 0.50 && value < 0.75 then Down
                else Left

            let encounter = encounterDecision()

            // world update 
                
            let player = state.Character |> applyDecision size move
            let board = updateGameBoard state.Board player
            let gain = computeScoreGain state.Board player encounter
            let score = state.Score + gain
            Console.WriteLine(score) 
            Console.WriteLine(player.Position.ToString())
            Console.WriteLine(player.Direction.ToString())
            let updated = { Board = board; Character = player; Score = score }

            Console.ReadLine() |> ignore<string>
            Thread.Sleep 5 
            loop updated

        let _ = loop gameState

        0


//            loop updated 
//        for i= 1 to 100 do 
//            Console.WriteLine(loop gameState)
        
        Console.ReadLine()
        0
        
               