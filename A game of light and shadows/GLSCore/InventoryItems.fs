    module GLSCore.InventoryItems

    open System
    open System.Collections
    open Akka.Actor

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

     type WeaponRank =
        | RankE
        | RankD
        | RankC
        | RankB
        | RankA
        | RankS

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
    let removeUnitFromFloat (x: float<_>) =
        float x
    let removeUnitFromInt (x: int<_>) =
        int x

    //Max weight of the inventory bag
    [<Literal>]
    let MaxWeight = 500.00<kg>

    [<AutoOpen>]
    module ConsummableItems =
        type ItemVariationProvenance =
            | FromTheShop
            | FromTheInventory

        type ConsummableItem =
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

        type Dagger =
            | RustedDagger
            | IronDagger
            | SteelDagger
        with
            override x.ToString() =
                match x with
                | RustedDagger  -> "Rusted dagger"
                | IronDagger    -> "Iron dagger"
                | SteelDagger   -> "Steel dagger"
            member x.WeaponStats =
                match x with
                | RustedDagger -> { Damage = 5.60<dmg>; Defense = 1.20<def>; Intelligence = None; Speed = 1.00<spd>; Critical = 0.02<ctr>; HitLimit = 20<hl>; Rank = RankE }
                | IronDagger   -> { Damage = 9.80<dmg>; Defense = 2.30<def>; Intelligence = None; Speed = 1.10<spd>; Critical = 0.04<ctr>; HitLimit = 25<hl>; Rank = RankD }
                | SteelDagger  -> { Damage = 13.10<dmg>; Defense = 3.00<def>; Intelligence = None; Speed = 1.15<spd>; Critical = 0.05<ctr>; HitLimit = 30<hl>; Rank = RankC }

            member x.Weight =
                match x with
                | RustedDagger -> 2.10<kg>
                | IronDagger   -> 2.80<kg>
                | SteelDagger  -> 4.25<kg>

            member x.Price =
                match x with
                | RustedDagger -> 80<usd>
                | IronDagger   -> 200<usd>
                | SteelDagger  -> 350<usd>

        type Sword =
            | BrokenSword
            | RustedSword
            | IronSword
            | SteelSword
        with
            override x.ToString() =
                match x with
                | BrokenSword -> "Broken sword"
                | RustedSword -> "Rusted sword"
                | IronSword   -> "Iron sword"
                | SteelSword  -> "Steel sword`"
            member x.WeaponStats =
                match x with
                | BrokenSword -> { Damage = 5.4<dmg>; Defense = 2.50<def>; Intelligence = None; Speed = 1.2<spd>; Critical = 0.01<ctr>; HitLimit = 10<hl>; Rank = RankE }
                | RustedSword -> { Damage = 8.75<dmg>; Defense = 2.90<def>; Intelligence = None ;Speed = 1.05<spd>; Critical = 0.03<ctr>; HitLimit = 20<hl>; Rank = RankD }
                | IronSword   -> { Damage = 11.1<dmg>; Defense = 3.40<def>; Intelligence = None ;Speed = 1.00<spd>; Critical = 0.04<ctr>; HitLimit = 25<hl>; Rank = RankC }
                | SteelSword  -> { Damage = 15.25<dmg>; Defense = 4.30<def>;Intelligence = None ; Speed = 0.85<spd>; Critical = 0.06<ctr>; HitLimit = 35<hl>; Rank = RankB }
            member x.Weight =
                match x with
                | BrokenSword  -> 7.20<kg>
                | RustedSword  -> 8.50<kg>
                | IronSword    -> 12.35<kg>
                | SteelSword   -> 15.00<kg>

            member x.Price =
                match x with
                | BrokenSword  -> 90<usd>
                | RustedSword  -> 120<usd>
                | IronSword    -> 250<usd>
                | SteelSword   -> 525<usd>

        type Axe =
            | RustedAxe
            | IronAxe
            | RustedBattleAxe
            | IronBattleAxe
            | SteelBattleAxe
        with
            override x.ToString() =
                match x with
                | RustedAxe -> "Rusted axe"
                | IronAxe -> "Iron axe"
                | RustedBattleAxe -> "Rusted battle axe"
                | IronBattleAxe -> "Iron battle axe"
                | SteelBattleAxe -> "SteelBattleAxe"

            member x.Weight =
                match x with
                | RustedAxe       -> 8.00<kg>
                | IronAxe         -> 10.00<kg>
                | RustedBattleAxe -> 9.00<kg>
                | IronBattleAxe   -> 13.00<kg>
                | SteelBattleAxe  -> 16.00<kg>
            member x.Price =
                match x with
                | RustedAxe        ->  125<usd>
                | IronAxe          ->  280<usd>
                | RustedBattleAxe  ->  150<usd>
                | IronBattleAxe    ->  300<usd>
                | SteelBattleAxe   ->  425<usd>

            member x.WeaponStats =
                match x with
                | RustedAxe ->          { Damage = 7.20<dmg>; Defense = 2.10<def>; Speed = -1.00<spd>;  Intelligence = None ; Critical =0.03<ctr> ; HitLimit =   20<hl>; Rank = RankE }
                | IronAxe ->            { Damage = 11.80<dmg>; Defense = 2.90<def>; Speed = -1.50<spd>; Intelligence = None ; Critical = 0.06<ctr> ;  HitLimit = 25<hl>; Rank = RankD }
                | RustedBattleAxe ->    { Damage = 7.10<dmg>; Defense = 2.30<def>; Speed = -1.20<spd>; Intelligence = None ; Critical = 0.04<ctr> ; HitLimit =  20<hl>; Rank = RankE }
                | IronBattleAxe ->      { Damage = 12.00<dmg>; Defense = 3.05<def>; Speed = -1.60<spd>;Intelligence = None ; Critical = 0.07<ctr> ; HitLimit =  25<hl>; Rank = RankD }
                | SteelBattleAxe ->     { Damage = 16.20<dmg>; Defense = 3.50<def>; Speed = -2.60<spd>;Intelligence = None ; Critical = 0.095<ctr> ; HitLimit =  30<hl>; Rank = RankC }

        type Spear =
            | RustedSpear
            | IronSpear
            | SteelSpear
        with
            override x.ToString() =
                match x with
                | RustedSpear -> "Rusted spear"
                | IronSpear -> "Iron spear"
                | SteelSpear -> "Steel spear"
            member x.WeaponStats =
                match x with
                | RustedSpear  -> { Damage = 8.20<dmg>; Intelligence = None; Defense = -3.30<def>; Speed = -1.10<spd>; Critical = 0.05<ctr>; HitLimit = 12<hl>; Rank = RankE }
                | IronSpear    -> { Damage = 12.00<dmg>; Intelligence = None; Defense = -4.25<def>; Speed = -1.50<spd>; Critical = 0.075<ctr>; HitLimit = 15<hl>; Rank = RankD }
                | SteelSpear   -> { Damage = 14.75<dmg>; Intelligence = None; Defense = -5.05<def>; Speed = -1.75<spd>; Critical = 0.0925<ctr>; HitLimit = 20<hl>; Rank = RankC }
            member x.Weight =
                match x with
                | RustedSpear -> 15.0<kg>
                | IronSpear   -> 20.0<kg>
                | SteelSpear  -> 30.0<kg>
            member x.Price =
                match x with
                | RustedSpear  -> 200<usd>
                | IronSpear    -> 325<usd>
                | SteelSpear   -> 550<usd>

        type Staff =
            | RookieStaff
            | AdeptStaff
            | SorcererStaff
            | NecromancerStaff
        with
            override x.ToString() =
                match x with
                | RookieStaff -> "Rookie staff"
                | AdeptStaff -> "Adept staff"
                | SorcererStaff -> "Sorcerer staff"
                | NecromancerStaff  -> "Necromancer staff"
            member x.WeaponStats =
                match x with
                | RookieStaff       ->  { Damage = 3.00<dmg>; Defense = 1.50<def>; Intelligence = Some 4.00<intel>; Speed = 1.00<spd>; Critical = 0.02<ctr>; HitLimit= 10<hl>; Rank = RankE }
                | AdeptStaff        ->{ Damage = 5.00<dmg>; Defense = 2.00<def>; Intelligence = Some 7.00<intel>; Speed = 0.80<spd>; Critical = 0.045<ctr>; HitLimit= 12<hl>; Rank = RankD }
                | SorcererStaff     ->{ Damage = 6.70<dmg>; Defense = 4.50<def>; Intelligence = Some 9.20<intel>; Speed = 1.10<spd>; Critical = 0.075<ctr>; HitLimit = 20<hl>; Rank = RankC }
                | NecromancerStaff  ->{ Damage = 8.00<dmg>; Defense = 5.00<def>; Intelligence = Some 13.00<intel>; Speed = 1.00<spd>; Critical = 0.00<ctr>; HitLimit = 25<hl>; Rank = RankA }
            member x.Weight =
                match x with
                | RookieStaff       -> 2.20<kg>
                | AdeptStaff        -> 4.20<kg>
                | SorcererStaff     -> 5.10<kg>
                | NecromancerStaff  -> 3.20<kg>
            member x.Price =
                match x with
                | RookieStaff      -> 180<usd>
                | AdeptStaff       -> 270<usd>
                | SorcererStaff    -> 445<usd>
                | NecromancerStaff -> 650<usd>

        type Blade =
            | RustedLongBlade
            | RustedKatana
            | IronLongBlade
            | CurvedLongBlade
            | SteelKatana
            | SteelLongBlade
        with
            override x.ToString() =
                match x with
                | RustedLongBlade -> "Rusted long blade"
                | RustedKatana    -> "Rusted katana"
                | IronLongBlade   -> "Iron long blade"
                | CurvedLongBlade  -> "Curved long blade"
                | SteelKatana     -> "Steel katana"
                | SteelLongBlade  -> "Steel long blade"
            member x.WeaponStats =
                match x with
                | RustedLongBlade -> { Damage = 6.00<dmg>; Defense = 0.5<def>; Intelligence = None; Speed = 1.10<spd>; Critical = 0.01<ctr>; HitLimit = 10<hl>; Rank = RankE }
                | RustedKatana    -> { Damage = 5.50<dmg>; Defense = 0.45<def>; Intelligence = None; Speed = 1.07<spd>; Critical = 0.02<ctr>; HitLimit = 10<hl>; Rank = RankE }
                | IronLongBlade   -> { Damage = 8.50<dmg>; Defense = 0.65<def>; Intelligence = None; Speed = 1.20<spd>; Critical = 0.03<ctr>; HitLimit = 10<hl>; Rank = RankD }
                | CurvedLongBlade -> { Damage = 7.00<dmg>; Defense = 0.80<def>; Intelligence = None; Speed = 1.25<spd>; Critical = 0.055<ctr>; HitLimit = 10<hl>; Rank = RankD }
                | SteelKatana     -> { Damage = 13.0<dmg>; Defense = 1.00<def>; Intelligence = None; Speed = 1.60<spd>; Critical = 0.07<ctr>; HitLimit = 15<hl>; Rank = RankC }
                | SteelLongBlade  -> { Damage = 15.00<dmg>; Defense = 1.10<def>; Intelligence = None; Speed = 1.55<spd>; Critical = 0.085<ctr>; HitLimit = 15<hl>; Rank = RankC }
            member x.Weight =
                match x with
                | RustedLongBlade -> 6.00<kg>
                | RustedKatana    -> 7.75<kg>
                | IronLongBlade   -> 14.25<kg>
                | CurvedLongBlade -> 11.20<kg>
                | SteelKatana     -> 16.78<kg>
                | SteelLongBlade  -> 15.30<kg>
            member x.Price =
                match x with
                | RustedLongBlade ->  120<usd>
                | RustedKatana    ->  100<usd>
                | IronLongBlade   ->  215<usd>
                | CurvedLongBlade ->  240<usd>
                | SteelKatana     ->  350<usd>
                | SteelLongBlade  ->  410<usd>

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

        type Spellbook =
            | Fireball
            | Thunder
            | Frost
            | Hellfire
            | BlackFire
            | StormOfBlades
        with
            override x.ToString() =
                match x with
                | Fireball -> "Fireball"
                | Thunder -> "Thunder"
                | Frost -> "Frost"
                | Hellfire -> "Hellfire"
                | BlackFire -> "Black fire"
                | StormOfBlades -> "Storm of blades"
            member x.SpellStats =
                match x with
                | Fireball         -> { Damage = 8.0<dmg>; AttackRange = 1; Rank = RankE; Uses = 30 ; ManaCost = 12.0<mp> }
                | Thunder          -> { Damage = 8.0<dmg>; AttackRange = 1; Rank = RankE; Uses = 30 ; ManaCost = 12.0<mp> }
                | Frost            -> { Damage = 8.0<dmg>; AttackRange = 1; Rank = RankE; Uses = 30 ; ManaCost = 12.0<mp> }
                | Hellfire         -> { Damage = 6.50<dmg>; AttackRange = 2; Rank = RankD; Uses = 25; ManaCost = 20.0<mp> }
                | BlackFire        -> { Damage = 11.2<dmg>; AttackRange = 2; Rank = RankC; Uses = 20; ManaCost = 25.0<mp> }
                | StormOfBlades    -> { Damage = 5.80<dmg>; AttackRange = 3; Rank = RankD; Uses = 30; ManaCost = 22.0<mp> }
            member x.Weight =
                match x with
                | Fireball         -> 0.05<kg>
                | Thunder          -> 0.05<kg>
                | Frost            -> 0.05<kg>
                | Hellfire         -> 0.05<kg>
                | BlackFire        -> 0.05<kg>
                | StormOfBlades    -> 0.05<kg>
            member x.Price =
                match x with
                | Fireball        -> 150<usd>
                | Thunder         -> 150<usd>
                | Frost           -> 150<usd>
                | Hellfire        -> 350<usd>
                | BlackFire       -> 350<usd>
                | StormOfBlades   -> 350<usd>

        type Weaponry =
            | Dagger        of Dagger
            | Sword         of Sword
            | Axe           of Axe
            | Spear         of Spear
            | Staff         of Staff
            | LongBlade     of Blade
            | Spellbook     of Spellbook
        with
            member x.Name =
                match x with
                | Dagger d -> d.ToString()
                | Sword  s -> s.ToString()
                | Axe    a -> a.ToString()
                | Spear  s -> s.ToString()
                | Staff  s -> s.ToString()
                | LongBlade lb -> lb.ToString()
                | Spellbook sb -> sb.ToString()
            member x.Price =
                match x with
                | Dagger     w -> w.Price
                | Sword      w -> w.Price
                | Axe        w -> w.Price
                | Spear      w -> w.Price
                | Staff      w -> w.Price
                | LongBlade  w -> w.Price
                | Spellbook  w -> w.Price
            member x.Weight =
                match x with
                | Dagger     w -> w.Weight
                | Sword      w -> w.Weight
                | Axe        w -> w.Weight
                | Spear      w -> w.Weight
                | Staff      w -> w.Weight
                | LongBlade  w -> w.Weight
                | Spellbook  w -> w.Weight

            member x.Stats =
                match x with
                | Dagger     w -> w.WeaponStats :> IStats
                | Sword      w -> w.WeaponStats :> IStats
                | Axe        w -> w.WeaponStats :> IStats
                | Spear      w -> w.WeaponStats :> IStats
                | Staff      w -> w.WeaponStats :> IStats
                | LongBlade  w -> w.WeaponStats :> IStats
                | Spellbook  w -> w.SpellStats  :> IStats
        let computeCharacterOverallOffensive
            (rank: WeaponRank)
            (weapon: Weaponry)
            (cStats: CharacterStats) =
            let strength =
                cStats.Strength *
                    match rank with
                    | RankE -> 1.0100
                    | RankD -> 1.0375
                    | RankC -> 1.0925
                    | RankB -> 1.1250
                    | RankA -> 1.1785
                    | RankS -> 1.2105

            let overallDamage =
                strength *
                    match weapon with
                    | Dagger d ->
                        d.WeaponStats.Damage
                    | Sword s ->
                        s.WeaponStats.Damage
                    | Axe a ->
                        a.WeaponStats.Damage
                    | Spear s ->
                        s.WeaponStats.Damage
                    | Staff s ->
                        s.WeaponStats.Damage
                    | LongBlade lb ->
                        lb.WeaponStats.Damage
                    | Spellbook sb ->
                        sb.SpellStats.Damage
            overallDamage

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
            member x.Name =
                match x with
                | _ -> x.ToString()

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
            member x.Name =
                match x with
                | _ -> x.ToString()

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

            member x.Name :string=
                match x with
                | _ -> x.ToString()

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
            member x.Name =
                match x with
                | _ -> x.ToString()
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

            member x.Name =
                match x with
                | _ -> x.ToString()

            member x.Weight : float<kg> =
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

            member x.Name =
                 match x with
                 | _ -> x.ToString()
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
            | Hat           of Hat

        with
            member x.Name =
                match x with
                | Shield s -> s.Name
                | Ring r -> r.Name
                | Gloves g -> g.Name
                | Legs l -> l.Name
                | Armor a -> a.Name
                | Hat h -> h.Name

            member x.Weight =
                match x with
                | Shield s -> s.Weight
                | Ring r -> r.Weight
                | Gloves g -> g.Weight
                | Legs l -> l.Weight
                | Armor a -> a.Weight
                | Hat h -> h.Weight

            member x.ProtectionStats =
                match x with
                | Shield s -> s.CharacterProtectionStat :> IStats
                | Ring r -> r.CharacterProtectionStat :> IStats
                | Gloves g -> g.CharacterProtectionStats :> IStats
                | Legs l -> l.CharacterProtectionStats :> IStats
                | Armor a -> a.CharacterProtectionStats :> IStats
                | Hat h -> h.CharacterProtectionStats   :> IStats
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
    module Inventory =

        type GameItem =
            | Consummable   of ConsummableItem * int
            | Weapon        of Weaponry * int
            | Protection    of CharacterProtection * int
        with
            member x.Quantity =
                match x with
                | Consummable (_, q) -> q
                | Weapon (_,q) -> q
                | Protection (_,q) -> q
            member x.updateQuantity
                (value: int)
                (provenance: ItemVariationProvenance) : GameItem =
                let value =
                    match provenance with
                    | FromTheShop -> value
                    | FromTheInventory -> value * -1
                match x with
                | Consummable (ci, q) -> Consummable(ci, q + value)
                | Weapon (w, q) -> Weapon(w, q + value)
                | Protection (p, q) -> Protection(p, q + value)

            member x.Name =
                match x with
                | _ -> x.Name

            member x.Weight : float<kg> =
                match x with
                | _ -> x.Weight

        type ItemCategory = 
            | Weapon 
            | Shield 
            | Loot 
            | Head 
            | Body 
            | Pant 
            | Finger 
            | Hand

        type Equipment = {
            Hat :   GameItem option
            Armor : GameItem option
            Legs  : GameItem option
            Gloves : GameItem option
            Ring  : GameItem option
            Weapon : GameItem option
            Shield : GameItem option
            Loot1  : GameItem option
            Loot2  : GameItem option 
            Loot3  : GameItem option 
            InventoryManager : IActorRef
        }
        with 
            member x.canAddMoreLoot() = 
                not (x.Loot1.IsSome && x.Loot2.IsSome && x.Loot3.IsSome)

            member x.putItemInEquipment 
                (gi: GameItem) 
                (cat: ItemCategory) = 
                let mutable equipment = x 
                match cat with 
                | Head -> 
                     equipment <-  { x with Hat = Some gi } 
                     match x.Hat with 
                     | None ->  ()
                     | Some h ->  x.InventoryManager <! AddItem h

                | Weapon -> 
                     equipment <- { x with Weapon = Some gi } 
                     match x.Weapon with 
                     | None -> () 
                     | Some w -> x.InventoryManager <! AddItem w

                | Shield -> 
                     equipment <- { x with Weapon = Some gi } 
                     match x.Shield with 
                     | None -> () 
                     | Some sh -> x.InventoryManager <! AddItem sh

                | Loot -> 
                     if not (x.canAddMoreLoot()) then x.InventoryManager <! AddItem gi
                     else 
                        match x.Loot1 with 
                        | Some l -> 
                            match x.Loot2 with 
                            | Some l -> equipment <- { x with Loot3 = Some gi } 
                            | None -> equipment <- { x with Loot2 = Some gi }
                        | None -> equipment <- { x with Loot1 = Some gi } 

                | Finger -> 
                    equipment <- { x with Ring = Some gi } 
                    match x.Ring with
                    | None -> () 
                    | Some r -> x.InventoryManager <! AddItem r 

                | Body -> 
                    equipment <- { x with Armor = Some gi } 
                    match x.Armor with 
                    | None -> () 
                    | Some a -> x.InventoryManager <! AddItem a 
                | Pant ->
                    equipment <- { x with Legs = Some gi } 
                    match x.Legs with 
                    | None -> () 
                    | Some l -> x.InventoryManager <! AddItem l 
                | Hand -> 
                    equipment <- { x with Gloves = Some gi } 
                    match x.Gloves with 
                    | None -> () 
                    | Some g -> x.InventoryManager <! AddItem g 
                equipment
               
      
        let makeBagItemsDistinct (bag: GameItem array) =
            bag |> Seq.distinct |> Seq.toArray

        type Inventory = {
            Bag : GameItem array
            Weight: float<kg>
            Excess : ImmutableStack<GameItem>
        }
        with
            member x.filterFromLightestToHeaviest() =
                x.Bag |> Array.sortBy(fun item -> item.Weight)
            member x.filterFromHeaviestToLightest() =
                x.Bag  |> Array.sortBy (fun x -> -x.Weight - 1.0<kg>) // Sort in a descending order
            member x.isBagTooHeavy() =
                x.Weight > MaxWeight
            member x.addItem (gi : GameItem): Inventory =
                if not (x.isBagTooHeavy()) then { x with Excess = x.Excess.Push gi }
                elif (x.Weight + gi.Weight) >= MaxWeight then x
                else
                    let oItemIndex = x.Bag |> Array.tryFindIndex(fun x -> x = gi)
                    match oItemIndex with
                    | Some index ->
                        let item = x.Bag.[index]
                        let item = item.updateQuantity gi.Quantity FromTheShop
                        let bag =  x.Bag
                        bag.[index] <- item
                        let itemTotalWeight = gi.Weight * (gi.Quantity |> float)
                        { x with
                             Bag = bag
                             Weight = x.Weight + itemTotalWeight
                        }
                    | None ->
                        let newBag = x.Bag |> Array.append [|gi|] |> makeBagItemsDistinct
                        let itemTotalWeight = gi.Weight * (gi.Quantity |> float)
                        let inventory = { x with
                                            Bag = newBag
                                            Weight = x.Weight + itemTotalWeight
                                        }
                        inventory
            member x.addItems (giArr: GameItem array) =
                let mutable inventory = x
                for item in giArr do
                    inventory <- inventory.addItem item
                inventory
            member x.dropItem (item: GameItem) =
                let oItemIndex = x.Bag |> Array.tryFindIndex (fun x -> x = item)
                oItemIndex
                |> Option.bind(fun index ->
                    let bag = x.Bag |> Array.filter( (<>) x.Bag.[index])
                    let inventory = { x with Bag = bag }
                    Some { inventory with Weight = inventory.Weight - item.Weight * (item.Quantity |> float) }
                )

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
