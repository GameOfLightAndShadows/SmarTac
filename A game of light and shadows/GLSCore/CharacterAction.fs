module GLSCore.CharacterAction

open GLSCore.CharacterInformation
open GLSCore.GameElement

type EngageAction = 
    | AttackedTarget
    | EvadeAttacker
    | EliminatedTarget
    | BlockedAttacker
    | NoAction

type Command = 
    | NormalAttack of CharacterBase * CharacterBase
    | SpecialAttack of CharacterBase * CharacterBase
    | LookAtInventory of CharacterBase 
    | Defend of CharacterBase
    | RotateChar of CharacterBase
    | Move of CharacterBase
    | EndTurn of CharacterBase

