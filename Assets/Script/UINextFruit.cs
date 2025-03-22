using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINextFruit : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetNextFruit(Sprite fruit)
    {
        image.rectTransform.DOScale(Vector3.one, 0.2f).OnComplete(delegate
        {
            image.sprite = fruit;
            image.transform.DOScale(Vector3.one, 0.2f);
        });
    }    
}
