using Assets.Scripts.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AssetHolder), menuName = "AssetHolder/" + nameof(AssetHolder), order = 0)]
public class AssetHolder : SingletonScriptableObject<AssetHolder>
{
    public Block BlockPrefab;

    public List<BlockGroupData> BlockGroupConfigurations = new();

    public List<BlockColorData> BlockColors = new();

    public BlockColorData GetRandomColorData() => BlockColors[Random.Range(0, BlockColors.Count)];

    public BlockGroupData GetGroupConfigByBlockNumber(int num) =>
            BlockGroupConfigurations.FirstOrDefault(x => num >= x.MinBlockNumber &&
                                                         num <= x.MaxBlockNumber);
}
