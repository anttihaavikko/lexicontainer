using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class TileBlock : MonoBehaviour
{
    public List<Tile> tiles;
    public LayerMask gridMask, hitMask;
    public SortingGroup sortingGroup;
    public List<float> angles;
    public Vector3 handOffset;

    private bool holding;
    private Vector3 prevPos;
    private float checkRadius = 0.4f;
    private Vector3 offset;
    private Hand theHand;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        prevPos = transform.position;
    }

    private void Update()
    {
        if(holding)
        {
            var mp = Input.mousePosition;
            mp.z = 10f;
            var mouseInWorld = cam.ScreenToWorldPoint(mp);
            
            transform.position = mouseInWorld + offset;

            HoverOut();
            CheckValidity();
            CheckBlocking();
        }
    }

    public void Setup(Hand hand, WordDictionary dict)
    {
        theHand = hand;
        var angle = angles[Random.Range(0, angles.Count)];
        handOffset = Quaternion.Euler(0, 0, angle) * handOffset;
        transform.position += handOffset;
        transform.Rotate(new Vector3(0, 0, angle));
        tiles.ForEach(tile =>
        {
            tile.transform.Rotate(new Vector3(0, 0, -angle));
            if(!tile.isDecoration) tile.SetLetter(dict.GetRandomLetter());
        });
    }

    public void Drop()
    {
        holding = false;
        SetOutlineColor(Color.white);

        var ok = CheckValidity(false) && !CheckBlocking(false);

        if(ok)
        {
            AudioManager.Instance.PlayEffectAt(Random.Range(4, 8), transform.position, 1.5f);
            
            tiles.ForEach(tile => tile.gameObject.layer = 7);
            
            var gridPos = Physics2D.OverlapCircleAll(transform.position, checkRadius, gridMask);
            var p = transform.position;
            var rounded = new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
            var signs = new Vector2(Mathf.Sign(p.x - rounded.x), Mathf.Sign(p.y - rounded.y));
            var snapPos = new Vector3(Mathf.Round(p.x) + 0.5f * signs.x, Mathf.Round(p.y) + 0.5f * signs.y, Mathf.Round(p.z));

            Tweener.Instance.MoveTo(transform, snapPos, 0.12f, 0f, TweenEasings.BounceEaseOut);

            Invoke("AfterMouseUp", 0.12f);
            return;
        }
        
        Tweener.Instance.MoveTo(transform, prevPos, 0.15f, 0f, TweenEasings.BounceEaseOut);
        this.StartCoroutine(() =>
        {
            AudioManager.Instance.PlayEffectAt(Random.Range(4, 8), transform.position, 1f);
        }, 0.15f);
    }

    private void SetOutlineColor(Color color)
    {
        tiles.ForEach(tile => tile.focus.color = color);
    }

    private void AfterMouseUp()
    {
        sortingGroup.sortingOrder = 0;
        theHand.definer.appearer.Hide();
        theHand.ClearCheckMemory();
        tiles.Where(tile => !tile.isDecoration).ToList().ForEach(tile => theHand.Check(tile));
        theHand.StartWait();
    }

    public void HoverOut()
    {
        SetOutlineColor(Color.clear);
    }

    public void HoverIn()
    {
        SetOutlineColor(Color.white);
    }

    public void Grab()
    {
        AudioManager.Instance.PlayEffectAt(Random.Range(4, 8), transform.position, 1f);
        
        holding = true;
        HoverOut();

        var mp = Input.mousePosition;
        mp.z = 10f;
        var mouseInWorld = cam.ScreenToWorldPoint(mp);

        offset = transform.position - mouseInWorld;
    }

    private bool CheckBlocking(bool color = true)
    {
        var blocked = false;
        var onGrid = 0;
        var offGrid = 0;

        tiles.ForEach(tile =>
        {
            if (tile.isDecoration)
                return;

            var gridHits = Physics2D.OverlapCircleAll(tile.transform.position, checkRadius, gridMask);
            var hits = Physics2D.OverlapCircleAll(tile.transform.position, checkRadius, hitMask);

            if (hits.Length > 0)
                blocked = true;

            if (gridHits.Length == 0)
                offGrid++;

            if (gridHits.Length > 0)
                onGrid++;
        });

        var ret = blocked || onGrid > 0 && offGrid > 0;

        if (ret && color)
            SetOutlineColor(theHand.red);

        return ret;
    }

    private bool CheckValidity(bool color = true)
    {
        var ok = true;

        tiles.ForEach(tile =>
        {
            if (tile.isDecoration)
                return;
            
            var gridHits = Physics2D.OverlapCircleAll(tile.transform.position, checkRadius, gridMask);

            if (gridHits.Length == 0)
                ok = false;
        });

        if(ok && color)
            SetOutlineColor(theHand.green);

        return ok;
    }
}
