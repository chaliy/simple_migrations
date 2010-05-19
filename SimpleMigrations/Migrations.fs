module Migrations

module Model =

    type EntityAlteration =
    | Drop
    // Add fields
    | Reference
    | References
    | Boolean

    type AlterEntityArgs = {
        Target : string
        Alterations : EntityAlteration list
    }

    type CreateEntityArgs = {
        Target : string
        Fields : EntityAlteration list
    }
    
    type Operation =
    | AlterEntity of AlterEntityArgs
    | CreateEntity of CreateEntityArgs
    | Script of string list
    
    type Migration = {
        Version : string
        Operations : Operation list
    }

// DSL
open Model
let migration version operations =
    { Version = version
      Operations = operations }

type AlterEntityCtx() = 

    let alterations = System.Collections.Generic.List<EntityAlteration>()
    let add = alterations.Add

    member this.drop target = add EntityAlteration.Drop
    member this.reference name target = add EntityAlteration.Reference
    member this.references name target = add EntityAlteration.References
    member this.boolean name = add EntityAlteration.Boolean
    member this.Alterations = List.ofSeq(alterations);


let alterEntity target alterations =
    Operation.AlterEntity { Target = target
                            Alterations = alterations }

let drop target = EntityAlteration.Drop
let reference name target = EntityAlteration.Reference
let references name target = EntityAlteration.References
let boolean name = EntityAlteration.Boolean

let script ss = Operation.Script ss

let createEntity target fields = 
    Operation.CreateEntity { Target = target
                             Fields = fields }