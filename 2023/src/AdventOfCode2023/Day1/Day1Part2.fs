namespace AdventOfCode2023

module Day1Part2 =

    open System
    open System.IO
    open System.Text.RegularExpressions
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type private LineFirstAndLastNumber = {
        First : int
        Last : int
        }

    [<Literal>]
    let private _digitPattern = @"(one|two|three|four|five|six|seven|eight|nine|\d)"

    let private _rxFirstDigit = Regex(_digitPattern, RegexOptions.Compiled)
    let private _rxLastDigit = Regex(_digitPattern, RegexOptions.Compiled ||| RegexOptions.RightToLeft)

    let private textToDigits = dict[
        ("one", 1)
        ("two", 2)
        ("three", 3)
        ("four", 4)
        ("five", 5)
        ("six", 6)
        ("seven", 7)
        ("eight", 8)
        ("nine", 9)
        ]

    let private matchToDigit (digitMatch : Match) =
        if digitMatch.Value.Length = 1 then
            Char.GetNumericValue digitMatch.Value[0] |> int
        else
            textToDigits[digitMatch.Value]

    let private computeLineCalibrationValue (ln) =
        (10 * ln.First) + ln.Last


    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day1Part2_input.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)

        let totalCalibrationValue =
            inputFileLines
            |> Array.map (fun line ->
                let firstDigit = _rxFirstDigit.Match line |> matchToDigit
                let lastDigit = _rxLastDigit.Match line |> matchToDigit
                { First = firstDigit; Last = lastDigit })
            |> Array.map computeLineCalibrationValue
            |> Array.sum

        _logger.Information( "The total calibration value is {TotalCalibrationValue}.", totalCalibrationValue)
