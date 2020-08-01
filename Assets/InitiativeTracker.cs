using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeTracker : MonoBehaviour
{
    public List<Pawn> pwn;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        index = -1;
        pwn.AddRange(GameObject.FindObjectsOfType<Pawn>());
        pwn.Sort((a, b) => a.initiative - b.initiative);
        next();
    }

    public void Start(int dex,List<Pawn> range)
    {
        pwn.Clear();

        pwn.AddRange(range);
        pwn.Sort((a, b) => a.initiative - b.initiative);
        pwn.RemoveAll(t => t.tile == null);
        prev(dex);
    }

    public void next()
    {

        pwn.RemoveAll(t => t == null);
        index++;
        if (index >= pwn.Count)
        {
            index = 0;
        }
       

        Pawn.selected = pwn[index];

       
        

        

        GameObject.FindObjectOfType<BoardStateEvaluation>().save();
        if (Pawn.selected.color)
        {
            GameObject.FindObjectOfType<BoardStateEvaluation>().AIMove();
        }
        else
        {
            Pawn.selected.activate();
        }
    }

    public void prev(int outdex)
    {
        index = outdex;
        Debug.Log(outdex);
        if (index < 0)
            index = pwn.Count - 1;
        if (index >= pwn.Count)
        {
            index = 0;
        }
        pwn.RemoveAll(t => t == null);

        Pawn.selected = pwn[index];

        //index++;

        Pawn.selected.activate();

        
    }

    internal void remove(Pawn occupier)
    {
        int remIndex = pwn.IndexOf(occupier);
        if (remIndex < index)
            index--;
        pwn.Remove(occupier);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
