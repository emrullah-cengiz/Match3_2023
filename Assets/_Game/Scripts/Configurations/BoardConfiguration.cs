using Assets.Scripts.Infrastructure;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BoardConfiguration), menuName = "Configuration/" + nameof(BoardConfiguration), order = 0)]
public class BoardConfiguration : SingletonScriptableObject<BoardConfiguration>
{
    [Header("Board")]
    public int ColumnNumber = 10;
    public int RowNumber = 10;

    [Header("Blocks")]
    public int BlockColorNumber = 10;
    public int MinBlockNumberForBlowUp = 2;
    public float BlockDropDuration = .8f;

    [Header("Style")]
    public float BlockMargin = .1f;
}
