namespace AdventOfCode2023

module Day3 =

    open System
    open System.IO
    open System.Text.RegularExpressions
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day3_input_sample.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)

        
