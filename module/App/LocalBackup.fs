module LocalBackup

let Create () =
    targetDirList |> Seq.iter Zip.Create

let Delete () =
    for file in System.IO.DirectoryInfo(localBackupPath).EnumerateFiles() do
        file.Delete()
