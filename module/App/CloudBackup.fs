module CloudBackup

let Create () =
    System.IO.Directory.EnumerateFiles(localBackupPath) |> Seq.iter GoogleDrive.File.Upload.Try

let Delete () =
    for m in GoogleDrive.File.GetIdList.Try() do
        GoogleDrive.File.Delete.Try m.Groups.[1].Value
