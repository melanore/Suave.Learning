namespace SuaveRestApi.Rest
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters
open Suave.RequestErrors

[<AutoOpen>]
module RestFul =
    type RestResource<'a> = {
        GetAll : unit   -> 'a seq
        Create : 'a     -> 'a
        Update : 'a     -> 'a option
        Delete : int    -> unit option
    }

    let toJson v =
        let settings = JsonSerializerSettings (ContractResolver = CamelCasePropertyNamesContractResolver())
        JsonConvert.SerializeObject(v, settings)

    let fromJson<'a> = JsonConvert.DeserializeObject<'a>

    let getResourceFromReq<'a> (req : HttpRequest) = 
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let resourceIdPath =
            let path = resourcePath + "/%d"
            PrintfFormat<(int -> string), unit, string, string, int>(path)

        let badRequest = BAD_REQUEST "Resource not found"
        let json v = toJson v |> OK >=> Writers.setMimeType "application/json; charset=utf-8"   

        let handleResource requestError = function
            | Some r -> r |> json
            | _      -> requestError

        let deleteResourceById id =
            match resource.Delete id with
            | Some _ -> NO_CONTENT
            | _      -> badRequest

        choose [
            path resourcePath >=> choose [
                GET     >=> warbler (fun _ -> resource.GetAll () |> json)
                POST    >=> request (getResourceFromReq >> resource.Create >> json) 
                PUT     >=> request (getResourceFromReq >> resource.Update >> handleResource badRequest)
            ]
            DELETE  >=> pathScan resourceIdPath deleteResourceById
        ]