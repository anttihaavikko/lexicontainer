using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<TileBlock> blockPrefabs;
    public WordDictionary dict;
    public LayerMask blockMask;
    public Score score;
    public Color green, red;

    private List<float> columnsChecked, rowsChecked;

    private List<Tile> marked;
    private int checks;
    
    private void Start()
    {
        marked = new List<Tile>();
        columnsChecked = new List<float>();
        rowsChecked = new List<float>();
        Spawn();
    }

    public void Spawn()
    {
        var prefab = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
        var b = Instantiate(prefab, transform.position, Quaternion.identity);
        b.Setup(this, dict);
    }

    public void ClearCheckMemory()
    {
        columnsChecked.Clear();
        rowsChecked.Clear();
    }

    public void StartWait()
    {
        StartCoroutine(WaitForAll());
    }

    IEnumerator WaitForAll()
    {
        while (checks > 0) yield return 0;
        var uniques = marked.Distinct().ToList();
        Debug.Log("Clearing " + marked.Count + " (" + uniques.Count + ")");
        score.Add(uniques.Count);
        uniques.ForEach(tile => tile.Boom(green));
        marked.Clear();
    }
    
    IEnumerator CheckTrack(List<Tile> tiles)
    {
        var text = string.Join("", tiles.Select(tile => tile.GetLetter()));
        // Debug.Log("Checking " + text);
        var found = false;
        //var total = 0f;

        if (text.Length >= 3)
        {
            for (var len = text.Length; len > 2; len--)
            {
                yield return 0;

                //var watch = System.Diagnostics.Stopwatch.StartNew();

                for (var start = 0; start <= text.Length - len; start++)
                {
                    var word = text.Substring(start, len);
                    if (dict.IsWord(word))
                    {
                        yield return 0;

                        // BoomCards(start, len);
                        
                        marked.AddRange(tiles.GetRange(start, len));

                        found = true;
                        // totalWords++;

                        break;
                    }
                }

                //watch.Stop();
                //total += watch.ElapsedMilliseconds;

                yield return 0;

                if (found)
                    break;
            }
        }

        checks--;

        //Debug.Log("Took " + total);
    }

    public void Check(Tile tile)
    {
        var position = tile.transform.position;
        var x = position.x;
        var y = position.y;
        
        if (!columnsChecked.Contains(y))
        {
            CheckAxis(tile, Vector3.left, Vector3.right);
            columnsChecked.Add(y);
            checks++;
        }
        
        if (!rowsChecked.Contains(x))
        {
            CheckAxis(tile, Vector3.up, Vector3.down);
            rowsChecked.Add(x);
            checks++;
        }
    }

    private void CheckAxis(Tile tile, Vector3 back, Vector3 forward)
    {
        var before = CheckDirection(tile, back);
        before.Reverse();
        var after = CheckDirection(tile, forward);
        var stringBefore = string.Join("", before.Select(t => t.GetLetter()));
        var stringAfter = string.Join("", after.Select(t => t.GetLetter()));
        var letters = stringBefore + tile.GetLetter() + stringAfter;
        var all = new List<Tile>();
        all.AddRange(before);
        all.Add(tile);
        all.AddRange(after);
        StartCoroutine(CheckTrack(all));
    }

    private List<Tile> CheckDirection(Tile tile, Vector3 direction)
    {
        var stack = new List<Tile>();

        var edgeFound = false;
        var p = tile.transform.position;
        var safe = 0;
        
        while (!edgeFound && safe < 100)
        {
            p += direction;
            Debug.DrawRay(p, Vector3.up, Color.red, 3f);
            var go = Physics2D.OverlapCircle(p, 0.1f, blockMask);
            edgeFound = !go;
            if (go)
            {
                var t = go.GetComponent<Tile>();
                if (tile)
                {
                    stack.Add(t);
                }
            }

            safe++;
        }

        return stack;
    }
}
