using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _pipePoints;

    [SerializeField]
    private GameObject[] _buffs;

    private void Start(){
        SpawnBuff();
    }

    private void SpawnBuff(){
        int randomPointIndex = Random.Range(0, _pipePoints.Length);
        int randomBuffIndex = Random.Range(0, _buffs.Length);

        Vector3 spawnPos = _pipePoints[randomPointIndex].transform.position;
        
        GameObject buff = Instantiate(_buffs[randomBuffIndex], spawnPos, Quaternion.identity, transform.parent);

        Destroy(buff, 15f);
    }
}
