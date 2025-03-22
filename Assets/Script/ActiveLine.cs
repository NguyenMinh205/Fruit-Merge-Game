using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveLine : MonoBehaviour
{
    [SerializeField] private GameObject line;
    private Coroutine checkLine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkLine = StartCoroutine(CheckLine(2f, collision));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StopCoroutine(checkLine);
    }

    IEnumerator CheckLine (float time, Collider2D collider2D)
    {
        yield return new WaitForSeconds (time);
        if (collider2D != null && collider2D.bounds.Intersects(GetComponent<Collider2D>().bounds))
        {
            line.SetActive(true);
        }
        else
        {
            line.SetActive (false);
        } 
            
    }    
}
