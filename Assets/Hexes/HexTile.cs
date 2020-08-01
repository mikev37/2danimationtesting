using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public SpriteRenderer display;
    public SpriteRenderer overlay;

    public Sprite selected;
    public Sprite attack;

    public Sprite blocked;
    public Sprite free;


    public Pawn occupier;

    public List<HexTile> neighbors;

    public int x, y, z,type;

    // Start is called before the first frame update
    void Start()
    {
       if(type == 0)
        {
            display.sprite = free;
        }
        else
        {
            display.sprite = blocked;
        }
        display.sortingOrder = -20 - z;
    }

    public void OnMouseDown()
    {
      
       HexGenerator grid = GameObject.FindObjectOfType<HexGenerator>();
       if(overlay.sprite)
        {
          
            if (overlay.sprite == selected)
            {
               
                Pawn.selected.tile.occupier = null;
                Pawn.selected.tile = this;
                /*Pawn.selected.x = x;
                Pawn.selected.y = y;
                Pawn.selected.z = z;*/
                occupier = Pawn.selected;
                Pawn.selected = null;
            }
            else if (overlay.sprite == attack)
            {
                GameObject.FindObjectOfType<InitiativeTracker>().remove(occupier);
                Animator Anim1 = occupier.GetComponentInChildren<Animator>();
                Anim1.transform.parent = null;
                Anim1.Play("Die");

                Destroy(occupier.gameObject);
                Pawn.selected.tile.occupier = null;
                Pawn.selected.tile = this;
                
                /*Pawn.selected.x = x;
                Pawn.selected.y = y;
                Pawn.selected.z = z;*/
                occupier = Pawn.selected;
                Debug.Log("HAAA");
                Debug.Log(occupier);
                Animator Anim2 = occupier.GetComponentInChildren<Animator>();

                switch (occupier.GetComponentInChildren<BodyGenerator>().weapons.GetLabel())
                {
                    case "Bow": Anim2.Play("Attack Bow"); Debug.Log("ssAA"); break;
                    case "Falchion": Anim2.Play("Attack Smasher"); Debug.Log("wwA"); break;
                    case "Shield": Anim2.Play("Attack Blocker"); Debug.Log("aaaAA"); break;
                    case "Spear": Anim2.Play("Attack Lancer"); Debug.Log("HsssAA"); break;
                    default: Anim2.Play("Attack Brawler"); Debug.Log("fffA"); break;
                }
                occupier.attacking = true;
                Debug.Log(Anim2.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

                Pawn.selected = null;
            }

            grid.all((t => t.overlay.sprite = null));

            
        }
        else
        {
            throw new NullReferenceException("" + x +","+y +"," + z);
        }






        //overlay.sprite = selected;
        /*foreach(HexTile t in neighbors)
        {
            t.overlay.sprite = selected;
        }
        /**/
        /*for(int i = 2; i <= 4; i++)
        {
            if(grid.get(x, y + i, z - i))
                grid.get(x, y + i, z - i).overlay.sprite = selected;
            if (grid.get(x + i, y - i, z))
                grid.get(x + i, y - i, z).overlay.sprite = selected;
            if(grid.get(x + i, y, z - i))
                grid.get(x + i, y, z - i).overlay.sprite = selected;
            if(grid.get(x, y - i, z + i))
                grid.get(x, y - i, z + i).overlay.sprite = selected;
            if(grid.get(x - i, y + i, z))
                grid.get(x - i, y + i, z).overlay.sprite = selected;
            if(grid.get(x - i, y, z + i))
                grid.get(x - i, y , z + i).overlay.sprite = selected;
        }
        */
        /*Debug.
        Handles.Label(transform.position, x + "," + y + "," + z);*/
        //Handles.Label(transform.position, "test");
    }

    public void Select()
    {
       
        if (!occupier)
            overlay.sprite = selected;
        else
            overlay.sprite = attack;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Camera.main.WorldToScreenPoint(transform.position);
            Vector2 po = (Vector2)RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
            GUIStyle syle = new GUIStyle();
            syle.fontStyle = FontStyle.Bold;
            GUI.color = Color.black;
            GUI.Label(new Rect(po.x - 15, -po.y + Screen.height - 15, 100, 20), x + "," + y + "," + z, syle);
        }
    }
}
