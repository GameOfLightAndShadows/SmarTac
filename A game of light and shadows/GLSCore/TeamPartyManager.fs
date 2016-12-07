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
    PartyMembers  : GameCharacter array 
    TeamInformation : TeamInformation
}
with    
    static member InitialState = { PartyMembers = [||]; TeamInformation = TeamInformation.Initial }

let teamPartyManager (mailbox: Actor<TeamPartyProtocol>) =
    let rec handleProtocol (state: TeamPartyState) = actor { 
        let! message = mailbox.Receive()
        match message with 
        | ShareTeamInformation -> 
            mailbox.Sender() <! AcceptTeamInformation state.TeamInformation
            return! handleProtocol state

        | RemoveCharacterFromParty gc -> 
            let oPartyMember = state.PartyMembers |> Array.tryFind(fun character -> character = gc)
            match oPartyMember with 
            | None -> 
                return! handleProtocol state
            | Some _ ->     
                return! handleProtocol { state with PartyMembers = state.PartyMembers |> Array.filter(fun c -> c <> gc) }    

        | RemoveCharacterFromTeam gc -> 
            let teamInfo = { state.TeamInformation with Members = state.TeamInformation.Members |> List.filter(fun c -> c <> gc) }
            let state = { state with TeamInformation = teamInfo }
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


let teamPartySystem = spawn system "Team party System" <| teamPartyManager