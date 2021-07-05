let App () =
    LocalBackup.Delete()
    LocalBackup.Create()
    CloudBackup.Delete()
    CloudBackup.Create()

[<EntryPoint>]
do
    ExecutionTime.Of App
    PowrProf.SetSuspendState(false, false, false) |> ignore
