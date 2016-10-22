module GameElementTests

open System 

open FSharp.Core

open NUnit.Framework

open GLSCore.GameElement

let [<Test>] ``When max health is N, health cannot be bigger than N``() = 
    let n = 100.0
    let lifePoints = { Current = n; Max = n}

    Assert.AreEqual(lifePoints.Current, (lifePoints.raiseHealth 100).Current)

let [<Test>] ``When max health is 0, character is dead``() = 
    let n = 100.0
    let lifePoints = { Current = n; Max = n}
    let deadLifePoints = lifePoints.takeHit 100 

    Assert.AreEqual(true, deadLifePoints.isDead)

//let [<Test>] ``Score gain is positive when we find currency``() = 
//        let character = { Position = { Top = 10; Left = 10 }; Direction = East;  } 
//        let randomizer = Random () 
//        let size = { Width = 20; Height = 20 }
//
//        let gameboard = 
//            [ for top in 0 .. size.Height - 1 do
//                for left in 0 .. size.Width - 1 do 
//                        let pos = { Top = top; Left = left }
//                        let cell = CollectibleTreasure(Currency)
//                        yield pos, cell]
//            |> Map.ofList  
//
//        let score = 0      
//        let state = { Board = gameboard; Character = character; Score = score }  
//
//        let encounter = [| Up; Down; Left; Right; RaiseDefense; MeleeAttack; SpecialMove |]
//                            .[randomizer.Next(7)]
//           
//        // world update
//        let player = state.Character |> applyDecision size encounter
//        let board = updateGameBoard state.Board player
//        let gain = computeScoreGain state.Board player encounter
//        let score = state.Score + gain
//
//
//        Assert.AreEqual(100, score)