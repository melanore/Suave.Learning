module SuaveRestApi.App
open SuaveRestApi.Rest.RestFul
open Suave.Web

[<EntryPoint>]
let main _ =
    let personWebPart = rest "people" {
        GetAll      = PersonDb.getPeople
        Create      = PersonDb.createPerson
        Update      = PersonDb.updatePerson
        Delete      = PersonDb.deletePerson
        GetById     = PersonDb.getPersonById
        UpdateById  = PersonDb.updatePersonById
        Exists      = PersonDb.personExists
    }
    startWebServer defaultConfig personWebPart
    0
