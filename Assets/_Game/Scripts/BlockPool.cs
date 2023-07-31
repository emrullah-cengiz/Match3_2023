using UnityEngine;

public class BlockPool : ObjectPool<Block>
{
    public Transform blocksParent;

    private void Awake() =>
        Setup(AssetHolder.Instance.BlockPrefab, blocksParent, BoardConfiguration.Instance.InitialPoolBlockCount);
}
