    module GLSCore.InventoryItems

    open System 

    //Represents unit for weight in kilograms
    [<Measure>] type kg

    //Represents unit for currency
    [<Measure>] type usd

    // units of measure representing the character and weapon stats
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
 
    let removeUnitFromFloat (x: float<_>) =
        float x 

    let removeUnitFromInt (x: int<_>) =
        int x

    //Max weight of the inventory bag
    [<Literal>] 
    let MaxWeight = 500.00<kg>

    type IStats = 
        abstract member showStat : unit -> string
    
    type IEnergyPoint =
        abstract CurrentPoints : float 
        abstract MaxPoints     : float
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
   
    type WeaponRank = 
        | RankE 
        | RankD
        | RankC
        | RankB 
        | RankA
        | RankS

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
        Rank        : WeaponRank
        Uses        : int
    }
    with
        interface IStats with 
            member x.showStat() = 
                sprintf "Attack range : %O - Number of use: %O - Rank of spell: %O" x.AttackRange x.Uses x.Rank

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
            | Fireball         -> { AttackRange = 1; Rank = RankE; Uses = 30 }
            | Thunder          -> { AttackRange = 1; Rank = RankE; Uses = 30 }
            | Frost            -> { AttackRange = 1; Rank = RankE; Uses = 30 }
            | Hellfire         -> { AttackRange = 2; Rank = RankD; Uses = 25 }
            | BlackFire        -> { AttackRange = 2; Rank = RankC; Uses = 20 }
            | StormOfBlades    -> { AttackRange = 3; Rank = RankD; Uses = 30 }    
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
            | Dagger     w -> w.WeaponStats  :> IStats
            | Sword      w -> w.WeaponStats :> IStats
            | Axe        w -> w.WeaponStats :> IStats
            | Spear      w -> w.WeaponStats :> IStats
            | Staff      w -> w.WeaponStats :> IStats
            | LongBlade  w -> w.WeaponStats :> IStats
            | Spellbook  w -> w.SpellStats :> IStats

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

    type CharacterProtection = 
        | Shield        of Shield 
        | Ring          of Ring 
        | Gloves        of Gauntlets 
        | Legs          of Pants
        | Armor         of Armor 
        | Hat           of Hat 

    type GameItem = 
        | Consummable   of ConsummableItem
        | Weapon        of Weaponry
        | Protection    of CharacterProtection
    with 
        member x.UpdateQuantity value variationProvenance = 
            let itemVariation = 
                match variationProvenance with 
                | FromTheShop -> value 
                | FromTheInventory -> value * -1

            let qty = x.Quantity + itemVariation
            let qty=
                match qty with 
                | t when t > 99 -> 99
                | t when t <0 -> 0
                | _ -> qty
            qty  

    let makeBagItemsDistinct (bag: GameItem array) = 
        bag |> Seq.distinct |> Seq.toArray
    
    type Inventory = {
        Bag : GameItem array
        Weight: float<kg>
    }
    with 
        member x.filterFromLightestToHeaviest() =
            x.Bag |> Array.sortBy(fun item -> item.ItemWeight)

        member x.filterFromHeaviestToLightest() = 
            x.Bag  |> Array.sortBy (fun x -> -x.ItemWeight - 1.0<kg>) // Sort in a descending order

        member x.addItem (ii: InventoryItem): Inventory = 
            if x.Weight >= MaxWeight <> true then x 
            elif (x.Weight + ii.ItemWeight) >= MaxWeight then x
            else 
                let oItemIndex = x.Bag |> Array.tryFindIndex(fun x -> x = ii)
                match oItemIndex with 
                | Some index -> 
                    // There already an item of this type in the bag
                    let item = x.Bag |> Array.find(fun x -> x = ii)
                    let newBag = 
                        x.Bag
                        |> Array.filter((<>) item)
                        |> Array.append 
                            [| 
                                { item with Quantity = item.Quantity + ii.Quantity }
                             |]
                        |> makeBagItemsDistinct

                    let inventory = { x with Bag = newBag }
                    { inventory with Weight = inventory.Weight + item.ItemWeight }
                | None -> 
                    let newBag = x.Bag |> Array.append [|ii|] |> makeBagItemsDistinct
                    let inventory = { x with Bag = newBag }
                    { inventory with Weight = inventory.Weight + ii.ItemWeight }

        member x.addItems (iiArr: InventoryItem array) = 
            let newBag = x.Bag |> Array.append iiArr |> makeBagItemsDistinct
            let inventory = { x with Bag = newBag }

            { inventory with Weight = inventory.Weight + 0.0<kg> }

        member x.dropItem (ii:InventoryItem) = 
            let newBag = x.Bag |> Array.filter( (<>) ii)
            let inventory = { x with Bag = newBag }
            { inventory with Weight = inventory.Weight - ii.ItemWeight }
       
    type Equipment = {
        Hat : Hat 
        Armor : Armor 
        Legs  : Pants 
        Hands : Gauntlets 
        Ring  : Ring 
        Weapon : Weaponry
        Shield : Shield option
    }