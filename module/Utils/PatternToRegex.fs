module PatternToRegex

let private escape (text: string) =
    text
        .Replace("\\", "\\\\")
        .Replace("/", "\\\\")
        .Replace(".", "\\.")

let private replace (text: string) =
    text
        .Replace("*", "[^\\\\]*?")

let private convert (text: string) =
    $"^.*\\\\{text}$"

let private regex (text: string) =
    System.Text.RegularExpressions.Regex(
        text,
        System.Text.RegularExpressions.RegexOptions.Compiled
    )

let Execute = escape >> replace >> convert >> regex
