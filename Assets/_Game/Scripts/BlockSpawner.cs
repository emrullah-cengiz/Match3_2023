using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public Transform blocksParent;
    public BlockPool BlockPool;

    private void Awake()
    {
        BlockPool = BoardManager.Instance.BlockPool;
    }

    public Block Spawn(int x, int y)
    {
        var blockColor = AssetHolder.Instance.GetRandomColorData();

        Block block = BlockPool.GetObject();

        block.Setup(blockColor, new(x, y));
        block.gameObject.SetActive(true);

        return block;
    }
}
