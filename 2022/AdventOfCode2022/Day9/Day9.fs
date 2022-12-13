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
            |> Array.fold (fun (headLocation, tailLocation, locationVisits : Dictionary<Location, Visit>) cmd ->
                match cmd.Direction with
                | Up -> ()
                | Down -> ()
                | Left -> ()
                | Right ->
                    // Move Count steps to the Right, incrementing the Head and Tail visits
                    //   as appropriate.
                    [ 1 .. cmd.Count ]
                    |> List.iter (fun _ ->
                        // Move one location to the right.
                        let newHeadLocation = { headLocation with X = headLocation.X + 1 }
                        
                        // Update the tail location to keep up with the head.
                        let newTailLocation =
                            if newHeadLocation.X = tailLocation.X && newHeadLocation.Y = tailLocation.Y then
                                // Head moved to where Tail currently is. Leave Tail's location unchanged.
                                { tailLocation with X = tailLocation.X; Y = tailLocation.Y }
                            elif newHeadLocation.X = tailLocation.X then
                                // Head moved to the same X as Tail. They have different Ys. Move Tails's Y
                                //   one location closer to Head.
                                let increment = if newHeadLocation.Y > tailLocation.Y then 1 else -1 
                                { tailLocation with Y = tailLocation.Y + increment }
                            elif newHeadLocation.Y = tailLocation.Y then
                                // Head moved to the same Y as Tail. They have different Xs. Move Tail's X
                                //   one location closer to Head.
                                let increment = if newHeadLocation.X > tailLocation.X then 1 else -1
                                { tailLocation with X = tailLocation.X + increment }
                            else
                                // Head moved to a different X and Y than Tail. If they're not diagonally adjacent, move
                                //   Tail diagonally to catch up to Head.
                                // They're diagonally adjacent if they're within 1 in the X and Y dimension.
                                let xDiff = Math.Abs(newHeadLocation.X - tailLocation.X)
                                let yDiff = Math.Abs(newHeadLocation.Y - tailLocation.Y)
                                
                                if xDiff = 1 && yDiff = 1 then
                                    // Tail is diagonally adjacent to Head. Tail can remain where it is.
                                    { tailLocation with X = tailLocation.X; Y = tailLocation.Y }
                                // elif xDiff = 1 then
                                else
                                    // Tail is only 1 away from Head in X, but > 1 away in Y. Move Tail diagonally to be
                                    //   just above or below Head.
                                    let xIncrement = if newHeadLocation.X > tailLocation.X then 1 else -1
                                    let yIncrement = if newHeadLocation.Y > tailLocation.Y then 1 else -1
                                    { tailLocation with X = tailLocation.X + xIncrement; Y = tailLocation.Y + yIncrement }
                                // elif yDiff = 1 then
                                //     // Tail is only 1 away from Head in Y, but > 1 away in X. Move Tail diagonally to be
                                //     //   just to the left or right of Head.
                                //     let xIncrement = if newHeadLocation.X > tailLocation.X then 1 else -1
                                //     let yIncrement = if newHeadLocation.Y > tailLocation.Y then 1 else -1
                                //     { tailLocation with X = tailLocation.X + xIncrement; Y = tailLocation.Y + yIncrement }
                                // else
                                //     ()
                        
                        match locationVisits.TryGetValue(newHeadLocation) with
                        | true, visits ->
                            locationVisits[newHeadLocation] <- { visits with HeadCount = visits.HeadCount + 1 }
                        | false, _ -> ()
                        
                        match locationVisits.TryGetValue(newTailLocation) with
                        | true, visits ->
                            locationVisits[newTailLocation] <- { visits with TailCount = visits.TailCount + 1 }
                        | false, _ -> ()
                        
                        (newHeadLocation, newTailLocation, locationVisits))
                    ()
                
                (headLocation, tailLocation, locationVisits)
                ) ((rootHeadLocation, rootTailLocation, Dictionary<Location, Visit>(rootLocationVisitsDict)))
        
        
        
        ()