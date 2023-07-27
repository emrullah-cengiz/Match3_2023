using UnityEngine;

public class Board
{
    public Block[,] Blocks { get; set; }

    public void Setup(Block[,] blocks) => Blocks = blocks;
}
