using System.Collections;
using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Pengu : MonoBehaviour
{

    [SerializeField] private Animator anim;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private Canvas popUp;
    [SerializeField] private TMP_Text popupText;

    [SerializeField] private Vector2 speed = new Vector2(3, 3);

    [Header("My monos")]
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private GameObject headIceBlock;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap iceMap;
    [SerializeField] private Tilemap borderMap;
    [SerializeField] private Tilemap waterMap;
    [SerializeField] private Tilemap collisionMap;

    private PenguDir direction = PenguDir.IDLE_DOWN;
    private bool isCaryingIce = false;
    private float timeSurvived = 0;
    private bool isDead = false;
    AudioSource squeak;
    void Start()
    {
        squeak = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        this.rigidbody.velocity = speed * input;

        var rightPressed = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        var leftPressed = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        var upPressed = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        var downPressed = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        var rightDown = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        var leftDown = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        var upDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        var downDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (rightPressed)
        {
            direction = PenguDir.RIGHT;
            anim.SetTrigger("IsRight");
            squeak.Play(0);
        }

        if (leftPressed)
        {
            direction = PenguDir.LEFT;
            anim.SetTrigger("IsLeft");
            squeak.Play(0);
        }

        if (upPressed)
        {
            direction = PenguDir.UP;
            anim.SetTrigger("IsUp");
            squeak.Play(0);
        }

        if (downPressed)
        {
            direction = PenguDir.DOWN;
            anim.SetTrigger("IsDown");
            squeak.Play(0);
        }

        if (!(rightDown || leftDown || upDown || downDown))
        {
            switch (direction)
            {
                case PenguDir.RIGHT:
                    direction = PenguDir.IDLE_RIGHT;
                    anim.SetTrigger("IsIdleRight");
                    break;
                case PenguDir.LEFT:
                    direction = PenguDir.IDLE_LEFT;
                    anim.SetTrigger("IsIdleLeft");
                    break;
                case PenguDir.UP:
                    direction = PenguDir.IDLE_UP;
                    anim.SetTrigger("IsIdleUp");
                    break;
                case PenguDir.DOWN:
                    direction = PenguDir.IDLE_DOWN;
                    anim.SetTrigger("IsIdleDown");
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryHoistOrPlaceIceBlock();
        }

        timeSurvived += Time.deltaTime;
    }

    public void KillIfOnWater(Vector3Int tilePos)
    {
        if (iceMap.WorldToCell(transform.position) == tilePos && popUp.gameObject.activeSelf == false)
        {
            anim.SetTrigger("IsPerish");
            // Game over
            Debug.Log("You lose :(");
            collisionMap.SetTile(tilePos, null);
            boxCollider.enabled = false;
            popupText.text = popupText.text + " " + string.Format("{0:F1}", timeSurvived) + "s";
            popUp.gameObject.SetActive(true);
            isDead = true;
        }
    }

    private void TryHoistOrPlaceIceBlock()
    {
        if (isCaryingIce)
        {
            // audioManager.PlaySound(Sound.INVALID_ACTION);
            TryPlaceIceBlock();
        }
        else
        {
            TryHoistIceBlock();
        }
    }

    private void TryHoistIceBlock()
    {
        var tilesInFront = GetTilesInFront();
        if (tilesInFront[0] != null)
        {
            isCaryingIce = true;
            headIceBlock.SetActive(true);

            tileManager.SetTileNonIcey(GetTileInFrontPos());
        }
        else
        {
            audioManager.PlaySound(Sound.INVALID_ACTION);
        }
    }

    private void TryPlaceIceBlock()
    {
        var tilesInFront = GetTilesInFront();
        if (tilesInFront[0] == null)
        {
            isCaryingIce = false;
            headIceBlock.SetActive(false);

            tileManager.SetTileIcey(GetTileInFrontPos());
        }
        else
        {
            audioManager.PlaySound(Sound.INVALID_ACTION);
        }
    }

    private Vector3Int GetTileInFrontPos()
    {
        Vector3Int tileInFront = iceMap.WorldToCell(transform.position);
        if (direction == PenguDir.LEFT || direction == PenguDir.IDLE_LEFT) { tileInFront += Vector3Int.left; }
        if (direction == PenguDir.RIGHT || direction == PenguDir.IDLE_RIGHT) { tileInFront += Vector3Int.right; }
        if (direction == PenguDir.UP || direction == PenguDir.IDLE_UP) { tileInFront += Vector3Int.up; }
        if (direction == PenguDir.DOWN || direction == PenguDir.IDLE_DOWN) { tileInFront += Vector3Int.down; }

        return tileInFront;
    }

    private List<TileBase> GetTilesInFront()
    {
        var tileInFrontPos = GetTileInFrontPos();
        return new List<TileBase>() {
            iceMap.GetTile<TileBase>(tileInFrontPos),
            borderMap.GetTile<TileBase>(tileInFrontPos),
            waterMap.GetTile<TileBase>(tileInFrontPos),
            collisionMap.GetTile<TileBase>(tileInFrontPos)
        };
    }
}
