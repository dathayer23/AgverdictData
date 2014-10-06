namespace WilburEllis.RisingTide.Test.DevData

open System
open System.Collections.Generic
open System.Data
open System.IO
open System.Text
open StringExtensions
open IValue
open WilburEllis.RisingTide.Common.DBLayer
open WilburEllis.RisingTide.Common.DBLayer.PostgreSQLWrapper

module ScanTables = 

   let dataConnectionString = "server=127.0.0.1;port=5432;database=RisingTide;user id=postgres;password=Aditi01*;enlist=true;pooling=false;minpoolsize=1;maxpoolsize=100;timeout=50;"
   
   let NewDataConnection() = new PostgresWrapper(dataConnectionString)

   let ExecuteNonQuery (dbLayer:PostgresWrapper) sql = 
       try
         Some(dbLayer.ExecuteNonQuery(sql, new Dictionary<string,obj>()))         
       with 
       | _ as ex -> 
         do Console.WriteLine(sql + ";")
         //do Console.WriteLine(ex.Message)
         None 

   let ExecutePostGresSql (dbLayer:PostgresWrapper) sql = 
       try
         Some(dbLayer.ExecuteReaderSqlStatement(sql, new Dictionary<string,obj>()))         
       with 
       | _ as ex -> 
         do Console.WriteLine("Command: " + sql)
         do Console.WriteLine(ex.Message)
         None 

   /// get all table names in rt schema
   let GetTableNames() = 
      let tableNames : string list ref  = ref []
      use dbLayer = NewDataConnection()
      
      match ExecutePostGresSql dbLayer @"Select Table_Name from information_schema.tables where table_schema iLike 'rt' AND table_type iLike 'BASE TABLE'" with
      | None -> !tableNames
      | Some reader -> 
         while reader.Read() do
            tableNames := ((string)reader.["Table_Name"]) :: !tableNames
         !tableNames

   /// return list of field names tupled with field types
   let GetTableFieldTypes tblName = 
      let cmd = sprintf @"Select * as rows from rt.%s" tblName
      use dbLayer = NewDataConnection()
      match ExecutePostGresSql dbLayer cmd with
      | None -> []
      | Some rdr -> [0 .. rdr.FieldCount - 1] |> List.map (fun i -> (rdr.GetName(i), rdr.GetDataTypeName(i)))

   /// get count if rows in table
   let GetTableCount tblName = 
      let cmd = sprintf @"Select count(*) as rows from rt.%s" tblName
      use dbLayer = NewDataConnection()

      match ExecutePostGresSql dbLayer cmd with
      | None -> "-1"
      | Some reader -> 
         if reader.Read()
         then try reader.["rows"].ToString()
              with | _ as ex -> Console.WriteLine(ex.Message); "-1"
         else "-1"

   /// delete all rows from table
   let DeleteTableRows tblName = 
      let cmd = sprintf @"Delete from rt.%s" tblName
      use dbLayer = NewDataConnection()

      match ExecuteNonQuery dbLayer cmd with
      | None -> 0
      | Some rows -> rows

   /// get IValue for data in field n
   let GetColumnData (rdr:IDataReader) (typ:string) n =
      if (not (rdr.IsClosed))         
      then     
         if (rdr.IsDBNull(n))
         then IValue.Null
         else     
            match typ with 
            | "System.Boolean" -> IValue.Boolean(rdr.GetBoolean(n))
            | "System.Byte"    -> IValue.Byte(rdr.GetByte(n))
            | "System.Int16"   -> IValue.Int16(rdr.GetInt16(n))
            | "System.Int32"   -> IValue.Int32(rdr.GetInt32(n))
            | "System.Int64"   -> IValue.Int64(rdr.GetInt64(n))
            | "System.String"  -> IValue.String(rdr.GetString(n))
            | "System.Single"  -> IValue.Single(rdr.GetFloat(n))
            | "System.Double"  -> IValue.Double(rdr.GetDouble(n))
            | "System.Decimal" -> IValue.Decimal(rdr.GetDecimal(n))
            | "System.Guid"    -> IValue.Guid(rdr.GetGuid(n))
            //| ""
            | _ -> IValue.Object(rdr.GetValue(n))
      else IValue.Nothing
     
   /// return list of table rows as lists if IValue types   
   let GetTableData tblName  = 
      let cmd = sprintf @"Select * from rt.%s" tblName
      use dbLayer = NewDataConnection()

      match ExecutePostGresSql dbLayer cmd with
      | None -> [[IValue.Int32(- 1)]]
      | Some reader -> 
         let vals : list<list<IValue>> ref = ref []
         let ncol = reader.FieldCount
         let cols = List.map (fun i -> reader.GetDataTypeName(i)) [0 .. ncol - 1]
         let names = List.map (fun i -> reader.GetName(i)) [0 .. ncol - 1] |> List.map (fun (s:string) -> IValue.String(s))
         vals := names :: !vals
         while reader.Read() do
            let values = List.mapi (fun n s -> GetColumnData reader s n) cols
            vals := values :: !vals
         List.rev !vals
                        
   /// write one row of table data to a stream
   let WriteDataValues tblName (sw:StreamWriter) : seq< unit> = 
      let vals = GetTableData tblName 
      let size = List.fold (fun s ls -> s + (List.length ls)) 0 vals 
      
      let vs = Seq.map (fun row -> Seq.fold (fun (s:StringBuilder) v -> s.Append(v.ToString() + ", ")) (new StringBuilder(2000)) row) (Seq.ofList vals)       
      vs |> Seq.map (fun (s:StringBuilder) -> 
            let (str:string) = s.ToString().TrimEnd([|' '; ','|])
            sw.WriteLine(str))
   
   /// write rows of table to Console out         
   let ShowDataValues tblName = WriteDataValues tblName (Console.Out :?> StreamWriter)   

   /// write rows of table to file
   let WriteTableToFile dir tblName = 
      do Console.WriteLine(sprintf "Writing data for %s to file" tblName)
      use sw = new StreamWriter(Path.Combine(dir, tblName + ".csv"));
      WriteDataValues tblName sw |> Seq.last

   /// given a string and a typ create a value type
   let GetValueForField name typ (str:System.String) = 
      if String.IsNullOrEmpty(str) then name,IValue.Null
      else
         match typ with 
         | "System.Boolean" -> name, IValue.Boolean(str.GetBoolean())
         | "System.Byte"    -> name, IValue.Byte(str.GetByte())
         | "System.Int16"   -> name, IValue.Int16(str.GetInt16())
         | "System.Int32"   -> name, IValue.Int32(str.GetInt32())
         | "System.Int64"   -> name, IValue.Int64(str.GetInt64())
         | "System.String"  -> name, IValue.String(str)
         | "System.Single"  -> name, IValue.Single(str.GetFloat())
         | "System.Double"  -> name, IValue.Double(str.GetDouble())
         | "System.Decimal" -> name, IValue.Decimal(str.GetDecimal())
         | "System.Guid"    -> name, IValue.Guid(str.GetGuid())
         | _ -> name, IValue.Error (str, typ)

   /// read one line of csv data
   let ReadDataLine (metadata : (string * string) list) (str:string) = 
      let fields = List.ofArray (str.Split([|','|]))
      List.zip metadata fields |> List.map (fun ((nm,typ),v) -> GetValueForField nm typ v)
      
   /// read on line of csv data into a row for this table
   let ReadDataValues tblName (sr:StreamReader) =
      use dbLayer = NewDataConnection()
      let tableMetaData = GetTableFieldTypes tblName
      let lines = List.ofArray (sr.ReadToEnd().Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries))
      lines |> List.map (fun s -> ReadDataLine tableMetaData s)

   
   /// read data for table from csv file                
   let ReadDataTableFromFile dir tblName = 
      do Console.WriteLine(sprintf "reading data from %s to database" tblName)
      use sr = new StreamReader(Path.Combine(dir, tblName + ".csv"))
      ReadDataValues tblName sr |> Seq.last
