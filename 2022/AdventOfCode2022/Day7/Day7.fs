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
    // HEAVILY inspired by: https://github.com/paulp74/AdventOfCode2022/blob/main/Day07/Program.fs
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

    /// A directory name and its total used space, which includes its files and all files in any subdirectories.
    type private DirectorySize = {
            Name : string
            TotalSpaceUsed : int64
        }

    let rec private processDirectory (stack : Stack<string>) name =
        let mutable line = ""

        // Recursively populate this list with the directory's files, and also the contents of any subdirectories.
        let dirContents = [
            // Exit the loop if the stack runs out OR we change back to the parent directory.
            while stack.Count > 0 && line <> "$ cd .." do
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
        // Ignore the very first "cd /" line, and start processing with the initial directory assumed to be "/".
        stack.Pop() |> ignore
        processDirectory stack "/"


    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day7_input.txt")

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
                // Files have no subdirectories that we can recurse into. Count file sizes when processing
                //   their containing directories.
                0L

        let bigDirsSum = sumBigDirectories fileSystemRoot

        _logger.Information("[Part 1] Sum of file sizes of all directories sized <= {maxDirectorySize:N0}: {bigDirsSum:N0}", maxDirectorySize, bigDirsSum)


        //
        // Part 2: Find the smallest directory that, if deleted, would free up enough space on the 
        //   filesystem to run the update. What is the total size of that directory?
        //

        let totalDiskSpace = 70_000_000L
        let requiredUnusedSpace = 30_000_000L
        
        // This only gets the total disk space. While cool, it's ultimately not helpful in solving the problem.
        //   We need a collection of directories and their sizes.
        //let sumAllUsedSpace root =
        //    let rec loop root =
        //        match root with
        //        // A file doesn't have child contents, so we can't recurse with it. Sum the file sizes
        //        //   while handling directories.
        //        | File (_) -> 0L
        //        | Directory (_, contents) as d ->
        //            // Sum the file sizes in the current directory.
        //            let currentDirFileSizes = 
        //                contents
        //                |> List.sumBy (fun n ->
        //                    match n with
        //                    | File (_, size) -> size
        //                    | Directory (_) -> 0L)

        //            currentDirFileSizes + (contents |> List.sumBy loop)

        //    loop root

        //let currentUsedSpace = sumAllUsedSpace fileSystemRoot
        //let unusedSpace = totalDiskSpace - currentUsedSpace
        //let spaceToFree = requiredUnusedSpace - unusedSpace
        //_logger.Information("Current used space: {currentUsedSpace:N0}; Unused space: {unusedSpace:N0}", currentUsedSpace, unusedSpace)
        //_logger.Information("Space to free: {spaceToFree:N0}", spaceToFree)

        
        /// Returns a list of directory name/space total space used pairs.
        let rec getDirectoriesSpaceUsed root =
            match root with
            | File(_) -> 
                // We can't recurse into a file, so don't count their space here. Count them while descending
                //   into directories.
                List.empty
            | Directory(name, contents) as d ->
                let currentDirSpaceUsed = d |> sumContainedFiles

                { Name = name; TotalSpaceUsed = currentDirSpaceUsed } :: (contents |> List.collect getDirectoriesSpaceUsed)

        let directorySizes = getDirectoriesSpaceUsed fileSystemRoot
        //directorySizes
        //|> List.iter (fun ds -> _logger.Information("[Part 2] {name}: {size:N0}", ds.Name, ds.TotalSpaceUsed))

        // The directory with the largest size is the root directory. That's the total space currently in use.
        let currentUsedSpace = 
            directorySizes 
            |> List.maxBy (fun ds -> ds.TotalSpaceUsed) 
            |> fun ds -> ds.TotalSpaceUsed
        let unusedSpace = totalDiskSpace - currentUsedSpace
        let spaceToFree = requiredUnusedSpace - unusedSpace
        _logger.Information("[Part 2] Current used space: {currentUsedSpace:N0}; Unused space: {unusedSpace:N0}", currentUsedSpace, unusedSpace)
        _logger.Information("[Part 2] Space to free: {spaceToFree:N0}", spaceToFree)

        // The directory to delete is one that is >= the amount of space to free. If there are multiple,
        //   choose the smallest one.
        let directoryToDelete =
            directorySizes
            |> List.filter (fun ds -> ds.TotalSpaceUsed >= spaceToFree)
            |> List.minBy (fun ds -> ds.TotalSpaceUsed)

        _logger.Information("[Part 2] Delete directory {name} to free up {totalSpaceUsed:N0}", directoryToDelete.Name, directoryToDelete.TotalSpaceUsed)
        ()
