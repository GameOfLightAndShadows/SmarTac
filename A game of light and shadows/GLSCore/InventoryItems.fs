    module GLSCore.InventoryItems

    open System 

    //Represents unit for weight in kilograms
    [<Measure>] type kg

    //Represents unit for currency
    [<Measure>] type usd

    //Max weight of the inventory bag
    [<Literal>] 
    let MaxWeight = 100.00<kg>
   
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

    type InventoryItem =

        abstract member Quantity : int with get, set


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
    interface InventoryItem 
    with 
        override x.ToString() = 
            match x with 
            | HealthPotion      -> "Health Potion"
            | HighHealthPotion  -> "High Health Potion"
            | MegaHealthPotion  -> "Mega Health Potion"
            | Elixir            -> "Elixir"
            | HighElixir        -> "High Elixir"
            | MegaElixir        -> "Mega Elixir"
            | PhoenixFeather    -> "Phoenix Feather"
            | MedicinalHerb     -> "Medicinal Herb"
            | Antidote          -> "Antidote"

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
            | Antidote           -> 0.10<kg>

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
            | Antidote           -> 125<usd>

        member x.ItemQuantity = 
            



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

    let makeBagItemsDistinct (bag: InventoryItem array) = 
        bag |> Seq.distinct |> Seq.toArray

    type GameItems = 
        | Consummable   of ConsummableItem
        | Shield        of Shield 
        | Ring          of Ring 
        | Gloves        of Gauntlets 
        | Legs          of Pants
        | Armor         of Armor 
        | Hat           of Hat 
        | Weapon        of Weaponry
        | Spells        of Spellbook
        | LongBlade     of Blade 
        | MagicStaff    of Staff
        | LongSpear     of Spear 
        | Axe           of Axe 
        | Sword         of Sword
        | Dagger        of Dagger 
    with 

    (*
        abstract member ItemName : string 

        abstract member ItemDescription : string 

        abstract member ItemWeight : float<kg> 

        abstract member ItemPrice : float<usd> 

        abstract member Quantity : int with get, set

    *)
        member x.Name 

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