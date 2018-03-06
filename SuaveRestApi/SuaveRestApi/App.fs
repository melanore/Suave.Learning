module SuaveRestApi.App
open SuaveRestApi.Rest.RestFul
open Suave.Web
open Suave

type Person = {
    Id      : int
    Name    : string
    Age     : int
    Email   : string
}

type Book = {
    Id          : int
    Title       : string
    Description : string option
    Author      : Person
}

[<EntryPoint>]
let main _ =
    let personDb = Db.init<Person>()
    let bookDb = Db.init<Book>()

    choose [
        rest "people" {
            GetAll      = personDb.GetAll
            Create      = personDb.Create
            Update      = fun record -> personDb.Update record.Id record
            Delete      = personDb.Delete
            GetById     = personDb.GetById
            UpdateById  = personDb.Update
            Exists      = personDb.Exists
        }
        rest "books" {
            GetAll      = bookDb.GetAll
            Create      = bookDb.Create
            Update      = fun record -> bookDb.Update record.Id record
            Delete      = bookDb.Delete
            GetById     = bookDb.GetById
            UpdateById  = bookDb.Update
            Exists      = bookDb.Exists
        }
    ] |> startWebServer defaultConfig
    0
