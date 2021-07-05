module Ignore

let private regexList = ignoreList |> List.map PatternToRegex.Execute

let private isMatch (path: string) (regex: System.Text.RegularExpressions.Regex) =
    regex.IsMatch(path)

let Has (path: string) =
    (regexList |> List.tryFind (isMatch path)).IsSome