using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    public GameObject FloatingTextPrefab;
    public Vector3 mousepoint;
    public RuleTile groundTile;
    public TileBase caveTile;
    public Tilemap groundTileMap, caveTileMap;
    private GameSession gameSession;
    private OreDatabase oreDatabase;

    BoxCollider2D myBodyCollider;
    public LayerMask layer;

    public Animator mainAnimator, hammer, cover;

    float blockDestroyTime = 0.2f;
    bool isAlive = true;

    // Sprite renderers for items
    public SpriteRenderer hammerRend, gunRed;

    private void Start()
    {
        myBodyCollider = GetComponent<BoxCollider2D>();
        gameSession = FindObjectOfType<GameSession>();
        oreDatabase = FindObjectOfType<OreDatabase>();
    }

    private void Update()
    {
        if (!isAlive)
            return;
        //Set Mouse position
        mousepoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if(groundTileMap != null)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int selectedtile = groundTileMap.WorldToCell(point);
            if (Input.GetMouseButtonDown(0))
            {
                if(groundTileMap.GetTile(selectedtile) != null)
                {
                    StartCoroutine(DestroyBlock(selectedtile));
                    StartCoroutine(ResetAnimations());
                    StartCoroutine(GridScanRoutine());
                    giveMoney(selectedtile);
                }
                
            }

            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(PlacingBlock(selectedtile));
                StartCoroutine(ResetAnimations());
                StartCoroutine(GridScanRoutine());
            } 
        }
        Die();
    }


    private IEnumerator GridScanRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        yield return wait;
        AstarPath.active.Scan();
    }

    IEnumerator ResetAnimations()
    {
        yield return new WaitForSeconds(1.5f);
        mainAnimator.ResetTrigger("IsMining");
        hammer.ResetTrigger("IsMining");
        hammerRend.enabled = false;
        
    }

    IEnumerator DestroyBlock(Vector3Int pos)
    {
        // BUG: when mining blocks faster than the timer, it creates error saying object hasn't been instantiated (so far fixed by setting timer to 0f)
        yield return new WaitForSeconds(blockDestroyTime);

        // Digging block (destroying the block)
        hammerRend.enabled = true;
        mainAnimator.SetTrigger("IsMining");
        hammer.SetTrigger("IsMining");
        groundTileMap.SetTile(pos, null);
        caveTileMap.SetTile(pos, caveTile);
    }

    IEnumerator PlacingBlock(Vector3Int pos)
    {
        yield return new WaitForSeconds(0f);

        
        if (groundTileMap.GetTile(pos) == null)
        {
            hammerRend.enabled = true;
            mainAnimator.SetTrigger("IsMining");
            hammer.SetTrigger("IsMining");
            groundTileMap.SetTile(pos, groundTile);
        }
        else
        {
            Debug.Log(groundTileMap.GetTile(pos).ToString());
        }

        hammerRend.enabled = false;
    }

    void giveMoney(Vector3Int pos)
    {
        // Giving player money for ore
        string oreNameOriginal = groundTileMap.GetTile(pos).ToString();
        string[] tokens = oreNameOriginal.Split(' ');
        string oreName = tokens[0];
        //Debug.Log("Ore name = " + oreName);

        int OreValue = oreDatabase.getOreValue(oreName);
        gameSession.updateMoney(OreValue);

        Debug.Log("Prefab: " + FloatingTextPrefab);
        Debug.Log("Ore Value: " + OreValue);
        // Trigger text here
        if (FloatingTextPrefab && OreValue > 0)
        {
            ShowFloatingText(pos, OreValue);
        }
    }

    void ShowFloatingText(Vector3Int pos, int oreValue)
    {
        MeshRenderer mesh = FloatingTextPrefab.GetComponent<MeshRenderer>();
        mesh.sortingOrder = 10;
        var go =  Instantiate(FloatingTextPrefab, pos, Quaternion.identity);
        go.GetComponent<TextMesh>().text = oreValue.ToString();
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            isAlive = false;
        }
            
    }
}



