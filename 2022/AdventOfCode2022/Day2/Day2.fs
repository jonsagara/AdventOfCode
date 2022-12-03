namespace AdventOfCode2022

module Day2 =    

    open System
    open System.IO

    type Play =
        | Rock of string
        | Paper of string
        | Scissors of string

    type Outcome =
        | Win of score: int
        | Loss of score: int
        | Draw of score: int

    type Round =
        { OpponentPlay: Play
          OpponentOutcome: Outcome
          MyPlay: Play
          MyOutcome: Outcome }


    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day2_input.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        printfn $"Lines read: {inputFileLines.Length}"    

        let mapToPlay rawPlay =
            match rawPlay with
            | "A" | "X" -> Rock(rawPlay)
            | "B" | "Y" -> Paper(rawPlay)
            | "C" | "Z" -> Scissors(rawPlay)
            | _ -> invalidArg (nameof rawPlay) ($"Invalid {nameof rawPlay} value: {rawPlay}")

        let scorePlay play =
            match play with
            | Rock _ -> 1
            | Paper _ -> 2
            | Scissors _ -> 3

        let scoreRound myScore opponentScore =
            if myScore > opponentScore then Win(myScore + 6)
            elif myScore < opponentScore then Loss(myScore + 0)
            else Draw(myScore + 3)

        let rounds =
            inputFileLines
            |> Array.take 4
            |> Array.map (fun line ->
                let plays = line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                let opponentPlay = mapToPlay plays[0]
                let opponentScore = scorePlay opponentPlay

                let myPlay = mapToPlay plays[1]
                let myScore = scorePlay myPlay

                let opponentOutcome = scoreRound opponentScore myScore
                let myOutcome = scoreRound myScore opponentScore

                { OpponentPlay = opponentPlay
                  OpponentOutcome = opponentOutcome
                  MyPlay = myPlay
                  MyOutcome = myOutcome })

        printfn $"Rounds mapped: {rounds.Length}"

        let opponentsTotalScore =
            rounds
            |> Array.map (fun r ->
                match r.OpponentOutcome with
                | Win winScore -> winScore
                | Loss lossScore -> lossScore
                | Draw drawScore -> drawScore
                )
            |> Array.sum

        printfn $"Opponent's total score is: {opponentsTotalScore}"

        let myTotalScore =
            rounds
            |> Array.map (fun r ->
                match r.MyOutcome with
                | Win winScore -> winScore
                | Loss lossScore -> lossScore
                | Draw drawScore -> drawScore)
            |> Array.sum

        printfn $"My total score is: {myTotalScore}"

        rounds
        |> Array.iter (fun r -> printfn "%A" r)
