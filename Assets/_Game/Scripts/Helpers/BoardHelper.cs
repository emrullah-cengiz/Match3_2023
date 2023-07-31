using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Actors
{
    public struct MatrixBounds
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
    }

    public static class BoardHelper
    {
        public static Vector2 GetBoardPositionByMatrixPosition(Vector2Int matrixPos)
        {
            int rowNumber = BoardConfiguration.Instance.RowNumber;
            int columnNumber = BoardConfiguration.Instance.ColumnNumber;
            float blockMargin = BoardConfiguration.Instance.BlockMargin;
            float blockScale = AssetHolder.Instance.BlockPrefab.GetWidth();

            float blockWith = blockMargin + blockScale * 2;

            // For centering board
            matrixPos.x -= rowNumber / 2;
            matrixPos.y -= columnNumber / 2;

            return new Vector2(blockWith * matrixPos.x - (rowNumber % 2 == 0 ? 0 : blockScale / 2),
                              -blockWith * matrixPos.y + (columnNumber % 2 == 0 ? 0 : blockScale / 2));
        }

        public static MatrixBounds GetMatrixBoundsFromPositionSet(HashSet<Vector2Int> positions, int strechAmount = 0)
        {
            var xValues = positions.Select(x => x.x).ToHashSet();
            var yValues = positions.Select(x => x.y).ToHashSet();

            return new MatrixBounds()
            {
                MinX = Mathf.Clamp(xValues.Min() - strechAmount, 0, BoardConfiguration.Instance.ColumnNumber -1),
                MinY = 0,
                MaxX = Mathf.Clamp(xValues.Max() + strechAmount, 0, BoardConfiguration.Instance.ColumnNumber -1),
                MaxY = Mathf.Clamp(yValues.Max() + strechAmount, 0, BoardConfiguration.Instance.RowNumber -1),
            };
        }

    }
}