module GLSCore.GameItemsModel

open System
open System.Collections
open Akka.Actor

open GLSCore.HelperFunctions

[<AutoOpen>]
module Units = 
    //Used to represent coordinates in the map
    [<Measure>] type abscissa
    [<Measure>] type ordinate

    //Represents unit for weight in kilograms
    [<Measure>] type kg

    //Represents unit for currency
    [<Measure>] type usd

    // units of measure representing the character and equipment stats
    [<Measure>] type dmg // damage
    [<Measure>] type def // defense
    [<Measure>] type str // strenght
    [<Measure>] type spd // speed
    [<Measure>] type intel // intelligence
    [<Measure>] type res // resistance
    [<Measure>] type mgpwr  // magic power
    [<Measure>] type mgres // magic resistance
    [<Measure>] type evd    // evade
    [<Measure>] type lck  // luck
    [<Measure>] type ctr // critical
    [<Measure>] type hl // hit limit
    [<Measure>] type hp // health points
    [<Measure>] type mp // mana points
    [<Measure>] type eu // equipment usage

[<AutoOpen>]
module Energy =

    type IStats =
        abstract member showStat : unit -> string

    type IEnergyPoint =
        abstract capPoints : unit -> IEnergyPoint
        abstract raisePoints : float -> IEnergyPoint
        abstract reducePoints : float -> IEnergyPoint

    type HealthPoint =
        { CurrentLife: float<hp>
          MaxLife : float<hp> }
        interface IEnergyPoint with
            member x.capPoints () =
                if x.CurrentLife > x.MaxLife then { CurrentLife = x.MaxLife; MaxLife = x.MaxLife } :> IEnergyPoint else x :> IEnergyPoint

            member x.raisePoints (raisePoint:float) =
                let lifePoints = raisePoint * 1.0<hp>
                let raisedHealth = { x with CurrentLife = x.CurrentLife + lifePoints }
                (raisedHealth :> IEnergyPoint).capPoints()

            member x.reducePoints (hitPoint: float) =
                let lifePoints = hitPoint * 1.0<hp>
                let reducedLife = { x with CurrentLife = x.CurrentLife - lifePoints }
                (reducedLife :> IEnergyPoint)
        member x.raiseMaxLife (healthPoints: float<hp>) =
            { CurrentLife = x.CurrentLife + healthPoints; MaxLife = x.MaxLife + healthPoints }
        member x.isCharacterDead() =
            x.CurrentLife <= 0.0<hp>

    type ManaPoint =
        { CurrentMana: float<mp>
          MaxMana : float<mp> }
        interface IEnergyPoint with
            member x.capPoints () =
                if x.CurrentMana > x.MaxMana then { ManaPoint.CurrentMana = x.MaxMana; ManaPoint.MaxMana = x.MaxMana } :> IEnergyPoint else x :> IEnergyPoint

            member x.raisePoints (raisePoint:float) =
                let manaPoints = raisePoint * 1.0<mp>
                let raisedHealth = { x with CurrentMana = x.CurrentMana + manaPoints }
                (raisedHealth :> IEnergyPoint).capPoints()

            member x.reducePoints (reducePoint: float) =
                let manaPoints = reducePoint * 1.0<mp>
                let reducedLife = { x with CurrentMana = x.CurrentMana - manaPoints }
                (reducedLife :> IEnergyPoint)
        member x.canCharacterPerformMagic() =
             x.CurrentMana > 0.00<mp>
        member x.capManaAtZero() : ManaPoint =
            if x.CurrentMana < 0.00<mp> then
                { x with CurrentMana = 0.00<mp>}
            else
                x
        member x.raiseMaxMana (manaPoints: float<mp>) =
            { CurrentMana = x.CurrentMana + manaPoints; MaxMana = x.MaxMana + manaPoints }

    type CharacterStats = {
        Health          : HealthPoint
        Mana            : ManaPoint
        Speed           : float<spd>
        Strength        : float<str>
        MagicPower      : float<mgpwr> option
        Defense         : int<def>
        Resistance      : int<res>
        MagicResist     : float<mgres>
        Evade           : int<evd>
        Luck            : int<lck>
    }
    with
        interface IStats with
            member x.showStat() =
                sprintf "Max health : %O - Max mana : %O - Strenght : %O - Defense : %O - Magic power : %O - Resistance : %O - Magic resistance : %O - Evade : %O - Luck : %O" x.Health.MaxLife x.Mana.MaxMana x.Strength x.Defense x.MagicPower x.Resistance x.MagicResist x.Evade x.Luck
        member x.applyTemporaryDefense (tPoints: int<def>) =
            { x with Defense = x.Defense + tPoints }

        static member InitialStats = 
        {
            Health = { MaxLife = 100.00<hp>; CurrentLife = 100.00<hp> }
            Mana = { MaxMana = 50.00<mp>; CurrentMana = 50.00<mp> }
            Speed = 1.00<spd> 
            Strength = 11.00<str>
            MagicPower = None 
            Defense = 12<def> 
            Resistance = 6<res> 
            MagicResist = 3.00<mgres> 
            Evade = 0<evd> 
            Luck = 2<lck> 
        }

    //Max weight of the inventory bag
    [<Literal>]
    let MaxWeight = 500.00<kg>

[<AutoOpen>]
module ConsummableItems =
    type ItemVariationProvenance =
        | FromTheShop
        | FromTheInventory

    type ConsumableItem =
        | HealthPotion
        | HighHealthPotion
        | MegaHealthPotion
        | Elixir
        | HighElixir
        | MegaElixir
        | PhoenixFeather
        | MedicinalHerb
        override x.ToString() =
            match x with
            | HealthPotion     -> "Health Potion"
            | HighHealthPotion  -> "High Health Potion"
            | MegaHealthPotion -> "Mega Health Potion"
            | Elixir            -> "Elixir"
            | HighElixir        -> "High Elixir"
            | MegaElixir        -> "Mega Elixir"
            | PhoenixFeather    -> "Phoenix Feather"
            | MedicinalHerb     -> "Medicinal Herb"

        member x.Name =
            match x with
            | _ -> x.ToString()

        member x.Weight =
            match x with
            | HealthPotion       -> 0.02<kg>
            | HighHealthPotion   -> 0.03<kg>
            | MegaHealthPotion   -> 0.05<kg>
            | Elixir             -> 0.02<kg>
            | HighElixir         -> 0.03<kg>
            | MegaElixir         -> 0.05<kg>
            | PhoenixFeather     -> 0.05<kg>
            | MedicinalHerb      -> 0.03<kg>

        member x.Price =
            match x with
            | HealthPotion       -> 100<usd>
            | HighHealthPotion   -> 150<usd>
            | MegaHealthPotion   -> 225<usd>
            | Elixir             -> 125<usd>
            | HighElixir         -> 175<usd>
            | MegaElixir         -> 275<usd>
            | PhoenixFeather     -> 350<usd>
            | MedicinalHerb      -> 50<usd>

        member x.ConsummeItem (characterStat: CharacterStats)=
            match x with
            | HealthPotion      -> { characterStat with  Health = (characterStat.Health :> IEnergyPoint).raisePoints 15.0:?> HealthPoint }
            | HighHealthPotion   -> { characterStat with Health = (characterStat.Health :> IEnergyPoint).raisePoints 30.0 :?> HealthPoint }
            | MegaHealthPotion   -> { characterStat with Health = (characterStat.Health :> IEnergyPoint).raisePoints 50.0 :?> HealthPoint }
            | Elixir             -> { characterStat with Mana = (characterStat.Health :> IEnergyPoint).raisePoints 10.0 :?> ManaPoint }
            | HighElixir         -> { characterStat with Mana = (characterStat.Health :> IEnergyPoint).raisePoints 20.0 :?> ManaPoint }
            | MegaElixir         -> { characterStat with Mana = (characterStat.Health :> IEnergyPoint).raisePoints 30.0 :?> ManaPoint }
            | PhoenixFeather     ->
                if not (characterStat.Health.isCharacterDead()) then characterStat
                else
                    { characterStat with
                        Health = (characterStat.Health :> IEnergyPoint).raisePoints ((removeUnitFromFloat characterStat.Health.MaxLife) * 0.50) :?> HealthPoint
                        Mana = (characterStat.Mana :> IEnergyPoint).raisePoints ((removeUnitFromFloat characterStat.Mana.MaxMana) * 0.50) :?> ManaPoint
                    }

            | MedicinalHerb ->  { characterStat with Health = (characterStat.Health :> IEnergyPoint).raisePoints 5.0 :?> HealthPoint }

[<AutoOpen>]
module Weapons =
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


    type WeaponRank =
        | RankE
        | RankD
        | RankC
        | RankB
        | RankA
        | RankS
    with 
        member x.rankMultiplier = 
            match x with
            | RankE -> 1.0100
            | RankD -> 1.0375
            | RankC -> 1.0925
            | RankB -> 1.1250
            | RankA -> 1.1785
            | RankS -> 1.2105

    type WeaponStat = {
        Damage : float<dmg>
        Intelligence : float<intel> option
        Defense : float<def>
        Speed : float<spd>
        Critical : float<ctr>
        HitLimit : int<hl>
        Rank : WeaponRank
    }
    with
        interface IStats with
            member x.showStat() =
                let oIntelVal =
                    match x.Intelligence with
                    | Some v -> v
                    | None -> 0.00<intel>

                sprintf "Weapon damage : %O - Intelligence : %O - Defense : %O - Speed : %O - Critical hit : %O - Weapon hit limit : %O - Weapon rank : %O"
                    x.Damage oIntelVal x.Defense x.Speed x.Critical x.HitLimit x.Rank

    type SpellbookStats = {
        AttackRange : int
        Damage      : float<dmg>
        Rank        : WeaponRank
        Uses        : int
        ManaCost    : float<mp>
    }
    with
        interface IStats with
            member x.showStat() =
                sprintf "Damage : %O - Attack range : %O - Mana cost: %O - Number of use: %O - Rank of spell: %O" x.Damage x.AttackRange x.ManaCost x.Uses x.Rank

    type ItemDetails = { Weight: float<kg>; Price: int<usd> }

    type PhysicalWeaponType =
        | Dagger
        | Sword
        | Axe
        | Spear
        | Blade
        | Staff 

    type MagicalWeaponType =
        | Spellbook
        // Could later add wands, amulets, etc.

    type WeaponDetails =
        | PhysicalWeapon of PhysicalWeaponType * WeaponStat
        | MagicalWeapon of MagicalWeaponType * SpellbookStats
        
    type Weaponry =
        { Name: string
          ItemDetails: ItemDetails
          WeaponDetails: WeaponDetails }
        with member x.Weight = x.ItemDetails.Weight
             member x.Price  = x.ItemDetails.Price
             member x.Stats  = match x.WeaponDetails with
                               | PhysicalWeapon (_, stats) -> stats :> IStats
                               | MagicalWeapon  (_, stats) -> stats :> IStats

    [<AutoOpen>]
    module PhysicalWeapons =
        [<AutoOpen>]
        module Daggers =
            let rustedDagger = {
                Name = "Rusted dagger"
                ItemDetails = { Weight = 2.10<kg>; Price = 80<usd> }
                WeaponDetails = PhysicalWeapon (Dagger, { Damage = 5.60<dmg>; Defense = 1.20<def>; Intelligence = None; Speed = 1.00<spd>; Critical = 0.02<ctr>; HitLimit = 20<hl>; Rank = RankE })
            }

            let ironDagger = {
                Name = "Iron dagger"
                ItemDetails = { Weight = 2.80<kg>; Price = 200<usd> }
                WeaponDetails = PhysicalWeapon (Dagger, { Damage = 9.80<dmg>; Defense = 2.30<def>; Intelligence = None; Speed = 1.10<spd>; Critical = 0.04<ctr>; HitLimit = 25<hl>; Rank = RankD })
            }

            let steelDagger = {
                Name = "Steel dagger"
                ItemDetails = { Weight = 4.25<kg>; Price = 350<usd> }
                WeaponDetails = PhysicalWeapon (Dagger, { Damage = 13.10<dmg>; Defense = 3.00<def>; Intelligence = None; Speed = 1.15<spd>; Critical = 0.05<ctr>; HitLimit = 30<hl>; Rank = RankC })
            }

        [<AutoOpen>]
        module Swords =
            let brokenSword = {
                Name = "Broken sword"
                ItemDetails = { Weight = 7.20<kg>; Price = 90<usd> }
                WeaponDetails = PhysicalWeapon (Sword, { Damage = 5.40<dmg>; Defense = 2.50<def>; Intelligence = None; Speed = 1.20<spd>; Critical = 0.01<ctr>; HitLimit = 10<hl>; Rank = RankE })
            }

            let rustedSword = {
                Name = "Rusted sword"
                ItemDetails = { Weight = 8.50<kg>; Price = 120<usd> }
                WeaponDetails = PhysicalWeapon (Sword, { Damage = 8.75<dmg>; Defense = 2.90<def>; Intelligence = None; Speed = 1.05<spd>; Critical = 0.03<ctr>; HitLimit = 20<hl>; Rank = RankD })
            }

            let ironSword =  {
                Name = "Iron sword"
                ItemDetails = { Weight = 12.35<kg>; Price = 250<usd> }
                WeaponDetails = PhysicalWeapon(Sword, { Damage = 11.1<dmg>; Defense = 3.40<def>; Intelligence = None ;Speed = 1.00<spd>; Critical = 0.04<ctr>; HitLimit = 25<hl>; Rank = RankC })
            }

            let steelSword = { 
                Name = "Steel sword"
                ItemDetails = { Weight = 15.00<kg>; Price = 525<usd>}
                WeaponDetails = PhysicalWeapon(Sword, { Damage = 15.25<dmg>; Defense = 4.30<def>;Intelligence = None ; Speed = 0.85<spd>; Critical = 0.06<ctr>; HitLimit = 35<hl>; Rank = RankB } )
            }

        [<AutoOpen>]
        module Axes = 
            let rustedAxe = {
                Name = "Rusted axe"
                ItemDetails = { Weight = 8.00<kg>; Price = 125<usd> }
                WeaponDetails = PhysicalWeapon(Axe, { Damage = 7.20<dmg>; Defense = 2.10<def>; Speed = -1.00<spd>;  Intelligence = None ; Critical =0.03<ctr> ; HitLimit =   20<hl>; Rank = RankE } )
            }

            let ironAxe = {
                Name = "Iron axe"
                ItemDetails = { Weight = 10.00<kg>; Price = 280<usd>}
                WeaponDetails = PhysicalWeapon(Axe, { Damage = 11.80<dmg>; Defense = 2.90<def>; Speed = -1.50<spd>; Intelligence = None ; Critical = 0.06<ctr> ;  HitLimit = 25<hl>; Rank = RankD } )
            }

            let rustedBattleAxe = {
                Name = "Rusted battle axe"
                ItemDetails = { Weight = 9.00<kg>; Price = 150<usd>}
                WeaponDetails = PhysicalWeapon(Axe, { Damage = 7.10<dmg>; Defense = 2.30<def>; Speed = -1.20<spd>; Intelligence = None ; Critical = 0.04<ctr> ; HitLimit =  20<hl>; Rank = RankE } )
            }

            let ironBattleAxe = {
                Name = "Iron battle axe"
                ItemDetails = { Weight = 13.00<kg>; Price = 300<usd>}
                WeaponDetails = PhysicalWeapon(Axe, { Damage = 12.00<dmg>; Defense = 3.05<def>; Speed = -1.60<spd>;Intelligence = None ; Critical = 0.07<ctr> ; HitLimit =  25<hl>; Rank = RankD } )
            }

            let steelBattleAxe = {
                Name = "Steel battle axe"
                ItemDetails = { Weight = 16.00<kg>; Price = 425<usd>}
                WeaponDetails = PhysicalWeapon(Axe, { Damage = 16.20<dmg>; Defense = 3.50<def>; Speed = -2.60<spd>;Intelligence = None ; Critical = 0.095<ctr> ; HitLimit =  30<hl>; Rank = RankC })
            }

        [<AutoOpen>]
        module Spears = 
            let rustedSpear = {
                Name = "Rusted spear"
                ItemDetails = { Weight = 15.00<kg>; Price = 325<usd>}
                WeaponDetails = PhysicalWeapon(Spear, { Damage = 8.20<dmg>; Intelligence = None; Defense = -3.30<def>; Speed = -1.10<spd>; Critical = 0.05<ctr>; HitLimit = 12<hl>; Rank = RankE } )
            }

            let ironSpear = {
                Name = "Iron spear"
                ItemDetails = { Weight = 20.00<kg>; Price = 325<usd>}
                WeaponDetails = PhysicalWeapon(Spear, { Damage = 12.00<dmg>; Intelligence = None; Defense = -4.25<def>; Speed = -1.50<spd>; Critical = 0.075<ctr>; HitLimit = 15<hl>; Rank = RankD } )
            }

            let steelSpear = {
                Name = "Steel spear"
                ItemDetails = { Weight = 30.00<kg>; Price = 550<usd>}
                WeaponDetails = PhysicalWeapon(Spear, { Damage = 14.75<dmg>; Intelligence = None; Defense = -5.05<def>; Speed = -1.75<spd>; Critical = 0.0925<ctr>; HitLimit = 20<hl>; Rank = RankC } )
            }

        [<AutoOpen>]
        module Blades = 
            let rustedLongBlade = {
                Name = "Rusted long blade"
                ItemDetails = { Weight = 6.00<kg>; Price = 120<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 6.00<dmg>; Defense = 0.5<def>; Intelligence = None; Speed = 1.10<spd>; Critical = 0.01<ctr>; HitLimit = 10<hl>; Rank = RankE } )
            }

            let rustedKatana = {
                Name = "Rusted katana"
                ItemDetails = { Weight = 7.75<kg>; Price = 100<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 5.50<dmg>; Defense = 0.45<def>; Intelligence = None; Speed = 1.07<spd>; Critical = 0.02<ctr>; HitLimit = 10<hl>; Rank = RankE })
            }

            let ironLongBlade = {
                Name = "Iron long blade"
                ItemDetails = { Weight = 14.25<kg>; Price = 215<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 8.50<dmg>; Defense = 0.65<def>; Intelligence = None; Speed = 1.20<spd>; Critical = 0.03<ctr>; HitLimit = 10<hl>; Rank = RankD })
            }

            let curvedLongBlade = {
                Name = "Curved long blade"
                ItemDetails = { Weight = 11.20<kg>; Price = 240<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 7.00<dmg>; Defense = 0.80<def>; Intelligence = None; Speed = 1.25<spd>; Critical = 0.055<ctr>; HitLimit = 10<hl>; Rank = RankD })
            }

            let steelKatana = {
                Name = "Steel katana"
                ItemDetails = { Weight = 16.78<kg>; Price = 350<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 13.0<dmg>; Defense = 1.00<def>; Intelligence = None; Speed = 1.60<spd>; Critical = 0.07<ctr>; HitLimit = 15<hl>; Rank = RankC })
            }

            let steelLongBlade = {
                Name = "Steel long blade"
                ItemDetails = { Weight = 15.30<kg>; Price = 410<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 15.00<dmg>; Defense = 1.10<def>; Intelligence = None; Speed = 1.55<spd>; Critical = 0.085<ctr>; HitLimit = 15<hl>; Rank = RankC })
            }

        [<AutoOpen>]
        module MagicalStaffs = 
            let rookieStaff = {
                Name = "Rookie staff"
                ItemDetails = { Weight = 2.20<kg>; Price = 180<usd>}
                WeaponDetails = PhysicalWeapon(Staff, { Damage = 3.00<dmg>; Defense = 1.50<def>; Intelligence = Some 4.00<intel>; Speed = 1.00<spd>; Critical = 0.02<ctr>; HitLimit= 10<hl>; Rank = RankE })
            }

            let adeptStaff = {
                Name = "Adept staff"
                ItemDetails = { Weight = 4.20<kg>; Price = 270<usd>}
                WeaponDetails = PhysicalWeapon(Staff, { Damage = 5.00<dmg>; Defense = 2.00<def>; Intelligence = Some 7.00<intel>; Speed = 0.80<spd>; Critical = 0.045<ctr>; HitLimit= 12<hl>; Rank = RankD })
            }

            let sorcererStaff = {
                Name = "Sorcerer staff"
                ItemDetails = { Weight = 5.10<kg>; Price = 445<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 15.00<dmg>; Defense = 1.10<def>; Intelligence = None; Speed = 1.55<spd>; Critical = 0.085<ctr>; HitLimit = 15<hl>; Rank = RankC })
            }

            let necromancerStaff = {
                Name = "Necromancer staff"
                ItemDetails = { Weight = 3.20<kg>; Price = 550<usd>}
                WeaponDetails = PhysicalWeapon(Blade, { Damage = 15.00<dmg>; Defense = 1.10<def>; Intelligence = None; Speed = 1.55<spd>; Critical = 0.085<ctr>; HitLimit = 15<hl>; Rank = RankC })
            }


    [<AutoOpen>]
    module MagicalWeapons = 
        let rank1SpellbookDetails = { Weight = 0.05<kg>; Price = 150<usd> }
        let rank2SpellbookDetails = { Weight = 0.05<kg>; Price = 350<usd> }

        let bookOfFireball = {
            Name = "Fireball"
            ItemDetails = rank1SpellbookDetails
            WeaponDetails = MagicalWeapon (Spellbook, { Damage = 8.0<dmg>; AttackRange = 1; Rank = RankE; Uses = 30 ; ManaCost = 12.0<mp> })
        }

        let bookOfThunder = {
            Name = "Thunder"
            ItemDetails = rank1SpellbookDetails
            WeaponDetails = MagicalWeapon (Spellbook, { Damage = 8.0<dmg>; AttackRange = 1; Rank = RankE; Uses = 30 ; ManaCost = 12.0<mp> })
        }

        let bookOfFrost= {
            Name = "Thunder"
            ItemDetails = rank1SpellbookDetails
            WeaponDetails = MagicalWeapon (Spellbook, { Damage = 8.0<dmg>; AttackRange = 1; Rank = RankE; Uses = 30 ; ManaCost = 12.0<mp> })
        }

        let bookOfHellfire = {
            Name = "Hellfire"
            ItemDetails = rank2SpellbookDetails
            WeaponDetails = MagicalWeapon (Spellbook, { Damage = 6.50<dmg>; AttackRange = 2; Rank = RankD; Uses = 25; ManaCost = 20.0<mp> })
        }

        let bookOfBlackFire = {
            Name = "Black fire"
            ItemDetails = rank2SpellbookDetails
            WeaponDetails = MagicalWeapon (Spellbook, { Damage = 11.2<dmg>; AttackRange = 2; Rank = RankC; Uses = 20; ManaCost = 25.0<mp> })
        }

        let bookOfStormOfBlades = {
            Name = "Storm of blades"
            ItemDetails = rank2SpellbookDetails
            WeaponDetails = MagicalWeapon (Spellbook, { Damage = 5.80<dmg>; AttackRange = 3; Rank = RankD; Uses = 30; ManaCost = 22.0<mp> })
        }
        
    let computeCharacterOverallOffensive
        (weapon: Weaponry)
        (cStats: CharacterStats) =

        let weaponDamage =
            match weapon.WeaponDetails with
            | PhysicalWeapon (_, stats) -> stats.Damage
            | MagicalWeapon  (_, stats) -> stats.Damage

        let weaponRank =
            match weapon.WeaponDetails with
            | PhysicalWeapon (_, stats) -> stats.Rank
            | MagicalWeapon  (_, stats) -> stats.Rank

        cStats.Strength * weaponRank.rankMultiplier * weaponDamage

[<AutoOpen>]
module CharacterWearableProtection =
    type CharacterProtectionStats = {
        Defense : float<def>
        Resistance : float<res>
        Intelligence : float<intel> option
        MagicResist : float<mgres>
        Speed  : float<spd>
        EquipmentUsage : int<eu>
    }
    with
        interface IStats with
            member x.showStat() =
                sprintf "Defense : %O - Resistance : %O - Magic resistance : %O - Speed : %O - Equipment usage : %O" x.Defense x.Resistance x.MagicResist x.Speed x.EquipmentUsage

    type Hat =
        | SorcererHat
        | InfantryHat
        | JourneyManHelmet
        | RustedHelmet
        | MerlinsHat
        | IronHelmet
        | SteelHelmet
    with
        override x.ToString() =
            match x with
            | SorcererHat -> "Sorcerer hat"
            | InfantryHat -> "Infantry hat"
            | JourneyManHelmet -> "Journey man helmet"
            | RustedHelmet -> "Rusted helmet"
            | MerlinsHat -> "Merlin's hat"
            | IronHelmet -> "Iron helmet"
            | SteelHelmet -> "Steel helmet"

        member x.Name = match x with | _ -> x.ToString()

        member x.Price =
            match x with 
            | SorcererHat   -> 120<usd>     
            | InfantryHat   -> 70<usd>
            | JourneyManHelmet -> 180<usd>
            | RustedHelmet -> 95<usd>
            | MerlinsHat   -> 275<usd>
            | IronHelmet   -> 160<usd>
            | SteelHelmet  -> 325<usd>

        member x.Weight =
            match x with
            | SorcererHat -> 1.0<kg>
            | InfantryHat -> 3.20<kg>
            | JourneyManHelmet -> 2.25<kg>
            | RustedHelmet -> 2.50<kg>
            | MerlinsHat -> 4.10<kg>
            | IronHelmet -> 6.25<kg>
            | SteelHelmet -> 8.00<kg>

        member x.CharacterProtectionStats =
            match x with
            | SorcererHat       -> { Defense = 1.20<def>; Resistance = 1.30<res>; Intelligence = Some 3.00<intel>; MagicResist = 1.80<mgres>; Speed = 1.00<spd>; EquipmentUsage = 100<eu> }
            | InfantryHat  -> { Defense = 2.20<def>; Resistance = 1.80<res>; Intelligence = None; MagicResist = 0.80<mgres>; Speed = 0.95<spd>; EquipmentUsage = 100<eu> }
            | JourneyManHelmet -> { Defense = 5.60<def>; Resistance = 3.20<res>; Intelligence = None; MagicResist = 0.60<mgres>; Speed = 0.90<spd>; EquipmentUsage = 100<eu> }
            | RustedHelmet -> { Defense = 4.80<def>; Resistance = 2.70<res>; Intelligence = None; MagicResist = 0.20<mgres>; Speed = 1.00<spd>; EquipmentUsage = 75<eu> }
            | MerlinsHat -> { Defense = 3.10<def>; Resistance = 1.70<res>; Intelligence = Some 6.50<intel>; MagicResist = 3.45<mgres>; Speed = 1.03<spd>; EquipmentUsage = 100<eu> }
            | IronHelmet -> { Defense = 7.20<def>; Resistance = 3.50<res>; Intelligence = None; MagicResist = 0.40<mgres>; Speed = 0.98<spd>; EquipmentUsage = 100<eu> }
            | SteelHelmet -> { Defense = 10.80<def>; Resistance = 4.20<res>; Intelligence = None; MagicResist = 0.40<mgres>; Speed = 0.95<spd>; EquipmentUsage = 100<eu> }

    type Armor =
        | InfantryArmor
        | RustedArmor
        | IronArmor
        | SteelArmor
        | MyrtilleArmor
    with
        override x.ToString() =
            match x with
            | InfantryArmor -> "Infantry armor"
            | RustedArmor -> "Rusted armor"
            | IronArmor   -> "Iron armor"
            | SteelArmor  -> "Steel armor"
            | MyrtilleArmor -> "Myrtille armor"

        member x.Weight =
            match x with
            | InfantryArmor  -> 6.0<kg>
            | RustedArmor    -> 5.5<kg>
            | IronArmor      -> 10.0<kg>
            | SteelArmor     -> 15.0<kg>
            | MyrtilleArmor  -> 22.75<kg>

        member x.Price =
            match x with 
            | InfantryArmor -> 120<usd> 
            | RustedArmor -> 100<usd> 
            | IronArmor -> 220<usd>
            | SteelArmor -> 450<usd> 
            | MyrtilleArmor -> 790<usd>

        member x.Name = match x with | _ -> x.ToString()

        member x.CharacterProtectionStats =
            match x with
            | InfantryArmor -> { Defense = 6.50<def>; Resistance = 2.20<res>; Intelligence = None; MagicResist = 0.00<mgres>; Speed = 0.99<spd>; EquipmentUsage = 100<eu> }
            | RustedArmor   -> { Defense = 4.20<def>; Resistance = 1.10<res>; Intelligence = None; MagicResist = 0.30<mgres>; Speed = 1.00<spd>; EquipmentUsage = 50<eu> }
            | IronArmor     -> { Defense = 12.00<def>; Resistance = 4.30<res>; Intelligence = None; MagicResist = 1.00<mgres>; Speed = 0.975<spd>; EquipmentUsage = 100<eu> }
            | SteelArmor    -> { Defense = 17.40<def>; Resistance = 6.10<res>; Intelligence = None; MagicResist = 2.30<mgres>; Speed = 0.945<spd>; EquipmentUsage = 100<eu> }
            | MyrtilleArmor -> { Defense = 23.55<def>; Resistance = 10.10<res>; Intelligence = None; MagicResist = 5.70<mgres>; Speed = 0.9725<spd>; EquipmentUsage = 100<eu> }

    type Pants =
        | InfantryPants
        | IronPants
        | SteelPants
        | MyrtillePants
    with
        override x.ToString() =
            match x with
            | InfantryPants -> "Infantry pants"
            | IronPants -> "Iron pants"
            | SteelPants -> "Steel pants"
            | MyrtillePants -> "Myrtille pants"

        member x.Name = match x with | _ -> x.ToString()

        member x.Price =
            match x with 
            | InfantryPants -> 50<usd> 
            | IronPants -> 140<usd> 
            | SteelPants -> 190<usd> 
            | MyrtillePants -> 400<usd> 

        member x.Weight =
            match x with
            | InfantryPants -> 4.00<kg>
            | IronPants -> 5.75<kg>
            | SteelPants -> 10.80<kg>
            | MyrtillePants -> 15.20<kg>

        member x.CharacterProtectionStats =
            match x with
            | InfantryPants ->
                { Defense = 0.85<def>; Resistance = 0.30<res>; Intelligence = None; MagicResist = 0.00<mgres>; Speed = 0.99<spd>; EquipmentUsage = 100<eu> }
            | IronPants -> { Defense = 3.50<def>; Resistance = 1.20<res>; Intelligence = None; MagicResist = 0.50<mgres>; Speed = 0.98<spd>; EquipmentUsage = 100<eu> }
            | SteelPants -> { Defense = 5.10<def>; Resistance = 2.00<res>; Intelligence = None; MagicResist = 1.00<mgres>; Speed = 0.95<spd>; EquipmentUsage = 100<eu> }
            | MyrtillePants -> { Defense = 6.50<def>; Resistance = 2.70<res>; Intelligence = None; MagicResist = 3.00<mgres>; Speed = 0.9765<spd>; EquipmentUsage = 100<eu> }

    type Gauntlets =
        | InfantryGauntless
        | RustedGauntless
        | IronGauntless
        | SteelGauntless
    with
        override x.ToString() =
            match x with
            | InfantryGauntless -> "Infantry gauntlets"
            | RustedGauntless -> "Rusted gauntlets"
            | IronGauntless -> "Iron gauntlets"
            | SteelGauntless -> "Steel gauntlets"

        member x.Price = 
            match x with 
            | InfantryGauntless -> 40<usd> 
            | RustedGauntless -> 25<usd> 
            | IronGauntless -> 95<usd> 
            | SteelGauntless -> 185<usd> 

        member x.Name = match x with | _ -> x.ToString()

        member x.Weight =
            match x with
            | InfantryGauntless -> 1.00<kg>
            | RustedGauntless -> 1.70<kg>
            | IronGauntless -> 3.20<kg>
            | SteelGauntless -> 6.55<kg>

        member x.CharacterProtectionStats =
            match x with
            | InfantryGauntless ->
                { Defense = 0.85<def>; Resistance = 0.15<res>; Intelligence = None; MagicResist = 0.10<mgres>; Speed = 0.995<spd>; EquipmentUsage = 100<eu> }
            | RustedGauntless ->
                { Defense = 1.2<def>; Resistance = 0.45<res>; Intelligence = None; MagicResist = 0.20<mgres>; Speed = 0.99<spd>; EquipmentUsage = 50<eu> }
            | IronGauntless ->
                { Defense = 3.65<def>; Resistance = 0.85<res>; Intelligence = None; MagicResist = 0.25<mgres>; Speed = 0.975<spd>; EquipmentUsage = 100<eu> }
            | SteelGauntless ->
                { Defense = 5.15<def>; Resistance = 1.35<res>; Intelligence = None; MagicResist = 0.45<mgres>; Speed = 0.95<spd>; EquipmentUsage = 100<eu> }

    type RingStats = {
        ExtraStrength : float<str> option
        ExtraDamage   : float<dmg> option
        ExtraHealth   : float<hp> option
        ExtraMana     : float<mp> option
    }
    with
        interface IStats with
            member x.showStat() =
                sprintf ""
        static member Initial =
            { ExtraDamage = None; ExtraStrength = None; ExtraHealth = None; ExtraMana = None }

    type Ring =
        | ExtraStrenghtRing
        | ExtraDamageRing
        | ExtraHealthRing
        | ExtraManaRing
    with
        override x.ToString() =
            match x with
            | ExtraStrenghtRing -> "Extra strenght ring"
            | ExtraDamageRing -> "Extra damage ring"
            | ExtraHealthRing -> "Extra health ring"
            | ExtraManaRing -> "Extra mana ring"

        member x.Name = match x with | _ -> x.ToString()

        member x.Price = 
            match x with 
            | _ -> 275<usd>

        member x.Weight =
            match x with
            | _ -> 0.75<kg>

        member x.CharacterProtectionStat =
            match x with
            | ExtraDamageRing ->  { RingStats.Initial with ExtraDamage = Some 5.00<dmg> }
            | ExtraStrenghtRing -> { RingStats.Initial with ExtraStrength = Some 4.50<str> }
            | ExtraHealthRing -> { RingStats.Initial with ExtraHealth = Some 12.00<hp> }
            | ExtraManaRing -> { RingStats.Initial with ExtraMana = Some 7.00<mp> }

    type ShieldStats = {
        Defense : float<def>
        Resistance :float<res>
        Speed   : float<spd>
        MagicResist : float<mgres>
        ShieldRank  : WeaponRank
        EquipmentUsage : int<eu>
    }
    with
        interface IStats with
            member x.showStat() =
                sprintf "Defense : %O - Resistance : %O - Magic resistance : %O - Equipment usage : %O" x.Defense x.Resistance x.MagicResist x.EquipmentUsage

    type Shield =
        | RustedShield
        | SmallShield
        | KnightShield
        | HeavyShield
        | SteelShield
    with
        override x.ToString()  =
            match x with
            | RustedShield -> "Rusted shield"
            | SmallShield -> "Small shield"
            | KnightShield -> "Knight shield"
            | HeavyShield -> "Heavy shield"
            | SteelShield -> "Steel shield"

        member x.Name = match x with | _ -> x.ToString()

        member x.Price = 
            match x with 
            | RustedShield -> 100<usd> 
            | SmallShield -> 75<usd> 
            | KnightShield -> 215<usd> 
            | HeavyShield -> 425<usd> 
            | SteelShield -> 500<usd>

        member x.Weight =
            match x with
            | RustedShield -> 5.00<kg>
            | SmallShield -> 3.25<kg>
            | KnightShield -> 6.80<kg>
            | HeavyShield -> 12.50<kg>
            | SteelShield -> 10.80<kg>

        member x.CharacterProtectionStat =
            match x with
            | RustedShield ->
                { Defense = 5.60<def>; Resistance = 3.10<res>; Speed = 0.96<spd>; MagicResist = 1.85<mgres>; ShieldRank = RankE; EquipmentUsage = 50<eu> }
            | SmallShield ->
                { Defense = 3.80<def>; Resistance = 2.70<res>; Speed = 0.985<spd>; MagicResist = 0.50<mgres>; ShieldRank = RankE; EquipmentUsage = 100<eu> }
            | KnightShield ->
                { Defense = 9.80<def>; Resistance = 5.10<res>; Speed = 0.94<spd>; MagicResist = 3.40<mgres>; ShieldRank = RankC; EquipmentUsage = 100<eu> }
            | HeavyShield ->
                { Defense = 12.75<def>; Resistance = 6.40<res>; Speed = 0.88<spd>; MagicResist = 4.60<mgres>; ShieldRank = RankC; EquipmentUsage = 100<eu> }
            | SteelShield ->
                { Defense = 17.20<def>; Resistance = 9.30<res>; Speed = 0.93<spd>; MagicResist = 5.90<mgres>; ShieldRank = RankB; EquipmentUsage = 100<eu> }

    type CharacterProtection =
        | Shield        of Shield
        | Ring          of Ring
        | Gloves        of Gauntlets
        | Legs          of Pants
        | Armor         of Armor
        | Helmet        of Hat

    with
        member x.Name =
            match x with
            | Shield s -> s.Name
            | Ring r -> r.Name
            | Gloves g -> g.Name
            | Legs l -> l.Name
            | Armor a -> a.Name
            | Helmet h -> h.Name

        member x.Price =
            match x with 
            | Shield s -> s.Price 
            | Ring r -> r.Price
            | Gloves g -> g.Price
            | Legs l -> l.Price
            | Armor a -> a.Price 
            | Helmet a -> a.Price

        member x.Weight =
            match x with
            | Shield s -> s.Weight
            | Ring r -> r.Weight
            | Gloves g -> g.Weight
            | Legs l -> l.Weight
            | Armor a -> a.Weight
            | Helmet h -> h.Weight

        member x.ProtectionStats =
            match x with
            | Shield s -> s.CharacterProtectionStat :> IStats
            | Ring r -> r.CharacterProtectionStat :> IStats
            | Gloves g -> g.CharacterProtectionStats :> IStats
            | Legs l -> l.CharacterProtectionStats :> IStats
            | Armor a -> a.CharacterProtectionStats :> IStats
            | Helmet h -> h.CharacterProtectionStats   :> IStats

[<AutoOpen>]
module ExccesItems =
    type ImmutableStack<'T> =
        | Empty
        | Stack of 'T * ImmutableStack<'T>
    with
        member s.Push x = Stack(x, s)
        member s.Pop() =
            match s with
            | Empty -> failwith "Underflow"
            | Stack(t,_) -> t
        member s.Top() =
            match s with
            | Empty -> failwith "Contain no elements"
            | Stack(_,st) -> st
        member s.isEmpty =
            match s with
            | Empty -> true
            | _ -> false
        member s.All() =
            let rec loop acc = function
                | Empty -> acc
                | Stack(t,st) -> loop (t::acc) st
            loop [] s

[<AutoOpen>]
module GameItems =

    type GameItem =
        | Consumable    of ConsumableItem 
        | Weapon        of Weaponry 
        | Protection    of CharacterProtection 
    with 
        member x.Weight = 
            match x with 
            | Consumable c -> c.Weight 
            | Weapon w -> w.Weight 
            | Protection p -> p.Weight

        member x.Name = 
            match x with
            | Consumable c -> c.Name 
            | Weapon w -> w.Name 
            | Protection p -> p.Name

        member x.Price = 
            match x with 
            | Consumable c -> c.Price 
            | Weapon w -> x.Price 
            | Protection p -> p.Price 

    type ItemStack = {
        Item  : GameItem 
        Count : int
    }                
    with
        member x.updateQuantity
            (value: int)
            (provenance: ItemVariationProvenance) =
            let value =
                match provenance with
                | FromTheShop -> value
                | FromTheInventory -> value * -1

            { x with Count = x.Count + value }

[<AutoOpen>]
module SharedInventory = 

    type Inventory = {
        Bag : ItemStack array
        Weight: float<kg>
        Excess : ImmutableStack<ItemStack>
    }
    with
        static member InitialInventory = 
            { Bag = [| |]; Weight = 0.00<kg>; Excess = Empty }

        member x.filterFromLightestToHeaviest() =
            let filteredBag =  x.Bag |> Array.sortBy(fun is -> is.Item.Weight)
            { x with Bag = filteredBag }

        member x.filterFromHeaviestToLightest() =
            x.Bag  |> Array.sortBy (fun x -> -x.Item.Weight - 1.0<kg>) // Sort in a descending order

        member x.isBagTooHeavy() =
            x.Weight > MaxWeight

        member x.addItem (is : ItemStack): Inventory =
            if not (x.isBagTooHeavy()) then { x with Excess = x.Excess.Push is }
            elif (x.Weight + is.Item.Weight) >= MaxWeight then x
            else
                let oItemIndex = x.Bag |> Array.tryFindIndex(fun x -> x.Item = is.Item)
                match oItemIndex with
                | Some index ->
                    let item = x.Bag.[index]
                    let item = item.updateQuantity is.Count FromTheShop
                    let bag =  x.Bag
                    bag.[index] <- item
                    let itemTotalWeight = is.Item.Weight * (is.Count |> float)
                    { x with Bag = bag;  Weight = x.Weight + itemTotalWeight }
                | None ->
                    let newBag = x.Bag |> Array.append [| is |]
                    let itemTotalWeight = is.Item.Weight * (is.Count |> float)
                    let inventory = { x with Bag = newBag; Weight = x.Weight + itemTotalWeight }
                    inventory

        member x.addItems (giArr: ItemStack array) =
            let mutable inventory = x
            for item in giArr do
                inventory <- inventory.addItem item
            inventory

        member x.dropItem (item: ItemStack) =
            let oItemIndex = x.Bag |> Array.tryFindIndex (fun x -> x.Item = item.Item)
            match oItemIndex with 
            | None -> x
            | Some index -> 
                let bag = x.Bag |> Array.filter( (<>) x.Bag.[index])
                let itemStack = bag.[index]
                { x with Weight = x.Weight - (itemStack.Count |> float) * itemStack.Item.Weight }


        member x.canItemsBeRemovedFromExcess() =
            not (x.isBagTooHeavy() && x.Excess.isEmpty)

        member x.removeItemFromExcess() =
            if x.canItemsBeRemovedFromExcess() then
                let mutable inventory = x
                while (inventory.canItemsBeRemovedFromExcess()) do
                    let excessItem = inventory.Excess.Pop()
                    inventory <- inventory.addItem excessItem
                inventory
            else
                x               
