namespace AdventOfCode2023

module Day1 =

    open System
    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type private LineFirstAndLastNumber = {
        First : int option
        Last : int option
        }

    let private charNumericValue (ch : char) =
        int(Char.GetNumericValue(ch))

    let private computeLineCalibrationValue (ln) =
        (10 * Option.get (ln.First)) + Option.get (ln.Last)

    let private updateFirstOrLast (state : LineFirstAndLastNumber) (currentDigitChar : char) =
        if Option.isNone state.First then
            { state with First = Some(currentDigitChar |> charNumericValue) }
        else
            { state with Last = Some (currentDigitChar |> charNumericValue) }

    let run () =

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day1_input_sample.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)

        let lineNumbers = 
            inputFileLines
            |> Array.map (fun line ->

                let initialFirstAndLast = { First = None; Last = None }
                
                line.ToCharArray()
                |> Array.filter Char.IsAsciiDigit
                |> Array.fold updateFirstOrLast initialFirstAndLast
                )
            |> Array.map (fun firstAndLast ->
                if Option.isNone firstAndLast.Last then
                    { firstAndLast with Last = firstAndLast.First }
                else
                    firstAndLast
                )

        let totalCalibrationValue = 
            lineNumbers
            |> Array.map computeLineCalibrationValue
            |> Array.sum

        _logger.Information( "The total calibration value is {TotalCalibrationValue}.", totalCalibrationValue)

        ()



