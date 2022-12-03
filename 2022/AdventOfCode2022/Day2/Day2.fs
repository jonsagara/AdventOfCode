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

        printfn $"Rounds mapped: {rounds.Length}"

        let opponentsTotalScore =
            rounds
            |> Array.map (fun r -> getOutcomeScore r.OpponentOutcome)
            |> Array.sum

        printfn $"Opponent's total score is: {opponentsTotalScore}"

        let myTotalScore =
            rounds
            |> Array.map (fun r -> getOutcomeScore r.MyOutcome)
            |> Array.sum

        printfn $"My total score is: {myTotalScore}"

        //printfn ""
        //rounds
        //|> Array.iter (fun r -> printfn "%A" r)

        let myManualScore =
            inputFileLines
            |> Array.map (fun line ->
                let shapes = line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

                let round = (shapes[0], shapes[1])
                match round with
                | ("A", "X") -> 1 + 3 // rock vs. rock: draw
                | ("A", "Y") -> 2 + 6 // rock vs. paper: win
                | ("A", "Z") -> 3 + 0 // rock vs. scissors: loss
                | ("B", "X") -> 1 + 0 // paper vs. rock: loss
                | ("B", "Y") -> 2 + 3 // paper vs. paper: draw
                | ("B", "Z") -> 3 + 6 // paper vs. scissors: win
                | ("C", "X") -> 1 + 6 // scissors vs. rock: win
                | ("C", "Y") -> 2 + 0 // scissors vs. paper: loss
                | ("C", "Z") -> 3 + 3 // scissors vs. scissors: draw
                | (x, y) -> invalidArg (nameof line) $"Line contains invalid shapes: {line}"
                )
            |> Array.sum

        printfn $"My manual score is: {myManualScore}"
