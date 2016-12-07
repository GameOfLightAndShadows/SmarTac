module GLSManager.TeamPartyManager

open GLSCore.CharacterInformation
open GLSCore.GameItemsModel
open GLSCore.HelperFunctions

open GLSManager.Protocol
open GLSManager.StateServer

open Akka.Actor
open Akka.FSharp

[<Literal>]
let MaxSizePartyAdmissable = 5 

type TeamPartyState = { 
    TeamInventory : Inventory 
    PartyMembers  : GameCharacter array 
    AllMembers    : GameCharacter list 
}
with    
    static member InitialState = { TeamInventory = Inventory.InitialInventory; PartyMembers = [||]; AllMembers = [] }

let teamPartyManager (mailbox: Actor<TeamPartyProtocol>) =
    let rec handleProtocol (state: TeamPartyState) = actor { 
        let! message = mailbox.Receive()

        match message with 
        | RemoveCharacterFromParty gc -> 
            let oPartyMember = state.PartyMembers |> Array.tryFind(fun character -> character = gc)
            match oPartyMember with 
            | None -> 
                return! handleProtocol state
            | Some _ ->     
                return! handleProtocol { state with PartyMembers = state.PartyMembers |> Array.filter(fun c -> c <> gc) }    

        | RemoveCharacterFromTeam gc -> 
            let state = { state with AllMembers = state.AllMembers |> List.filter(fun c -> c <> gc) }
            let oPartyMember = state.PartyMembers |> Array.tryFind(fun character -> character = gc)
            match oPartyMember with 
            | None -> 
                return! handleProtocol state
            | Some _ ->     
                return! handleProtocol { state with PartyMembers = state.PartyMembers |> Array.filter(fun c -> c <> gc) }    
        
        | AddCharacterToParty gc -> 
            if not (state.PartyMembers.Length < MaxSizePartyAdmissable) then return! handleProtocol state
            else 
                return! handleProtocol { state with PartyMembers = [|gc|] |> Array.append state.PartyMembers }
        | AddCharacterToTeam gc -> 
            return! handleProtocol { state with PartyMembers = [|gc|] |> Array.append state.PartyMembers }
                
    } handleProtocol TeamPartyState.InitialState


