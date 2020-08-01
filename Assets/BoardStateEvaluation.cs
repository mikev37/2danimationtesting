using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

public class BoardStateEvaluation : MonoBehaviour
{
    public List<BoardState> states;

    public BoardState temp;

    public Transition last;

    public GameObject pawn;

    private static int ready = -1000;

    
    // Start is called before the first frame update
    void Start()
    {
        ready = -1000;// true;// Pawn.selected && Pawn.selected.color;
        cache = new Dictionary<BoardState, List<BoardState>>();
        
    }

    public void transit(Transition t)
    {
        HexGenerator hxg = GameObject.FindObjectOfType<HexGenerator>();
        Pawn.selected.activate();
        last = t;
        hxg.get(t.x, t.y, t.z).OnMouseDown();
    }

    bool exists(int[,,] board, int x, int y, int z)
    {
        int side = (int)Mathf.Pow(board.Length, 1 / 3f);
        return x >= 0 && x < side &&
            y >= 0 && y < side &&
            z >= 0 && z < side &&
            board[x, y, z] == 1;
    }

    public Transition generateTransit(BoardState a, BoardState b)
    {
        Transition output = new Transition();
        PawnState current = a.pawns[a.initiative];
        //difference in initiatives should be 1 but could be 0 if the initiative got shoved back due to a pawn dying.
        //in that case init will be set to -1
        int init = b.initiative - 1;
        if (init >= b.pawns.Count)
            init = 0;
        if (init < 0)
            init = b.pawns.Count - 1;
        PawnState future = b.pawns[init];
        output.id = current.id;
        output.fx = current.x;
        output.fy = current.y;
        output.fz = current.z;
        output.x = future.x;
        output.y = future.y;
        output.z = future.z;
        return output;
    }

    int cached, generated;

    public List<BoardState> Extrapolate(BoardState bs)
    {
        lock (cache)
        {
            if (!cache.ContainsKey(bs))
            {
                List<BoardState> possibilities = new List<BoardState>();

                move(bs, possibilities, 1, -1, 0);
                move(bs, possibilities, 1, 0, -1);
                move(bs, possibilities, 0, 1, -1);
                move(bs, possibilities, -1, 1, 0);
                move(bs, possibilities, -1, 0, 1);
                move(bs, possibilities, 0, -1, 1);

                //cache[bs] = possibilities;
                //Debug.Log(bs.GetHashCode());
                cache.Add(bs, possibilities);
                //Debug.Log(cache.ContainsKey(bs));
                // Debug.Log(cache.ContainsValue(possibilities));
                //Debug.Log("uncached!");
                generated++;
                return cache[bs];
            }
            else
            {
                cached++;
                //Debug.Log("cached!");
                return cache[bs];
            }
        }
    }

    void OnGUI()
    {
        //if (Input.GetKey(KeyCode.S))
        {
            Camera.main.WorldToScreenPoint(transform.position);
            Vector2 po = (Vector2)RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
            GUIStyle syle = new GUIStyle();
            syle.fontStyle = FontStyle.Bold;
            GUI.color = Color.black;
            GUI.Label(new Rect(po.x - 15, -po.y + Screen.height - 15, 100, 20), "Score " + Grade(temp) + " Minimax " + minimax(temp,5,int.MinValue,int.MaxValue), syle);
        }
    }

    void move(BoardState bs, List<BoardState> possibilities, int x, int y, int z)
    {

        int index = bs.initiative;
        if (index < 0)
            index = bs.pawns.Count - 1;
        if (index >= bs.pawns.Count)
        {
            index = 0;
        }
        PawnState current;
        try
        {
           current = bs.pawns[index];

        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.Log(index);
            foreach (PawnState ps in bs.pawns)
                Debug.Log(ps);
            throw e;
        }
        int offset = (int)Mathf.Pow(bs.grid.Length, 1 / 3f) / 2;

        int gridx = current.x + offset;
        int gridy = current.y + offset;
        int gridz = current.z + offset;
        if (exists(bs.grid, gridx + x, gridy + y, gridz + z))
        {
            BoardState bs1 = new BoardState();

            bs1.grid = bs.grid;
            bs1.pawns = new List<PawnState>();
            bs1.pawns.AddRange(bs.pawns);
            bs1.pawns.Sort((a, b) => a.initiative - b.initiative);
            PawnState p = new PawnState();
            int found = bs1.pawns.FindIndex(pl =>
                pl.x == current.x + x &&
                pl.y == current.y + y &&
                pl.z == current.z + z &&
                pl.initiative != index);
            if(found > -1)
            {
                if (found < index)
                    index--;
                bs1.pawns.RemoveAt(found);
            }
            p.color = current.color;
            p.x = current.x + x;
            p.y = current.y + y;
            p.z = current.z + z;
            p.initiative = index;
            bs1.pawns[index] = p;
            bs1.initiative = index + 1;
            
            if (bs1.initiative >= bs1.pawns.Count)
            {
                bs1.initiative = 0;
            }
            bs1.grade = Grade(bs1);
            possibilities.Add(bs1);
        }
    }

    public int explored, pruned;

    Dictionary<BoardState, List<BoardState>> cache;
    /*
     * 
     * fun minimax(n: node, d: int): int =
   if leaf(n) or depth=0 return evaluate(n)
   if n is a max node
      v := L
      for each child of n
         v' := minimax (child,d-1)
         if v' > v, v:= v'
      return v
   if n is a min node
      v := W
      for each child of n
         v' := minimax (child,d-1)
         if v' < v, v:= v'
      return v
     */
    public int minimax(BoardState state,int depth,int min, int max)
    {
        if(depth == 0)
        {
            explored++;
            return state.grade;
        }
        if (state.pawns[state.initiative].color)
        {
            int val = min;
            foreach(BoardState m in Extrapolate(state))
            {
                val = Mathf.Max(minimax(m, depth - 1,val,max),val);
                if (val > max)
                {
                    pruned++;
                    return max;
                }
            }
            return val;
        }
        else
        {
            int val = max;
            foreach (BoardState m in Extrapolate(state))
            {
                val = Mathf.Min(minimax(m, depth - 1,min,val),val);
                if (val < min)
                {
                    pruned++;
                    return min;
                }
            }
            return val;
        }
    }

    public int Grade(BoardState bs)
    {
        int f = 0;
        int w = 0;
        int b = 0;
        foreach(PawnState po in bs.pawns)
        {
            int multiplier = 1;
            if (po.color)
            {
                b++;
            }
            else {
                multiplier = -1;
                w++;
            }

            f += 1000 * multiplier;

            f += (Math.Abs(po.x) + Math.Abs(po.y) + Math.Abs(po.z)) * -2 * multiplier;

            if(po.color)
                foreach(PawnState bo in bs.pawns)
                { 
                    if (bo.Equals(po)) continue;
                    if (po.color != bo.color && neighbors(po, bo))
                    {
                        int popri = (bs.pawns.IndexOf(po) - bs.initiative + bs.pawns.Count) % bs.pawns.Count;
                        int bopri = (bs.pawns.IndexOf(bo) - bs.initiative + bs.pawns.Count) % bs.pawns.Count;
                        int multpri = 1;
                        if (popri > bopri)
                        {
                            multpri = -1;
                        }

                        f += multpri * 100 * multiplier;
                    }
                }

                //TODO check neighborness
                //if(po)

            }
        if (b == 0)
            f -= 100000;
        if (w == 0)
            f += 100000;



        return f;
    }

    public bool neighbors(PawnState po, PawnState bo)
    {
        return Math.Sqrt(Math.Pow(po.x - bo.x,2) + Math.Pow(po.y - bo.y, 2) + Math.Pow(po.z - bo.z, 2)) < 2;
    }

    bool init = false;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            temp = save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            load(temp);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            states = Extrapolate(temp);
            states.Sort((t1, t2) => t1.grade - t2.grade); 
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            temp = save();
            List < BoardState > moves= Extrapolate(temp);
            temp = moves[Random.Range(0,moves.Count)];
            load(temp);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            temp = save();
            List<BoardState> moves = Extrapolate(temp);
            moves.Sort((t1, t2) => (int)(Grade(t2) - Grade(t1)));
            temp = moves[0];
            load(temp);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AIMove();
        }
        if (!Monitor.IsEntered(cache) && Pawn.selected && trans.id == Pawn.selected.id)
        {
            transit(trans);
            ready = -1000;
        }
        else
        {
            debugPawn = Pawn.selected;
        }
    }
    public Pawn debugPawn;

    public Transition trans;

    public void AIMove()
    {
        BoardState now = save();
        //ThreadPool.QueueUserWorkItem(() => AIPlan(now));
        new Thread(() => AIPlan(now)).Start();
        //StartCoroutine(AIPlan());

    }

    void AIPlan(BoardState now)
    {
        lock (cache)
        {
            //ready = false;
            cached = 0;
            generated = 0;
            pruned = 0;
            explored = 0;

            List<BoardState> moves = Extrapolate(temp);
            moves.Sort((t1, t2) => minimax(t2, 5, int.MinValue, int.MaxValue) - minimax(t1, 5, int.MinValue, int.MaxValue));
            states = moves;

            //load(temp);
            /*
            Debug.ClearDeveloperConsole();
            Debug.Log("cached " + cached);
            Debug.Log("generated " + generated);
            Debug.Log("pruned " + pruned);
            Debug.Log("explored " + explored);
            */
            trans = generateTransit(now, moves[0]);
            //ready = moves[0].initiative;
        }
    }

    public BoardState save()
    {
        
        BoardState bs = new BoardState();

        HexTile[,,] Grid = GameObject.FindObjectOfType<HexGenerator>().Grid;

        int wid = (int)Mathf.Pow(Grid.Length, 1 / 3f) ;

        bs.grid = new int[wid, wid, wid];

        for (int i = 0; i < wid; i++)
        {
            for (int j = 0; j < wid; j++)
            {
                for (int k = 0; k < wid; k++)
                {
                    if (Grid[i, j, k])
                    {
                        bs.grid[i, j, k] = Grid[i, j, k].type+1;
                    }
                }
            }
        }

        

        bs.initiative = GameObject.FindObjectOfType<InitiativeTracker>().index;

        bs.pawns = new List<PawnState>();

        foreach(Pawn go in GameObject.FindObjectsOfType<Pawn>())
        {
            go.save(bs);
        }
        bs.pawns.Sort((a, b) => a.initiative - b.initiative);

        bs.grade = Grade(bs);

        temp = bs;

        return bs;
    }

    public void load(BoardState bs)
    {
        HexGenerator hxg = GameObject.FindObjectOfType<HexGenerator>();

        hxg.Clear();
        hxg.load(bs);

        foreach (Pawn go in GameObject.FindObjectsOfType<Pawn>())
        {
            go.tile = null;
            Destroy(go.gameObject);
        }

        InitiativeTracker it = GameObject.FindObjectOfType<InitiativeTracker>();

        it.pwn.Clear();
        List<Pawn> range = new List<Pawn>();
        foreach (PawnState go in bs.pawns)
        {
            Pawn po = GameObject.Instantiate(pawn).GetComponent<Pawn>();
            po.load(go);
            range.Add(po);
        }

        it.Start(bs.initiative,range);
    }
}

[Serializable]
public struct BoardState
{
    public int[,,] grid;

    public int grade;

    public int initiative;

    public List<PawnState> pawns;

    public override bool Equals(object obj) {
        try
        {
            BoardState bs = (BoardState)obj;
            return Equals(bs);
        }
        catch(InvalidCastException e)
        {
            return false;
        }
       
    }

    public bool Equals(BoardState bs)
    {
        return GetHashCode() - bs.GetHashCode() == 0;
    }

    public override int GetHashCode()
    {
        int hash = 0;
        foreach (var foo in pawns)
        {
            hash = hash * 324217 + foo.GetHashCode();
        }
        hash = hash * 426383 + initiative;
        return hash;
    }
}

[Serializable]
public struct Transition
{
    public int id;
    public int x, y, z;
    public int fx, fy, fz;

    public override int GetHashCode()
    {
        return (((((id * 457 + 
            x )* 743 +
             y )* 347 +
             z )* 307 +
             fx )* 197 +
             fy  )* 367 +
             fz  * 433
            ;
    }
}

[Serializable]
public struct PawnState
{
    public int x, y, z;
    public bool color;
    public int id;
    public int initiative;

    public override int GetHashCode()
    {
        return (((((id * 107)
            + x )* 127
            +  y) * 647
            + z) * 631
            + initiative) * 457;
    }
}
