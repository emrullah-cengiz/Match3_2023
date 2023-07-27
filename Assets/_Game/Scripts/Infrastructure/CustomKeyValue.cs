using System;

[Serializable]
public class CustomKeyValue<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}
