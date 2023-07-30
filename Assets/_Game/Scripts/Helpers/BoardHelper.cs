using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Actors
{
    public static class BoardHelper
    {
        public static Vector2 GetBoardPositionByMatrixPosition(Vector2Int matrixPos)
        {
            int rowNumber = BoardConfiguration.Instance.RowNumber;
            int columnNumber = BoardConfiguration.Instance.ColumnNumber;
            float blockMargin = BoardConfiguration.Instance.BlockMargin;
            float blockScale = AssetHolder.Instance.BlockPrefab.spriteRenderer
                                                   .transform.localScale.x;

            float blockWith = blockMargin + blockScale * 2;

            // For centering board
            matrixPos.x -= rowNumber / 2;
            matrixPos.y -= columnNumber / 2;

            return new Vector2(blockWith * matrixPos.x - (rowNumber % 2 == 0 ? 0 : blockScale / 2),
                              -blockWith * matrixPos.y + (columnNumber % 2 == 0 ? 0 : blockScale / 2));
        }
    }
}