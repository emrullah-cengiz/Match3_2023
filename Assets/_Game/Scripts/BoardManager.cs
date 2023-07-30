using Assets.Scripts.Actors;
using Assets.Scripts.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;

public class BoardManager : SingletonMonoBehaviour<BoardManager>
{
    public BlockSpawner BlockSpawner;

    public Block[,] Blocks;
    public Dictionary<int, HashSet<Block>> Groups;

    bool[,] visitedBlocks;
    int groupIdCounter;

    static readonly IEnumerable<Vector2Int> NEIGHBORS_DIRECTIONS = new HashSet<Vector2Int>() {
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
        };

    private void Awake()
    {
        Groups = new();
    }

    private void Start()
    {
        //ScanBlocksForGroups(Blocks);
    }

    #region Blow up

    public void OnBlockClicked(Block block)
    {
        if (block.GroupId.HasValue)
            BlowUpGroup(block.GroupId.Value);
    }

    private void BlowUpGroup(int groupId)
    {
        var blocks = Groups[groupId];

        RemoveGroupRecordByBlock(groupId);

        foreach (var block in blocks)
        {
            Blocks[block.MatrixPos.x, block.MatrixPos.y] = null;
            block.BlowUp();
        }

        DropBlocksAndSpawnNewBlocksOnMatrix(blocks.Select(x => x.MatrixPos).ToHashSet(),
                                       out HashSet<Block> updatedBlocks);

        ScanBlocksForGroups(updatedBlocks);

        AnimateFallOfBlocks(updatedBlocks);

    }
    //blocksToBeFall'u matris pozisyonları ile değiştir
    public void DropBlocksAndSpawnNewBlocksOnMatrix(HashSet<Vector2Int> emptySlots, out HashSet<Block> updatedBlocks)
    {
        updatedBlocks = new();
        var groupedEmptySlotsByColumn = emptySlots.GroupBy(d => d.x);

        //Drop blocks to down 
        foreach (var column in groupedEmptySlotsByColumn)
        {
            int x = column.FirstOrDefault().x;
            int emptySlotCount = column.Count();
            int topEmptySlot = column.Min(s => s.y);//Mathf.Clamp(column.Min(s => s.y), 0, BoardConfiguration.Instance.RowNumber);
            int bottomEmptySlot = topEmptySlot + emptySlotCount - 1;

            for (int y = bottomEmptySlot; y >= 0; y--)
            {
                Block blockToFall = null;

                if (y - emptySlotCount >= 0)
                {
                    //Drop upper block down to here
                    blockToFall = Blocks[x, y - emptySlotCount];
                    RemoveGroupRecordByBlock(blockToFall.GroupId);
                }
                else
                {
                    //Spawn new blocks 
                    blockToFall = BlockSpawner.Spawn(x, y);
                    blockToFall.SetLocalPosition(BoardHelper.GetBoardPositionByMatrixPosition(new(x, y - emptySlotCount)));
                }

                Blocks[x, y] = blockToFall;
                blockToFall.SetMatrixPos(new(x, y));
                updatedBlocks.Add(blockToFall);
            }
        }
    }

    private void AnimateFallOfBlocks(HashSet<Block> blocksToBeFall)
    {
        foreach (var block in blocksToBeFall)
            block.FallToOwnPosition();
    }

    #endregion

    #region Group detection

    public void ScanBlocksForGroups(HashSet<Block> blocksToBeScan)
    {
        //Groups = new();

        visitedBlocks = new bool[BoardConfiguration.Instance.RowNumber,
                                 BoardConfiguration.Instance.ColumnNumber];

        foreach (var block in blocksToBeScan)
        {
            if (visitedBlocks[block.MatrixPos.x, block.MatrixPos.y])
                continue;

            HashSet<Block> foundedBlocks = new() { block };

            //if (block.GroupId.HasValue)
            //    Groups.Remove(block.GroupId.Value);

            int groupId = GetNextGroupId();

            DeepSearchByBlock(block, block.Color, ref groupId, ref foundedBlocks);

            if (foundedBlocks.Count >= BoardConfiguration.Instance.MinBlockNumberForBlowUp)
            {
                if (Groups.TryGetValue(groupId, out var _blocks))
                    AppendBlocksToGroup(foundedBlocks, groupId);
                else
                    AddGroup(groupId, foundedBlocks);
            }
            else
                SetGroupIdToBlocks(foundedBlocks, null);//gerek olmayablir
        }

        SetGroupStyles();
    }


    private static void SetGroupIdToBlocks(HashSet<Block> foundedBlocks, int? groupId)
    {
        foreach (var block in foundedBlocks)
            block.SetGroupId(groupId);
    }

    /// <summary>
    /// Deep-First Searching
    /// </summary>
    public void DeepSearchByBlock(Block block, BlockColorData color,
                           ref int groupId,
                           ref HashSet<Block> foundedBlocks,
                           ref HashSet<int> foundedGroupIds,
                           Block previousBlock = null)
    {
        if (visitedBlocks[block.MatrixPos.x, block.MatrixPos.y] ||
            color.Id != block.Color.Id)
            return;

        if (block.GroupId.HasValue)
        {
            //A group was found on this path, join that group directly
            groupId = block.GroupId.Value;
            return;
        }

        RemoveGroupRecordByBlock(block.GroupId);

        foundedBlocks.Add(block);

        visitedBlocks[block.MatrixPos.x, block.MatrixPos.y] = true;

        Block neighbor;

        foreach (var dir in NEIGHBORS_DIRECTIONS)
            if (CheckHasNeighbor(block, dir, out neighbor))
                DeepSearchByBlock(neighbor, color, ref groupId, ref foundedBlocks, ref foundedGroupIds, previousBlock: block);
    }

    public bool CheckHasNeighbor(Block block, Vector2Int direction, out Block neighbor)
    {
        var neighborPos = block.MatrixPos + direction;

        neighborPos.Clamp(Vector2Int.zero, new Vector2Int(BoardConfiguration.Instance.RowNumber - 1,
                                                          BoardConfiguration.Instance.ColumnNumber - 1));

        //There is no neighbor if position has not changed after clamp 
        bool hasNeighbor = block.MatrixPos != neighborPos;

        neighbor = hasNeighbor ? Blocks[neighborPos.x, neighborPos.y] : null;

        if (!neighbor && neighborPos.x != 0 && neighborPos.x != 7 && neighborPos.y != 0 && neighborPos.y != 7)
            print("Empty neighbor : " + neighborPos);

        return hasNeighbor;
    }

    //optimize et
    private void SetGroupStyles()
    {
        foreach (var group in Groups)
        {
            var groupConfig = AssetHolder.Instance.GetGroupConfigByBlockNumber(group.Value.Count);

            if (!(groupConfig == null && group.Value.Count >= 2))
                foreach (var block in group.Value)
                {
                    if (!block)
                    { }

                    block.SetSprite(groupConfig.GetSpriteByColor(block.Color));
                }
        }
    }

    public void ClearGroups() => Groups = new();

    public void AppendBlocksToGroup(HashSet<Block> blocks, int groupId)
    {
        foreach (var block in blocks)
            Groups[groupId].Add(block);

        SetGroupIdToBlocks(blocks, groupId);
    }

    public void AddGroup(int groupId, HashSet<Block> blocks)
    {
        Groups[groupId] = blocks;

        SetGroupIdToBlocks(blocks, groupId);
    }

    public void RemoveGroupRecordByBlock(int? groupId)
    {
        if (!groupId.HasValue)
            return;

        var groupBlocks = Groups[groupId.Value];

        SetGroupIdToBlocks(groupBlocks, null);

        Groups.Remove(groupId.Value);
    }

    private int GetNextGroupId() => groupIdCounter++;
    #endregion
}