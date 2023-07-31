using Assets.Scripts.Actors;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardInitializer : MonoBehaviour
{
    public Transform blocksParent;

    public void Awake()
    {
        CreateBoard();
    }

    public void ResetBoard()
    {
        var blocks = blocksParent.GetComponentsInChildren<Block>();
        foreach (var block in blocks)
            Destroy(block.gameObject);

        CreateBoard();
    }

    public void CreateBoard()
    {
        HashSet<Block> blocksSet = new();
        Block[,] blocks = new Block[BoardConfiguration.Instance.ColumnNumber,
                                    BoardConfiguration.Instance.RowNumber];

        for (int x = 0; x < BoardConfiguration.Instance.ColumnNumber; x++)
        {
            for (int y = 0; y < BoardConfiguration.Instance.RowNumber; y++)
            {
                var blockColor = AssetHolder.Instance.GetRandomColorData();
                var blockPrefab = AssetHolder.Instance.BlockPrefab;

                var block = Instantiate(blockPrefab, blocksParent);

                block.Setup(blockColor, new(x, y));

                blocks[x, y] = block;
                blocksSet.Add(block);
            }
        }

        BoardManager.Instance.Blocks = blocks;

        BoardManager.Instance.ClearGroups();

        BoardManager.Instance.ScanBlocksForGroupsBySpecifiedBounds(new()
        {
            MinX = 0,
            MinY = 0,
            MaxX = BoardConfiguration.Instance.ColumnNumber - 1,
            MaxY = BoardConfiguration.Instance.RowNumber - 1
        });
    }

}
