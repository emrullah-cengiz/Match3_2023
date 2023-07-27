using Assets.Scripts.Actors;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Logic properties

    [SerializeField] private BlockColorData _color;
    public BlockColorData Color { get => _color; set => _color = value; }

    [HideInInspector] public Vector2Int MatrixPos { get; private set; }
    [HideInInspector] public int? GroupId { get; private set; }

    #endregion

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Sequence tweenSequence;

    private void Awake()
    {
        GroupId = null;
    }

    #region Set methods
    public void Setup(BlockColorData blockColorData, Vector2Int matrixPos)
    {
        Color = blockColorData;

        SetSprite(blockColorData.DefaultSprite);

        SetMatrixPos(matrixPos);

        LocateByMatrixPosition(matrixPos);
    }

    public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;

    public void SetMatrixPos(Vector2Int matrixPos)
    {
        MatrixPos = matrixPos;

        spriteRenderer.sortingOrder = BoardConfiguration.Instance.RowNumber - matrixPos.y;
    }

    public void SetGroupId(int? id) => GroupId = id;

    #endregion

    private void LocateByMatrixPosition(Vector2Int matrixPos)
    {
        //For centering board
        matrixPos.x -= BoardConfiguration.Instance.RowNumber / 2;
        matrixPos.y -= BoardConfiguration.Instance.ColumnNumber / 2;

        float blockScale = spriteRenderer.transform.localScale.x;

        transform.localPosition = new Vector2((BoardConfiguration.Instance.BlockMargin + blockScale * 2) * matrixPos.x
                                           - (BoardConfiguration.Instance.RowNumber % 2 == 0 ? 0 : blockScale / 2),

                                              -(BoardConfiguration.Instance.BlockMargin + blockScale * 2) * matrixPos.y
                                            + (BoardConfiguration.Instance.ColumnNumber % 2 == 0 ? 0 : blockScale / 2));
    }

    public void BlowUp()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        BoardManager.Instance.OnBlockClicked(this);
    }
}
