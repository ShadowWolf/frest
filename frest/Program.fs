module Program
open FrestModel
open PatternMatchers
open RequestModel
open System
open System.Text.RegularExpressions


let defaultOpt arg model =
    printfn "Unknown option %A" arg
    model

let printHelp model =
    printfn "frest is a very basic HTTP(S) RESTful endpoint tester that supports json and custom headers."
    printfn "For GET requests, key/value pairs are included as query strings; such as http://www.sampleurl.com/?key=value."
    printfn "For PUT, PATCH, and POST requests, key/value pairs are serialized as json properties in the body and sent as text."
    printfn "Arguments: frest [header:value]{0+} REQUEST URL [Key=Value]{0+}"
    model

let processArgs cmdLine model =
    let rec processor list innerModel =  
        let doArgs args m : frest_model = 
            match args with 
                | InvariantEqual "Post" -> {m with request = Post}
                | InvariantEqual "Get" -> {m with request = Get}
                | InvariantEqual "Put" -> {m with request = Put}
                | InvariantEqual "Patch" -> {m with request = Patch}
                | StartsWith "http" -> {m with url = args}
                | StartsWith "www" -> {m with url = args}
                | HasMatch "([0-9A-Z]+)=([0-9A-Z]+)" -> {m with content = (build_content args "=") :: m.content}
                | HasMatch "([0-9A-Z]+):([0-9A-Z]+)" -> {m with headers = m.headers.Add(build_header args ":")}
                | HasMatch "[/-?]" -> printHelp m
                | _ -> defaultOpt args m        
           
        match list with 
            | [] -> innerModel       
            | head :: tail -> processor tail (doArgs head innerModel)
    processor cmdLine model

// Parameters:
// frest [header:value]{0+} REQUEST URL [KEY=Value]{0+}
// 
[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    let args = argv |> List.ofSeq
    let model = { frest_model.content = []; frest_model.headers = Set.ofList [{name = "Accept"; value = "application/json"}]; frest_model.request = Get; frest_model.url = ""; quick_exit = false }
    let processed = processArgs args model

    match processed.quick_exit with
        | false -> printfn "Response from the web server: %A" (build_request processed)
        | _ -> printfn "No request to send"

    0 // return an integer exit code
