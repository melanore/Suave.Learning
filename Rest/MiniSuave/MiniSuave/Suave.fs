namespace Suave

module Successful = 
    open Http

    let OK content context = 
        { context with Response = { Content = content; StatusCode = 200 } }
        |> Some
        |> async.Return

module Combinators =

    let (>=>) first second context = async {
        let! firstContext = first context
        match firstContext with
        | None          -> return None
        | Some context  ->
        return! second context
    }

module Filters =
    open Http

    let private iff condition context =
        if condition context 
        then context |> Some |> async.Return
        else None |> async.Return

    let private strOrEmpty str = 
        if System.String.IsNullOrEmpty(str) then System.String.Empty else str.Trim()

    let GET = iff (fun context -> context.Request.Type = GET)

    let POST = iff (fun context -> context.Request.Type = POST)

    let Path path = iff (fun context -> strOrEmpty context.Request.Route = strOrEmpty path)

    let rec Choose webparts context = async {
        match webparts with
        | []        -> return None
        | x :: xs   ->
        let! result = x context
        match result with
        | Some x    -> return Some x
        | None      -> return! Choose xs context
    }

module Console = 
    open Http
    open System

    let execute inputContext webpart = 
        async {
            let! outputContext = webpart inputContext
            match outputContext with
            | Some context ->
                printfn "---------------"
                printfn "Code : %d" context.Response.StatusCode
                printfn "Output : %s" context.Response.Content
                printfn "---------------"
            | None -> 
                printfn "No Output"    
        } |> Async.RunSynchronously

    let parseRequest (input : string) =
        let parts = input.Split([|';'|])
        let rawType = parts.[0]
        let route = parts.[1]
        match rawType with
        | "GET"     -> { Type = GET; Route = route }
        | "POST"    -> { Type = POST; Route = route }
        | _         -> failwith "invalid request"

    let executeInLoop inputContext webpart = 
        let handleInput inputContext webpart = 
            printf "Enter Input Route : "
            match Console.ReadLine() with 
            | "exit" -> 0
            | route  -> 
                try
                    execute { inputContext with Request = parseRequest route } webpart
                    1
                with | ex -> 
                    printfn "Error : %s" ex.Message 
                    -1

        seq { while true do yield handleInput inputContext webpart }
        |> Seq.takeWhile (fun s -> s <> 0)
        |> Seq.iter ignore