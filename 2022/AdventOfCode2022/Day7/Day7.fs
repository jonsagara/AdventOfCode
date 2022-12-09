namespace AdventOfCode2022

module Day7 =

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

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day7_input_sample.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())
            |> Array.toList

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)


        //let processLines inputLines =
            
        //    if "cmd" = "cd" then
        //        // Check the current directory's children for the specified directory.
        //        //   If found, noop. Otherwise, create a new directory and add it to the children.
        //        // Change to the specified directory. This really only affects the children
        //        ()
        //    elif "cmd" = "dir" then
        //        // the next line(s) will list the contents of the current directory
        //        ()        

        let cwd = []
        
        ()
        
        //
        // Part 1
        //

        //_logger.Information("[Part 1] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)


        //
        // Part 2
        //

        //_logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)
