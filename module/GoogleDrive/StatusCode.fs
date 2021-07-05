module GoogleDrive.StatusCode

let private retry =
    Set [
        System.Net.HttpStatusCode.TooManyRequests
        System.Net.HttpStatusCode.InternalServerError
        System.Net.HttpStatusCode.BadGateway
        System.Net.HttpStatusCode.ServiceUnavailable
        System.Net.HttpStatusCode.GatewayTimeout
    ]

let IsRetry (statusCode: System.Net.HttpStatusCode) =
    retry |> Set.contains statusCode
