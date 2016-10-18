module GLSManager.BrainManager

open GLSCore.HelperFunctions
open GLSCore.GameElement
open System


module Brains =
    type CurrentGameState = PlayerDirection * (GameCell option) list

    type Experience = { State: CurrentGameState; Action: Act; Reward: float; NextState: CurrentGameState } //SARSA

    type Strategy = { State: CurrentGameState; Action: Act } //Possible course of action

    type Brain = Map<Strategy, float> //Strategies tried and rewards collected in the process 

    let randomizer = Random ()
    let moveChoices = [| Up; Down; Left; Right |]
    let encounterChoices = [| MeleeAttack; SpecialMove; RaiseDefense; |]
    let randomDecideMove () = moveChoices.[randomizer.Next(4)]
    let randomDecideEncounter () = encounterChoices.[randomizer.Next(3)]


    let alpha = 0.30 //learning rate

    let learn(brain: Brain) (xp: Experience) = 
        let s = { State = xp.State; Action = xp.Action }
        let oStrat = brain |> Map.tryFind s
        match oStrat with 
        | Some reward -> 
            brain |> Map.add (s, 1.0-alpha * reward + alpha * xp.Reward )
        | None -> 
            brain |> Map.add (s, alpha * xp.Reward)

    let 



