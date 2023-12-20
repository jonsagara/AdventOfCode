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
                let numberMatches = _rxNumber.Matches(line)
            
                let capturedNumbersOnLine = 
                    numberMatches
                    |> Seq.map (fun m ->
                        let capture = m.Captures |> Seq.head
                        { Value = int capture.Value; StartIndex = capture.Index; EndIndex = capture.Index + capture.Length })
                    |> Seq.toArray

                let numbersOnLine =
                    capturedNumbersOnLine
                    |> Array.map (fun num ->
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
                        if ixRow > 0 then lines[ixRow - 1][num.StartIndex - 1 .. num.EndIndex] |> Seq.iter addChar

                        // Left, on current line
                        if num.StartIndex > 0 then line[num.StartIndex - 1] |> addChar

                        // Right, on current line
                        if num.EndIndex < line.Length then line[num.EndIndex] |> addChar

                        // Below (includes diagonal), if we're not on the last row.
                        if ixRow < lines.Length - 1 then lines[ixRow + 1][num.StartIndex - 1 .. num.EndIndex] |> Seq.iter addChar

                        let isPartNumber = not(surroundingChars |> Seq.tryExactlyOne = Some '.')

                        { Value = num.Value; IsPartNumber = isPartNumber })

                numbersOnLine |> Array.filter _.IsPartNumber)
            |> Array.filter (fun numsOnLine -> numsOnLine.Length > 0)

        let partNumberSum =
            partNumbers
            |> Array.sumBy (fun partNums -> partNums |> Array.sumBy _.Value)

        _logger.Information("Part number sum is {PartNumberSum}", partNumberSum)
        ()
        
