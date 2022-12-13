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

    /// <summary>
    /// Get the new value for Tail's X or Y coordinate. If passing X for Head, then also pass X for
    /// tail; same for Y. If Head's dimension is greater than Tail's, then add 1; otherwise,
    /// subtract 1.
    /// NOTE: This should never be called when the head and tail coordinate dimensions are equal.
    /// </summary>
    let private getTailCoordinateIncrement headLocationDimension tailLocationDimension =
        if headLocationDimension = tailLocationDimension then
            invalidOp $"{nameof headLocationDimension} ({headLocationDimension}) must not equal {nameof tailLocationDimension} ({tailLocationDimension})."

        if headLocationDimension > tailLocationDimension then 1 else -1 

    /// Increment the Head visit count at the give locations coordinates.
    let private incrementHeadVisitCount (locationVisits : Dictionary<Location, Visit>) newHeadLocation =
        match locationVisits.TryGetValue(newHeadLocation) with
        | true, visits ->
            // Location already exists. Increment the existing Head visit count.
            locationVisits[newHeadLocation] <- { visits with HeadCount = visits.HeadCount + 1 }
        | false, _ -> 
            // We're not yet tracking this location. Create a new visit count record.
            locationVisits.Add(newHeadLocation, { HeadCount = 1; TailCount = 0 })

    /// Increment the Tail visit count at the given location coordinates.
    let private incrementTailVisitCount (locationVisits : Dictionary<Location, Visit>) newTailLocation =
        match locationVisits.TryGetValue(newTailLocation) with
        | true, visits ->
            // Location already exists. Increment the existing Tail visit count.
            locationVisits[newTailLocation] <- { visits with TailCount = visits.TailCount + 1 }
        | false, _ -> 
            // We're not yet tracking this location. Create a new visit count record.
            locationVisits.Add(newTailLocation, { HeadCount = 0; TailCount = 1 })

    let private getNewTailLocation newHeadLocation tailLocation =
        if newHeadLocation.X = tailLocation.X && newHeadLocation.Y = tailLocation.Y then
            // Head how shares the same location as Tail. Leave Tail's location unchanged.
            tailLocation
        elif newHeadLocation.X = tailLocation.X then
            // Head and Tail have the same horizontal position. If the Y distance is >= 2, then
            //   move Tail one position closer to Head. Otherwise, leave Tail unchanged.
            let yDiff = Math.Abs(newHeadLocation.Y - tailLocation.Y)
            if yDiff > 1 then
                let yIncrement = getTailCoordinateIncrement newHeadLocation.Y tailLocation.Y
                { tailLocation with Y = tailLocation.Y + yIncrement }
            else
                tailLocation
        elif newHeadLocation.Y = tailLocation.Y then
            // Head and Tail have the same vertical position. If the X distance is >= 2, then
            //   move Tail one position closer to Head. Otherwise, leave Tail unchanged.
            let xDiff = Math.Abs(newHeadLocation.X - tailLocation.X)
            if xDiff > 1 then
                let xIncrement = getTailCoordinateIncrement newHeadLocation.X tailLocation.X
                { tailLocation with X = tailLocation.X + xIncrement }
            else
                tailLocation
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
                // Tail is >= 1 away from Head in X or >= 1 away in Y. Regardless of the actual
                //   distance, we're only going to move Tail diagonally one space to be just above or
                //   below Head.
                let xIncrement = getTailCoordinateIncrement newHeadLocation.X tailLocation.X
                let yIncrement = getTailCoordinateIncrement newHeadLocation.Y tailLocation.Y
                { tailLocation with X = tailLocation.X + xIncrement; Y = tailLocation.Y + yIncrement }

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day9_input_sample.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
        
        
        let commands =
            inputFileLines
            |> Array.map toCommand
        
        // We assume that Head and Tail both start in the same location. We also count this as a
        //   visit for both.
        let initialLocation = { X = 0; Y = 0 }
        let initialLocationVisitsDict = dict [
            (initialLocation, { HeadCount = 1; TailCount = 1 })
        ]
        
        let (finalHeadLocation, finalTailLocation, visits) =
            commands
            |> Array.fold (fun (headLocation, tailLocation, locationVisits : Dictionary<Location, Visit>) cmd ->
                match cmd.Direction with
                | Up -> 
                    // Move Count steps Up, incrementing the Head and Tail visits as appropriate.

                    [ 1 .. cmd.Count ]
                    |> List.fold (fun (headLoc, tailLoc, locVisits) _ -> 
                            // Move Head one location Up and increment its visit count.
                        let newHeadLoc = { headLoc with Y = headLoc.Y + 1 }
                        incrementHeadVisitCount locVisits newHeadLoc

                        // Update the Tail location to keep up with the Head.
                        let newTailLoc = getNewTailLocation newHeadLoc tailLoc

                        // Only increment Tail's visit count if its location changed.
                        if newTailLoc <> tailLoc then do
                            incrementTailVisitCount locVisits newTailLoc

                        (newHeadLoc, newTailLoc, locVisits)) (headLocation, tailLocation, locationVisits)

                | Down -> 
                    // Move Count steps Down, incrementing the Head and Tail visits as appropriate.

                    [ 1 .. cmd.Count ]
                    |> List.fold (fun (headLoc, tailLoc, locVisits) _ ->
                        // Move Head one location Down and increment its visit count.
                        let newHeadLoc = { headLoc with Y = headLoc.Y - 1 }
                        incrementHeadVisitCount locVisits newHeadLoc

                        // Update the Tail location to keep up with the Head.
                        let newTailLoc = getNewTailLocation newHeadLoc tailLoc

                        // Only increment Tail's visit count if its location changed.
                        if newTailLoc <> tailLoc then do
                            incrementTailVisitCount locVisits newTailLoc

                        (newHeadLoc, newTailLoc, locVisits)) (headLocation, tailLocation, locationVisits)

                | Left ->
                    // Move Count steps to the Left, incrementing the Head and Tail visits as appropriate.

                    [ 1 .. cmd.Count ]
                    |> List.fold (fun (headLoc, tailLoc, locVisits) _ ->
                        // Move Head one location to the left and increment its visit count.
                        let newHeadLoc = { headLoc with X = headLoc.X - 1 }
                        incrementHeadVisitCount locVisits newHeadLoc

                        // Update the Tail location to keep up with the Head.
                        let newTailLoc = getNewTailLocation newHeadLoc tailLoc

                        // Only increment Tail's visit count if its location changed.
                        if newTailLoc <> tailLoc then do
                            incrementTailVisitCount locVisits newTailLoc

                        (newHeadLoc, newTailLoc, locVisits)) (headLocation, tailLocation, locationVisits)

                | Right ->
                    // Move Count steps to the Right, incrementing the Head and Tail visits as appropriate.

                    [ 1 .. cmd.Count ]
                    |> List.fold (fun (headLoc, tailLoc, locVisits) _ -> 
                        // Move Head one location to the right and increment its visit count.
                        let newHeadLoc = { headLoc with X = headLoc.X + 1 }
                        incrementHeadVisitCount locVisits newHeadLoc

                        // Update the Tail location to keep up with the Head.
                        let newTailLoc = getNewTailLocation newHeadLoc tailLoc
                        
                        // Only increment Tail's visit count if its location changed.
                        if newTailLoc <> tailLoc then do
                            incrementTailVisitCount locVisits newTailLoc

                        (newHeadLoc, newTailLoc, locVisits)) (headLocation, tailLocation, locationVisits)
                
                ) ((initialLocation, initialLocation, Dictionary<Location, Visit>(initialLocationVisitsDict)))
        
        
        //
        // Part 1: How many positions does the tail of the rope visit at least once?
        //

        let tailLocationVisitCount =
            visits
            |> Seq.filter (fun kvp -> kvp.Value.TailCount > 0)
            |> Seq.length

        _logger.Information("[Part 1] The tail of the rope visited {tailLocationVisitCount:N0} positions at least once.", tailLocationVisitCount)
        
        ()