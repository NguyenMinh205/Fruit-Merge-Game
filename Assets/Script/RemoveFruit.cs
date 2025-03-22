using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveFruit : MonoBehaviour
{
    private bool isActive = false;

    public bool IsActive
    {
        get => isActive;
    }
    private void OnMouseDown()
    {
        isActive = !isActive;
    }

    public void BreakFruit(GameObject fruit)
    {
        if (isActive)
        {
            transform.DOShakeScale(0.2f, 0.3f)
            .OnComplete(() => PoolingManager.Despawn(fruit));
        } 
    }    
}
