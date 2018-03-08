namespace Suave
module Http = 
    
    type RequestType = GET | POST
    
    type Request = {
        Route : string
        Type : RequestType
    }

    type Response = {
        Content : string
        StatusCode : int
    }

    type Context = {
        Request : Request
        Response : Response
    }

    type WebPart = Context -> Async<Context option>