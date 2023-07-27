using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Actors
{
    public static class BoardHelper
    {
        //public static BoardSettings BoardSettings => GameManager.Instance.BoardSettings;

        //public static Vector2 BoardTopLeftCorner => new Vector2(-BoardSettings.HorizontalTileCount / 2 * BoardSettings.TileWidth,
        //                                                         BoardSettings.VerticalTileCount / 2 * BoardSettings.TileWidth);

        //public static Vector2 GetBlockPositionByMatrixPosition(int x, int y) =>
        //    BoardTopLeftCorner + new Vector2(BoardSettings.BlockMargin + x * BoardSettings.TileWidth
        //                                  - (BoardSettings.HorizontalTileCount % 2 == 0 ? 0 : BoardSettings.TileWidth / 2),

        //                                     -BoardSettings.BlockMargin - y * BoardSettings.TileWidth
        //                                   + (BoardSettings.VerticalTileCount % 2 == 0 ? 0 : BoardSettings.TileWidth / 2));

        //public static Block[,] SetupBlockMatrix() =>
        //     new Block[BoardSettings.HorizontalTileCount,
        //              BoardSettings.VerticalTileCount];

        //public static Block GetBlockPrefabByType(BlockType blockType) =>
        //    BoardSettings.BlockPrefabs.FirstOrDefault(x => x.BlockType == blockType);

    }
}