// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open WilburEllis.RisingTide.Test.DevData.ScanTables

[<EntryPoint>]
let main argv = 
    let tables = 
      List.map (fun tblname -> (tblname, GetTableCount tblname)) (List.sort (GetTableNames()))
      |> List.map (fun (name, count) -> Console.WriteLine(sprintf "%s has %s rows" name count); (name, count))

    let rows = List.iter (fun (name, _ ) -> WriteTableToFile @"c:\agverdictdata\" name) tables

    let _ = ShowDataValues "applicationtiming"
    0 // return an integer exit code
