using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class BodyGenerator : MonoBehaviour
{
    public SpriteResolver eyes;

    public SpriteResolver mouth;

    public SpriteResolver hair;

    public SpriteResolver body;

    public SpriteResolver weapons;

    public SpriteLibraryAsset library;

    // Start is called before the first frame update
    void Start()
    {
        eyes.SetCategoryAndLabel(eyes.GetCategory(), "" + Random.Range(1, 10));
        mouth.SetCategoryAndLabel(mouth.GetCategory(), "" + Random.Range(1, 10));
        hair.SetCategoryAndLabel(hair.GetCategory(), "" + Random.Range(1, 10));
        List<string> s = library.GetCategoryLabelNames(weapons.GetCategory()).ToList();
        weapons.SetCategoryAndLabel(weapons.GetCategory(),s[Random.Range(0,5)]);
        Pawn pawn = GetComponentInParent<Pawn>();
        if(pawn.color)
        {
            string label = "Thief";
            List<string> s1 = library.GetCategoryLabelNames(label).ToList();
            body.SetCategoryAndLabel(label, s1[Random.Range(0, 3)]);
        }
        else
        {
            string label = "Raider";
            List<string> s1 = library.GetCategoryLabelNames(label).ToList();
            body.SetCategoryAndLabel(label, s1[Random.Range(0, 3)]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Start();
        }
    }
}
