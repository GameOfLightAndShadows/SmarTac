module GLSCore.CharacterAction

open GLSCore.CharacterInformation

type Command = 
    | NormalAttack of CharacterBase * CharacterBase
    | SpecialAttack of CharacterBase * CharacterBase
    | LookAtInventory of CharacterBase * CharacterBase
    | Defend of CharacterBase
    | RotateChar of CharacterBase
    | Move of CharacterBase
    | EndTurn of CharacterBase
    | Undo 
    | Redo 


type CommandPerformedHandler() = class
    end 