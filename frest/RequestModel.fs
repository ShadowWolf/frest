module RequestModel
open System
open FSharp.Data
open FrestModel
open Newtonsoft.Json

let makeDictionary content = 
    content 
    |> List.map (fun m -> (m.name, m.value)) 
    |> dict

let makeJsonContent (model : frest_model) =
    if (String.IsNullOrEmpty model.jsonFile = true) then
        makeDictionary model.content
        |> JsonConvert.SerializeObject
    else
        System.IO.File.ReadAllText model.jsonFile


let mapHeaders (headers : Set<frest_header>) = 
    headers 
    |> Set.map (fun m -> (m.name, m.value))

let mapQuery request content = 
    match request with
        | Get -> content |> List.map (fun m -> (m.name, m.value))
        | _ -> []

let mapBody request content =
    match request with
        | Post | Patch | Put -> HttpRequestBody.TextRequest content
        | _ -> HttpRequestBody.TextRequest("")

let send (model : frest_model) =
    printfn "Performing request %A to url %A" model.request model.url
    try
        Http.RequestString(model.url, 
                       httpMethod = model.request.ToString(), 
                       headers = (mapHeaders model.headers), 
                       query = (mapQuery model.request model.content), 
                       body = (mapBody model.request (makeJsonContent model)))
    with
        | :? System.Net.WebException as webEx -> webEx.ToString()
        
let addContentType (model : frest_model) = 
    if (model.content.IsEmpty = true) then
        model
    else
        match model.request with
            | Post | Put | Patch -> {model with headers = model.headers.Add (build_header "content-type:application/json" ":")}
            | _ -> model      

let build_request (model : frest_model) = 
    if ((String.IsNullOrEmpty model.jsonFile) = false && model.content.IsEmpty = false) then
        "Both a json file and a list of key/value pairs was supplied."
    else 
        send (addContentType model)
