namespace AdventOfCode2022

module Day7 =

    open System
    open System.Collections.Generic
    open System.IO
    open Serilog

    type private Marker = class end
    let private _logger = Log.Logger.ForContext(typeof<Marker>.DeclaringType)

    type NodeType =
        | File
        | Directory
        
    type Node = {
        Name : string
        // Type : NodeType
        // FileSize : int64 option
        Children : Node list
    }

    let run () =

        //
        // Read all of the lines in from the file.
        //

        let inputFilePath = Path.Combine(__SOURCE_DIRECTORY__, "Day7_input_sample.txt")
        let inputFileLines = 
            File.ReadAllLines(inputFilePath) 
            |> Array.map (fun l -> l.Trim())
            |> Array.toList

        _logger.Information("Lines read: {inputFileLinesLength}", inputFileLines.Length)

        let processLines inputLines =
            
            if "cmd" = "cd" then
                // Check the current directory's children for the specified directory.
                //   If found, noop. Otherwise, create a new directory and add it to the children.
                // Change to the specified directory. This really only affects the children
                ()
            elif "cmd" = "dir" then
                // the next line(s) will list the contents of the current directory
                ()
            
        
        
        ()

        
        //
        // Part 1
        //

        //_logger.Information("[Part 1] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)


        //
        // Part 2
        //

        //_logger.Information("[Part 2] Characters consumed until marker found: {charsConsumed}; line: {line}", charsConsumed, line |> left 50)

(*
    I tried doing it first in F# with recursion, because trees = recursion, right?

    I ended up falling back to imperative code in C# just to get it working.

void Main()
{
	var lines = new[] 
	{
		"... data snipped ...",
	};


	// Sanity check. First command has to be a "cd" so that we know where we're starting.
	if (lines.Length == 0) { throw new InvalidOperationException($"List of commands is empty."); }
	if (!lines[0].StartsWith("$ cd")) { throw new InvalidOperationException($"First command must be a 'cd'. Instead, it's {lines[0]}"); }

	Node? rootNode = null;
	Node? currentNode = null;

	foreach (var line in lines)
	{
		var lineParts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		if (lineParts[0] == "$")
		{
			// This is a comment issued by the user at the command prompt.

			if (lineParts[1] == "cd")
			{
				// This is a "cd" change directory command.
				var targetDir = lineParts[2];

				// The very first command has to be a cd to set the root directory. If we haven't yet initialized
				//   the root node, then this is the first command. Initialize the root now.
				if (rootNode is null)
				{
					rootNode = new Node
					{
						Name = targetDir,
						Type = NodeType.Directory,
						Parent = null,
					};

					currentNode = rootNode;
				}
				else if (targetDir == "..")
				{
					// We're going back up one directory.
					currentNode = currentNode!.Parent;
				}
				else
				{
					// We're going down into a child directory.

					// Set the current node to the child Node.
					currentNode = currentNode!.Children.Single(n => n.Name == targetDir);
				}
			}
		}
		else if (lineParts[0] == "dir")
		{
			// This is a directory name. Add it to the children.
			currentNode!.Children.Add(new Node
			{
				Name = lineParts[1],
				Type = NodeType.Directory,
				Parent = currentNode,
			});
		}
		else 
		{
			// This is a file size and name. Add it to the children.
			currentNode!.Children.Add(new Node
			{
				Name = lineParts[1],
				Type = NodeType.File,
				Size = int.Parse(lineParts[0]),
				Parent = currentNode,
			});
		}
	}
	
	//rootNode.Dump();
	var currentPath = new Stack<string>();
	var dirSizes = new List<DirectorySize>();
	ComputeChildDirectorySizes(rootNode!, currentPath, dirSizes);
	
	//
	// Part 1
	//
	
	//dirSizes.Dump();
	var sum = dirSizes
		.Where(ds => ds.Size <= 100_000L)
		.Select(ds => ds.Size)
		.Sum();
	Console.WriteLine($"Sum of directory sizes <= 100,000: {sum:N0}");
	
	//
	// Part 2
	//
	
	const long DiskCapacity = 70_000_000L;
	const long SpaceRequiredForUpdate = 30_000_000L;
	var largeDirs = dirSizes
		.OrderByDescending(ds => ds.Size)
		.Take(25)
		.ToArray();
		
	var totalUsedSpace = dirSizes.Single(ds => ds.FullPath == "/").Size;
	var availableDiskSpace = DiskCapacity - largeDirs[0].Size;
	var spaceToFree = SpaceRequiredForUpdate - availableDiskSpace;
	Console.WriteLine($"Disk capacity: {DiskCapacity:N0}");
	Console.WriteLine($"Used space: {totalUsedSpace:N0}");
	Console.WriteLine($"Free space: {availableDiskSpace:N0}");
	Console.WriteLine($"Space to free for update: {spaceToFree:N0}");
	
	//largeDirs.Dump();
	var dirsToDelete = largeDirs
		.Where(ds => ds.Size >= spaceToFree)
		.ToArray();
	dirsToDelete.Dump();
}

public static long ComputeChildDirectorySizes(Node node, Stack<string> currentPath, List<DirectorySize> dirSizes)
{
	currentPath.Push(node.Name);
	
	var directSize = 0L;
	
	foreach (var childFile in node.Children.Where(cn => cn.Type == NodeType.File))
	{
		directSize += childFile.Size;
	}
	
	foreach (var childDir in node.Children.Where(cn => cn.Type == NodeType.Directory))
	{
		directSize += ComputeChildDirectorySizes(childDir, currentPath, dirSizes);
	}

	var fullPath = currentPath.ToPath();
	//Console.WriteLine($"{fullPath}: {directSize:N0}");
	dirSizes.Add(new DirectorySize(fullPath, directSize));
	
	// We're returning to the parent directory.
	currentPath.Pop();
	
	return directSize;
}

public record DirectorySize(string FullPath, long Size);

public static class StackExtensions
{
	public static string ToPath(this Stack<string> parts)
	{
		var sb = new StringBuilder();
		
		foreach (var part in parts.Reverse())
		{
			sb.Append(part);
			if (part != "/")
			{
				sb.Append("/");
			}
		}
		
		return sb.Length == 1
			? sb.ToString()
			: sb.ToString().TrimEnd('/');
	}
}
public enum NodeType
{
	File = 1,
	Directory = 2,
}

public class Node
{
	public string Name { get; set; } = null!;
	public NodeType Type { get; set; }
	public long Size {get; set;}
	
	public Node? Parent { get; set; }
	public List<Node> Children { get; private set; } = new();
}

*)