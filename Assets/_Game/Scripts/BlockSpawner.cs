using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public BlockPool BlockPool;

    private int totalColorNumber;

    private void Awake()
    {
        BlockPool = BoardManager.Instance.BlockPool;
        totalColorNumber = BoardConfiguration.Instance.BlockColorNumber;
    }

    public Block Spawn(int x, int y)
    {
        var blockColor = AssetHolder.Instance.GetRandomColorData(totalColorNumber);

        Block block = BlockPool.GetObject();

        block.Setup(blockColor, new(x, y));
        block.gameObject.SetActive(true);

        return block;
    }
}
