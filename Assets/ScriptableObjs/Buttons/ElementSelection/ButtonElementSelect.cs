using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonElementSelect", menuName = "Assets/CreateButtonElementSelection", order = 1)]
public class ButtonElementSelect : ScriptableObject
{
    public Material to_select;
    public AudioClip[] snd;
    public void Select()
    {
        if (GameManager._gm.material_in_hand == null)
        {
            GameManager._gm.material_in_hand = to_select;
            GameManager._gm.playRandSFX(snd);

        }
        else if (GameManager._gm.material_in_hand == to_select)
        {
            Destroy(GameManager._gm.gameobject_mat_in_hand);
            GameManager._gm.gameobject_mat_in_hand = null;
            GameManager._gm.material_in_hand = null;
            GameManager._gm.has_material_in_hand = false;
            GameManager._gm.playRandSFX(snd);
        }
    }
}
