using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoverChecker : MonoBehaviour
{
    public LayerMask blockMask;
    public TileBlock heldBlock;

    private Camera cam;
    private TileBlock hoveredBlock;
    
    private void Start()
    {
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

                if(Input.GetMouseButtonDown(0))
                {
                    heldBlock = hoveredBlock;
                    heldBlock.Grab();
                }
            }

            return;
        }
        else
        {
            if(hoveredBlock)
            {
                hoveredBlock.HoverOut();
                hoveredBlock = null;
            }
        }
    //
    //     if (defaultCharacter) return;
    //
    //     var charHits = Physics2D.OverlapCircleAll(mouseInWorld, radius, characterMask);
    //
    //     if (charHits.Length == 1 && hoveredBlock == null)
    //     {
    //         var prev = hoveredChar;
    //         hoveredChar = charHits.First().GetComponent<CharacterInfo>();
    //
    //         if (prev && hoveredChar != prev)
    //         {
    //             prev.HoverOut();
    //
    //         }
    //
    //         if (hoveredChar)
    //         {
				// hoveredChar.HoverIn();
				// ShowDefault();
    //         }
    //
    //         return;
    //     }
    //     else
    //     {
    //         if(hoveredChar)
    //         {
    //             hoveredChar.HoverOut();
    //             hoveredChar = null;
    //         }
    //     }
    }
}
