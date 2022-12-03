namespace AdventOfCode2022

module Day2 =    

    open System
    open System.IO

    type private Shape =
        | Rock
        | Paper
        | Scissors

    type private Outcome =
        | Win of score: int
        | Loss of score: int
        | Draw of score: int

    type private Round =
        { OpponentShape: Shape
          OpponentOutcome: Outcome
          MyShape: Shape
          MyOutcome: Outcome }

    type private DesiredOutcome = 
        | Loser
        | Tie
        | Winner
        
        
    [<Literal>]
    let private LossScore = 0
    [<Literal>]
    let private DrawScore = 3
    [<Literal>]
    let private WinScore = 6
    
    [<Literal>]
    let private RockScore = 1
    [<Literal>]
    let private PaperScore = 2
    [<Literal>]
    let private ScissorsScore = 3


    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day2_input.txt")
        
        let inputFileLines = File.ReadAllLines(inputFilePath)
        printfn $"Lines read: {inputFileLines.Length}"    

        let mapToShape rawShape =
            match rawShape with
            | "A" | "X" -> Rock
            | "B" | "Y" -> Paper
            | "C" | "Z" -> Scissors
            | _ -> invalidArg (nameof rawShape) ($"Invalid {nameof rawShape} value: {rawShape}")

        let scoreRound opponentShape myShape =
            match opponentShape, myShape with
            | Rock, Rock -> Draw(RockScore + DrawScore)
            | Rock, Paper -> Win(PaperScore + WinScore)
            | Rock, Scissors -> Loss(ScissorsScore + LossScore)
            | Paper, Rock -> Loss(RockScore + LossScore)
            | Paper, Paper -> Draw(PaperScore + DrawScore)
            | Paper, Scissors -> Win(ScissorsScore + WinScore)
            | Scissors, Rock -> Win(RockScore + WinScore)
            | Scissors, Paper -> Loss(PaperScore + LossScore)
            | Scissors, Scissors -> Draw(ScissorsScore + DrawScore)

        let getOutcomeScore outcome =
            match outcome with
            | Win winScore -> winScore
            | Loss lossScore -> lossScore
            | Draw drawScore -> drawScore


        //
        // Part 1
        //

        let rounds =
            inputFileLines
            |> Array.map (fun line ->
                let shapes = line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                let opponentShape = mapToShape shapes[0]
                let myShape = mapToShape shapes[1]

                let opponentOutcome = scoreRound myShape opponentShape
                let myOutcome = scoreRound opponentShape myShape

                { OpponentShape = opponentShape
                  OpponentOutcome = opponentOutcome
                  MyShape = myShape
                  MyOutcome = myOutcome })

        let opponentsTotalScore =
            rounds
            |> Array.map (fun r -> getOutcomeScore r.OpponentOutcome)
            |> Array.sum

        printfn $"[Part 1] Opponent's total score is: {opponentsTotalScore}"

        let myTotalScore =
            rounds
            |> Array.map (fun r -> getOutcomeScore r.MyOutcome)
            |> Array.sum

        printfn $"[Part 1] My total score is: {myTotalScore}"


        //
        // Part 2
        //
        
        let mapToDesiredResult resultRaw =
            match resultRaw with
            | "X" -> Loser 
            | "Y" -> Tie 
            | "Z" -> Winner 
            | r -> invalidArg (nameof resultRaw) $"Invalid desired result raw value: {resultRaw}"

        let splitLine (line : string) =
            let shapes = line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
            (shapes[0] |> mapToShape, shapes[1] |> mapToDesiredResult)

        let mapToDesiredShape opponentShape desiredResult =
            match opponentShape, desiredResult with
            | Rock, Loser -> Scissors
            | Rock, Tie -> Rock
            | Rock, Winner -> Paper
            | Paper, Loser -> Rock
            | Paper, Tie -> Paper
            | Paper, Winner -> Scissors
            | Scissors, Loser -> Paper
            | Scissors, Tie -> Scissors
            | Scissors, Winner -> Rock

        let rounds2 =
            inputFileLines
            |> Array.map(fun line ->
                let (opponentShape, desiredResult) = splitLine line
                let myDesiredShape = mapToDesiredShape opponentShape desiredResult

                let opponentOutcome = scoreRound myDesiredShape opponentShape
                let myOutcome = scoreRound opponentShape myDesiredShape

                { OpponentShape = opponentShape
                  OpponentOutcome = opponentOutcome
                  MyShape = myDesiredShape
                  MyOutcome = myOutcome })

        let myTotalScore2 =
            rounds2
            |> Array.map (fun r -> getOutcomeScore r.MyOutcome)
            |> Array.sum

        printfn $"[Part 2] My total score is: {myTotalScore2}"



