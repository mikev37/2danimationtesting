using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Pawn : MonoBehaviour
{

    public SpriteRenderer sprite;

    public HexTile tile;
    public int x, y, z, initiative,id;

    public bool color;

    public static Pawn selected;
    internal bool attacking;


    // Start is called before the first frame update
    void Start()
    {
        HexGenerator grid = GameObject.FindObjectOfType<HexGenerator>();

        tile = grid.get(x, y, z);
        tile.occupier = this;
        transform.position =tile.transform.position;
        x =tile.x;
        y =tile.y;
        z =tile.z;
        id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        if (color)
            GetComponentInChildren<BodyGenerator>().transform.localScale *= new Vector2(-1,1);

        name += "" + initiative;
    }

    public void activate()
    {
        //selected = this;
        if (!tile)
            Start();
        HexGenerator grid = GameObject.FindObjectOfType<HexGenerator>();
        grid.all((tt => tt.overlay.sprite = null));

        foreach (HexTile tt in tile.neighbors)
        {
            if(tt.type == 0)
                tt.Select();
        }
    }

    private void OnMouseDown()
    {
        if (!selected)
        {
            
        }
        else
        {
            tile.OnMouseDown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!tile)
            Start();
        if (Vector2.Distance(transform.position,tile.transform.position) > .1f)
        {
            transform.position = Vector2.Lerp(transform.position,tile.transform.position, .1f);
            Animator anim = GetComponentInChildren<Animator>();
            if (!attacking && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                anim.Play("Walk");
            else if (attacking)
                attacking = false;
        }
        else if( x!= tile.x || y != tile.y || z!= tile.z)
        {

            x = tile.x;
            y = tile.y;
            z = tile.z;
            GameObject.FindObjectOfType<InitiativeTracker>().next();
        }
    }

    internal void save(BoardState bs)
    {
        PawnState ps = new PawnState();

        ps.x = x;
        ps.y = y;
        ps.z = z;
        ps.id = id;
        ps.color = color;
        ps.initiative = initiative;
        
        bs.pawns.Add(ps);
    }

    internal void load(PawnState ps)
    {
        x = ps.x;
        y = ps.y;
        z = ps.z;
        id = ps.id;
        initiative = ps.initiative;
        color = ps.color;
        Start();
    }
}
