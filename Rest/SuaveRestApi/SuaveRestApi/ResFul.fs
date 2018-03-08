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
        GetAll      : unit      -> 'a seq
        Create      : 'a        -> 'a
        Update      : 'a        -> 'a option
        Delete      : int       -> unit option
        GetById     : int       -> 'a option
        UpdateById  : int -> 'a -> 'a option
        Exists      : int       -> bool
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
        let json v = toJson v |> OK >=> Writers.setMimeType "application/json; charset=utf-8"   
        let badRequest = BAD_REQUEST "Resource not found"
        let handleResource requestError = function
            | Some r -> r |> json
            | _      -> requestError

        let getResource             = fun _ -> resource.GetAll () |> json
        let createResource          = getResourceFromReq >> resource.Create >> json
        let updateResource          = getResourceFromReq >> resource.Update >> handleResource badRequest
        let getResourceById         = resource.GetById >> handleResource (NOT_FOUND "Resource not found")
        let updateResourceById id   = request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest) 
        let deleteResourceById id   =
            match resource.Delete id with
            | Some _ -> NO_CONTENT
            | _      -> badRequest
        let doesResourceExist id    = if resource.Exists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET     >=> warbler getResource
                POST    >=> request createResource
                PUT     >=> request updateResource
            ]
            GET     >=> pathScan resourceIdPath getResourceById
            PUT     >=> pathScan resourceIdPath updateResourceById
            DELETE  >=> pathScan resourceIdPath deleteResourceById
            HEAD    >=> pathScan resourceIdPath doesResourceExist
        ]