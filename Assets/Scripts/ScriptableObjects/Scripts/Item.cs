using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public GameObject ItemGameObject;
    public string Name;
    public string Description;
    public Sprite Image;
}
