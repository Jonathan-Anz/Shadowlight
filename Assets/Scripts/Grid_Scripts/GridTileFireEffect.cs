using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTileFireEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _fireEffects;

    public void UpdateFireEffect(Color color)
    {
        for (int i = 0; i < _fireEffects.Length; i++)
        {
            var main = _fireEffects[i].main;
            main.startColor = color;
        }
    }

}
