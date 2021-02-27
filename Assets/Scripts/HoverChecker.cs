using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoverChecker : MonoBehaviour
{
    public LayerMask blockMask;
    public TileBlock heldBlock;
    public Texture2D defaultCursor, handCursor, grabCursor;

    private Camera cam;
    private TileBlock hoveredBlock;
    private Vector2 hotSpot = new Vector3(32f, 32f);
    
    private void Start()
    {
        hotSpot = new Vector2(defaultCursor.width / 2f, defaultCursor.height / 2f);
        cam = Camera.main;
    }

    private void Update()
    {
        var radius = 0.3f;
        Vector3 mp = Input.mousePosition;
        mp.z = 10f;
        Vector3 mouseInWorld = cam.ScreenToWorldPoint(mp);

        if (heldBlock && Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
            heldBlock.Drop();

            var colliders = heldBlock.GetComponents<Collider2D>();
            var contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;
            contactFilter.SetLayerMask(blockMask);
            contactFilter.useLayerMask = true;

            heldBlock = null;
        }

        var blockHits = Physics2D.OverlapCircleAll(mouseInWorld, radius, blockMask);
        
        if (blockHits.Length > 0)
        {
            var prev = hoveredBlock;
            hoveredBlock = blockHits.First().GetComponentInParent<TileBlock>();

            if (prev && hoveredBlock != prev)
            {
                prev.HoverOut();
            }

            if(hoveredBlock)
            {
                hoveredBlock.HoverIn();
                
                Cursor.SetCursor(handCursor, hotSpot, CursorMode.Auto);

                if(Input.GetMouseButtonDown(0))
                {
                    heldBlock = hoveredBlock;
                    heldBlock.Grab();
                    Cursor.SetCursor(grabCursor, hotSpot, CursorMode.Auto);
                }
            }

            return;
        }
        else
        {
            if(hoveredBlock)
            {
                if(!heldBlock) Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
                hoveredBlock.HoverOut();
                hoveredBlock = null;
            }
        }
    }
}
