module GLSManager.GlobalGLSState

open GLSCore.PartyCharacter
open GLSCore.GameMap
 
type GameMode = 
    | Easy
    | Moderate 
    | Hard 

type ActivationMode = 
    | On 
    | Off

type GameOptions = 
    | ModifyVolume
    | VibrateMode of ActivationMode
    | ModifyGameMode of GameMode

type Storyline = 
    | GameMechanicsIntroduction 
    | Prelude
    | FirstBattle 
    | LostCityOfBaltazaar
    | ImpenetrableFortressOfBarbas
    | KaltheasRiver
    | BeyondLostWoods
with 
    static member FirstLevel = GameMechanicsIntroduction
    static member LastLevel  = BeyondLostWoods

type MenuOptions =
    | SelectOpenRecentGame
    | SelectSavedFiles
    | SelectOptions
    | NoOptionSelected
with 
    static member NothingSelected = NoOptionSelected

type GLSPlayer = { 
    Username : string 
    BestCharacter : PartyCharacter option
}
with 
    static member Initial = {
        Username = ""
        BestCharacter = None
    }

type MenuState = {
    SelectedMenuOption : MenuOptions
    Player             : GLSPlayer 
}
with 
    static member Initial = {
        SelectedMenuOption = NoOptionSelected
        Player = GLSPlayer.Initial
    }

type BattlePhase = 
    | Move
    | EngageCharacter
    | EndTurn

type BattleSequenceState = {
    ActivePhase     : BattlePhase
    PlayerTeamParty : PartyCharacter array
    BrainTeamParty  : PartyCharacter array
    Board           : GameBoard
}

with 
    static member Initial = {
        ActivePhase = Move 
        PlayerTeamParty = [| |]
        BrainTeamParty = [| |]
        Board = GameBoard.InitialBoard()
    }

// Basic implementation of the weapon & item store 
// Will be move later. 
type WeaponStore = {
    RustedSword : string
    BattleAxe : string
    RustedHelmet : string 
    RustedArmor : string 
    RustedShield : string 
    CheapMagicWand : string 
}
with 
    member x.selectWeapon(weapon: string) =     
        weapon
    member x.sellWeapon(weapon: string) = 
        ()

    static member Initial = {
        RustedSword = ""
        BattleAxe = ""
        RustedHelmet = ""
        RustedArmor = ""
        RustedShield = ""
        CheapMagicWand = ""
    }

type ItemStore = {
    HealthPotion : string
    MagicPotion : string 
    PhoenixFeather : string
}
with 
    static member Initial = {
        HealthPotion = ""
        MagicPotion = ""
        PhoenixFeather = ""
    }

type WeaponStoreState = {
    Store : WeaponStore    
}
with 
    static member Initial = { Store = WeaponStore.Initial }

type ItemStoreState = {
    Store: ItemStore
}
with 
    static member Initial = { Store = ItemStore.Initial }

type GlobalGameState = {
    Menu : MenuState
    Story : Storyline
    BattleSequence : BattleSequenceState
    ItemStore : ItemStoreState
    WeaponStore : WeaponStoreState
}
with 
    static member Initial = 
    {
        Menu = MenuState.Initial
        Story = Storyline.FirstLevel
        BattleSequence = BattleSequenceState.Initial
        ItemStore = ItemStoreState.Initial 
        WeaponStore = WeaponStoreState.Initial
    }