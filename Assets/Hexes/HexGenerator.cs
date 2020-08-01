using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGenerator : MonoBehaviour
{

    public HexTile hextile;

    public float off;

    public int width, height;

    public HexTile[,,] Grid;
    // Start is called before the first frame update

    public HexTile get(int x, int y, int z)
    {
        try
        {
            return Grid[x + width / 2, y + width / 2, z + width / 2];
        }catch(IndexOutOfRangeException e)
        {
            return null;
        }
    }

    public void load(BoardState bs)
    {
        width = (int)Mathf.Pow(bs.grid.Length, 1 / 3f);
        Grid = new HexTile[width, width, width];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    if(bs.grid[i,j,k] == 1)
                    {
                        int t = (i - (i & 1)) / 2 + k;
                        float offset = 0;
                        if (i % 2 == 0)
                            offset += hextile.display.sprite.bounds.extents.y;
                        HexTile go =
                            GameObject.Instantiate(hextile, (Vector2)transform.position + new Vector2(i * hextile.display.sprite.bounds.extents.x * 2 * 1 / 1.5f, t * hextile.display.sprite.bounds.extents.y * 2 - offset), Quaternion.identity, transform);
                        Grid[i,j,k] = go;
                        Grid[i, j, k].x = i - width / 2;
                        Grid[i, j, k].y = j - width / 2;
                        Grid[i, j, k].z = k - width / 2;
                        go.type = bs.grid[i,j,k] - 1;
                    }
                }
            }
                
        }

        neighborup();
    }

    internal void Clear()
    {
        foreach (HexTile t in GetComponentsInChildren<HexTile>())
        {
            Destroy(t.gameObject);
        }

        Grid = null;
    }

    void Awake()
    {
        Grid = new HexTile[width, width,width];
         
        for (int i = -width; i < width; i++)
        {
            for (int j = -width; j < width; j++)
            {
                HexTile go = null;
                try
                {
                    float offset = 0;
                    if (i % 2 == 0)
                        offset += hextile.display.sprite.bounds.extents.y;

                    int x = i;// i - (j + (j & 1)) / 2;
                    int z = j - (i - (i & 1)) / 2;
                    int y = -x - z;
                    go = 
                        GameObject.Instantiate(hextile, (Vector2)transform.position + new Vector2(i * hextile.display.sprite.bounds.extents.x * 2 * 1 / 1.5f, j * hextile.display.sprite.bounds.extents.y * 2 - offset), Quaternion.identity, transform);
                    Grid[x + width / 2, y + width / 2, z + width / 2] = go;
                    Grid[x + width / 2, y + width / 2, z + width / 2].x = x;
                    Grid[x + width / 2, y + width / 2, z + width / 2].y = y;
                    Grid[x + width / 2, y + width / 2, z + width / 2].z = z;
                    go.type = Math.Min(UnityEngine.Random.Range(0,3)/2,(Math.Abs(x * y + z) % 2));
                }
                catch(IndexOutOfRangeException e)
                {
                    Destroy(go.gameObject);
                }
            }
        }
        neighborup();
    }

    void neighborup()
    {
        for (int i = -width; i < width; i++)
        {
            for (int j = -width; j < width; j++)
            {
                for (int k = -width; k < width; k++)
                {
                    if (get(i, j, k))
                    {
                        if (get(i - 1, j, k + 1))
                        {
                            get(i, j, k).neighbors.Add(get(i - 1, j, k + 1));
                        }
                        if (get(i, j - 1, k + 1))
                        {
                            get(i, j, k).neighbors.Add(get(i, j - 1, k + 1));
                        }
                        if (get(i - 1, j + 1, k))
                        {
                            get(i, j, k).neighbors.Add(get(i - 1, j + 1, k));
                        }
                        if (get(i + 1, j, k - 1))
                        {
                            get(i, j, k).neighbors.Add(get(i + 1, j, k - 1));
                        }
                        if (get(i, j + 1, k - 1))
                        {
                            get(i, j, k).neighbors.Add(get(i, j + 1, k - 1));
                        }
                        if (get(i + 1, j - 1, k))
                        {
                            get(i, j, k).neighbors.Add(get(i + 1, j - 1, k));
                        }
                    }
                }
            }
        }
    }

    public delegate void withFunction(HexTile t);

    public void all(withFunction func)
    {
        for (int i = -width; i < width; i++)
        {
            for (int j = -width; j < width; j++)
            {
                for (int k = -width; k < width; k++)
                {
                    if(get(i, j, k))
                        func(get(i,j,k));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float offset = 0;
                if (j % 2 == 0)
                    offset += hextile.display.sprite.bounds.extents.x * off;
                GameObject.Instantiate(hextile, (Vector2)transform.position + new Vector2(i * hextile.display.sprite.bounds.extents.x * 2 * 1.5f + offset, j * hextile.display.sprite.bounds.extents.y), Quaternion.identity, transform);
            }
        }
        */
    }
}
