module GLSCore.PartyCharacter

open GLSCore.GameElement
open GLSCore.CharacterInformation

open System

(* - After Move, player will be able to choose any action
   - After Attack, player won't be able to use move or defend
   - After Defend, player won't be able to use move or atttack 
   - After Rotate, player won't be able to move 
   - After EndTurn, battle sequence will move to the next character
   - CheckInventory can be use at any time during the character's turn 
*)
type PartyCharacter(job: CharacterJob, 
                    equipment: CharacterEquipement,
                    style: CombatStyle, 
                    id: int,
                    ap: int,
                    oTiers: UnitTiers option,
                    state: CharacterState,
                    direction: PlayerDirection,
                    team: Object array) = 
    inherit CharacterBase(job,state)

    member x.Job = job
    
    member x.Equipment = equipment 
    
    member x.CombatStyle = style

    member x.CharacterDirection = direction

    override x.ActionPoints = style.actionPoints
    
    override x.TeamParty = team  

    override x.MoveRange = job.Role.moveRange

    override x.CharacterState = state 

    override x.CharacterID = id

    override x.Tiers = oTiers

    override x.ExperiencePoints = 0.00

    override x.LevelUpPoints = 0

        