﻿module PatternMatchers
open System
open System.Text.RegularExpressions

let (|InvariantEqual|_|) (str : string) arg =
    if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0 then 
        Some() 
    else 
        None

let (|StartsWith|_|) (str : string) (arg : string) =
    if arg.StartsWith(str, StringComparison.OrdinalIgnoreCase) then 
        Some()
    else
        None

// Pointless literal, I'm just playing with it :-D
[<Literal>]
let regexOptions = RegexOptions.IgnoreCase ||| RegexOptions.IgnorePatternWhitespace
let regexTimeout = TimeSpan.FromSeconds(5.0)

let (|HasMatch|_|) (pattern : string) (arg : string) =
    let r = Regex(pattern, regexOptions, regexTimeout).Match(arg)
    if r.Success then
        Some()
    else
        None