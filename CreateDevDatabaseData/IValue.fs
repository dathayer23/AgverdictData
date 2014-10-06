namespace WilburEllis.RisingTide.Test.DevData
open System


module IValue = 
   type IValue = 
      Nothing
    | Null
    | Boolean of System.Boolean  
    | Byte of System.Byte 
    | Bytes of System.Byte []
    | SByte of System.SByte 
    | UInt16 of System.UInt16 
    | Int16 of System.Int16 
    | UInt32 of System.UInt32 
    | Int32 of System.Int32 
    | UInt64 of System.UInt64 
    | Int64 of System.Int64
    | Single of System.Single 
    | Double of System.Double 
    | Decimal of System.Decimal 
    | String of System.String 
    | Guid of System.Guid
    | Geometry of string
    | Object of obj
    | Error of string * string

   with
      override x.ToString() = 
         match x with 
         | Nothing    -> "Nothing"
         | Null       -> ""
         | Boolean b  -> if b then "True" else "False"
         | Byte byt   -> sprintf "%d" byt
         | Bytes byts -> sprintf "%A" byts
         | SByte sbyt -> sprintf "%d" sbyt
         | UInt16 ui  -> sprintf "%u" ui
         | Int16 i    -> sprintf "%d" i
         | UInt32 ui  -> sprintf "%u" ui
         | Int32 i    -> sprintf "%d" i
         | UInt64 ui  -> sprintf "%u" ui
         | Int64 i    -> sprintf "%d" i
         | Single f   -> sprintf "%f" f
         | Double d   -> sprintf "%f" d
         | Decimal d  -> sprintf "%M" d
         | String str -> str
         | Guid g     -> g.ToString()
         | Geometry s -> s
         | Object obj -> obj.ToString()
         | Error (str, typ)  -> sprintf "Error( %s :: %s )" str typ

