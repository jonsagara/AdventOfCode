namespace AdventOfCode2023

module Day2 =

    open System
    open System.IO
    open System.Text.RegularExpressions
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    // The bag has been loaded with this combination of cubes:
    let private _redCubeMaxCount = 12
    let private _greenCubeMaxCount = 13
    let private _blueCubeMaxCount = 14

    type private Subset = {
        RedCubes : int
        GreenCubes : int
        BlueCubes : int
        }

    type private Game = {
        Id : int
        Subsets : Subset[]
    }

    let private _rxGameId = Regex(@"Game (?<id>\d+)", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private getGameId (game : string) =
        let gameIdStr = _rxGameId.Match(game).Groups["id"].Value
        int(gameIdStr)


    let private _rxCountAndColor = Regex(@"(?<count>\d+) (?<color>red|green|blue)", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private assignCounts (state : Subset) (colorAndCount : string) =
        let countColorMatch = _rxCountAndColor.Match(colorAndCount)

        match countColorMatch.Groups["color"].Value with
        | "red" -> { state with RedCubes = int(countColorMatch.Groups["count"].Value) }
        | "green" -> { state with GreenCubes = int(countColorMatch.Groups["count"].Value) }
        | "blue" -> { state with BlueCubes = int(countColorMatch.Groups["count"].Value) }
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
                let gameId = getGameId gameAndSubsets[0]

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

                { Id = gameId; Subsets = subsets })

        // Filter out impossible games.
        let possibleGames =
            games
            |> Array.filter (fun g ->
                let impossibleSubsets =
                    g.Subsets
                    |> Array.filter (fun s -> s.RedCubes > _redCubeMaxCount || s.GreenCubes > _greenCubeMaxCount || s.BlueCubes > _blueCubeMaxCount)

                impossibleSubsets.Length = 0
                )

        // Sum the game Ids of the possible games.
        let gameIdSum =
            possibleGames
            |> Array.sumBy _.Id

        _logger.Information("The sum of possible Game IDs is {GameIdSum}.", gameIdSum)
