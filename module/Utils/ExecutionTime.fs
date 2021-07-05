module ExecutionTime

let Of (f: unit -> unit) =
    let stopwatch = System.Diagnostics.Stopwatch()
    stopwatch.Start()
    f()
    stopwatch.Stop()
    printfn "%O" stopwatch.Elapsed