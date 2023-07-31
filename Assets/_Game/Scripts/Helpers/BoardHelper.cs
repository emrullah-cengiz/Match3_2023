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
        public static readonly IEnumerable<Vector2Int> NEIGHBOR_DIRECTIONS = new HashSet<Vector2Int>() {
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
        };

        public static Vector2 GetBoardPositionByMatrixPosition(Vector2Int matrixPos)
        {
            int rowNumber = BoardConfiguration.Instance.RowNumber;
            int columnNumber = BoardConfiguration.Instance.ColumnNumber;
            float blockMargin = BoardConfiguration.Instance.BlockMargin;
            float blockScale = AssetHolder.Instance.BlockPrefab.GetWidth();

            float blockWith = blockMargin + blockScale * 2;

            Vector2 pos = matrixPos;

            // For centering board
            pos.x -= (float)columnNumber / 2;
            pos.y -= (float)rowNumber / 2;

            return new Vector2((blockWith * pos.x + blockScale), -(blockWith * pos.y + blockScale));
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