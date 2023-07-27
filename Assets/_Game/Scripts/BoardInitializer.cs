using Assets.Scripts.Actors;
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
        Block[,] blocks = new Block[BoardConfiguration.Instance.ColumnNumber,
                                    BoardConfiguration.Instance.RowNumber];

        for (int x = 0; x < BoardConfiguration.Instance.ColumnNumber; x++)
        {
            for (int y = 0; y < BoardConfiguration.Instance.RowNumber; y++)
            {
                var blockColor = AssetHolder.Instance.GetRandomColor();
                var blockPrefab = AssetHolder.Instance.BlockPrefab;

                var block = Instantiate(blockPrefab, blocksParent);

                block.Setup(blockColor, new(x, y));

                blocks[x, y] = block;
            }
        }

        BoardManager.Instance.Blocks = blocks;

        BoardManager.Instance.DetectGroups();
    }

}
