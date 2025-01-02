namespace advent_of_code_2024.challenges;

internal class Day9
{
    static DiskMap ReadDiskMap()
    {
        return new DiskMap(Utils.ReadChallengeSource(9));
    }

    public static double ResolvePartOne()
    {
        return ReadDiskMap().CompactBreakingFileBlocks();
    }

    public static double ResolvePartTwo()
    {
        return ReadDiskMap().CompactWholeFileBlocks();
    }
}

interface Block { }

class EmptyBlock(int size) : Block
{
    public int Size = size;
}

class FileBlock(int id) : Block
{
    public int Id = id;

    public override string ToString() => Id.ToString();
}

class DiskMap(string source)
{
    public long CompactBreakingFileBlocks()
    {
        var blocks = IdentifyBlocks();

        var lastFileBlock = blocks.Last(b => b is FileBlock);
        var lastFileBlockIndex = blocks.IndexOf(lastFileBlock);
        var firstEmptyBlock = blocks.First(b => b is EmptyBlock);
        var firstEmptyBlockIndex = blocks.IndexOf(firstEmptyBlock);
        while (firstEmptyBlockIndex < lastFileBlockIndex)
        {
            blocks.RemoveAt(firstEmptyBlockIndex);
            blocks.Insert(firstEmptyBlockIndex, lastFileBlock);
            blocks.RemoveAt(lastFileBlockIndex);
            blocks.Insert(lastFileBlockIndex, firstEmptyBlock);

            lastFileBlock = blocks.Last(b => b is FileBlock);
            lastFileBlockIndex = blocks.IndexOf(lastFileBlock);
            firstEmptyBlock = blocks.First(b => b is EmptyBlock);
            firstEmptyBlockIndex = blocks.IndexOf(firstEmptyBlock);
        }

        return CalculateChecksum(blocks);
    }

    public long CompactWholeFileBlocks()
    {
        var blocks = IdentifyBlocks();

        var currentFileBlockToBeMovedId = ((FileBlock)blocks.Last(b => b is FileBlock)).Id;

        // PrintBlocks(blocks);

        while (currentFileBlockToBeMovedId > 0)
        {
            var fileBlocksToBeMoved = blocks.Where(block => block is FileBlock fileBlock && fileBlock.Id == currentFileBlockToBeMovedId).ToList();
            var fileBlocksToBeMovedSize = fileBlocksToBeMoved.Count;

            if (fileBlocksToBeMovedSize == 0)
            {
                currentFileBlockToBeMovedId--;
                continue;
            }

            var firstAvailableEmptyBlock = blocks.FirstOrDefault(b => b is EmptyBlock emptyBlock && emptyBlock.Size >= fileBlocksToBeMovedSize) as EmptyBlock;
            if (firstAvailableEmptyBlock == null)
            {
                currentFileBlockToBeMovedId--;
                continue;
            }

            var firstAvailableEmptyBlockIndex = blocks.IndexOf(firstAvailableEmptyBlock);
            var firstFileBeingMovedIndex = blocks.IndexOf(fileBlocksToBeMoved[0]);

            if (firstAvailableEmptyBlockIndex > firstFileBeingMovedIndex)
            {
                currentFileBlockToBeMovedId--;
                continue;
            }

            for (var i = 0; i < fileBlocksToBeMovedSize; i++)
            {
               blocks.RemoveAt(firstAvailableEmptyBlockIndex);
            }

            foreach (var fileBlockToBeMoved in fileBlocksToBeMoved)
            {
                blocks.Remove(fileBlockToBeMoved);
                blocks.Insert(firstAvailableEmptyBlockIndex, fileBlockToBeMoved);
            }

            for (var i = 0; i < fileBlocksToBeMovedSize; i++)
            {
                blocks.Insert(firstFileBeingMovedIndex, firstAvailableEmptyBlock);
            }

            var remainingBlocksFromBrokenEmptyBlocks = firstAvailableEmptyBlock.Size - fileBlocksToBeMovedSize;
            for (var i = 0; i < remainingBlocksFromBrokenEmptyBlocks; i++)
            {
                ((EmptyBlock)blocks[firstAvailableEmptyBlockIndex + (firstAvailableEmptyBlock.Size - remainingBlocksFromBrokenEmptyBlocks) + i]).Size = remainingBlocksFromBrokenEmptyBlocks;
            }

            currentFileBlockToBeMovedId--;

            // PrintBlocks(blocks);
        }

        return CalculateChecksum(blocks);
    }

    private void PrintBlocks(IList<Block> blocks)
    {
        Console.WriteLine(string.Concat(blocks.Select(b => b is FileBlock fileBlock ? fileBlock.Id.ToString() : ".")));
    }

    private long CalculateChecksum(IList<Block> blocks)
    {
        long checksum = 0;
        var blockIndex = 0;

        foreach (var block in blocks)
        {
            if (block is FileBlock fileBlock)
            {
                checksum += blockIndex * fileBlock.Id;
            }
            blockIndex++;
        }

        return checksum;
    }

    private IList<Block> IdentifyBlocks()
    {
        IList<Block> blocks = [];
        var currentId = 0;

        for (var index = 0; index < source.Length; index++)
        {
            var size = int.Parse(source[index].ToString());

            if (index % 2 == 0)
            {
                for (var i = 0; i < size; i++)
                {
                    blocks.Add(new FileBlock(currentId));
                }
                currentId++;
            }
            else
            {
                for (var i = 0; i < size; i++)
                {
                    blocks.Add(new EmptyBlock(size));
                }
            }
        }

        return blocks;
    }
}
