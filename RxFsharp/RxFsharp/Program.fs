open Suave.Filters
open Suave

[<EntryPoint>]
let main _ = 
    let webpart = pathScan "/api/profile/%s" ApiGateway.getProfile
    startWebServer defaultConfig webpart
    0 