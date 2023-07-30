using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public Transform blocksParent;

    public Block Spawn(int x, int y)
    {
        var blockColor = AssetHolder.Instance.GetRandomColorData();
        var blockPrefab = AssetHolder.Instance.BlockPrefab;

        var block = Instantiate(blockPrefab, blocksParent);

        block.Setup(blockColor, new(x, y));

        return block;
    }
}
