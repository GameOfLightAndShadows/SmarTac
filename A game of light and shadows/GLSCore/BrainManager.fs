module GLSManager.BrainManager

open GLSCore.HelperFunctions
open GLSCore.GameElement
open System

[<AutoOpen>]
module PrimitiveBrain =
    type CurrentGameState = PlayerDirection * (GameCell option) list

    type Experience = { State: CurrentGameState; Action: Act; Reward: float; NextState: CurrentGameState } //SARSA

    type Strategy = { State: CurrentGameState; Action: Act } //Possible course of action

    type Brain = Map<Strategy, float> //Strategies tried and rewards collected in the process 

    let randomizer = Random ()
    let options = [| Left; Right; Up; Down; MeleeAttack; SpecialMove; RaiseDefense; |]
    let randomMove () = options.[randomizer.Next(7)]


    let alpha = 0.30 //learning rate

    let learn(brain: Brain) (xp: Experience) = 
        let strat = { State = xp.State; Action =xp.Action }
        match brain.TryFind strat with 
        | Some value -> 
            brain.Add (strat, (1.0 - alpha) * value + alpha * xp.Reward)
        | None -> brain.Add (strat, (alpha * xp.Reward))

    let decide (brain: Brain) (state: CurrentGameState) = 
        let oldStrategies =  
            options
            |> Array.map (fun choice -> { State = state; Action = choice })
            |> Array.filter(fun s -> brain.ContainsKey s)

        if oldStrategies.Length = 0 then randomMove()
        else 
            options
            |> Seq.maxBy (fun choice -> 
                let s = { State = state; Action = choice }
                let oStrategy = brain |> Map.tryFind s
                match oStrategy with 
                | Some v -> v 
                | None -> 0.0    
            )

    let activeCell (board: GameBoard) (pos: Pos) = board |> Map.tryFind pos

    let offsets (range: int) = 
        [ 
            for i = -1 * range to range do
                for j = -1 * range to range do
                    yield i, j 
            ]

    let visibleState 
        (range:int)
        (size: MapSize) 
        (board: GameBoard) 
        (character: Character) = 
        let dir, pos = character.Direction, character.Position
        let visibleCells = 
            offsets range
            |> List.map(fun (x,y) -> 
                onboard size { Top = pos.Top + x; Left = pos.Left + y }
                |> activeCell board
            
            )
        dir, visibleCells
            



