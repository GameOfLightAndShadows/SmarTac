module GLSManager.GlobalGLSState

open GLSCore.PartyCharacter
open GLSCore.GameMap
open GLSCore.GameElement
 
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

type BattleChoice = 
    | MeleeAttack 
    | ClassAttack 
    | Defense 
    | SelectItem 

type BattlePhase = 
    | Move
    | EngageCharacter of BattleChoice
    | EndTurn

type MatchState = 
    | InProcess
    | BrainWon 
    | PlayerWon
with 
    static member Initial = InProcess

type BattleSequenceState = {
    ActivePhase     : BattlePhase
    PlayerTeamParty : PartyCharacter array
    BrainTeamParty  : PartyCharacter array
    Board           : GameBoard
    MatchState      : MatchState
}

with 
    member x.updateBoardState (b: GameBoard) = 
        { x with Board = b }

    member x.updateBrainTeamParty (team: PartyCharacter array) = 
        { x with BrainTeamParty = team }

    member x.updatePlayerParty (team: PartyCharacter array) = 
        { x with PlayerTeamParty = team }

    member x.updateMatchState (state: MatchState) = 
        { x with MatchState = state }


    static member Initial = {
        ActivePhase = Move 
        PlayerTeamParty = [| |]
        BrainTeamParty = [| |]
        Board = GameBoard.InitialBoard()
        MatchState = MatchState.Initial
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
    member x.updateMenuState (menu: MenuState) = 
        { x with Menu = menu }

    member x.updateStoryline (sl: Storyline) = 
        { x with Story = sl }

    member x.updateBattleSequence (bss: BattleSequenceState) = 
        { x with BattleSequence = bss }

    member x.updateItemStore (iss: ItemStoreState) = 
        { x with ItemStore = iss }

    member x.updateWeaponStore (wss: WeaponStoreState) = 
        { x with WeaponStore = wss }

    static member Initial = {
        Menu = MenuState.Initial
        Story = Storyline.FirstLevel
        BattleSequence = BattleSequenceState.Initial
        ItemStore = ItemStoreState.Initial 
        WeaponStore = WeaponStoreState.Initial
    }