using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [Space]
    [Header("Set up")]
    [SerializeField] private ModelFruits model;
    [SerializeField] private UINextFruit uiNextFruit;

    public ModelFruits Model
    {
        get => model;
    }

    [SerializeField] private InfoFruit fruit;
    [SerializeField] private Transform objectSpawn;
    [SerializeField] private Transform objectPool;
    [SerializeField] private ParticleSystem effectMerge;
    [SerializeField] private float posValid;
    [SerializeField] private GameObject scoreLine;

    private bool canSwipe;
    private bool isDelay;
    private int indexNextFruit;

    [Space]
    [Header("Time Spawn")]
    [SerializeField] private float timeSpawn;

    [Space]
    [Header("Game State")]
    [SerializeField] private bool isLose;
    public bool IsLose
    {
        get => isLose;
        set => isLose = value;
    }

    [Space]
    [Header("Tips")]
    [SerializeField] List<InfoFruit> allFruits;
    [SerializeField] private Animator animatorBox;
    

    private void Start()
    {
        NextFruit();
        SpawnFruit();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (isUseTip1 || isUseTip2 || isUseTip3 || isUseTip4)
                    {
                        if (isUseTip1)
                        {
                            ChoiceRemoveFruit(touch.position);
                        }    
                        if (isUseTip2)
                        {
                            RemoveFruitLever1AndLever2();
                        }
                        return;
                    }    
                    OnDown(touch);
                    break;
                case TouchPhase.Moved:
                    OnMove(touch);
                    break;
                case TouchPhase.Ended:
                    OnUp();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            isUseTip1 = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            isUseTip2 = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            isUseTip3 = true;
        }
        if (isUseTip2)
        {
            RemoveFruitLever1AndLever2();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isUseTip1 || isUseTip2 || isUseTip3 || isUseTip4)
            {
                if (isUseTip1)
                {
                    ChoiceRemoveFruit(Input.mousePosition);
                }
                if (isUseTip3)
                {
                    UpLeverOneFruit(Input.mousePosition);
                }    
                return;
            }
            OnDown();
        }

        if (Input.GetMouseButton(0))
        {
            OnMove();
        }    
        if (Input.GetMouseButtonUp(0))
        {
            OnUp();
        } 
            
        if (isLose)
        {
            GameOver();
        }    
    }

    private void OnDown()
    {
        if (!isDelay)
        {
            canSwipe = true;
            MoveObject(Input.mousePosition);
            scoreLine.SetActive(true);
        }
    }

    private void OnDown(Touch touch)
    {
        if (!isDelay)
        {
            canSwipe = true;
            MoveObject(touch);
        }
    }

    private void OnMove()
    {
        if (canSwipe)
        {
            MoveObject(Input.mousePosition);
        }
    }

    private void OnMove(Touch touch)
    {
        if (canSwipe)
        {
            MoveObject(touch);
        }
    }

    private void OnUp()
    {
        if (!canSwipe) return;
        fruit.OnFall();
        scoreLine.SetActive(false);
        isDelay = true;
        canSwipe = false;
        if (fruit)
        {
            allFruits.Add(fruit);
            fruit.transform.SetParent(objectPool);
            fruit = null;
        }
        DOVirtual.DelayedCall(timeSpawn, delegate
        {
            SpawnFruit();
            isDelay = false;
        });
    }

    private void MoveObject(Vector3 position)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(position);
        if (pos.x > posValid || pos.x < -posValid) return;
        objectSpawn.transform.position = new Vector3(pos.x, objectSpawn.transform.position.y, 0f);
    }

    private void MoveObject(Touch touch)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
        if (pos.x > posValid || pos.x < -posValid) return;
        objectSpawn.transform.position = new Vector3(pos.x, objectSpawn.transform.position.y, 0f);
    }

    private void NextFruit()
    {
        indexNextFruit = Random.Range(0, model.LimitFruit);
        //UI next Fruit
        uiNextFruit.SetNextFruit(model.DataFruit[indexNextFruit].GetComponentInChildren<SpriteRenderer>().sprite);
    }

    private void SpawnFruit()
    {
        if (isLose) return;
        fruit = PoolingManager.Spawn(model.DataFruit[indexNextFruit], objectSpawn.position, Quaternion.identity, objectSpawn);
        fruit.Init(indexNextFruit, MergeFruit, GameOver);
        fruit.transform.localScale = Vector3.zero;
        fruit.transform.DOScale(Vector3.one, 0.2f);
        NextFruit();
    }

    private void MergeFruit(InfoFruit fruit1, InfoFruit fruit2, int level)
    {
        if (fruit1.IsCollider && fruit2.IsCollider) return;

        Vector3 newPosSpawn = (fruit1.transform.position + fruit2.transform.position) / 2;

        ParticleSystem effect = PoolingManager.Spawn(effectMerge, newPosSpawn, Quaternion.identity, objectPool);
        effect.Play();
        InfoFruit newFruit = PoolingManager.Spawn(model.DataFruit[level], newPosSpawn, Quaternion.identity, objectPool);
        newFruit.Init(level, MergeFruit, GameOver, true);
        PoolingManager.Despawn(fruit1.gameObject);
        PoolingManager.Despawn(fruit2.gameObject);

        if (allFruits.Contains(fruit1)) allFruits.Remove(fruit1);
        if (allFruits.Contains(fruit2)) allFruits.Remove(fruit2);


        ObserverManager<EventID>.PostEvent(EventID.UpdateScore, 10);
        DOVirtual.DelayedCall(1.2f, delegate
        {
            PoolingManager.Despawn(effect.gameObject);
        });
    }

    public void GameOver()
    {
        PoolingManager.Despawn(fruit.gameObject);
        Debug.Log("Game Over");
    }

    #region Tips

    private bool isUseTip1 = false;
    private bool isUseTip2 = false;
    private bool isUseTip3 = false;
    private bool isUseTip4 = false;

    private void RemoveFruitLever1AndLever2()
    {
        for(int i = allFruits.Count - 1; i >= 0; i--)
    {
            InfoFruit fruit = allFruits[i];
            if (fruit.Level == 0 || fruit.Level == 1)
            {
                ParticleSystem effect = PoolingManager.Spawn(effectMerge, fruit.transform.position, Quaternion.identity, objectPool);
                effect.Play();
                PoolingManager.Despawn(fruit.gameObject);
                allFruits.RemoveAt(i);
            }
        }

        isUseTip2 = false;
    }

    public void UseTipRemoveOneFruit()
    {
        isUseTip1 = true;
    }
    private void ChoiceRemoveFruit(Vector3 position)
    {
        Vector3 inputPos = Camera.main.ScreenToWorldPoint(position);
        RaycastHit2D hit = Physics2D.Raycast(inputPos, Vector2.down);
        if (hit.collider.TryGetComponent(out InfoFruit fruit1))
        {
            ParticleSystem effect = PoolingManager.Spawn(effectMerge, fruit1.transform.position, Quaternion.identity, objectPool);
            effect.Play();
            PoolingManager.Despawn(fruit1.gameObject);
            allFruits.Remove(fruit1);

            isUseTip1 = false;
        }    
    }

    private void UpLeverOneFruit(Vector3 position)
    {
        Vector3 inputPos = Camera.main.ScreenToWorldPoint(position);
        RaycastHit2D hit = Physics2D.Raycast(inputPos, Vector2.down);
        if (hit.collider.TryGetComponent(out InfoFruit fruit))
        {
            ParticleSystem effect = PoolingManager.Spawn(effectMerge, fruit.transform.position, Quaternion.identity, objectPool);
            effect.Play();
            int newLever = fruit.Level + 1;
            InfoFruit newFruit = PoolingManager.Spawn(model.DataFruit[newLever], fruit.transform.position, Quaternion.identity, objectPool);
            newFruit.Init(newLever, MergeFruit, GameOver, true);
            PoolingManager.Despawn(fruit.gameObject);

            isUseTip3 = false;
        }
    }    

    private void ShakeBox()
    {
        isUseTip4 = true;
        animatorBox.SetTrigger("ShakeBox");
        StartCoroutine(ResetTipShake(2.4f));
    }    

    IEnumerator ResetTipShake(float timeDelay)
    {
        yield return timeDelay;
        //Thực hiện điều gì đấy
        isUseTip4 = false;
    }    

    #endregion
}
