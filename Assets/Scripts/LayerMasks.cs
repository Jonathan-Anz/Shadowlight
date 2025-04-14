using UnityEngine;

public class LayerMasks : MonoBehaviour
{
    public static LayerMask EnemyMask = LayerMask.GetMask("Enemy");
    public static LayerMask PathMask = LayerMask.GetMask("Path");
}