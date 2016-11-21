module GLSCore.InventoryItems

open System 

//Represents unit for weight in kilograms
[<Measure>] type kg

//Represents unit for currency
[<Measure>] type usd

[<AbstractClass>]
type InventoryItem() =

    abstract member ItemName : string 

    abstract member ItemDescription : string 

    abstract member ItemWeight : float<kg> 

    abstract member ItemPrice : float<usd> 

    abstract member Quantity : int 

type WeaponRank = 
    | RankE 
    | RankD
    | RankC
    | RankB 
    | RankA
    | RankS

type WeaponStat = {
    Damage : float 
    Defense : float 
    Speed : float 
    Critical : float 
    HitLimit : int 
    Rank : WeaponRank
}

type ConsummableItem = 
    | HealthPotion      
    | HighHealthPotion  
    | MegaHealthPotion  
    | Elixir            
    | HighElixir        
    | MegaElixir        
    | PhoenixFeather    
    | MedicinalHerb     
    | Antidote    

type Dagger = 
    | RustedDagger of InventoryItem * WeaponStat
    | IronDagger of InventoryItem  * WeaponStat 
    | SteelDagger of InventoryItem * WeaponStat

type Sword = 
    | BrokenSword of InventoryItem * WeaponStat
    | RustedSword of InventoryItem * WeaponStat
    | IronSword of InventoryItem * WeaponStat
    | SteelSword of InventoryItem * WeaponStat

type Axe = 
    | RustedAxe of InventoryItem * WeaponStat
    | IronAxe of InventoryItem * WeaponStat
    | RustedBattleAxe of InventoryItem * WeaponStat
    | IronBattleAxe of InventoryItem * WeaponStat
    | StellBattleAxe of InventoryItem * WeaponStat

type Spear =
    | RustedSpear of InventoryItem  * WeaponStat
    | IronSpear  of InventoryItem * WeaponStat
    | SteelSpear of InventoryItem * WeaponStat

type Staff = 
    | RookieStaff of InventoryItem * WeaponStat
    | AdeptStaff  of InventoryItem * WeaponStat
    | SorcererStaff of InventoryItem * WeaponStat
    | NecromancerStaff of InventoryItem * WeaponStat

type Blade = 
    | RustedLongBlade of InventoryItem * WeaponStat
    | RustedKatana of InventoryItem * WeaponStat
    | IronLongBlade of InventoryItem * WeaponStat
    | CurvedLongBlade of InventoryItem * WeaponStat
    | SteelKatana of InventoryItem * WeaponStat
    | SteelLongBlade of InventoryItem * WeaponStat

type SpellbookStats = {
    AttackRange : int 
    Rank        : WeaponRank
    Uses    : int
}

type Spellbook =
    | Fireball of InventoryItem * SpellbookStats
    | Thunder of InventoryItem * SpellbookStats
    | Frost of InventoryItem * SpellbookStats
    | Hellfire of InventoryItem * SpellbookStats
    | BlackFire of InventoryItem * SpellbookStats
    | Tornado of InventoryItem * SpellbookStats
    | WildWind of InventoryItem * SpellbookStats
    | StormOfBlades of InventoryItem * SpellbookStats
    | ThorStorm of InventoryItem * SpellbookStats
   
type Weaponry = 
    | Dagger of Dagger 
    | Sword of Sword 
    | Axe of Axe
    | Spear of Spear
    | Staff of Staff 
    | LongBlade of Blade
    | Spellbook of Spellbook

type Hat = 
    | SorcererHat of InventoryItem 
    | InfantryHat of InventoryItem 
    | JourneyManHelmet of InventoryItem 
    | RustedHelmet of InventoryItem 
    | MerlinsHat of InventoryItem 
    | IronHelmet of InventoryItem 
    | SteelHelmet of InventoryItem 

type Armor = 
    | InfantryArmor of InventoryItem 
    | RustedArmor of InventoryItem 
    | IronArmor of InventoryItem
    | SteelArmor of InventoryItem
    | MyrtilleArmor of InventoryItem

type Pants = 
    | InfantryPants of InventoryItem
    | IronPants of InventoryItem
    | SteelPants of InventoryItem
    | MyrtillePants of InventoryItem

type Gauntlets = 
    | InfantryGauntless of InventoryItem
    | RustedGauntless of InventoryItem
    | IronGauntless of InventoryItem
    | SteelGauntless of InventoryItem

type Ring = 
    | ExtraStrenghtRing of InventoryItem
    | ExtraDamageRing of InventoryItem
    | ExtraHeathRing  of InventoryItem
    | ExtraMana        of InventoryItem

type Shield = 
    | RustedShield of InventoryItem
    | SmallShield of InventoryItem
    | KnightShield of InventoryItem
    | HeavyShield of InventoryItem
    | SteelShield of InventoryItem

type Inventory = {
    Inventory : InventoryItem array
}
with 
    member x.filterFromLightestToHeaviest() =
        x.Inventory
        |> Array.sortBy(fun item -> item.ItemWeight)

    member x.filterFromHeaviestToLightest() = 
        x.Inventory 
        |> Array.sortBy (fun x -> -x.ItemWeight - 1.0<kg>) // Sort in a descending order 

type Equipment = {
    Hat : Hat 
    Armor : Armor 
    Legs  : Pants 
    Hands : Gauntlets 
    Ring  : Ring 
    Weapon : Weaponry
    Shield : Shield option
}