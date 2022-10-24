using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool isPlayer;
    public bool isClear;
    public bool isFirst;
    public bool isFight;
    public bool isBossRoom;
    public bool isMainRoom;
    public int enemyCount;
    public int phase = 0;

    public List<GameObject> enemySpawners_First = new List<GameObject>();
    public List<GameObject> enemySpawners_Second = new List<GameObject>();

    public GameObject[] doors;
    public Box box;

    void Awake()
    {
        isFirst = true;
        enemyCount = enemySpawners_First.Count;
    }

    
    void Update()
    {
        if (isPlayer)
        {
            // 첫 입장, 소환할 적 존재
            if (enemyCount > 0)
            {
                isClear = false;
                isFight = true;
            }
            
            else if(enemyCount < 1)
            {
                isFight = false;

                if (enemySpawners_Second.Count > 0 && phase == 1)   // 2페이즈가 있을 시
                {
                    isFight = true;
                    enemyCount = enemySpawners_Second.Count;
                    StartCoroutine(EnemySpanwer(enemySpawners_Second));
                }
            }

            // 전부 잡았을 때
            if (enemyCount < 1 && !isClear)
            {
                isFight = false;
                isClear = true;
                isFirst = false;
                RoomClearReward();
                Debug.Log(this.name + " OpenDoor");
                StartCoroutine(OpenDoor());

                GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

                for(int i = 0; i < coins.Length; i++)
                {
                    Item coin = coins[i].GetComponent<Item>();
                    coin.roomIsClear = true;
                }
            }

            if(isBossRoom && enemyCount > 0)
            {
                GameManager.Instance.boss = gameObject.GetComponentInChildren<Enemy>();
            }
        }
    }

    //방의 몹들을 전부 잡았을 때 보상
    private void RoomClearReward()
    {
        if (!isMainRoom)
        {
            int num = Random.Range(0, ItemDB.Instance.common_items.Count);
            GameObject reward = ItemDB.Instance.common_items[num];
            Debug.Log(reward.name);
            Instantiate(reward, GameManager.Instance.player.transform.position + Vector3.up, Quaternion.Euler(90,0,0));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.room = this;

            isPlayer = true;
            Debug.Log(this.name + " enter");
            if (isFirst && !isFight && !isClear && enemySpawners_First.Count > 0)
            {
                isFight = true;
                StartCoroutine(CloseDoor());
                StartCoroutine(EnemySpanwer(enemySpawners_First));
            }
        }      
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayer = false;
            Debug.Log(this.name + " exit");
        }
    }

    IEnumerator EnemySpanwer(List<GameObject> enemySpawners)
    {
        isFight = true;
        phase++;
        Debug.Log("phase++");

        for(int i = 0; i < enemySpawners.Count; i++)
        {
            enemySpawners[i].SetActive(true);
            yield return new WaitForSeconds(1f);
            
            enemySpawners[i].GetComponent<Spawner>().Spawn();
            
            GameObject enemy = enemySpawners[i].GetComponent<Spawner>().spawnEntity;
            
            enemySpawners[i].SetActive(false);
        }

    }

    IEnumerator CloseDoor()
    {
        for(int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(true);
        }

        yield return null;
    }

    IEnumerator OpenDoor()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(false);
        }

        yield return null;
    }
}
