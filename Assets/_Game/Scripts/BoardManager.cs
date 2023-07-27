using Assets.Scripts.Infrastructure;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BoardManager : SingletonMonoBehaviour<BoardManager>
{
    public Block[,] Blocks;
    public Dictionary<int, HashSet<Block>> Groups;

    bool[,] visitedBlocks;

    int groupIdCounter;

    private void Awake()
    {
        Groups = new();
    }

    private void Start()
    {
        //DetectGroups();
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

        foreach (var block in blocks)
            block.BlowUp();

        Groups.Remove(groupId);
    }

    #endregion

    #region Group detection

    public void DetectGroups()
    {
        Groups = new();

        visitedBlocks = new bool[BoardConfiguration.Instance.RowNumber,
                                 BoardConfiguration.Instance.ColumnNumber];

        for (int r = 0; r < BoardConfiguration.Instance.RowNumber; r++)
        {
            for (int c = 0; c < BoardConfiguration.Instance.ColumnNumber; c++)
            {
                if (visitedBlocks[r, c])
                    continue;

                var searchStartBlock = Blocks[r, c];

                HashSet<Block> foundedBlocks = new();

                int groupId = GetNextGroupId();

                DFSByBlock(searchStartBlock, searchStartBlock.Color, groupId, ref foundedBlocks);

                if (foundedBlocks.Count >= BoardConfiguration.Instance.MinBlockNumberForBlowUp)
                    Groups.Add(groupId, foundedBlocks);
                else
                    SetGroupIdToBlocks(foundedBlocks, null);
            }
        }

        SetGroupStyles();
    }

    private void SetGroupStyles()
    {
        foreach (var group in Groups)
        {
            var groupConfig = AssetHolder.Instance.GetGroupConfigByBlockNumber(group.Value.Count);

            if (!(groupConfig == null && group.Value.Count >= 2))
                foreach (var block in group.Value)
                    block.SetSprite(groupConfig.GetSpriteByColor(block.Color));
        }
    }


    private static void SetGroupIdToBlocks(HashSet<Block> foundedBlocks, int? groupId)
    {
        foreach (var block in foundedBlocks)
            block.SetGroupId(groupId);
    }

    /// <summary>
    /// Deep-First Searching
    /// </summary>
    public void DFSByBlock(Block block, BlockColorData color, int groupId, ref HashSet<Block> foundedBlocks)
    {
        if (visitedBlocks[block.MatrixPos.x, block.MatrixPos.y] ||
            color.Id != block.Color.Id)
            return;

        block.SetGroupId(groupId);
        foundedBlocks.Add(block);
        visitedBlocks[block.MatrixPos.x, block.MatrixPos.y] = true;

        Block neighbor;

        if (CheckHasNeighbor(block, Vector2Int.left, out neighbor))
            DFSByBlock(neighbor, color, groupId, ref foundedBlocks);
        if (CheckHasNeighbor(block, Vector2Int.up, out neighbor))
            DFSByBlock(neighbor, color, groupId, ref foundedBlocks);
        if (CheckHasNeighbor(block, Vector2Int.right, out neighbor))
            DFSByBlock(neighbor, color, groupId, ref foundedBlocks);
        if (CheckHasNeighbor(block, Vector2Int.down, out neighbor))
            DFSByBlock(neighbor, color, groupId, ref foundedBlocks);
    }

    public bool CheckHasNeighbor(Block block, Vector2Int direction, out Block neighbor)
    {
        var neighborPos = block.MatrixPos + direction;

        neighborPos.Clamp(Vector2Int.zero, new Vector2Int(BoardConfiguration.Instance.RowNumber - 1,
                                                          BoardConfiguration.Instance.ColumnNumber - 1));

        //There is no neighbor if position has not changed after clamp 
        bool hasNeighbor = block.MatrixPos != neighborPos;

        neighbor = hasNeighbor ? Blocks[neighborPos.x, neighborPos.y] : null;

        return hasNeighbor;
    }

    private int GetNextGroupId() => groupIdCounter++;
    #endregion
}
