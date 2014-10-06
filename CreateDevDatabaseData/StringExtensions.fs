namespace WilburEllis.RisingTide.Test.DevData

open System
module StringExtensions = 
   type String
   with
      member x.GetBoolean() = System.Boolean.TryParse(x.Trim()) |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to a boolean value" x)
      member x.GetByte()    = System.Byte.TryParse(x.Trim())    |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to a byte value" x)
      member x.GetInt16()   = System.Int16.TryParse(x.Trim())   |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to an int16 value" x)
      member x.GetInt32()   = System.Int32.TryParse(x.Trim())   |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to an int32 value" x)
      member x.GetInt64()   = System.Int64.TryParse(x.Trim())   |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to an int64 value" x)
      member x.GetFloat()   = System.Single.TryParse(x.Trim())  |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to a float value" x)
      member x.GetDouble()  = System.Double.TryParse(x.Trim())  |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to a double value" x)
      member x.GetDecimal() = System.Decimal.TryParse(x.Trim()) |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to a decimal value" x)
      member x.GetGuid()    = System.Guid.TryParse(x.Trim())    |> fun (f,v) ->  if f then v else failwith (sprintf "cannot convert %s to a GUID value" x)

