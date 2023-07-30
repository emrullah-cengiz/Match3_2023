using Assets.Scripts.Actors;
using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Logic properties

    [SerializeField] private BlockColorData _color;
    public BlockColorData Color { get => _color; set => _color = value; }

    [HideInInspector] public Vector2Int MatrixPos { get; private set; }
    [HideInInspector] public int? GroupId { get; private set; }

    #endregion

    public SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text group;

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

        transform.localPosition = BoardHelper.GetBoardPositionByMatrixPosition(matrixPos);
    }

    public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;

    public void SetMatrixPos(Vector2Int matrixPos)
    {
        MatrixPos = matrixPos;

        spriteRenderer.sortingOrder = BoardConfiguration.Instance.RowNumber - matrixPos.y;
    }

    public void SetLocalPosition(Vector2 position) => transform.localPosition = position;

    public void SetGroupId(int? id) => group.text = (GroupId = id).ToString();

    #endregion

    public void BlowUp()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        BoardManager.Instance.OnBlockClicked(this);
    }

    internal void FallToOwnPosition()
    {
        var targetPos = BoardHelper.GetBoardPositionByMatrixPosition(MatrixPos);

        transform.DOMoveY(targetPos.y, .5f);//.SetEase(Ease.OutBounce);
    }
}
