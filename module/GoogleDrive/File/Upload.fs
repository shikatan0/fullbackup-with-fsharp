module GoogleDrive.File.Upload

let private execute (zipPath: string) =
    let zipName = System.IO.FileInfo(zipPath).Name
    let metadata =
        new System.Net.Http.StringContent(
            $"""{{"mimeType":"application/zip","name":"{zipName}","parents":["{cloudBackupId}"]}}""",
            System.Text.Encoding.UTF8,
            @"application/json"
        )
    use stream = new System.IO.FileStream(zipPath, System.IO.FileMode.Open)
    let media = new System.Net.Http.StreamContent(stream)
    let content = new System.Net.Http.MultipartFormDataContent()
    content.Add(metadata, "Metadata")
    content.Add(media, "Media")
    let request =
        new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Post,
            "https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart",
            Content = content
        )
    request.Headers.Add("Authorization", $"Bearer {GoogleDrive.AccessToken.Get.Value}")
    Http.Send request

let rec Try (zipPath: string) =
    match execute zipPath with
    | r when r.IsSuccessStatusCode                                 -> ()
    | r when r.StatusCode = System.Net.HttpStatusCode.Unauthorized -> retry   zipPath
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry        -> backoff zipPath 0
    | _                                                            -> failwith "GetIdList.Try"
and private retry (zipPath: string) =
    GoogleDrive.AccessToken.Refresh.Try()
    Try zipPath
and private backoff (zipPath: string) (count: int) =
    if count > 9 then failwith "GetIdList.backoff"
    Backoff.Exponential count
    match execute zipPath with
    | r when r.IsSuccessStatusCode                                 -> ()
    | r when r.StatusCode = System.Net.HttpStatusCode.Unauthorized -> retry   zipPath
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry        -> backoff zipPath (count + 1)
    | _                                                            -> failwith "GetIdList.backoff"
