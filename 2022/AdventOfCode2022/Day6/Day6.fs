namespace AdventOfCode2022

module Day6 =

    open System
    open System.Collections.Generic
    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)
    

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day6_input_sample.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            // |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
        


        
        //
        // Part 1
        //

        //_logger.Information("[Part 1] Top crates: {topCrates}", topCrates)


        //
        // Part 2
        //

        // _logger.Information("[Part 2] There are {anyOverlappingAssignments} pairs with at least one overlapping assignment.", anyOverlappingAssignments)

        ()