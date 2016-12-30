namespace Common

type Result<'TSuccess> = 
    | Success of 'TSuccess
    | Failure of string