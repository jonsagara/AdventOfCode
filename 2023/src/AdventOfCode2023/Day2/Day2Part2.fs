namespace AdventOfCode2023

module Day2Part2 =

    open System
    open System.IO
    open System.Text.RegularExpressions
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type private Subset = {
        RedCubes : int
        GreenCubes : int
        BlueCubes : int
        }

    type private Game = {
        Subsets : Subset[]
    }

    let private _rxCountAndColor = Regex(@"(?<count>\d+) (?<color>red|green|blue)", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private assignCounts (state : Subset) (colorAndCount : string) =
        let countColorMatch = _rxCountAndColor.Match(colorAndCount)

        match countColorMatch.Groups["color"].Value with
        | "red" -> { state with RedCubes = int countColorMatch.Groups["count"].Value }
        | "green" -> { state with GreenCubes = int countColorMatch.Groups["count"].Value }
        | "blue" -> { state with BlueCubes = int countColorMatch.Groups["count"].Value }
        | unknown -> failwith $"Invalid color {unknown}"


    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day2_input.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)

        // Parse the list of games
        let games =
            inputFileLines
            |> Array.map (fun line ->
                // Split on ':' to separate the Game Id from the subsets of cubes.
                let gameAndSubsets = line.Split(":", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                // Split on ';' to separate the subsets.
                let subsetStrings = gameAndSubsets[1].Split(";", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                // Parse each subset into counts/colors.
                let subsets =
                    subsetStrings
                    |> Array.map (fun subsetStr ->
                        // Split on ',' to separate the cube color counts.
                        let cubeColorCounts = subsetStr.Split(",", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                        let subset = 
                            cubeColorCounts
                            |> Array.fold assignCounts { RedCubes = 0; GreenCubes = 0; BlueCubes = 0 }

                        subset)

                { Subsets = subsets })

        // For each game, select the max cubes per color. This is the minimum necessary to play each respective game.
        let minPossibleCubesPowersSum =
            games
            |> Array.map (fun game ->
                let maxReds =  game.Subsets |> Array.map _.RedCubes |> Array.max
                let maxGreens = game.Subsets |> Array.map _.GreenCubes |> Array.max
                let maxBlues = game.Subsets |> Array.map _.BlueCubes |> Array.max

                { RedCubes = maxReds; GreenCubes = maxGreens; BlueCubes = maxBlues })
            |> Array.map (fun minCubesSubset -> minCubesSubset.RedCubes * minCubesSubset.GreenCubes * minCubesSubset.BlueCubes)
            |> Array.sum

        _logger.Information("The sum of the powers of these sets is {PowersSum}.", minPossibleCubesPowersSum)
