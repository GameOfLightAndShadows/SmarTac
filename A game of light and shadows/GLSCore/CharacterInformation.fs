module GLSCore.CharacterInformation

open GLSCore.GameElement
open GLSCore.InventoryItems

open System

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

let doesHaveTacticalAdvantage (fstCharacter: CharacterJob) (sndCharacter: CharacterJob) =
    match fstCharacter.Role, sndCharacter.Role with
    | Wizard _, Fighter _ -> true // 25 % more damage
    | Wizard _, Sniper _ -> true // 15 % more damage
    | CharacterRole.Knight _, Wizard _ -> true // 20% more damage
    | Sniper _, Fighter _ -> true // 20% more damage
    | Sniper _, CharacterRole.Knight _ -> true // 5% more damage
    | MagicSoldier _, CharacterRole.Knight _ -> true // 3 % by sword only
    | _, _ -> false

// Describe how strong a unit is suppose to be
// Will impact the overall stats
[<Literal>]
let lowTiersStatsFactor = 1

[<Literal>]
let midLowTiersStatsFactor = 1.075

[<Literal>]
let midTiersStatsFactor = 1.1233

[<Literal>]
let highTiersStatsFactor = 1.1785

[<Literal>]
let heroClassTiersStatsFactor = 1.2375

type UnitTiers =
    | Low
    | MidLow
    | Mid
    | High
    | HeroClass

type Equipment = {
    Hat : Hat option
    Armor : Armor option
    Legs  : Pants option
    Hands : Gauntlets option
    Ring  : Ring option
    Weapon : Weaponry option
    Shield : Shield option
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
    CurrentPos       : Pos
    CurrentDirection : PlayerDirection
}

[<AbstractClass>]
type CharacterBase(job: CharacterJob, state: CharacterState, equipment: Equipment) =

    abstract member TeamParty: Object array

    abstract member ActionPoints: int with get

    abstract member ExperiencePoints: float with get

    abstract member LevelUpPoints: int with get

    abstract member MoveRange: MoveRange

    abstract member CharacterState: CharacterState

    abstract member CharacterID: int

    abstract member Tiers: UnitTiers option

    member x.Job = job

    member x.Stats = job.Stats

    member x.Equipment = equipment
