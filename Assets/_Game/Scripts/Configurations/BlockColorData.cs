using Assets.Scripts.Infrastructure;
using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BlockColorData), menuName = "Data/" + nameof(BlockColorData))]
public class BlockColorData : ScriptableObject
{
    public Sprite DefaultSprite;

    [ReadOnly] public string Id;

    private void Awake()
    {
        Id = Guid.NewGuid().ToString();
    }
}

