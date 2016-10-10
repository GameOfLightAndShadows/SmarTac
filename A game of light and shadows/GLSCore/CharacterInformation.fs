module GLSCore.CharacterInformation

open GLSCore.GameElement

open System

type CharacterStats = {
    Health          : LifePoints 
    Speed           : float 
    Strength        : float 
    MagicPower      : float option 
    Defense         : int
    Resistance      : int
    MagicResist     : float  
    Evade           : int 
    Luck            : int
}
with 
    member x.applyTemporaryDefense (tPoints: int) = 
        { x with Defense = x.Defense + tPoints }

type CharacterRole = 
    | Wizard of MoveRange
    | Knight of MoveRange
    | Fighter of MoveRange
    | MagicSoldier of MoveRange 
    | Sniper  of MoveRange
with    
    member x.moveRange = 
        match x with 
        | Wizard mr -> mr 
        | Knight mr -> mr 
        | Fighter mr -> mr 
        | MagicSoldier mr -> mr 
        | Sniper  mr -> mr

type CharacterJob = 
    | Healer        of CharacterRole  * CharacterStats
    | Knight        of CharacterRole  * CharacterStats
    | Berserker     of CharacterRole  * CharacterStats
    | Rider         of CharacterRole  * CharacterStats
    | Paladin       of CharacterRole  * CharacterStats
    | BowAndBlade   of CharacterRole  * CharacterStats
    | Necromancer   of CharacterRole  * CharacterStats
    | Nightblade    of CharacterRole  * CharacterStats
with 
    member x.Stats = 
        match x with
        | Healer       (_,stats) -> stats
        | Knight       (_,stats) -> stats
        | Berserker    (_,stats) -> stats
        | Rider        (_,stats) -> stats
        | Paladin      (_,stats) -> stats
        | BowAndBlade  (_,stats) -> stats
        | Necromancer  (_,stats) -> stats
        | Nightblade   (_,stats) -> stats

    member x.Role = 
        match x with
        | Healer       (role, _) -> role
        | Knight       (role, _) -> role
        | Berserker    (role, _) -> role
        | Rider        (role, _) -> role
        | Paladin      (role, _) -> role
        | BowAndBlade  (role, _) -> role
        | Necromancer  (role, _) -> role
        | Nightblade   (role, _) -> role


type CharacterEquipement = {
    Hat         : string 
    Gauntlets   : string 
    Armor       : string 
    Weapon      : string 
    Pants       : string 
    Boots       : string
}

type CombatStyle = 
    | DualWielder of int
    | MaceUser of int 
    |  ``Sword and shield`` of int
    | Archer of int 
    | ``Staff wielder`` of int
    | Polyvalent of int
with 
    member x.actionPoints = 
        match x with 
        | DualWielder ap -> ap 
        | MaceUser ap -> ap
        | Archer ap -> ap 
        | Polyvalent ap -> ap 
        | ``Sword and shield`` ap -> ap 
        | ``Staff wielder`` ap -> ap 


type CharacterState = {
    CurrentPos       : CharacPos 
    CurrentDirection : PlayerDirection
}

[<AbstractClass>]
type CharacterBase(job: CharacterJob, state: CharacterState) =

    abstract member TeamParty: Object array

    abstract member ActionPoints: int

    abstract member MoveRange: MoveRange

    abstract member CharacterState: CharacterState

    abstract member CharacterID: int

    member x.Job = job

    member x.Stats = job.Stats

