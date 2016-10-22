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
            
open PrimitiveBrain 

module AdvancedBrain = 


    let rotate dir (x,y) = 
        match dir with 
        | North -> (x,y)
        | South -> (-x,-y)
        | West -> (-y,x)
        | East -> (y,-x)

    // Modifies the perception oh the character of the game board when he changes direction
    let visibleState (size: MapSize) (moveRange:int) (board: GameBoard) (player: Character) = 
        let (dir, pos) = player.Direction, player.Position
        offsets moveRange
        |> List.map (rotate dir )
        |> List.map (fun (x,y) -> 
            onboard size { Top = pos.Top + x ; Left = pos.Left + y }
            |> activeCell board)
         


    let nextValue (brain: Brain) (state: CurrentGameState) = 
        options 
        |> Seq.map(fun action -> 
            match brain.TryFind { State = state; Action = action } with 
            | Some value -> value
            | None -> 0.0)

        |> Seq.max

    let alpha= 0.1 // learning rate
    let gamma = 0.3 //discount rate
    let epsilon = 0.07 // random learning

    let learn (brain: Brain) (exp: Experience) = 
        let strat = { State = exp.State; Action = exp.Action }
        let nValue = nextValue brain exp.NextState
        match brain.TryFind strat with 
        | Some v ->     
            let newV = (1.0 - alpha) * v + alpha * (exp.Reward + gamma * nValue)
            brain.Add (strat, newV)
        | None -> 
            brain.Add (strat, alpha * (exp.Reward + gamma* nValue))

    let decide (brain: Brain) (state: CurrentGameState) = 
        if randomizer.NextDouble() < epsilon then 
            randomMove()
        else 
            let eval = 
                options 
                |> Array.map (fun alt ->  { State = state; Action = alt })
                |> Array.filter( fun strat -> brain.ContainsKey strat)

            match eval.Length with 
            | 0 -> randomMove() 
            | _ -> 
                options
                |> Seq.maxBy(fun alt -> 
                    let strat = { State = state; Action = alt }
                    match brain.TryFind strat with 
                    | Some v -> v 
                    | None -> 0.0
                )


