module Http

let private client = new System.Net.Http.HttpClient()
client.Timeout <- System.Threading.Timeout.InfiniteTimeSpan

let Send (request) =
    client.SendAsync(request).Result