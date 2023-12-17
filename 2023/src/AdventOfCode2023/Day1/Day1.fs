namespace AdventOfCode2023

module Day1 =

    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    let run () =

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day1_input_sample.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)



