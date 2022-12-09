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

        //let fileSystemTree =
        //    inputFileLines
        //    |> Array.fold (fun accum line ->
        //        match line[0] with
        //        | '$' -> printfn $"This is a command: {line}"
        //        | _ -> printfn $"This is either a directory or a file: {line}"
        //        accum) (List<FileSystemTreeNode>())

        let mapFileSystem (fileSystem : FileSystem) (inputFileLines : string list) =

            let rec loop (node : Node option) (lines : string list) =
                match lines with
                | [] -> 
                    // There are no more input commands to parse. Return the accumulator as-is.
                    node
                | head::tail -> 
                    match head[0] with
                    | '$' -> 
                        // This is a command typed in by teh user.
                        // The first element in the split string is the prompt '$', and can be ignore.
                        // The second element is the command, either cd or ls.
                        //   * cd has one argument (the directory to change to), and thus there will be three total parts.
                        //   * ls has none, and thus there will be two parts.
                        let commandParts = head.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
                        
                        match commandParts[1] with
                        | "cd" -> 
                            // We're changing to a new directory. We may need to populate the node.
                            printfn $"cd {commandParts[2]}"
                            match node with
                            | Some n ->
                                // This node is already populated. Return it unchanged.
                                loop node tail
                                //loop (Some({ n with Name = commandParts[2]; Type = NodeType.Directory; FileSize = None; Children = [] })) tail
                                ()
                            | None ->
                                ()

                        | "ls" -> 
                            // We're already in a directory. List its contents.
                            printfn $"ls"
                            loop node tail
                        | _ -> invalidOp $"Invalid command {commandParts[1]}. line: {head}"

                    | _ ->
                        // This is output from the ls command.
                        //   * If it starts with dir, it is the name of a directory.
                        //   * If it starts with an integer, it is the size of the file and the file name.
                        // Either way, there are two parts when you split the string.
                        let outputParts = head.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
                        match outputParts[0] with
                        | "dir" -> printfn $"This is a dir command: {outputParts[0]} {outputParts[1]}"
                        | _ -> printfn $"This is a file: {outputParts[0]} {outputParts[1]}"


                        loop node tail

            loop (fileSystem.Root) inputFileLines

        let filledFileSystem = mapFileSystem ({ Root = None }) inputFileLines
        ()

        
        //
        // Part 1
        //

        //_logger.Information("[Part 1] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)


        //
        // Part 2
        //

        //_logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)
