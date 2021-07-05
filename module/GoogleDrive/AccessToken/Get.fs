module GoogleDrive.AccessToken.Get

let private regex =
    System.Text.RegularExpressions.Regex(
        "\"access_token\" *: *\"(.*?)\"",
        System.Text.RegularExpressions.RegexOptions.Compiled
    )

let private execute () =
    let request =
        new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Post,
            "https://www.googleapis.com/oauth2/v4/token",
            Content =
                new System.Net.Http.StringContent(
                    $"""{{"client_id":"{clientId}","client_secret":"{clientSecret}","grant_type":"refresh_token","refresh_token":"{refreshToken}"}}""",
                    System.Text.Encoding.UTF8,
                    @"application/json"
                )
        )
    Http.Send request

let rec Try () =
    match execute () with
    | r when r.IsSuccessStatusCode                          -> regex.Match(r.Content.ReadAsStringAsync().Result).Groups.[1].Value
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry -> backoff 0
    | _                                                     -> failwith "AccessToken.Get.Try"
and private backoff (count: int) =
    if count > 9 then failwith "AccessToken.Get.backoff"
    Backoff.Exponential count
    match execute() with
    | r when r.IsSuccessStatusCode                          -> regex.Match(r.Content.ReadAsStringAsync().Result).Groups.[1].Value
    | r when r.StatusCode |> GoogleDrive.StatusCode.IsRetry -> backoff (count + 1)
    | _                                                     -> failwith "AccessToken.Get.backoff"

let mutable Value = Try()
