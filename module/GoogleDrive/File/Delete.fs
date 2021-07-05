module GoogleDrive.File.Delete

let private execute (id: string) =
    let request =
        new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Delete,
            $"https://www.googleapis.com/drive/v3/files/{id}"
        )
    request.Headers.Add("Authorization", $"Bearer {GoogleDrive.AccessToken.Get.Value}")
    Http.Send request

let rec Try (id: string) =
    match execute id with
    | r when r.StatusCode = System.Net.HttpStatusCode.NoContent    -> ()
    | r when r.StatusCode = System.Net.HttpStatusCode.Unauthorized -> retry   id
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry        -> backoff id 0
    | _                                                            -> failwith "Delete.Try"
and private retry (id: string) =
    GoogleDrive.AccessToken.Refresh.Try()
    Try id
and private backoff (id: string) (count: int) =
    if count > 9 then failwith "GetIdList.backoff"
    Backoff.Exponential count
    match execute id with
    | r when r.StatusCode = System.Net.HttpStatusCode.NoContent    -> ()
    | r when r.StatusCode = System.Net.HttpStatusCode.Unauthorized -> retry   id
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry        -> backoff id (count + 1)
    | _                                                            -> failwith "Delete.backoff"
