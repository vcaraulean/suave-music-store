open Suave
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.Types
open Suave.Web

let browse = 
    request (fun r -> 
        match r.queryParam "genre" with
        | Choice1Of2 genre -> OK (sprintf "Browsing genre: %s" genre)
        | Choice2Of2 msg -> BAD_REQUEST msg
    )

let webPart = 
    choose [
        path "/" >>= OK "Home"
        path "/store" >>= OK "Store"
        path "/store/browse" >>= browse
        path "/store/details" >>= OK "Details"
        pathScan "/store/details/%d" (fun id -> OK (sprintf "Details: %d" id))
    ]

startWebServer defaultConfig webPart
