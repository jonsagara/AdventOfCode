namespace AdventOfCode2022

module Day2 =    

    open System
    open System.IO

    type Shape =
        | Rock of rawShape : string * score : int
        | Paper of rawShape : string * score : int
        | Scissors of rawShape : string * score : int

    type Outcome =
        | Win of score: int
        | Loss of score: int
        | Draw of score: int

    type Round =
        { OpponentShape: Shape
          OpponentOutcome: Outcome
          MyShape: Shape
          MyOutcome: Outcome }

    type DesiredOutcome = 
        | Loser
        | Tie
        | Winner


    let run () =
        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day2_input.txt")
        
        let inputFileLines = File.ReadAllLines(inputFilePath)
        printfn $"Lines read: {inputFileLines.Length}"    

        let mapToShape rawShape =
            match rawShape with
            | "A" | "X" -> Rock(rawShape, 1)
            | "B" | "Y" -> Paper(rawShape, 2)
            | "C" | "Z" -> Scissors(rawShape, 3)
            | _ -> invalidArg (nameof rawShape) ($"Invalid {nameof rawShape} value: {rawShape}")

        let scoreRound opponentShape myShape =
            match opponentShape, myShape with
            | (Rock _, Rock (_, myScore)) -> Draw(myScore + 3)
            | (Rock _, Paper (_, myScore)) -> Win(myScore + 6)
            | (Rock _, Scissors (_, myScore)) -> Loss(myScore + 0)
            | (Paper _, Rock (_, myScore)) -> Loss(myScore + 0)
            | (Paper _, Paper (_, myScore)) -> Draw(myScore + 3)
            | (Paper _, Scissors (_, myScore)) -> Win(myScore + 6)
            | (Scissors _, Rock (_, myScore)) -> Win(myScore + 6)
            | (Scissors _, Paper (_, myScore)) -> Loss(myScore + 0)
            | (Scissors _, Scissors (_, myScore)) -> Draw(myScore + 3)

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

        let splitLine (line : string) =
            let shapes = line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
            (shapes[0] |> mapToShape, shapes[1])

        let mapToDesiredResult resultRaw =
            match resultRaw with
            | "X" -> Loser 
            | "Y" -> Tie 
            | "Z" -> Winner 
            | r -> invalidArg (nameof resultRaw) $"Invalid desired result raw value: {resultRaw}"

        let mapToDesiredShape opponentShape desiredResult =
            match opponentShape, desiredResult with
            | Rock _, Loser -> Scissors(String.Empty, 3)
            | Rock _, Tie -> Rock(String.Empty, 1)
            | Rock _, Winner -> Paper(String.Empty, 2)
            | Paper _, Loser -> Rock(String.Empty, 1)
            | Paper _, Tie -> Paper(String.Empty, 2)
            | Paper _, Winner -> Scissors(String.Empty, 3)
            | Scissors _, Loser -> Paper(String.Empty, 2)
            | Scissors _, Tie -> Scissors(String.Empty, 3)
            | Scissors _, Winner -> Rock(String.Empty, 1)

        let rounds2 =
            inputFileLines
            |> Array.map(fun line ->
                let (opponentShape, myResultRaw) = splitLine line
                
                let desiredResult = mapToDesiredResult myResultRaw
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



