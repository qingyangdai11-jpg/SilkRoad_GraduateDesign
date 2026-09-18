using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Silk, RareGem, NormalGem, AdvancedGem }
    public ItemType itemType;

    public int GetValue()
    {
        switch (itemType)
        {
            case ItemType.Silk: return 8;
            case ItemType.RareGem: return 8;
            case ItemType.NormalGem: return 4;
            case ItemType.AdvancedGem: return 12;
            default: return 0;
        }
    }
}