module Program
open FrestModel
open PatternMatchers
open RequestModel
open System
open System.Text.RegularExpressions

let defaultHeaders : List<frest_header> = [{name = "Accept"; value = "application/json"}]
let defaultContent = []

let defaultOpt arg model =
    printfn "Unknown option %A" arg
    {model with quick_exit = true}

let printHelp model =
    printfn ""
    printfn "frest is a very basic HTTP(S) RESTful endpoint tester that supports json and custom headers."
    printfn "For GET requests, key/value pairs are included as query strings; such as http://www.sampleurl.com/?key=value."
    printfn "For PUT, PATCH, and POST requests, key/value pairs are serialized as a json dictionary and sent in the body."
    printfn "The content-type header is appropriately set for the PUT, POST, and PATCH settings so doesn't need to be supplied."
    printfn "Arguments: frest [header:value]{0+} REQUEST URL [Key=Value]{0+} [-json=filePath]"
    printfn "Optionally, -json can be specified to load a json file. If -json is specified then other json properties are ignored."
    printfn "Default headers: %A" defaultHeaders
    printfn "Default content: %A" defaultContent
    {model with quick_exit = true}  

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
                | HasMatch "[/-]json=([/\.\\ 0-9A-Z]+)" -> {m with jsonFile = strip_json(args)}
                | HasMatch "([0-9A-Z]+)=([0-9A-Z]+)" -> {m with content = (build_content args "=") :: m.content}
                | HasMatch "([0-9A-Z]+):([0-9A-Z]+)" -> {m with headers = m.headers.Add(build_header args ":")}
                | HasMatch "[/-\?]" -> printHelp m
                | InvariantEqual "Help" -> printHelp m
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
    let model = { frest_model.content = defaultContent; frest_model.headers = Set.ofList defaultHeaders; frest_model.request = Get; frest_model.url = ""; quick_exit = false; jsonFile = String.Empty }
    let processed = processArgs args model

    match processed.quick_exit with
        | true -> printfn "No request to send"
        | false -> printfn "Response from the web server: %A" (build_request processed)

    0 // return an integer exit code
