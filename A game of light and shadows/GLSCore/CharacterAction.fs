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

