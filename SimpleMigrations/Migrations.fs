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
    
    type Operation =
    | AlterEntity of AlterEntityArgs
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

let alterEntity target alterations =
    Operation.AlterEntity { Target = target
                            Alterations = alterations }

let drop target = EntityAlteration.Drop

let reference name target = EntityAlteration.Reference
let references name target = EntityAlteration.References
let boolean name = EntityAlteration.Boolean

let script ss = Operation.Script ss