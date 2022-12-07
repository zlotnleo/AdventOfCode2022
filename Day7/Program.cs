namespace Day7;

public interface IFileSystemItem
{
}

public record FileSystemDirectory(FileSystemDirectory? Parent, Dictionary<string, IFileSystemItem> Contents) : IFileSystemItem;

public record FileSystemFile(int Size) : IFileSystemItem;

public class Program
{
    private static FileSystemDirectory ParseFileSystem()
    {
        var commands = File.ReadAllLines("input.txt");
        var root = new FileSystemDirectory(null, new Dictionary<string, IFileSystemItem>());
        var currentDirectory = root;
        foreach (var command in commands)
        {
            switch (command.Split(' '))
            {
                case ["$", "cd", ".."]:
                    currentDirectory = currentDirectory.Parent!;
                    break;
                case ["$", "cd", "/"]:
                    currentDirectory = root;
                    break;
                case ["$", "cd", var newDirName]:
                    var newDir = new FileSystemDirectory(currentDirectory,
                        new Dictionary<string, IFileSystemItem>());
                    currentDirectory.Contents[newDirName] = newDir;
                    currentDirectory = newDir;
                    break;
                case ["$", "ls"] or ["dir", _]:
                    break;
                case [var strSize, var fileName] when int.TryParse(strSize, out var size):
                    currentDirectory.Contents[fileName] = new FileSystemFile(size);
                    break;
            }
        }

        return root;
    }

    private static List<int> GetTotalDirectorySizes(FileSystemDirectory root)
    {
        var directorySizes = new Dictionary<FileSystemDirectory, int>();

        int GetDirectorySize(FileSystemDirectory dir)
        {
            if (!directorySizes.TryGetValue(dir, out var size))
            {
                size = dir.Contents.Values.Sum(item => item switch
                {
                    FileSystemDirectory subDir => GetDirectorySize(subDir),
                    FileSystemFile(var fileSize) => fileSize,
                });
                directorySizes[dir] = size;
            }

            return size;
        }

        GetDirectorySize(root);
        return directorySizes.Values.ToList();
    }

    private static int Part1(List<int> directorySizes) =>
        directorySizes.Where(size => size <= 100000).Sum();

    private static int Part2(List<int> directorySizes)
    {
        directorySizes.Sort();

        const int maxAllowed = 70000000 - 30000000;
        var rootTotalSize = directorySizes[^1];
        var minSizeToDelete = rootTotalSize - maxAllowed;

        return directorySizes.First(x => x >= minSizeToDelete);
    }

    public static void Main()
    {
        var fileSystem = ParseFileSystem();
        var directorySizes = GetTotalDirectorySizes(fileSystem);
        Console.WriteLine(Part1(directorySizes));
        Console.WriteLine(Part2(directorySizes));
    }
}