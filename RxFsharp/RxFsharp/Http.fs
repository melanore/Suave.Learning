module Http

open FSharp.Control.Reactive
open HttpFs.Client
open Hopac
open Suave

type HttpResponse =
    | Ok of string
    | Error of int

let getResponseAsync url = 
    job {
        use! response = Request.createUrl Get url 
                        |> Request.setHeader (UserAgent "fsharprx")
                        |> getResponse
        match response.statusCode with
        | 200 -> 
            let! body = Response.readBodyAsString response
            return Ok body 
        | _   -> return Error response.statusCode
    }
    |> Job.toAsync


let asyncResponseToObservable = getResponseAsync >> Observable.ofAsync