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
    private  GameObject[] _pipes;

    private float _timer;

    private void Start(){
        Random.InitState((int)System.DateTime.Now.Ticks);
        SpawnPipe();
    }
    
    private void Update(){
        if (_timer > _maxTime){
            SpawnPipe();
            _timer = 0;
        }

        _timer += Time.deltaTime;

    }
    
    private void SpawnPipe(){
        Vector3 spawnPos = transform.position + new Vector3(0, Random.Range(-_heightRange, _heightRange));
        int randomPipeIndex = Random.Range(0, _pipes.Length);
        GameObject pipe = Instantiate(_pipes[randomPipeIndex], spawnPos, Quaternion.identity);
        Destroy(pipe, 15f); 
    }

}
