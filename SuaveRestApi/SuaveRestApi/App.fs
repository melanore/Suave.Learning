module SuaveRestApi.App
open SuaveRestApi.Rest.RestFul
open Suave.Web

[<EntryPoint>]
let main _ =
    let personWebPart = rest "people" {
        GetAll = Db.getPeople
        Create = Db.createPerson
        Update = Db.updatePerson
        Delete = Db.deletePerson
    }
    startWebServer defaultConfig personWebPart
    0
