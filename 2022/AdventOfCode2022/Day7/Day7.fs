namespace AdventOfCode2022

module Day7 =

    open System
    open System.Collections.Generic
    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type NodeType =
        | File
        | Directory
        
    type Node = {
        Name : string
        Type : NodeType
        FileSize : int64 option
        Children : Node list
    }

    type FileSystem = {
        Root : Node option
    }

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


        let processLines (inputLines : string list) =

            let rec loop accum lines =
                match lines with
                | [] -> 
                    // There are no more lines to process.
                    accum
                | head::tails ->
                    // head contains the current command or input line that we need to process and possibly
                    //   add to the file system tree.
                    // cd name: 
                    //   * get a node with the given name. This is the new current node.
                    //   -or- if none exists
                    //   * create a new node and add it to the current file system node
                    
                    // cd ..: 
                    // ls: noop - no changes to any node
                    // {number} name -> create a new file node and add it to the current file system node
                    // dir name -> create a new directory node and add it to the current file system node
                    loop accum tails
            ()
        
        ()

        
        //
        // Part 1
        //

        //_logger.Information("[Part 1] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)


        //
        // Part 2
        //

        //_logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)
