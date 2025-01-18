using UnityEngine;




[CreateAssetMenu(fileName = "new_Order", menuName = "SO/Order", order = 0)]
public class Order : ScriptableObject
{
    public string orderName;
    public float processTime;
    public Sprite orderSprite;
    public int orderPrice;
}
