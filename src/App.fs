module App

open Fable.Core.JsInterop
open Fable.Import
open Fetch
open Thoth.Json

type PictureInfo = { Url : string }

let window = Browser.Dom.window

let showPic url =
  // make image mutable since we need to mutate it's src field
  let mutable image : Browser.Types.HTMLImageElement = unbox window.document.createElement "img"
  let container = window.document.getElementById "myDogContainer"
  image.src <- url
  image.width <- 200.
  container.appendChild image |> ignore // ignore means we don't need to use the return value. Since F# is a functional language we always do return something.

// async/await (essentially the promise in action here)
let getRandomDogImage url =
    fetch url [] // use the fetch api to load our resource
    |> Promise.bind (fun res -> res.text()) // get the resul
    |> Promise.map (fun txt -> // bind the result to make further operation
   
      let decoded = Decode.Auto.fromString<PictureInfo> (txt, caseStrategy = CamelCase)
      match decoded with
      | Ok catURL ->  // everything went well! great!
        let actualDogURL = catURL.Url
        printfn "Woof! Woof! %s" actualDogURL
        showPic actualDogURL
      | Error decodingError -> // oh the decoder encountered an error. The string that was sent back from our fetch request does not map well to our PictureInfo type.
        failwith (sprintf "was unable to decode: %s. Reason: %s" txt decodingError)
      )

getRandomDogImage "https://random.dog/woof.json" |> ignore
printfn "done!"
