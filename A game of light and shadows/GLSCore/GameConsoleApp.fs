module GameConsoleApp

open System
open System.Threading 
open GLSCore.GameElement
open GLSCore.CharacterInformation
open GLSCore.GameItemsModel
open GLSCore.GameMap
open GLSManager.BrainManager

module Program = 

    let cellValues = [| -200; -150; -100; -50; 50; 100; 150; 200; 250 |] 
    type Board = int[,]
    let randomizer = new Random()

    let updateBoard (board: Board) (player:BrainCharacter) = 
        let pos = player.Position
        let updatedBoard = Array2D.copy board   
        updatedBoard.[pos.Left,pos.Top] <- randomizer.Next(cellValues.Length)
        updatedBoard

    type State = int list

    let activeCell (board: Board) (pos: Position) = board.[pos.Left, pos.Top]

    let initBoard (size: MapSize) = Array2D.init size.Width size.Height (fun left top -> randomizer.Next(cellValues.Length))
    
    [<EntryPoint>]
    let main argv =
        // Init world 
        let size = { Width = 64; Height = 64 }
        let characterMoveRange = 4
        let initialCharacter = BrainCharacter.InitialBrainCharacter
        let character = { initialCharacter with Position = { Top = 10; Left = 10 }; Direction = East;  } 
        let randomizer = Random () 

        let gameboard = 
            [ for top in 0 .. size.Height - 1 do
                for left in 0 .. size.Width - 1 do 
                        let pos = { Top = top; Left = left }
                        let cell = 
                            let value = randomizer.NextDouble()             
                            if value >= 0.0 && value < 0.15 then CollectibleTreasure(Health(HealthPotion))
                            else if value >= 0.15 && value < 0.25 then Empty 
                            else if value >= 0.25 && value < 0.5 then HiddenTrap(ReduceMoney)
                            else if value >= 0.5 && value < 0.65 then HiddenTrap(ReduceLifePoints)
                            else if value >= 0.65 && value < 0.90 then Enemy ( HumanCharacter.InitialGameCharacter ) // The found enemy doesn't matter now for the training purposes !!!
                            else CollectibleTreasure(Currency(0.00<usd>)) // The amount of money doesn't matter for the training purposes 
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
            let gain = computeScoreGain state.Board player encounter
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