namespace AdventOfCode2022

module Day1 = 

    open System
    open System.Collections.Generic
    open System.IO
    open System.Linq
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type private Elf = { 
        Id: int
        TotalCalories: int 
    }
    
    type private ElfMealState = {
        IxLine : int
        IxElf : int
        ElfMeals : Dictionary<int, List<int>>
    }

    let run () =

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day1_input.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
            
        let groupMealsByElf (accum : ElfMealState) (calories : string) =
            
            if String.IsNullOrWhiteSpace(calories) then
                // The previous iteration was the last calorie count for the prior elf.
                // Start a new line, and update the elf index for the next elf's meals,
                //   if any.
                { accum with
                    IxLine = accum.IxLine + 1
                    IxElf = accum.IxLine }
            else
                match accum.ElfMeals.TryGetValue(accum.IxElf) with
                | true, meals ->
                    // We have already encountered this elf. Add the new meal to its list of meals.
                    meals.Add(Int32.Parse(calories))
                | false, _ ->
                    // We have not yet seen this elf. Add them to the dictionary, and add the first meal.
                    accum.ElfMeals[accum.IxElf] <- List<int>([| Int32.Parse(calories) |])
                    
                { accum with IxLine = accum.IxLine + 1 }
        
        let elfMealState =
            inputFileLines
            |> Array.fold groupMealsByElf { IxLine = 0; IxElf = 0; ElfMeals = Dictionary<int, List<int>>()}
        

        let elfMeals =
            elfMealState.ElfMeals.ToArray()
            |> Array.map (fun kvp ->
                { Id = kvp.Key
                  TotalCalories = kvp.Value.Sum() })

        let mostCalsElf = elfMeals |> Array.maxBy (fun elf -> elf.TotalCalories)
        _logger.Information("Elf {mostCalsElfId} had {mostCalsElfTotalCalories} calories.", mostCalsElf.Id, mostCalsElf.TotalCalories)


        //
        // Top 3
        //

        let top3Elves =
            elfMeals |> Array.sortByDescending (fun e -> e.TotalCalories) |> Array.take 3

        top3Elves
        |> Array.iter (fun e -> _logger.Information("Elf {eId} has {eTotalCalories} calories.", e.Id, e.TotalCalories))

        let top3Sum = top3Elves |> Array.sumBy (fun e -> e.TotalCalories)
        _logger.Information("Top 3 Elf calories total: {top3Sum}", top3Sum)
