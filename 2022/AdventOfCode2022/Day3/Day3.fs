namespace AdventOfCode2022

module Day3 =

    open System
    open System.IO
    open System.Linq
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    /// All items of a given type are to go into exactly one of the two compartments per rucksack.
    /// Each rucksack always has the same number of items in each of its two compartments, so the first 
    /// half of the characters represent items in the first compartment, while the second half of the 
    /// characters represent items in the second compartment.
    type private Rucksack = {
        Line : int
        Compartment1 : string
        Compartment2 : string
    }

    type private RucksackIntersection = {
        Line : int
        Intersection : string
        }

    let private mapToPriority letter =
        match letter with
        | l when l >= 'a' && l <= 'z' -> 
            let priority = (int(l) - 97) + 1
            //printfn $"Lowercase: {l} has priority {priority}"
            priority
        | l when l >= 'A' && l <= 'Z' ->
            let priority = (int(l) - 65) + 1 + 26
            //printfn $"Uppercase: {l} has priority {priority}"
            priority
        | _ -> invalidArg (nameof letter) $"Unsupported letter {letter}"


    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day3_input.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)


        //
        // Do all lines have an even number of characters?
        //
        let linesWithOddNumberOfChars =
            inputFileLines
            |> Array.mapi (fun ix line -> (ix + 1, line))
            |> Array.filter (fun (ixLine, line) -> line.Length % 2 <> 0)

        linesWithOddNumberOfChars
        |> Array.iter (fun (ixLine, line) -> _logger.Warning("Line {lineNumber} has {charCount} characters: {line}", ixLine, line.Length, line))

        if linesWithOddNumberOfChars.Length > 0 then
            invalidOp $"{linesWithOddNumberOfChars.Length} lines had an odd number of characters. Unable to proceed."


        //
        // Each line represents a single rucksack, where the first half of characters are the item types
        //   in the first compartment, and the second half of characters are the items that go in the
        //   second compartment.
        //

        // Convert lines to rucksacks.
        let rucksacks = 
            inputFileLines
            |> Array.mapi (fun ix line ->
                let halfLineLength = line.Length / 2

                { Line = ix + 1
                  Compartment1 = line.Substring(0, halfLineLength)
                  Compartment2 = line.Substring(halfLineLength, halfLineLength) })

        // Find the item types that are in both compartments. The spec says there is exactly one per rucksack,
        //   but we'll make it generic.
        let rucksackIntersections =
            rucksacks
            |> Array.map (fun ruck ->
                { Line = ruck.Line
                  Intersection = String(ruck.Compartment1.Intersect(ruck.Compartment2).ToArray())}
                )

        //// DEBUG: print out the intersections.
        //rucksackIntersections
        //|> Array.iter (fun ri -> _logger.Information("[Line {line}] Intersection character {intersection}", ri.Line, ri.Intersection))


        //
        // *** What is the sum of the priorities of those item types? ***
        //
        
        let prioritySum =
            rucksackIntersections
            |> Array.map (fun ri -> 
                // We made this generic so that we can support more than one intersection. Sum each
                //   intersection and return the total value.
                ri.Intersection.ToCharArray()
                |> Array.map (fun ic -> mapToPriority ic)
                |> Array.sum
                )
            |> Array.sum

        _logger.Information("[Part 1] The sum of priorities of item types is: {prioritySum}", prioritySum)


        //
        // Part 2
        //

        // Chunk the rucksacks into groups of three. Verify that all chunks are of size 3, and fail if they're not.
        let rucksackChunks =
            inputFileLines
            |> Array.chunkBySize 3

        let nonSize3Chunks =
            rucksackChunks
            |> Array.filter (fun rc -> rc.Length <> 3)

        if nonSize3Chunks.Length > 0 then
            invalidOp $"Unable to chunk all rucksacks into equal groups of 3 rucksacks"


        let badgePrioritySum =
            rucksackChunks
            |> Array.map (fun rc -> 
                // The spec says that there is exactly one common badge character per group of 3.
                //   Blow things up if that's not true.
                rc[0].Intersect(rc[1]).Intersect(rc[2]).Single())
            |> Array.map (fun badgeChar -> mapToPriority badgeChar)
            |> Array.sum

        _logger.Information("[Part 2] The sum of badge priorities is: {badgePrioritySum}", badgePrioritySum)


        ()

