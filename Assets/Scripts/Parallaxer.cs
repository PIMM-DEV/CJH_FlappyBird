using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;
    public float moveSpeed;
    public int pipePerMove;

    float nextYPos;
    bool isMove = false;

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;

    PoolObject[] poolObjects;
    GameManager game;
    Transform moveTarget;

    void Awake()
    {
        Configure();
    }

    private void Start()
    {
        game = GameManager.Instance;
    }

    private void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed; 
    }


    void OnGameOverConfirmed()
    {
        for(int i =0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Update()
    {
        if (game.GameOver)
            return;

        Shift();
        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnRate * 1/(game.difficulty * 2))
        {
            Spawn();
            spawnTimer = 0;
        }

        if (isMove == true)
            Move();
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for(int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null)
            return;
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;

        if (isMove)
            return;
        int randomIndex = Random.Range(0, pipePerMove);
        if (randomIndex == 0)
        {
            moveTarget = t.GetComponent<Transform>();
            StartCoroutine(Wait());
            isMove = true;
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds((1 / game.difficulty) * 4);
        isMove = false;
    }

    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null)
            return;
        Vector3 pos = Vector3.zero;
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }

    void Move()
    {
        nextYPos += Time.deltaTime * moveSpeed;

        if (nextYPos >= ySpawnRange.max)
        {
            moveSpeed *= -1;
            nextYPos = ySpawnRange.max;
        }
        else if (nextYPos <= ySpawnRange.min)
        {
            moveSpeed *= -1;
            nextYPos = ySpawnRange.min;
        }

        moveTarget.position = new Vector3(moveTarget.position.x, nextYPos, moveTarget.position.z);
    }

    void Shift()
    {
        for(int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime * game.difficulty * 2;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if(poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect)
        {
            poolObject.Dispose();        
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for(int i = 0; i< poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
