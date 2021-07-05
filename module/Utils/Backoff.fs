module Backoff

let private randomNumberGenerator = System.Random()

let Exponential (retryCount: int) =
    let currentWaitMilliseconds =
        min
        <| (2. ** float retryCount) * 1000.
        <| 32000.
    let jitterMilliseconds = int currentWaitMilliseconds + randomNumberGenerator.Next(1, 1001)
    System.Threading.Tasks.Task.Delay(jitterMilliseconds).Wait()
