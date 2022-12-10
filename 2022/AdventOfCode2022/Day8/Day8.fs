namespace AdventOfCode2022

module Day8 =

    open System
    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type private Tree = {
        Row : int
        Column : int
        Value : int
        Visible : bool
        }

    let isEdgeTree row column rowLength columnLength =
        if row = 0 || row = rowLength - 1 then
            // Tree is in the top or bottom row.
            true
        else if column = 0 || column = columnLength - 1 then
            // Tree is in the left-most or right-most column.
            true
        else
            // Tree is somewhere in the middle.
            false

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day8_input.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)
        
        // Create a 2D array. The rows are the length of the file, and the columns are the length of a line.
        //   All lines have the same length (i.e., it's not a jagged array).
        let outerDimension = inputFileLines.Length
        let innerDimension = inputFileLines[0].Length
        _logger.Information("Create a int[{outerDimension}][{innerDimension}]", outerDimension, innerDimension)

        let grid = Array2D.create outerDimension innerDimension -1

        // Iterate through each line in the file, and through each character in each line, to populate the array.
        inputFileLines
        |> Array.iteri (fun ixOuter line ->
            line.ToCharArray()
            |> Array.iteri (fun ixInner numChar ->
                if numChar < '0' || numChar > '9' then
                    invalidOp $"[{ixOuter}][{ixInner}] Character '{numChar}' is not a valid numeric digit between 0-9"

                grid[ixOuter, ixInner] <- int(numChar - '0')
                ())
            ())

        //DEBUG:
        //grid
        //|> Array2D.iteri (fun i j value -> 
        //    printfn $"grid[{i}][{j}] = {value}")

        // All trees in the outer edges are visible. Subtract two each from Left and Right so that we don't count 
        //   the corners twice.
        let outerEdgesVisibleCount = (2 * outerDimension) + (2 * innerDimension) - 4
        _logger.Information("Outer edge visible trees count: {outerEdgesVisibleCount:N0}", outerEdgesVisibleCount)

        // Start at [0, 0] and go to [rowLength - 1, columnLength - 1]. For each element, it is visible
        //   from outside the grid if any of the following are true:
        //   * The tree is on one of the edges
        //   * Every element above it thas a smaller numeric value
        //   * Every element to the left of it has a smaller numeric value
        //   * Every element below it has a smaller numeric value
        //   * Every element to the right of it has a smaller numeric value
        let treeVisibilities = 
            [ for row in 0 .. outerDimension - 1 do
                for col in 0 .. innerDimension - 1 ->
                    if isEdgeTree row col outerDimension innerDimension then
                        { Row = row; Column = col; Value = grid[row, col]; Visible = true }
                    else
                        let currentTreeHeight = grid[row, col]

                        // Visible from the top IFF every tree above it has a lower value.
                        let visibleFromTop = 
                            let rowsToCheck = [ row - 1 .. -1 .. 0]
                            rowsToCheck
                            |> List.forall (fun checkRow -> grid[checkRow, col] < currentTreeHeight)

                        // Check visibility from the left
                        let visibleFromLeft = 
                            let columnsToCheck = [ 0 .. col - 1]
                            columnsToCheck
                            |> List.forall (fun checkCol -> grid[row, checkCol] < currentTreeHeight)
                        
                        // Check visibility from the bottom
                        let visibleFromBottom = 
                            let rowsToCheck = [ row + 1 .. outerDimension - 1 ]
                            rowsToCheck
                            |> List.forall (fun checkRow -> grid[checkRow, col] < currentTreeHeight)

                        // Check visibility from the right
                        let visibleFromRight = 
                            let columnsToCheck = [ col + 1 .. innerDimension - 1]
                            columnsToCheck
                            |> List.forall (fun checkCol -> grid[row, checkCol] < currentTreeHeight)

                        let visible = visibleFromTop || visibleFromLeft || visibleFromBottom || visibleFromRight

                        { Row = row; Column = col; Value = grid[row, col]; Visible = visible }
            ]

        //DEBUG:
        //treeVisibilities
        //|> List.filter (fun t -> t.Visible)
        //|> List.iter (fun t ->
        //    printfn $"[{t.Row}][{t.Column}] {t.Value} is visible"
        //    )

        
        //
        // Part 1: How many trees are visible from at least one edge?
        //
        
        let visibleTreeCount =
            treeVisibilities
            |> List.filter (fun t -> t.Visible)
            |> List.length

        _logger.Information("[Part 1] There are {visibleTreeCount:N0} trees visible from at least one edge.", visibleTreeCount)
        


        //
        // Part 2
        //

        //_logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)
        ()