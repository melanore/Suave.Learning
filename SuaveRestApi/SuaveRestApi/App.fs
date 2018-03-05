module SuaveRestApi.App
open SuaveRestApi.Rest.RestFul
open Suave.Web

[<EntryPoint>]
let main argv =
    let personWebPart = rest "people" {
        GetAll = Db.getPeople
    }
    startWebServer defaultConfig personWebPart
    0
