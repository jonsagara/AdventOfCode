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

    type private ScenicTree = {
        Row : int
        Column : int
        Value : int
        Score : int
        }

    /// True if the tree is on the top, left, bottom, or right edge; false otherwise.
    let isEdgeTree row column rowMaxIndex colMaxIndex =
        if row = 0 || row = rowMaxIndex then
            // Tree is in the top or bottom row.
            true
        else if column = 0 || column = colMaxIndex then
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

        let rowMaxIndex = outerDimension - 1
        let colMaxIndex = innerDimension - 1

        // Initialize with an invalid value. If we see it later on, we know we didn't populate the grid correctly.
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

        //// All trees in the outer edges are visible. Subtract two each from Left and Right so that we don't count 
        ////   the corners twice.
        //let outerEdgesVisibleCount = (2 * outerDimension) + (2 * innerDimension) - 4
        //_logger.Information("Outer edge visible trees count: {outerEdgesVisibleCount:N0}", outerEdgesVisibleCount)

        // Start at [0, 0] and go to [rowLength - 1, columnLength - 1]. For each element, it is visible
        //   from outside the grid if any of the following are true:
        //   * The tree is on one of the edges
        //   * Every element above it thas a smaller numeric value
        //   * Every element to the left of it has a smaller numeric value
        //   * Every element below it has a smaller numeric value
        //   * Every element to the right of it has a smaller numeric value
        let treeVisibilities = 
            [ for row in 0 .. rowMaxIndex do
                for col in 0 .. colMaxIndex ->
                    if isEdgeTree row col rowMaxIndex colMaxIndex then
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
                            let rowsToCheck = [ row + 1 .. rowMaxIndex ]
                            rowsToCheck
                            |> List.forall (fun checkRow -> grid[checkRow, col] < currentTreeHeight)

                        // Check visibility from the right
                        let visibleFromRight = 
                            let columnsToCheck = [ col + 1 .. colMaxIndex ]
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
        // Part 2: What is the highest scenic score possible for any tree?
        //

        // Start at [0, 0] and go to [rowLength - 1, columnLength - 1]. Trees on an edge will have a scenic score of 0.
        let treeScenicScores = 
            [ for row in 0 .. rowMaxIndex do
                for col in 0 .. colMaxIndex ->
                    if isEdgeTree row col rowMaxIndex colMaxIndex then
                        // Any edge tree will have at least one 0 score, thus its scenic score will also be 0.
                        { Row = row; Column = col; Value = grid[row, col]; Score = 0 }
                    else

                        let currentTreeHeight = grid[row, col]

                        // Look up: stop if you reach an edge or the first tree whose height is >= the current tree.
                        //   The count of trees is the viewing distance.
                        let lookUpTreeCount = 
                            let rowsToCheck = [ row - 1 .. -1 .. 0]
                            // takeWhile doesn't work in the case where, if the current tree is 5, the rows above it
                            //   are 3, 5, and 3. It goes all the way to the edge instead of stopping at 5.
                            //rowsToCheck
                            //|> List.takeWhile (fun ixRowCheck -> grid[ixRowCheck, col] <= currentTreeHeight)
                            //|> List.length
                            let firstBlockingTreeRowIndex =
                                rowsToCheck
                                |> List.tryFindIndex (fun ixRowCheck -> grid[ixRowCheck, col] >= currentTreeHeight)

                            match firstBlockingTreeRowIndex with
                            | Some index -> index + 1
                            | None -> row

                        // Look left: stop if you reach an edge or the first tree whose height is >= the current tree.
                        //   The count of trees is the viewing distance.
                        let lookLeftTreeCount = 
                            let colsToCheck = [ col - 1 .. -1 .. 0 ]
                            let firstBlockingTreeColIndex = 
                                colsToCheck
                                |> List.tryFindIndex (fun ixColCheck -> grid[row, ixColCheck] >= currentTreeHeight)

                            match firstBlockingTreeColIndex with
                            | Some index -> index + 1
                            | None -> col

                        // Look right: stop if you reach an edge or the first tree whose height is >= the current tree.
                        //   The count of trees is the viewing distance.
                        let lookRightTreeCount = 
                            let colsToCheck = [ col + 1 .. colMaxIndex ]
                            let firstBlockingTreeColIndex =
                                colsToCheck
                                |> List.tryFindIndex (fun ixColCheck -> grid[row, ixColCheck] >= currentTreeHeight)

                            match firstBlockingTreeColIndex with
                            | Some index -> index + 1
                            | None -> colMaxIndex - col

                        // Look down: stop if you reach an edge or the first tree whose height is >= the current tree.
                        //   The count of trees is the viewing distance.
                        let lookDownTreeCount = 
                            let rowsToCheck = [ row + 1 .. rowMaxIndex ]
                            let firstBlockingTreeRowIndex =
                                rowsToCheck
                                |> List.tryFindIndex (fun ixRowCheck -> grid[ixRowCheck, col] >= currentTreeHeight)

                            match firstBlockingTreeRowIndex with
                            | Some index -> index + 1
                            | None -> rowMaxIndex - row

                        
                        let scenicScore = lookUpTreeCount * lookLeftTreeCount * lookRightTreeCount * lookDownTreeCount

                        { Row = row; Column = col; Value = grid[row, col]; Score = scenicScore }
            ]

        //DEBUG:
        //treeScenicScores
        //|> List.iter (fun tss ->
        //    printfn $"[{tss.Row}][{tss.Column}] Scenic Score = {tss.Score}"
        //    )

        let maxScenicScore =
            treeScenicScores
            |> List.maxBy (fun tss -> tss.Score)

        _logger.Information("[Part 2] The max scenic score is {maxScenicScore:N0} at [{row}, {col}].", maxScenicScore.Score, maxScenicScore.Row, maxScenicScore.Column)
        ()