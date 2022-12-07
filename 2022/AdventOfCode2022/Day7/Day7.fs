﻿namespace AdventOfCode2022

module Day7 =

    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day7_input.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
        


        
        //
        // Part 1
        //

        //_logger.Information("[Part 1] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)


        //
        // Part 2
        //

        //_logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)
