module GLSCore.CharacterAction

open GLSCore.CharacterInformation
open GLSCore.GameElement
type Command = 
    | NormalAttack of CharacterBase * CharacterBase
    | SpecialAttack of CharacterBase * CharacterBase
    | LookAtInventory of CharacterBase 
    | Defend of CharacterBase
    | RotateChar of CharacterBase
    | Move of CharacterBase
    | EndTurn of CharacterBase
//    | Undo 
//    | Redo 
// Undo/ Redo are a nice to have feature that aren't useful right now in the development phase.

type commandServer(mailbox: MailboxProcessor<Command>) = 

    let commandHandler () = 
        let rec loop(mailbox: MailboxProcessor<Command>) = 
            async {
                let! cmd = mailbox.Receive()
                match cmd with 
                | NormalAttack (caller, target) 
                | SpecialAttack (caller, target) -> 
                    target.Stats.Health.takeHit (caller.Stats.Strength |> int32) |> ignore<LifePoints> //TODO: Cannot have ignore here.
                | Defend caller -> caller.Stats.applyTemporaryDefense 10 |> ignore<CharacterStats> // TODO: Cannot have ignore here
                | _ -> () 
                // The rest will have to wait for later. 
                // A game manager server will be implemented later. 
            }

        loop mailbox


