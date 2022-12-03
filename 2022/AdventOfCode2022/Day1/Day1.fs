namespace AdventOfCode2022

module Day1 = 

    open System
    open System.Collections.Generic
    open System.IO
    open System.Linq

    type private Elf = { 
        Id: int
        TotalCalories: int 
    }

    let run () =

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day1_input.txt")
        let inputFileLines = File.ReadAllLines(inputFilePath)
        printfn $"Lines read: {inputFileLines.Length}"

        let elfMeals = Dictionary<int, List<int>>()

        let mutable ixLine = 0
        let mutable ixElf = 0

        while ixLine < inputFileLines.Length do
            let calories = inputFileLines[ixLine]

            if String.IsNullOrWhiteSpace(calories) then
                // Finished processing the previous elf. Assign a new Id for any subsequent elf.
                ixElf <- ixLine
            else
                match elfMeals.TryGetValue(ixElf) with
                | true, meals ->
                    // We have already encountered this elf. Add the new meal to its list of meals.
                    meals.Add(Int32.Parse(calories))
                | false, _ ->
                    // We have not yet seen this elf. Add them to the dictionary, and add the first meal.
                    elfMeals[ixElf] <- List<int>([| Int32.Parse(calories) |])

            ixLine <- ixLine + 1
        

        let elves =
            elfMeals.ToArray()
            |> Array.map (fun kvp ->
                { Id = kvp.Key
                  TotalCalories = kvp.Value.Sum() })

        let mostCalsElf = elves |> Array.maxBy (fun elf -> elf.TotalCalories)

        printfn $"Elf {mostCalsElf.Id} had {mostCalsElf.TotalCalories} calories."


        //
        // Top 3
        //

        let top3Elves =
            elves |> Array.sortByDescending (fun e -> e.TotalCalories) |> Array.take 3

        top3Elves
        |> Array.iter (fun e -> printfn $"Elf {e.Id} has {e.TotalCalories} calories.")

        let top3Sum = top3Elves |> Array.sumBy (fun e -> e.TotalCalories)
        printfn $"Top 3 Elf calories total: {top3Sum}"
