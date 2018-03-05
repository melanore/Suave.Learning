module MiniSuave

open Suave.Http
open Suave.Console
open Suave.Successful
open Suave.Filters
open Suave.Combinators
open System

[<EntryPoint>]
let main argv =
    let request = { Route = ""; Type = Suave.Http.GET }
    let response = { Content = String.Empty; StatusCode = 200 }
    let context = { Request = request; Response = response }
    
    let app = Choose [
                GET >=> Path "/hello" >=> OK "Hello GET"
    ]

    executeInLoop context app
    0
