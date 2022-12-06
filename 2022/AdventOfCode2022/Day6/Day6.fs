namespace AdventOfCode2022

module Day6 =

    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    /// Look for the first occurrence of windowSize consecutive distinct characters in the input string.
    let private getCharsConsumedUntilMarker (windowSize : int) (line : string) =
        // Convert the string into a character array
        line.ToCharArray()
        // Create an array of sliding windows of the input character array. Each window is of size windowSize.
        |> Array.windowed windowSize
        // Find the first sliding window that contains all distinct characters. This is our marker.
        |> Array.findIndex (fun charWindow -> charWindow |> Set.ofArray |> Set.count = windowSize)
        // Add the windowSize to the array index to get the count of the number of characters processed until
        //   we found the marker. The smallest this can be is windowSize characters.
        |> (+) windowSize
    
    let private left count (s : string) =
        if s.Length < count then
            s
        else
            s.Substring(0, count) + "..."

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day6_input.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
        


        
        //
        // Part 1
        //

        inputFileLines
        |> Array.iter (fun line ->
            let charsConsumed = getCharsConsumedUntilMarker 4 line
            _logger.Information("[Part 1] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)
            )


        //
        // Part 2
        //

        inputFileLines
        |> Array.iter (fun line ->
            let charsConsumed = getCharsConsumedUntilMarker 14 line
            _logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)
            )
