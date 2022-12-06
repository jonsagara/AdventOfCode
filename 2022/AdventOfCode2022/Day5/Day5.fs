namespace AdventOfCode2022

module Day5 =

    open System
    open System.Collections.Generic
    open System.IO
    open System.Text.RegularExpressions
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)
    
    type CargoStack = {
        Id : int
        Crates : Stack<string>
    }

    type private CargoShip = {
        CargoStacks: CargoStack array
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
                let cargoStacks = List<CargoStack>()
                let stackCount =
                    inputFileLines[ixLine]
                    |> splitLine
                    |> Array.length
                    
                { CargoStacks = Array.init stackCount (fun ixStack -> { Id = ixStack; Crates = Stack<string>() })}

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
                    printfn $"{label}"
                    cargoShip.CargoStacks[ixStack].Crates.Push(label)
                    
                ixLabel <- ixLabel + 4
                ixStack <- ixStack + 1
            
            
            // The first crate, if any, is at position 1.
            
            // inputFileLines[ixLine]
            // |> splitLine
            // |> Array.iteri (fun ixCrate crate ->
            //     // This doesn't work. We need to figure out the crate's stack instead of just blindly parsing and adding.
            //     let crateLabel = rxCrateId.Match(crate).Groups["label"].Value
            //     cargoShip.CargoStacks[ixCrate].Crates.Push(crateLabel)
            //     
            //     
            //     ())
            ())

        printfn "%O" cargoShip
        //
        // Part 1
        //

        // _logger.Information("[Part 1] There are {overlappingAssignments} pairs with completely overlapping assignments.", overlappingAssignments)


        //
        // Part 2
        //

        // _logger.Information("[Part 2] There are {anyOverlappingAssignments} pairs with at least one overlapping assignment.", anyOverlappingAssignments)

        ()