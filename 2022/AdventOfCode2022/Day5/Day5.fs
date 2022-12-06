namespace AdventOfCode2022

module Day5 =

    open System
    open System.Collections.Generic
    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)
    
    type CargoStack = {
        Id : int
        Crates : Stack<string>
    }

    type private CargoShip = {
        Stacks: CargoStack array
        }
    
    // Split on space, remove empty entries, and trim those that remain.
    let splitLine (line : string) =
        line.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
        
    let isInt32 (linePart : string) =
        let success, _ = Int32.TryParse linePart
        success

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day5_input_sample.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            // |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
        
        
        //
        // Parse the visual graph of stacks of crates.
        //
        
        // Find the index of the line containing only numbers.
        let stackNumbersLineIndex =
            inputFileLines
            |> Array.tryFindIndex (fun line ->
                // Return true if all entries are integers; false otherwise.
                line
                |> splitLine
                |> Array.forall isInt32
                )
        
        // Parse the line of stack ids, and use that to create an instance of CargoShip with an empty array initialized
        //   to the correct number of CargoStacks.
        let cargoShip =
            match stackNumbersLineIndex with
            | None -> invalidOp $"None of the lines in the input file contain only integer stack ids."
            | Some ixLine ->
                let stackCount =
                    inputFileLines[ixLine]
                    |> splitLine
                    |> Array.length
                    
                { Stacks = Array.init stackCount (fun ixStack -> { Id = ixStack; Crates = Stack<string>() })}

        // We need to parse each line to get the crate ids for each stack. Start at the line closest to the list of stack
        //   ids. Work backwards so that we can continually push items onto the stack.
        [stackNumbersLineIndex.Value - 1 .. -1 .. 0]
        |> List.iter (fun ixLine ->
            // The first crate label, if any, is at index 1 on the line.
            let mutable ixLabel = 1
            let mutable ixStack = 0
            
            let line = inputFileLines[ixLine]
            while ixLabel < line.Length do
                let label = string(line[ixLabel])
                
                if not(String.IsNullOrWhiteSpace(label)) then do
                    //printfn $"{label}"
                    cargoShip.Stacks[ixStack].Crates.Push(label)
                    
                ixLabel <- ixLabel + 4
                ixStack <- ixStack + 1
            
            ())

        //DEBUG:
        //printfn $"{cargoShip}"

        // Now we need to start reading each "move 1 from 2 to 1" instruction, parse it, and make the appropriate 
        //   modifications to the cargo ship. There is a blank line after the stack numbers, and then the move
        //   commands start.
        //printfn $"Stack numbers line index: {stackNumbersLineIndex}"
        inputFileLines
        |> Array.skip (stackNumbersLineIndex.Value + 1)
        |> Array.filter (fun line -> not(String.IsNullOrWhiteSpace(line)))
        |> Array.iter (fun line ->
            //printfn $"Command: {line}"
            let lineParts = splitLine line

            // [1] is quantity to move
            // [3] is the 1-based "from" stack
            // [5] is the 1-based "to" stack
            let quantity = Int32.Parse(lineParts[1])
            let fromStackId = Int32.Parse(lineParts[3])
            let ixFromStack = fromStackId - 1
            let toStackId = Int32.Parse(lineParts[5])
            let ixToStack = toStackId - 1

            // From 1 to quantity, start moving items from the "from" stack to the "to" stack, one at a time.
            [| 1 .. quantity |]
            |> Array.iter (fun _ ->
                let crateToMove = cargoShip.Stacks[ixFromStack].Crates.Pop()
                cargoShip.Stacks[ixToStack].Crates.Push(crateToMove)
                ))

        
        //
        // Part 1
        //

        // After the rearrangement procedure completes, what crate ends up on top of each stack?
        // I guess we're assuming that each stack will have at least one crate?
        let topCrates =
            cargoShip.Stacks
            |> Array.map (fun stack -> stack.Crates.Peek())
            |> String.concat ""

        _logger.Information("[Part 1] Top crates: {topCrates}", topCrates)


        //
        // Part 2
        //

        // _logger.Information("[Part 2] There are {anyOverlappingAssignments} pairs with at least one overlapping assignment.", anyOverlappingAssignments)

        ()