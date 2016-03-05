module FrestModel
open System

type frest_header = { 
    name: string; 
    value: string;
}

type frest_content = {
    name: string;
    value: string;
}

type frest_request = 
    Get
    | Post
    | Put 
    | Patch 

type url = string

type frest_model = {
    headers: List<frest_header>;
    request: frest_request;
    url: url;
    content: List<frest_content>;
}

let build_header (input: string) (split : string) : frest_header = 
    let kvp = input.Split([|split|], StringSplitOptions.None)
    match kvp with
        | [|first; second|] -> {name = first; value = second}
        | _ -> failwith "Invalid input for split value"    

let build_content (input : string) (split : string) = 
    let kvp = input.Split([|split|], StringSplitOptions.None)
    match kvp with
        | [|first; second|] -> {name = first; value = second}
        | _ -> failwith "Invalid input for split value"
