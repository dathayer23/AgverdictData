// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open System.Linq
open System.IO

let Int64FromString (str:string) = match Int64.TryParse(str) with | (false, _) -> -1L | (true,n) -> n
let CreateStats (fields:string[]) =
   if fields.Count() = 23
   then
      let stats = [fields.[4]; fields.[9]; fields.[13]; fields.[18]; fields.[22] ]
      Some( stats |> List.map Int64FromString )
   else
      None  

let ReadLog (fileName:string) = 
   use sr = new StreamReader(fileName) 
   let lines = sr.ReadToEnd().Split([|'\n'|])    
   let memorystats = List.map (fun (l:string) -> l.StartsWith("NonPaged Memory") ) lines
   let stats = List.choose (fun (l:string) -> l.Split([|' '|], StringSplitOptions.RemoveEmptyEntries) |> CreateStats) memorystats
   stats

let WriteAnalysis (fileName:string) (stats:int64 list list) =
   use sw = new StreamWriter(fileName)
   List.iter (fun (ss: int64 list) -> List.iter (fun i -> sw.Write (sprintf "%d" i)) ss; sw.WriteLine()) stats


[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    0 // return an integer exit code
