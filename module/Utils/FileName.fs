module FileName

let Get (path: string) =
    let names = path.Split('\\')
    names.[names.Length - 1]