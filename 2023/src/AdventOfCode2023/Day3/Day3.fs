namespace AdventOfCode2023

module Day3 =

    // Inspiration: https://github.com/akhansari/advent_of_code/blob/main/2023/Day03.fs

    open System.Collections.Generic
    open System.IO
    open System.Text.RegularExpressions
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)


    let private _rxNumber = Regex("\d+", RegexOptions.Compiled)

    type private CapturedNumber = {
        Value : int
        StartIndex : int
        EndIndex : int
        }

    type private Number = {
        Value : int
        IsPartNumber : bool
        }

    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day3_input.txt")
        let lines = File.ReadAllLines(inputFilePath)
        _logger.Information("Lines read: {inputFileLinesLength}", lines.Length)

        let partNumbers =
            lines
            |> Array.mapi (fun ixRow line ->
                // Get any numbers from this line of text. Process the matched numbers.
                _rxNumber.Matches(line)
                    |> Seq.map (fun m ->
                        // The first and only capture in the match has our number and its place within the string.
                        let capture = m.Captures |> Seq.head
                        let capturedNum = { Value = int capture.Value; StartIndex = capture.Index; EndIndex = capture.Index + capture.Length }

                        //
                        // Look at all of the characters surrounding a number, and store each character in a HashSet<char>.
                        //   When finished, if the hashset contains only a single '.' character, then the number is not 
                        //   adjacent to any symbols, and thus is not a part number.
                        //
                        // NOTE: F# supports empty slices. So, for example, if we tell it to start at index -1, the sequence
                        //   it produces will start at 0.
                        //   See: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/slices#built-in-f-empty-slices
                        //

                        let surroundingChars = HashSet<char>()
                        let addChar c = surroundingChars.Add c |> ignore

                        // Above (includes diagonal), if we're not on the first row.
                        if ixRow > 0 then lines[ixRow - 1][capturedNum.StartIndex - 1 .. capturedNum.EndIndex] |> Seq.iter addChar

                        // Left, on current line
                        if capturedNum.StartIndex > 0 then line[capturedNum.StartIndex - 1] |> addChar

                        // Right, on current line
                        if capturedNum.EndIndex < line.Length then line[capturedNum.EndIndex] |> addChar

                        // Below (includes diagonal), if we're not on the last row.
                        if ixRow < lines.Length - 1 then lines[ixRow + 1][capturedNum.StartIndex - 1 .. capturedNum.EndIndex] |> Seq.iter addChar

                        let isPartNumber = not(surroundingChars |> Seq.tryExactlyOne = Some '.')

                        { Value = capturedNum.Value; IsPartNumber = isPartNumber })
                    |> Seq.filter _.IsPartNumber
                    |> Seq.toArray)
            |> Array.filter (fun numsOnLine -> numsOnLine.Length > 0)

        let partNumberSum =
            partNumbers
            |> Array.collect id
            |> Array.sumBy _.Value

        _logger.Information("Part number sum is {PartNumberSum}", partNumberSum)
