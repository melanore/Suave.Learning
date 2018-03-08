module ApiGateway

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave.Operators
open Suave
open Profile
open Suave.RequestErrors

let JSON v = 
    let settings = JsonSerializerSettings(ContractResolver = CamelCasePropertyNamesContractResolver())
    JsonConvert.SerializeObject(v, settings)
    |> OK
    >=> Writers.setMimeType "application/json; charset=utf-8"

let getProfile userName httpContext = 
    async {
        let! profile = getProfile userName
        match profile with
        | Some p -> return! JSON p httpContext
        | None   -> 
            let message = sprintf "Username %s not found" userName
            return! NOT_FOUND message httpContext
    }