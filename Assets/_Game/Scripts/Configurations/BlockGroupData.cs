using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BlockGroupData), menuName = "Configuration/" + nameof(BlockGroupData))]
public class BlockGroupData : ScriptableObject
{
    public int MinBlockNumber = 4;
    public int MaxBlockNumber = 6;

    public List<CustomKeyValue<BlockColorData, Sprite>> GroupSpritesByColor;

    public Sprite GetSpriteByColor(BlockColorData color) => 
        GroupSpritesByColor.FirstOrDefault(x => x.Key == color).Value;
}
