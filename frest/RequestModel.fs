module RequestModel
open System
open FSharp.Data
open FrestModel
open Newtonsoft.Json

let mapJson (content : List<frest_content>) = 
    JsonConvert.SerializeObject(content |> List.map (fun m -> (m.name, m.value)))

let mapHeaders (headers : List<frest_header>) = 
    headers |> List.map (fun m -> (m.name, m.value))

let mapQuery request content = 
    match request with
        | Get -> content |> List.map (fun m -> (m.name, m.value))
        | _ -> []

let mapBody request content =
    match request with
        | Post | Patch | Put -> HttpRequestBody.TextRequest(mapJson content)
        | _ -> HttpRequestBody.TextRequest("")

let send (model : frest_model) =
    let requestType = model.request.ToString()
    printfn "Performing request type %A" (model.request) 
    try
        Http.RequestString(model.url, 
                       httpMethod=model.request.ToString(), 
                       headers=(mapHeaders model.headers), 
                       query=(mapQuery model.request model.content), 
                       body=(mapBody model.request model.content))
    with
        | :? System.Net.WebException as webEx -> webEx.ToString()


let build_request (model : frest_model) = 
    send model
