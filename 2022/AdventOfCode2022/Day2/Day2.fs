namespace AdventOfCode2022

module Day2 =    

    open System
    open System.IO

    type Shape =
        | Rock of string
        | Paper of string
        | Scissors of string

    type Outcome =
        | Win of score: int
        | Loss of score: int
        | Draw of score: int

    type Round =
        { OpponentShape: Shape
          OpponentOutcome: Outcome
          MyShape: Shape
          MyOutcome: Outcome }


    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day2_input.txt")
        
        let inputFileLines = File.ReadAllLines(inputFilePath)
        printfn $"Lines read: {inputFileLines.Length}"    

        let mapToShape rawShape =
            match rawShape with
            | "A" | "X" -> Rock(rawShape)
            | "B" | "Y" -> Paper(rawShape)
            | "C" | "Z" -> Scissors(rawShape)
            | _ -> invalidArg (nameof rawShape) ($"Invalid {nameof rawShape} value: {rawShape}")

        let scoreShape shape =
            match shape with
            | Rock _ -> 1
            | Paper _ -> 2
            | Scissors _ -> 3

        let scoreRound opponentShapeScore myShapeScore =
            if opponentShapeScore > myShapeScore then Loss(myShapeScore + 0)
            elif opponentShapeScore < myShapeScore then Win(myShapeScore + 6)
            else Draw(myShapeScore + 3)

        let rounds =
            inputFileLines
            |> Array.map (fun line ->
                let shapes = line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                let opponentShape = mapToShape shapes[0]
                let opponentShapeScore = scoreShape opponentShape

                let myShape = mapToShape shapes[1]
                let myShapeScore = scoreShape myShape

                let opponentOutcome = scoreRound myShapeScore opponentShapeScore
                let myOutcome = scoreRound opponentShapeScore myShapeScore

                { OpponentShape = opponentShape
                  OpponentOutcome = opponentOutcome
                  MyShape = myShape
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

        //printfn ""
        //rounds
        //|> Array.iter (fun r -> printfn "%A" r)
