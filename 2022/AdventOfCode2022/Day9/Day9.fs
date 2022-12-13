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

    /// <summary>
    /// Get the new value for Tail's X or Y coordinate. If passing X for Head, then also pass X for
    /// tail; same for Y. If Head's dimension is greater than Tail's, then add 1; otherwise,
    /// subtract 1.
    /// NOTE: This should never be called when the head and tail coordinate dimensions are equal.
    /// </summary>
    let getTailCoordinateIncrement headLocationDimension tailLocationDimension =
        if headLocationDimension = tailLocationDimension then
            invalidOp $"{nameof headLocationDimension} ({headLocationDimension}) must not equal {nameof tailLocationDimension} ({tailLocationDimension})."

        if headLocationDimension > tailLocationDimension then 1 else -1 

    /// Increment the Head visit count at the give locations coordinates.
    let incrementHeadVisitCount (locationVisits : Dictionary<Location, Visit>) newHeadLocation =
        match locationVisits.TryGetValue(newHeadLocation) with
        | true, visits ->
            locationVisits[newHeadLocation] <- { visits with HeadCount = visits.HeadCount + 1 }
        | false, _ -> ()

    /// Increment the Tail visit count as the given location coordinates.
    let incrementTailVisitCount (locationVisits : Dictionary<Location, Visit>) newTailLocation =
        match locationVisits.TryGetValue(newTailLocation) with
        | true, visits ->
            locationVisits[newTailLocation] <- { visits with TailCount = visits.TailCount + 1 }
        | false, _ -> ()

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
                                let increment = getTailCoordinateIncrement newHeadLocation.Y tailLocation.Y
                                { tailLocation with Y = tailLocation.Y + increment }
                            elif newHeadLocation.Y = tailLocation.Y then
                                // Head moved to the same Y as Tail. They have different Xs. Move Tail's X
                                //   one location closer to Head.
                                let increment = getTailCoordinateIncrement newHeadLocation.X tailLocation.X
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
                                else
                                    // Tail is >= 1 away from Head in X and >= 1 away in Y. Regardless of the actual
                                    //   distance, we're only going to move Tail diagonally one space to be just above or
                                    //   below Head.
                                    let xIncrement = getTailCoordinateIncrement newHeadLocation.X tailLocation.X
                                    let yIncrement = getTailCoordinateIncrement newHeadLocation.Y tailLocation.Y
                                    { tailLocation with X = tailLocation.X + xIncrement; Y = tailLocation.Y + yIncrement }
                        
                        // Update the location visit counts for Head and Tail.
                        incrementHeadVisitCount locationVisits newHeadLocation
                        incrementTailVisitCount locationVisits newTailLocation
                        
                        (newHeadLocation, newTailLocation, locationVisits))
                    ()
                
                (headLocation, tailLocation, locationVisits)
                ) ((rootHeadLocation, rootTailLocation, Dictionary<Location, Visit>(rootLocationVisitsDict)))
        
        
        
        ()