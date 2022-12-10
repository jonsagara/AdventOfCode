namespace AdventOfCode2022

module Day7 =

    open System
    open System.Collections.Generic
    open System.IO
    open System.Text.RegularExpressions
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)


    //
    // All credit to: https://github.com/paulp74/AdventOfCode2022/blob/main/Day07/Program.fs
    //
    // See Day7_CSharp.txt for my imperative C# solution.
    //

    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then
            // Return all matched groups *except* the first one, which is the total matched expression.
            //   We only care about the individual groups.
            Some(List.tail [for g in m.Groups -> g.Value])
        else
            None

    type private Node =
        | File of name : string * size : int64
        | Directory of name : string * contents : Node list

    let rec private processDirectory (stack : Stack<string>) name =
        let mutable line = ""
        let dirContents = [
            while stack.Count > 0 do
                line <- stack.Pop()
                match line with
                | Regex @"\$ ls" _
                | Regex @"\$ cd \.\." _
                | Regex @"dir (.*)" _ -> 
                    // These are effectively no-ops. We don't need to process "dir" lines because
                    //   we already have the directory name as the argument to this function.
                    ()
                | Regex @"\$ cd (.*)" [ dirName ] ->
                    // The group will return us the name of the target directory that we're changing into.
                    //   We need to recursively get this directory's contents.
                    yield processDirectory stack dirName
                | Regex @"(\d*) (.*)" [ size; name] ->
                    // This is a file size and name.
                    yield File(name, int64 size)
                | _ -> invalidOp $"Unknown line type: {line}"
            ]

        Directory(name, dirContents)

    let private processStack (stack : Stack<string>) =
        // Ignore the very first "cd /" line, and start processing with the initial directory assumed to
        //   be "/".
        stack.Pop() |> ignore
        processDirectory stack "/"


    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day7_input_sample.txt")

        // Reverse the lines so that when we put them all onto the stack, they're popped off in their
        //   original order from the input file.
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())
            |> Array.rev

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)


        //
        // Build a model of the file system.
        //

        let stack = Stack(inputFileLines)
        let fileSystemRoot = processStack stack

        
        //
        // Part 1: Find all of the directories with a total size of at most 100000. 
        //   What is the sum of the total sizes of those directories?
        //

        let maxDirectorySize = 100_000L

        let rec sumContainedFiles root =
            match root with
            | Directory(_, contents) ->
                contents
                |> List.sumBy (fun c ->
                    match c with
                    | File(_, size) -> size
                    | Directory(_) as d -> sumContainedFiles d)
            | File(_) -> 
                0L

        let rec sumBigDirectories root =
            match root with
            | Directory(_, contents) as d ->
                // Sum the files in this directory, as well as any files in its subdirectories.
                let directoryTotalSum = sumContainedFiles d

                // Only keep the computed sum if the size of the directory's files, and the sizes of all 
                //   files in its subdirectories, are <= the maximum size. Otherwise, return 0.
                let actualSum =
                    if directoryTotalSum <= maxDirectorySize then 
                        directoryTotalSum 
                    else
                        0L
                        
                // Add
                actualSum + (contents |> List.sumBy sumBigDirectories)
            | File(_) ->
                // We're not summing file sizes in this branch. We do it recursively above.
                0L

        let bigDirsSum = sumBigDirectories fileSystemRoot

        _logger.Information("[Part 1] Sum of file sizes of all directories sized <= {maxDirectorySize:N0}: {bigDirsSum:N0}", maxDirectorySize, bigDirsSum)


        //
        // Part 2
        //

        //_logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)

        ()
