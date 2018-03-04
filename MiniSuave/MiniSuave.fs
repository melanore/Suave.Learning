module MiniSuave

open Suave.Http
open Suave.Console
open Suave.Successful
open System

[<EntryPoint>]
let main argv =
    let request = { Route = ""; Type = GET }
    let response = { Content = String.Empty; StatusCode = 200 }
    let context = { Request = request; Response = response }
    OK "Hello Suave!" |> executeInLoop context
    0
