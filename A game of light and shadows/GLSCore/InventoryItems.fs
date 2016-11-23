    module GLSCore.InventoryItems

    open System 

    //Represents unit for weight in kilograms
    [<Measure>] type kg

    //Represents unit for currency
    [<Measure>] type usd

    //Max weight of the inventory bag
    [<Literal>] 
    let MaxWeight = 500.00<kg>
    
    type IEnergyPoint =
        abstract CurrentPoints : float 
        abstract MaxPoints     : float
        abstract capPoints : unit -> IEnergyPoint 
        abstract raisePoints : int -> IEnergyPoint
        abstract reducePoints : int -> IEnergyPoint 

    type LifePoint = 
        { CurrentLife: float
          MaxLife : float }
        interface IEnergyPoint with
            member x.capPoints () =
                if x.CurrentLife > x.MaxLife then { LifePoint.CurrentLife = x.MaxLife; LifePoint.MaxLife = x.MaxLife } :> IEnergyPoint else x :> IEnergyPoint

            member x.raisePoints (lifePoint:int) = 
                let raisedHealth = { x with CurrentLife = x.CurrentLife + (lifePoint |> float) }
                (raisedHealth :> IEnergyPoint).capPoints()

            member x.reducePoints (hitPoint: int) = 
                let reducedLife = { x with CurrentLife = x.CurrentLife - (hitPoint |> float) }
                (reducedLife :> IEnergyPoint)

        member x.isCharacterDead() = 
            x.CurrentLife <= 0.0

    type ManaPoint = 
        { CurrentMana: float
          MaxMana : float }
        interface IEnergyPoint with
            member x.capPoints () =
                if x.CurrentMana > x.MaxMana then { ManaPoint.CurrentMana = x.MaxMana; ManaPoint.MaxMana = x.MaxMana } :> IEnergyPoint else x :> IEnergyPoint

            member x.raisePoints (manaPoint:int) = 
                let raisedHealth = { x with CurrentMana = x.CurrentMana + (manaPoint |> float) }
                (raisedHealth :> IEnergyPoint).capPoints()

            member x.reducePoints (manaPoint: int) = 
                let reducedLife = { x with CurrentMana = x.CurrentMana - (manaPoint |> float) }
                (reducedLife :> IEnergyPoint)      
       
    type CharacterStats = {
        Health          : LifePoint
        Mana            : ManaPoint 
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

    type ItemVariationProvenance = 
        | FromTheShop
        | FromTheInventory

    type ConsummableItem = 
        | HealthPotion        of int
        | HighHealthPotion    of int
        | MegaHealthPotion    of int
        | Elixir              of int
        | HighElixir          of int
        | MegaElixir          of int
        | PhoenixFeather      of int
        | MedicinalHerb       of int
        override x.ToString() = 
            match x with 
            | HealthPotion     _ -> "Health Potion"
            | HighHealthPotion _ -> "High Health Potion"
            | MegaHealthPotion _ -> "Mega Health Potion"
            | Elixir           _ -> "Elixir"
            | HighElixir       _ -> "High Elixir"
            | MegaElixir       _ -> "Mega Elixir"
            | PhoenixFeather   _ -> "Phoenix Feather"
            | MedicinalHerb    _ -> "Medicinal Herb"

        member x.Weight = 
            match x with 
            | HealthPotion       _-> 0.02<kg>
            | HighHealthPotion   _-> 0.03<kg>
            | MegaHealthPotion   _-> 0.05<kg>
            | Elixir             _-> 0.02<kg>
            | HighElixir         _-> 0.03<kg>
            | MegaElixir         _-> 0.05<kg>
            | PhoenixFeather     _-> 0.05<kg>
            | MedicinalHerb      _-> 0.03<kg>

        member x.Price = 
            match x with 
            | HealthPotion      _ -> 100<usd>
            | HighHealthPotion  _ -> 150<usd>
            | MegaHealthPotion  _ -> 225<usd>
            | Elixir            _ -> 125<usd>
            | HighElixir        _ -> 175<usd>
            | MegaElixir        _ -> 275<usd>
            | PhoenixFeather    _ -> 350<usd>
            | MedicinalHerb     _ -> 50<usd>

        member x.ItemQuantity = 
            match x with 
            | HealthPotion      q -> q
            | HighHealthPotion  q -> q
            | MegaHealthPotion  q -> q
            | Elixir            q -> q
            | HighElixir        q -> q
            | MegaElixir        q -> q
            | PhoenixFeather    q -> q
            | MedicinalHerb     q -> q

        member x.ConsummeItem (characterStat: CharacterStats) = 
            match x with 
            | HealthPotion      q -> { characterStat with Health = (characterStat.Health :> IEnergyPoint).raisePoints 15 :?> LifePoint }
            | HighHealthPotion  q -> { characterStat with Health = (characterStat.Health :> IEnergyPoint).raisePoints 30 :?> LifePoint }
            | MegaHealthPotion  q -> { characterStat with Health = (characterStat.Health :> IEnergyPoint).raisePoints 50 :?> LifePoint }
            | Elixir            q -> { characterStat with Mana = (characterStat.Health :> IEnergyPoint).raisePoints 10 :?> ManaPoint }
            | HighElixir        q -> { characterStat with Mana = (characterStat.Health :> IEnergyPoint).raisePoints 20 :?> ManaPoint }
            | MegaElixir        q -> { characterStat with Mana = (characterStat.Health :> IEnergyPoint).raisePoints 30 :?> ManaPoint }
            | PhoenixFeather    q -> 
                if characterStat.Health.isCharacterDead() <> true then characterStat
                else { characterStat with 
                        Health = (characterStat.Health :> IEnergyPoint).raisePoints ( Math.Round characterStat.Health.MaxLife * 0.50 |> int32) :?> LifePoint
                        Mana = (characterStat.Mana :> IEnergyPoint).raisePoints (Math.Round characterStat.Mana.MaxMana * 0.50 |> int32) :?> ManaPoint
                    }

            | MedicinalHerb     q ->  { characterStat with Health = (characterStat.Health :> IEnergyPoint).raisePoints 5 :?> LifePoint }
            
    type Dagger = 
        | RustedDagger of   WeaponStat  * int
        | IronDagger   of   WeaponStat  * int
        | SteelDagger  of   WeaponStat  * int
    with   
        override x.ToString() = 
            match x with 
            | RustedDagger (_,_) -> "Rusted dagger"
            | IronDagger   (_,_) -> "Iron dagger"
            | SteelDagger  (_,_) -> "Steel dagger"

        member x.WeaponRank =
            match x with 
            | RustedDagger (_,_) -> RankE
            | IronDagger   (_,_)-> RankD
            | SteelDagger  (_,_)-> RankC

        member x.WeaponStats = 
            match x with
            | RustedDagger (s,_) -> s 
            | IronDagger   (s,_) -> s
            | SteelDagger  (s,_) -> s 
            
        member x.Weight = 
            match x with 
            | RustedDagger (_,_) -> 2.10<kg>
            | IronDagger   (_,_) -> 2.80<kg>
            | SteelDagger  (_,_) -> 5.25<kg>
            
        member x.Price = 
             match x with 
             | RustedDagger (_,_) -> 80<usd> 
             | IronDagger   (_,_) -> 200<usd> 
             | SteelDagger  (_,_) -> 350<usd> 

        member x.Quantity = 
            match x with 
            | RustedDagger (_,q) -> q
            | IronDagger   (_,q) -> q 
            | SteelDagger  (_,q) -> q    

    type Sword = 
        | BrokenSword of WeaponStat * int
        | RustedSword of WeaponStat * int
        | IronSword   of WeaponStat * int
        | SteelSword  of WeaponStat * int
    with
        member x.WeaponRank =
            match x with 
            | BrokenSword (_,_) -> RankE
            | RustedSword (_,_) -> RankE
            | IronSword   (_,_) -> RankD
            | SteelSword  (_,_) -> RankC

        member x.WeaponStats = 
            match x with
            | BrokenSword (s,_) -> s
            | RustedSword (s,_) -> s
            | IronSword   (s,_) -> s
            | SteelSword  (s,_) -> s
            
        member x.Weight = 
            match x with 
            | BrokenSword (_,_) -> 7.20<kg>
            | RustedSword (_,_) -> 8.50<kg>
            | IronSword   (_,_) -> 12.35<kg>
            | SteelSword  (_,_) -> 15.00<kg>

        member x.Price = 
             match x with 
             | BrokenSword (_,_) -> 90<usd>
             | RustedSword (_,_) -> 120<usd>
             | IronSword   (_,_) -> 250<usd>
             | SteelSword  (_,_) -> 525<usd>

        member x.Quantity = 
            match x with 
            | BrokenSword (_,q) -> q
            | RustedSword (_,q) -> q
            | IronSword   (_,q) -> q 
            | SteelSword  (_,q) -> q   

    type Axe = 
        | RustedAxe         of WeaponStat  * int
        | IronAxe           of WeaponStat  * int
        | RustedBattleAxe   of WeaponStat  * int
        | IronBattleAxe     of WeaponStat  * int
        | StellBattleAxe    of WeaponStat  * int
    with
        member x.WeaponRank =
            match x with 
            | RustedAxe       (_,_) -> RankE
            | IronAxe         (_,_) -> RankD
            | RustedBattleAxe (_,_) -> RankE
            | IronBattleAxe   (_,_) -> RankD
            | StellBattleAxe  (_,_) -> RankC

        member x.WeaponStats = 
            match x with
            | RustedAxe       (s,_) -> s
            | IronAxe         (s,_) -> s
            | RustedBattleAxe (s,_) -> s
            | IronBattleAxe   (s,_) -> s
            | StellBattleAxe  (s,_) -> s

        member x.Weight = 
            match x with 
            | RustedAxe          (_,_) -> 8.00<kg>
            | IronAxe            (_,_) -> 10.00<kg>
            | RustedBattleAxe    (_,_) -> 9.00<kg>
            | IronBattleAxe      (_,_) -> 13.00<kg>
            | StellBattleAxe     (_,_) -> 16.00<kg>

        member x.Price = 
             match x with 
             | RustedAxe          (_,_) ->  125<usd>
             | IronAxe            (_,_) ->  280<usd>
             | RustedBattleAxe    (_,_) ->  150<usd>
             | IronBattleAxe      (_,_) ->  300<usd>
             | StellBattleAxe     (_,_) ->  425<usd>

        member x.Quantity = 
            match x with 
            | RustedAxe          (_,q) ->  q
            | IronAxe            (_,q) ->  q
            | RustedBattleAxe    (_,q) ->  q
            | IronBattleAxe      (_,q) ->  q
            | StellBattleAxe     (_,q) ->  q

    type Spear =
        | RustedSpear of WeaponStat * int
        | IronSpear   of WeaponStat * int
        | SteelSpear  of WeaponStat * int
    with
        member x.WeaponRank =
            match x with 
            | RustedSpear (_,_) -> RankE
            | IronSpear   (_,_) -> RankD
            | SteelSpear  (_,_) -> RankC
            
        member x.WeaponStats = 
            match x with
            | RustedSpear (s,_) -> s
            | IronSpear   (s,_) -> s
            | SteelSpear  (s,_) -> s
                      
        member x.Weight = 
            match x with 
            | RustedSpear (_,_) -> 15.0<kg>
            | IronSpear   (_,_) -> 20.0<kg>
            | SteelSpear  (_,_) -> 30.0<kg>

        member x.Price = 
             match x with 
             | RustedSpear (_,_) -> 200<usd>
             | IronSpear   (_,_) -> 325<usd>
             | SteelSpear  (_,_) -> 550<usd>

        member x.Quantity = 
            match x with 
            | RustedSpear (_,q) ->  q
            | IronSpear   (_,q) ->  q
            | SteelSpear  (_,q) ->  q             

    type Staff = 
        | RookieStaff       of WeaponStat * int 
        | AdeptStaff        of WeaponStat * int 
        | SorcererStaff     of WeaponStat * int 
        | NecromancerStaff  of WeaponStat * int 
    with
        member x.WeaponRank =
            match x with 
            | RookieStaff      (_,_) -> RankE
            | AdeptStaff       (_,_) -> RankD
            | SorcererStaff    (_,_) -> RankB
            | NecromancerStaff (_,_) -> RankA

        member x.WeaponStats = 
            match x with
            | RookieStaff      (s,_) -> s
            | AdeptStaff       (s,_) -> s
            | SorcererStaff    (s,_) -> s
            | NecromancerStaff (s,_) -> s
            
        member x.Weight = 
            match x with 
            | RookieStaff      (_,_) -> 2.20<kg>
            | AdeptStaff       (_,_) -> 4.20<kg>
            | SorcererStaff    (_,_) -> 5.10<kg>
            | NecromancerStaff (_,_) -> 3.20<kg>

        member x.Price = 
             match x with 
             | RookieStaff      (_,_) -> 180<usd>
             | AdeptStaff       (_,_) -> 270<usd>
             | SorcererStaff    (_,_) -> 445<usd>
             | NecromancerStaff (_,_) -> 650<usd>

        member x.Quantity = 
            match x with 
            | RookieStaff      (_,q)-> q
            | AdeptStaff       (_,q)-> q
            | SorcererStaff    (_,q)-> q 
            | NecromancerStaff (_,q)-> q

    type Blade = 
        | RustedLongBlade   of WeaponStat * int 
        | RustedKatana      of WeaponStat * int 
        | IronLongBlade     of WeaponStat * int 
        | CurvedLongBlade   of WeaponStat * int 
        | SteelKatana       of WeaponStat * int 
        | SteelLongBlade    of WeaponStat * int 
    with
        member x.WeaponRank =
            match x with 
            | RustedLongBlade    (_,_) -> RankE
            | RustedKatana       (_,_) -> RankE
            | IronLongBlade      (_,_) -> RankD
            | CurvedLongBlade    (_,_) -> RankD
            | SteelKatana        (_,_) -> RankC
            | SteelLongBlade     (_,_) -> RankC

        member x.WeaponStats = 
            match x with
            | RustedLongBlade    (s,_) -> s
            | RustedKatana       (s,_) -> s
            | IronLongBlade      (s,_) -> s
            | CurvedLongBlade    (s,_) -> s
            | SteelKatana        (s,_) -> s
            | SteelLongBlade     (s,_) -> s

        member x.Weight = 
            match x with 
            | RustedLongBlade    (_,_) -> 6.00<kg>
            | RustedKatana       (_,_) -> 7.75<kg>
            | IronLongBlade      (_,_) -> 14.25<kg>
            | CurvedLongBlade    (_,_) -> 11.20<kg>
            | SteelKatana        (_,_) -> 16.78<kg>
            | SteelLongBlade     (_,_) -> 15.30<kg>

        member x.Price = 
             match x with 
             | RustedLongBlade    (_,_) ->  120<usd>
             | RustedKatana       (_,_) ->  100<usd>
             | IronLongBlade      (_,_) ->  215<usd>
             | CurvedLongBlade    (_,_) ->  240<usd>
             | SteelKatana        (_,_) ->  350<usd>
             | SteelLongBlade     (_,_) ->  410<usd> 

        member x.Quantity = 
            match x with 
            | RustedLongBlade    (_,q) ->  q
            | RustedKatana       (_,q) ->  q
            | IronLongBlade      (_,q) ->  q
            | CurvedLongBlade    (_,q) ->  q
            | SteelKatana        (_,q) ->  q
            | SteelLongBlade     (_,q) ->  q

    type SpellbookStats = {
        AttackRange : int 
        Rank        : WeaponRank
        Uses    : int
    }

    type Spellbook =
        | Fireball      of SpellbookStats * int  
        | Thunder       of SpellbookStats * int 
        | Frost         of SpellbookStats * int 
        | Hellfire      of SpellbookStats * int 
        | BlackFire     of SpellbookStats * int 
        | StormOfBlades of SpellbookStats * int 
    with
        member x.SpellRank =
            match x with 
            | Fireball        (_, _) -> RankE
            | Thunder         (_, _) -> RankE
            | Frost           (_, _) -> RankE
            | Hellfire        (_, _) -> RankC
            | BlackFire       (_, _) -> RankC
            | StormOfBlades   (_, _) -> RankC


        member x.SpellStats = 
            match x with
            | Fireball        (s, _) -> s
            | Thunder         (s, _) -> s
            | Frost           (s, _) -> s
            | Hellfire        (s, _) -> s
            | BlackFire       (s, _) -> s
            | StormOfBlades   (s, _) -> s
            
        member x.Weight = 
            match x with 
            | Fireball        (_, _) -> 0.05<kg>
            | Thunder         (_, _) -> 0.05<kg>
            | Frost           (_, _) -> 0.05<kg>
            | Hellfire        (_, _) -> 0.05<kg>
            | BlackFire       (_, _) -> 0.05<kg>
            | StormOfBlades   (_, _) -> 0.05<kg>

        member x.Price = 
             match x with 
             | Fireball        (_, _) -> 150<usd>
             | Thunder         (_, _) -> 150<usd>
             | Frost           (_, _) -> 150<usd>
             | Hellfire        (_, _) -> 350<usd>
             | BlackFire       (_, _) -> 350<usd>
             | StormOfBlades   (_, _) -> 350<usd>

        member x.Quantity = 
            match x with 
            | Fireball        (_, q) -> q
            | Thunder         (_, q) -> q
            | Frost           (_, q) -> q
            | Hellfire        (_, q) -> q
            | BlackFire       (_, q) -> q
            | StormOfBlades   (_, q) -> q

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

        member x.Quantity =
            match x with
            | Dagger     w -> w.Quantity
            | Sword      w -> w.Quantity
            | Axe        w -> w.Quantity
            | Spear      w -> w.Quantity
            | Staff      w -> w.Quantity
            | LongBlade  w -> w.Quantity
            | Spellbook  w -> w.Quantity

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
            | Dagger     w -> w.WeaponStats
            | Sword      w -> w.WeaponStats
            | Axe        w -> w.WeaponStats
            | Spear      w -> w.WeaponStats
            | Staff      w -> w.WeaponStats
            | LongBlade  w -> w.WeaponStats
            // Might have problems for spell books stats... atm

        member x.Rank =
            match x with 
            | Dagger     w -> w.WeaponRank
            | Sword      w -> w.WeaponRank
            | Axe        w -> w.WeaponRank
            | Spear      w -> w.WeaponRank
            | Staff      w -> w.WeaponRank
            | LongBlade  w -> w.WeaponRank
            | Spellbook  w -> w.SpellRank

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
        | Wearable      of CharacterProtection
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