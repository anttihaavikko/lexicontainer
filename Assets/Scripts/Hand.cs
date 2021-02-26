using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<TileBlock> blockPrefabs;
    
    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        var prefab = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
        var b = Instantiate(prefab, transform.position, Quaternion.identity);
        b.Setup(this);
    }
}
