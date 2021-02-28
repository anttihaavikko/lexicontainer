using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hand : MonoBehaviour
{
    public List<TileBlock> blockPrefabs;
    public WordDictionary dict;
    public LayerMask blockMask, gridMask;
    public Score score;
    public Color green, red;
    public Transform gridFirst;
    public Appearer gameOver;
    public EffectCamera cam;
    public WordDefiner definer;
    public bool canAct = true;
    public Dude dude;

    private List<float> columnsChecked, rowsChecked;

    private List<Tile> marked;
    private int checks;
    private IEnumerator endCheck;
    private TileBlock current;

    private List<string> words;

    private void Start()
    {
        words = new List<string>();
        marked = new List<Tile>();
        columnsChecked = new List<float>();
        rowsChecked = new List<float>();

        this.StartCoroutine(() =>
        {
            if (!dude || !dude.HasSomething()) Spawn();
        }, dude ? 1f : 0f);

        // Debug.Log(PlayerPrefs.GetInt(Score.HiScoreKey));
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if(Input.GetKeyDown(KeyCode.Q)) DoEndCheck();
            if(Input.GetKeyDown(KeyCode.R)) ToScene("Main");
        }
    }

    public void ToScene(string scene)
    {
        AudioManager.Instance.targetPitch = 1f;
        SceneChanger.Instance.ChangeScene(scene);
    }

    public void Spawn()
    {
        var position = transform.position;
        var pos = canAct ? position + Vector3.right * 5f : position;
        var prefab = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
        var b = Instantiate(prefab, pos, Quaternion.identity);
        b.Setup(this, dict, position + b.handOffset);
        current = b;

        if (!canAct) return;

        var toPos = position + b.handOffset;
        Tweener.Instance.MoveTo(b.transform, toPos, 0.2f, 0f, TweenEasings.BounceEaseOut);
        AudioManager.Instance.PlayEffectAt(29, toPos, 1.804f);
        AudioManager.Instance.PlayEffectAt(9, toPos, 1.094f);

        
        Invoke(nameof(DoEndCheck), 0.5f);
    }

    private void DoEndCheck()
    {
        if (endCheck != null) StopCoroutine(endCheck);
        endCheck = CheckForEnd(current);
        StartCoroutine(endCheck);
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
        var addition = score.Add(uniques.Count);
        if (uniques.Any())
        {
            uniques.ForEach(tile => tile.Boom(green));
            this.StartCoroutine(() => cam.BaseEffect(Mathf.Min(uniques.Count * 0.05f, 10f)), Tile.boomDelay);

            var w = words.OrderBy(_ => Random.value).First();
            // Debug.Log("Random one: " + w);
            definer.DefineWord(w);

            var soundPos = uniques.First().transform.position;
            var veryNiceWords = new string[] {
                "sex", "tit", "ass", "poo", "bum", "rod", "tool", "piss", "pee", 
                "fart", "boob", "simp", "tits", "dick", "dicks", "cum", "boob", "boobs", "sexy", "tittie", "titty",
                "titties", "nut", "nuts", "porn"
            };
            dude.NiceAt(-soundPos.x, soundPos.y > 0, words.Intersect(veryNiceWords).Any());

            // AudioManager.Instance.targetPitch = 1.1f;
            // this.StartCoroutine(() => AudioManager.Instance.targetPitch = 1f, 0.75f);
        }
        marked.Clear();
        words.Clear();

        this.StartCoroutine(() =>
        {
            if (uniques.Any())
            {
                dude.ShowTutorial(Tutorial.Word);
                dude.ShowTutorial(Tutorial.MultiReset);
            }
            else
            {
                dude.ShowTutorial(Tutorial.Multiplier);
                dude.ShowTutorial(Tutorial.Three);
            }
            
            if (addition > 200)
            {
                dude.ShowTutorial(Tutorial.BigRound);
            }
            
            if (score.IsBest())
            {
                dude.ShowTutorial(Tutorial.HiScore);
            }

            if (!dude.HasSomething())
            {
                Spawn();
            }
            
        }, uniques.Any() ? 2.5f : 0.3f);
    }

    IEnumerator CheckForEnd(TileBlock block)
    {
        var points = block.tiles.Where(t => !t.isDecoration).Select(t => t.transform.position - block.handOffset - transform.position).ToList();
        var origin = gridFirst.position;
        
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                var p = origin + new Vector3(x, -y, 0);

                if(points.All(point => PointIsOk(point + p)))
                {
                    // points.ForEach(dp => Debug.DrawRay(dp + p, Vector3.up * 0.2f, Color.red, 2f));
                    // Debug.Log("Found spot at " + x + ", " + y);
                    yield break;
                }

                yield return 0;
            }
            
            yield return 0;
        }

        this.StartCoroutine(() =>
        {
            score.UploadScore();
            AudioManager.Instance.PlayEffectAt(11, Vector3.zero, 0.5f);
            gameOver.Show();
            AudioManager.Instance.targetPitch = 0.8f;
        }, 0.7f);
    }

    private bool PointIsOk(Vector3 p)
    {
        var gridHere = Physics2D.OverlapCircle(p, 0.1f, gridMask);
        var blockHere = Physics2D.OverlapCircle(p, 0.1f, blockMask);

        var ok = gridHere && !blockHere;

        return ok;
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
                        words.Add(word);
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
            // Debug.DrawRay(p, Vector3.up, Color.red, 3f);
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
