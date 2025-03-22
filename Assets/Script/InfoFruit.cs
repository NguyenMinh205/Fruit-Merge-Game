using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InfoFruit : MonoBehaviour
{
    private int currentScore = 0;
    private int level;

    public int Level
    {
        get => level;
    }    

    [SerializeField] private Rigidbody2D rb;
    private bool isCollider;

    public bool IsCollider
    {
        get => isCollider;
    }

    private Action<InfoFruit, InfoFruit, int> onMerge;
    private Action endGame;
    private Coroutine checkOver;

    [SerializeField] private CircleCollider2D _collider2D;

    private void OnEnable()
    {
        ObserverManager<EventID>.AddRegisterEvent(EventID.UpdateScore, param => UpdateScoreFruit((int)param));
    }

    private void OnDisable()
    {
        ObserverManager<EventID>.RemoveAddListener(EventID.UpdateScore, param => UpdateScoreFruit((int)param));
    }

    private void UpdateScoreFruit(int value)
    {
        currentScore = value;
    }


    public void Init(int level, Action<InfoFruit, InfoFruit, int> actionMerge, Action endGame, bool isFall = false)
    {
        this.level = level;
        onMerge = actionMerge;
        this.endGame = endGame;
        isCollider = false;
        rb.bodyType = !isFall ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }
    public void OnFall()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out InfoFruit otherfruit))
        {
            if (level + 1 >= GameController.Instance.Model.DataFruit.Count) return;
            if (otherfruit.level == level)
            {
                onMerge?.Invoke(this, otherfruit, level + 1);
                isCollider = true;
                otherfruit.isCollider = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Line"))
        {
            checkOver = StartCoroutine(CheckGameOver(1f, collision));
        }    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Line"))
        {
            StopCoroutine(checkOver);
        }
    }

    IEnumerator CheckGameOver (float time, Collider2D collider2D)
    {
        yield return new WaitForSeconds(time);
        if (collider2D != null && collider2D.bounds.Intersects(GetComponent<Collider2D>().bounds))
        {
            GameController.Instance.IsLose = true;
        }
    }    
}
