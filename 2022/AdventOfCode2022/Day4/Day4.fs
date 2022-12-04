namespace AdventOfCode2022

module Day4 =

    open System
    open System.IO
    open System.Linq
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type private CleaningAssignments = {
        Line : int
        Elf1 : int array
        Elf2 : int array
        }

    /// range is a string that looks like "2-4". Split that string and expand into an
    /// array containing the entire range.
    let private expandRange (range : string) =
        let rangeParts = range.Split("-", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
        [| Int32.Parse(rangeParts[0]) .. Int32.Parse(rangeParts[1]) |]

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day4_input.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)


        //
        // Part 1
        //

        // Map the lines to CleaningAssignments, expanding them into actual numbers.
        let cleaningAssignments =
            inputFileLines
            |> Array.mapi (fun ix line ->
                // Get the two ranges of section ids.
                let ranges = line.Split(",", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                // Expand each range of section ids into actual ids.
                { Line = ix + 1
                  Elf1 = expandRange ranges[0]
                  Elf2 = expandRange ranges[1] })

        // In how many assignment pairs does one range fully contain the other?
        let overlappingAssignments =
            cleaningAssignments
            |> Array.filter (fun ca ->
                // There's overlap if Elf1's assignments completely contain Elf2's, or vice versa.
                //   If the difference between assignments is an empty set, then there is a complete
                //   overlap. Check in both directions.
                (ca.Elf1 |> Array.except ca.Elf2 |> Array.length = 0) || (ca.Elf2 |> Array.except ca.Elf1 |> Array.length = 0)
                )
            |> Array.length

        _logger.Information("[Part 1] There are {overlappingAssignments} pairs with completely overlapping assignments.", overlappingAssignments)


        //
        // Part 2
        //

        // In how many assignment pairs do the ranges overlap?
        let anyOverlappingAssignments =
            cleaningAssignments
            |> Array.filter (fun ca ->
                // There's overlap if one or more of Elf1's assignments completely contain Elf2's, or 
                //   vice versa. If the intersection of assignments is a non-empty set, then there is 
                //   overlap. We only have to check one direction.
                let elf1Set = ca.Elf1 |> Set.ofArray
                let elf2Set = ca.Elf2 |> Set.ofArray

                (elf1Set |> Set.intersect elf2Set |> Set.count > 0)
                )
            |> Array.length

        _logger.Information("[Part 2] There are {anyOverlappingAssignments} pairs with at least one overlapping assignment.", anyOverlappingAssignments)

        ()