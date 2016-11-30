module GLSCore.CharacterInformation

open GLSCore.GameItemsModel

open Akka.Actor

// Position 
type Position = { Top: int; Left:int }
with 
    override x.ToString() = sprintf "(Top %d, Left %d)" x.Top x.Left

// Direction
type PlayerDirection = 
    | South
    | North 
    | East 
    | West
with 
    override x.ToString() = 
        match x with 
        | South -> "South"
        | North -> "North"
        | East ->  "East"
        | West ->  "West"

    member x.areDirectionOpposite (dir: PlayerDirection) = 
        match x,dir with 
        | South, North  
        | North, South  
        | East, West  
        | West, East -> true 
        | _,_ -> false

    member x.switchOppositeDirection () = 
        match x with 
        | South -> North 
        | North -> South 
        | East -> West 
        | West -> East 


type CharacterState = 
    | Alive 
    | NeedsHealth
    | Poisoned 
    | Burnt 
    | Frozen
    | Paralyzed
    | Dead

//When engaging a target, what kind of action is selected.
type EngageAction = 
    | AttackedTarget
    | EvadeAttacker
    | EliminatedTarget
    | BlockedAttacker
    | NoAction

type CharacterRole = 
    | Wizard of int
    | Knight of int 
    | Fighter of int
    | MagicSoldier of int 
    | Sniper  of int
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

type Equipment = {
    Helmet    : GameItem option
    Armor  : GameItem option
    Legs   : GameItem option
    Gloves : GameItem option
    Ring   : GameItem option
    Weapon : GameItem option
    Shield : GameItem option
    Loot   : GameItem option
    InventoryManager : IActorRef
}

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

type GameCharacter = { 
    Name : string 
    Job : CharacterJob
    ExperiencePoints: float 
    LevelUpPoints: int 
    TiersListRank : UnitTiers
    CombatStyle : CombatStyle 
    Equipment : Equipment
    State : CharacterState
    CurrentPosition : Position 
    CurrentDirection : PlayerDirection 
}

let doesHaveTacticalAdvantage (fstCharacter: CharacterJob) (sndCharacter: CharacterJob) =
    match fstCharacter.Role, sndCharacter.Role with
    | Wizard _, Fighter _ -> true // 25 % more damage
    | Wizard _, Sniper _ -> true // 15 % more damage
    | CharacterRole.Knight _, Wizard _ -> true // 20% more damage
    | Sniper _, Fighter _ -> true // 20% more damage
    | Sniper _, CharacterRole.Knight _ -> true // 5% more damage
    | MagicSoldier _, CharacterRole.Knight _ -> true // 3 % by sword only
    | _, _ -> false


