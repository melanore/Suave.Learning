namespace SuaveRestApi

open System.Collections.Generic

type DocumentDb<'a> = {
        GetAll      : unit      -> 'a seq
        Create      : 'a        -> 'a
        Update      : int -> 'a -> 'a option
        Delete      : int       -> unit option
        GetById     : int       -> 'a option
        Exists      : int       -> bool
    }

module Db =
    let init<'a> () : DocumentDb<'a> = 
        let storage = Dictionary<int, 'a>()
        let getAll () = storage.Values :> seq<'a>
        let create record = 
            let id = storage.Values.Count + 1
            storage.Add(id, record)
            record
        let delete id = 
            match storage.Remove id with
            | true  -> Some ()
            | _     -> None 
        let getById id = if storage.ContainsKey(id) then Some storage.[id] else None 
        let exists = storage.ContainsKey
        let update id record = 
            match delete id with 
            | Some _ -> create record |> Some
            | None   -> None
        {
            GetAll  = getAll
            Create  = create
            Delete  = delete
            GetById = getById
            Exists  = exists    
            Update  = update 
        }