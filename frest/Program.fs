module Program
open FrestModel
open PatternMatchers
open System
open System.Text.RegularExpressions

// Parameters:
// frest [header:value]{0+} REQUEST URL [KEY=Value]{0+}
// 



let defaultOpt arg model =
    printfn "Unknown option %A" arg
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
                | HasMatch "([0-9A-Z]+):([0-9A-Z]+)" -> {m with headers = (build_header args ":") :: m.headers}
                | _ -> defaultOpt args m        
           
        match list with 
            | [] -> innerModel       
            | head :: tail -> processor tail (doArgs head innerModel)
    processor cmdLine model

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    let args = argv |> List.ofSeq
    let model = { frest_model.content = []; frest_model.headers = []; frest_model.request = Get; frest_model.url = "" }
    let processed = processArgs args model
    printfn "%A" processed
    0 // return an integer exit code
