module CharacterInformation

type CharacterState = 
    | Alive 
    | Dead

type LifePoints ={
    Current : float 
    Max     : float
}
with 
    member x.isDead = x.Current < 0.0

    member x.capHealth () =
        if x.Current > x.Max then { Current = x.Max; Max = x.Max } else x

    member x.raiseHealth (lifePoint:int) = 
        let raisedHealth = { x with Current = x.Current + (lifePoint |> float) }.capHealth()
        raisedHealth

    member x.takeHit (hitPoint: int) = 
        let reducedLife = { x with Current = x.Current - (hitPoint |> float) }
        reducedLife

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
    | Wizard 
    | Knight 
    | Fighter
    | MagicSoldier
    | Sniper 

type CharacterJob = 
    | Healer        of CharacterRole  * CharacterStats
    | Knight        of CharacterRole  * CharacterStats
    | Berserker     of CharacterRole  * CharacterStats
    | Rider         of CharacterRole  * CharacterStats
    | Paladin       of CharacterRole  * CharacterStats
    | BowAndBlade   of CharacterRole  * CharacterStats
    | Necromancer   of CharacterRole  * CharacterStats
    | Nightblade    of CharacterRole  * CharacterStats