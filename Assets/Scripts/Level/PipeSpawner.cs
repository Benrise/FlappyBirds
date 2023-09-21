using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField]
    private float _maxTime = 2.5f;

    [SerializeField]
    private float _heightRange = 0;

    [SerializeField]
    private GameObject[] _pipes;

    [SerializeField]
    private int _initialPipeCount = 10;

    private float _timer;

    private bool _spawnStarted = false;

    public static PipeSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start(){
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public void StartSpawn(){
        if (!_spawnStarted){
            for (int i = 0; i < _initialPipeCount; i++){
                SpawnPipe();
                Vector3 currentPosition = transform.position;
                currentPosition.x += 2f;
                transform.position = currentPosition;
            }
            SpawnPipe();
            StartCoroutine(StartPipeSpawning());
            _spawnStarted = true;
        }
    }

    private IEnumerator StartPipeSpawning()
    {
        while (true)
        {
            yield return new WaitForSeconds(_maxTime);
            SpawnPipe();
        }
    }

    private void SpawnPipe()
    {
        Vector3 spawnPos = transform.position + new Vector3(0, Random.Range(-_heightRange, _heightRange));
        int randomPipeIndex = Random.Range(0, _pipes.Length);
        GameObject pipe = Instantiate(_pipes[randomPipeIndex], spawnPos, Quaternion.identity);
        Destroy(pipe, 60f);
    }
}
