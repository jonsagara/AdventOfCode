namespace AdventOfCode2022

module Day9 =

    open System
    open System.Collections.Generic
    open System.IO
    open System.Linq
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type private Direction =
        | Up
        | Down
        | Left
        | Right
        
    let private toDirection dir =
        match dir with
        | "U" -> Up
        | "D" -> Down
        | "L" -> Left
        | "R" -> Right
        | invalid -> invalidArg (nameof dir) $"Invalid direction: {dir}. Must be one: of U, D, L, or R."        
        
    type private Command = {
        Direction : Direction
        Count : int
    }
    
    let private toCommand (line : string) =
        let lineParts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
        if lineParts.Length <> 2 then
            invalidArg (nameof line) $"Invalid line: {line}"
            
        { Direction = toDirection lineParts[0]; Count = Int32.Parse(lineParts[1]) }
        
    type private Location = {
        X : int
        Y : int
    }
    
    type private Visit = {
        HeadCount : int
        TailCount : int
    }
    
    let private initialVisit () =
        { HeadCount = 0; TailCount = 0 }

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day9_input.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
        
        
        let commands =
            inputFileLines
            |> Array.map toCommand
        
        let rootHeadLocation = { X = 0; Y = 0 }
        let rootTailLocation = { X = 0; Y = 0 }
        let rootLocationVisitsDict = dict [
            (rootHeadLocation, initialVisit())
            (rootTailLocation, initialVisit())
        ]
        
        let (finalHeadLocation, finalTailLocation, visits) =
            commands
            |> Array.fold (fun (headLocation, tailLocation, locationVisits) cmd ->
                match cmd.Direction with
                | Up -> ()
                | Down -> ()
                | Left -> ()
                | Right ->
                    // Move Count steps to the Right, incrementing the Head and Tail visits
                    //   as appropriate.
                    ()
                
                (headLocation, tailLocation, locationVisits)
                ) ((rootHeadLocation, rootTailLocation, Dictionary<Location, Visit>(rootLocationVisitsDict)))
        
        
        
        ()