module FrestModel

type frest_header = { 
    name: string; 
    value: string;
}

type frest_request = 
    Get
    | Post
    | Put 
    | Patch 


type url = string

type frest_content = {
    name: string;
    value: string;
}

type frest_model = {
    headers: List<frest_header>;
    request: frest_request;
    url: url;
    content: List<frest_content>;
}