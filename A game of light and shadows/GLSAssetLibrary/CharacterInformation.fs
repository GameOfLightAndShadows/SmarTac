module CharacterInformation

type CharacterStats = {
    Health          : float 
    Speed           : float 
    Strength        : float 
    MagicPower      : float option 
    Resistance      : int
    MagicResist     : float  
    Evade           : int 
    Luck            : int
}

type CharacterRole = 
    | Wizard 
    | Knight 
    | Fighter
    | MagicSoldier
    | Sniper 

type CharacterJob = 
    | Healer        of CharacterRole
    | Knight        of CharacterRole
    | Berserker     of CharacterRole
    | Rider         of CharacterRole
    | Paladin       of CharacterRole
    | BowAndBlade   of CharacterRole
    | Necromancer   of CharacterRole
    | Nightblade    of CharacterRole
