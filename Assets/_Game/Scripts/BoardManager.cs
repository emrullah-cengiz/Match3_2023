using Assets.Scripts.Actors;
using Assets.Scripts.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BoardManager : SingletonMonoBehaviour<BoardManager>
{
    public BlockPool BlockPool;
    public BlockSpawner BlockSpawner;
    public Block[,] Blocks { get; private set; }
    public Dictionary<int, HashSet<Block>> Groups { get; private set; }

    private Block lastDroppingBlock;
    private bool[,] visitedBlocks;
    private int groupIdCounter;


    private void Awake() => Groups = new();

    public void SetBlocks(Block[,] blocks) => Blocks = blocks;

    #region Blow up / Fall / Spawn

    public void OnBlockClicked(Block block)
    {
        if (/*lastDroppingBlock == null &&*/
            block.GroupId.HasValue)
            BlowUpGroup(block.GroupId.Value);
    }

    private void BlowUpGroup(int groupId)
    {
        var blocks = Groups[groupId];

        RemoveGroupRecordById(groupId);

        foreach (var block in blocks)
        {
            Blocks[block.MatrixPos.x, block.MatrixPos.y] = null;
            block.BlowUp();
        }

        var emptySlots = blocks.Select(x => x.MatrixPos).ToHashSet();

        var matrixBoundsForScan = BoardHelper.GetMatrixBoundsFromPositionSet(emptySlots, strechAmount: 1);

        DropBlocksAndSpawnNewBlocksOnMatrix(emptySlots, out HashSet<Block> updatedBlocks);

        ScanBlocksForGroupsBySpecifiedBounds(matrixBoundsForScan);
    }

    public void DropBlocksAndSpawnNewBlocksOnMatrix(HashSet<Vector2Int> emptySlots, out HashSet<Block> updatedBlocks)
    {
        updatedBlocks = new();

        var groupedEmptySlotsByColumn = emptySlots.GroupBy(d => d.x);

        //Drop blocks to down 
        foreach (var column in groupedEmptySlotsByColumn)
        {
            int x = column.FirstOrDefault().x;
            int emptySlotCount = column.Count();
            int bottomEmptySlotIndex = column.Select(x => x.y).Max();

            Block[] affectedColumnBlocks = new Block[bottomEmptySlotIndex + 1];
            int oldBlocksIndexer = bottomEmptySlotIndex;
            int spawnCounter = 0;

            for (int y = bottomEmptySlotIndex; y >= 0; y--)
            {
                Block block = Blocks[x, y];

                if (block != null)
                {
                    affectedColumnBlocks[oldBlocksIndexer--] = block;
                    RemoveGroupRecordById(block.GroupId);
                }
                else
                {
                    //Spawn new block
                    var newBlock = BlockSpawner.Spawn(x, spawnCounter);
                    newBlock.SetLocalPosition(BoardHelper.GetBoardPositionByMatrixPosition(new(x, spawnCounter - emptySlotCount)));

                    affectedColumnBlocks[spawnCounter++] = newBlock;
                }
            }

            for (int y = 0; y <= bottomEmptySlotIndex; y++)
            {
                Block block = affectedColumnBlocks[y];
                block.SetMatrixPos(new(x, y));
                Blocks[x, y] = block;

                block.FallToOwnPosition();
            }

            updatedBlocks.UnionWith(affectedColumnBlocks.ToHashSet());
        }
    }

    #endregion

    #region Group detection

    public void ScanBlocksForGroupsBySpecifiedBounds(MatrixBounds bounds)
    {
        visitedBlocks = new bool[BoardConfiguration.Instance.ColumnNumber, BoardConfiguration.Instance.RowNumber];

        for (int x = bounds.MinX; x <= bounds.MaxX; x++)
        {
            for (int y = bounds.MinY; y <= bounds.MaxY; y++)
            {
                if (visitedBlocks[x, y])
                    continue;

                var block = Blocks[x, y];

                HashSet<Block> foundedBlocks = new();// { block };
                HashSet<int> foundedGroupIds = new();

                DeepSearchByBlock(block, block.Color, ref foundedBlocks, ref foundedGroupIds);

                if (foundedBlocks.Count >= BoardConfiguration.Instance.MinBlockNumberForBlowUp)
                {
                    foreach (var gId in foundedGroupIds)
                    {
                        Groups.Remove(gId, out var blocks);
                        foundedBlocks.UnionWith(blocks);
                    }

                    AddGroup(GetNextGroupId(), foundedBlocks);
                }
            }
        }
    }

    public void DeepSearchByBlock(Block block, BlockColorData color,
                                  ref HashSet<Block> foundedBlocks,
                                  ref HashSet<int> foundedGroupIds,
                                  Block previousBlock = null)
    {
        if (visitedBlocks[block.MatrixPos.x, block.MatrixPos.y] ||
            color.Id != block.Color.Id)
            return;

        foundedBlocks.Add(block);

        if (block.GroupId.HasValue)
        {
            //A group was found on this path, hold groupId
            foundedGroupIds.Add(block.GroupId.Value);
            return;
        }

        visitedBlocks[block.MatrixPos.x, block.MatrixPos.y] = true;

        foreach (var dir in BoardHelper.NEIGHBOR_DIRECTIONS)
            if (CheckHasNeighbor(block, dir, out Block neighbor))
                DeepSearchByBlock(neighbor, color, ref foundedBlocks, ref foundedGroupIds, previousBlock: block);
    }

    public bool CheckHasNeighbor(Block block, Vector2Int direction, out Block neighbor)
    {
        var neighborPos = block.MatrixPos + direction;

        neighborPos.Clamp(Vector2Int.zero, new Vector2Int(BoardConfiguration.Instance.ColumnNumber - 1,
                                                          BoardConfiguration.Instance.RowNumber - 1));

        //There is no neighbor if position has not changed after clamp 
        bool hasNeighbor = block.MatrixPos != neighborPos;

        neighbor = hasNeighbor ? Blocks[neighborPos.x, neighborPos.y] : null;

        return hasNeighbor;
    }

    private void SetGroupToBlocks(HashSet<Block> foundedBlocks, int? groupId)
    {
        var groupConfig = AssetHolder.Instance.GetGroupConfigByBlockNumber(foundedBlocks.Count);

        foreach (var block in foundedBlocks)
        {
            block.SetGroupId(groupId);

            Sprite sprite;
            if (groupConfig != null && groupId.HasValue)
                sprite = groupConfig.GetSpriteByColor(block.Color);
            else
                sprite = block.Color.DefaultSprite;

            block.SetSprite(sprite);
        }
    }

    public void ClearGroups() => Groups = new();

    private void AddGroup(int groupId, HashSet<Block> blocks)
    {
        Groups[groupId] = blocks;

        SetGroupToBlocks(blocks, groupId);
    }

    private void RemoveGroupRecordById(int? groupId)
    {
        if (!groupId.HasValue)
            return;

        Groups.Remove(groupId.Value, out var groupBlocks);

        SetGroupToBlocks(groupBlocks, null);
    }

    private int GetNextGroupId() => groupIdCounter++;
    #endregion

    public void SetDroppingBlock(Block block) => lastDroppingBlock = block;
    public void BlockDropped(Block block) => lastDroppingBlock = lastDroppingBlock == block ? 
                                                                null : lastDroppingBlock;
}