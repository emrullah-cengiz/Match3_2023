using Assets.Scripts.Actors;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Logic properties

    [SerializeField] private BlockColorData _color;
    public BlockColorData Color => _color;

    [HideInInspector] public Vector2Int MatrixPos { get; private set; }
    [HideInInspector] public int? GroupId { get; private set; }

    private float dropDuration;
    private int rowNumber;

    #endregion

    [SerializeField] private SpriteRenderer spriteRenderer;

    //[SerializeField] private TMP_Text txtGroupId;

    private void Awake()
    {
        rowNumber = BoardConfiguration.Instance.RowNumber;
        dropDuration = BoardConfiguration.Instance.BlockDropDuration;
    }

    public void Setup(BlockColorData blockColorData, Vector2Int matrixPos)
    {
        _color = blockColorData;

        SetSprite(blockColorData.DefaultSprite);

        SetMatrixPos(matrixPos);

        transform.localPosition = BoardHelper.GetBoardPositionByMatrixPosition(matrixPos);
    }

    private void OnMouseDown()
    {
        BoardManager.Instance.OnBlockClicked(this);
    }

    public void BlowUp()
    {
        gameObject.SetActive(false);
        BoardManager.Instance.BlockPool.SetObject(this);
    }

    public void FallToOwnPosition()
    {
        BoardManager.Instance.SetDroppingBlock(this);

        var targetPos = BoardHelper.GetBoardPositionByMatrixPosition(MatrixPos);

        transform.DOMoveY(targetPos.y, dropDuration)
                 .SetEase(Ease.OutBounce)
                 .OnComplete(() => BoardManager.Instance.BlockDropped(this));
    }


    #region Set methods

    public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;

    public void SetMatrixPos(Vector2Int matrixPos)
    {
        MatrixPos = matrixPos;
        spriteRenderer.sortingOrder = rowNumber - matrixPos.y;
    }

    public void SetLocalPosition(Vector2 position) => transform.localPosition = position;

    public void SetGroupId(int? id) => GroupId = id;

    #endregion

    public float GetWidth() => spriteRenderer.transform.localScale.x;

}
