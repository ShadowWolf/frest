module RequestModel
open System
open FSharp.Data
open FrestModel
open Newtonsoft.Json

let makeDictionary content = 
    content 
    |> List.map (fun m -> (m.name, m.value)) 
    |> dict

let mapJson (content : List<frest_content>) =
    JsonConvert.SerializeObject(makeDictionary(content))

let mapHeaders (headers : Set<frest_header>) = 
    headers |> Set.map (fun m -> (m.name, m.value))

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

//let validateModel (model : frest_model) :  = 
//    let rec validators innerModel = 
//        if (innerModel.request = Post || innerModel.request = Put || innerModel.request = Patch && (List.tryFind (fun (m : frest_header) -> m.name = "") innerModel.headers) = None) then
//            let updatedModel = {innerModel with headers = (build_header "" ":") :: innerModel.headers}
//            validators updatedModel
//        innerModel
//
//    validators model

let addContentType (model : frest_model) = 
    if (model.content.IsEmpty = true) then
        model
    else
        match model.request with
            | Post | Put | Patch -> {model with headers = model.headers.Add(build_header "content-type:application/json" ":")}
            | _ -> model
            
        

let build_request (model : frest_model) = 
    let updatedModel = addContentType model
    send updatedModel
