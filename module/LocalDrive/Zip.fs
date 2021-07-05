module Zip

open System.IO.Compression

type private ArchiveCreator (rootPath: string) =
    let zip =
        ZipFile.Open(
            $@"{localBackupPath}\{FileName.Get rootPath}.zip",
            ZipArchiveMode.Update
        )

    let relativeOffset = rootPath.Length + 1

    member this.Execute () =
        this.AddContent rootPath
        zip.Dispose()

    member private this.AddContent (location: string) =
        System.IO.Directory.EnumerateFiles       location |> Seq.iter this.CreateFile
        System.IO.Directory.EnumerateDirectories location |> Seq.iter this.CreateDir

    member private this.CreateFile (filePath: string) =
        if not (Ignore.Has filePath) then
            ignore <|
                zip.CreateEntryFromFile(
                    filePath,
                    filePath.[relativeOffset ..],
                    CompressionLevel.Optimal
                )

    member private this.CreateDir (dirPath: string) =
        if not (Ignore.Has dirPath) then
            ignore <|
                zip.CreateEntry(
                    $"{dirPath.[relativeOffset ..]}\\",
                    CompressionLevel.Optimal
                )
            this.AddContent dirPath

let Create (dirPath: string) =
    (ArchiveCreator dirPath).Execute()
