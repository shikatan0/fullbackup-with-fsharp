module GoogleDrive.File.GetIdList

let private regex =
    System.Text.RegularExpressions.Regex(
        "\"id\" *: *\"(.*?)\"",
        System.Text.RegularExpressions.RegexOptions.Compiled
    )

let private execute () =
    let request =
        new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Get,
            $"https://www.googleapis.com/drive/v3/files?fields=files(id)&q='{cloudBackupId}' in parents"
        )
    request.Headers.Add("Authorization", $"Bearer {GoogleDrive.AccessToken.Get.Value}")
    Http.Send request

let rec Try () =
    match execute() with
    | r when r.IsSuccessStatusCode                                 -> regex.Matches <| r.Content.ReadAsStringAsync().Result
    | r when r.StatusCode = System.Net.HttpStatusCode.Unauthorized -> retry()
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry        -> backoff 0
    | _                                                            -> failwith "GetIdList.Try"
and private retry () =
    GoogleDrive.AccessToken.Refresh.Try()
    Try()
and private backoff (count: int) =
    if count > 9 then failwith "GetIdList.backoff"
    Backoff.Exponential count
    match execute() with
    | r when r.IsSuccessStatusCode                                 -> regex.Matches <| r.Content.ReadAsStringAsync().Result
    | r when r.StatusCode = System.Net.HttpStatusCode.Unauthorized -> retry()
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry        -> backoff (count + 1)
    | _                                                            -> failwith "GetIdList.backoff"
