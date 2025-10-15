using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FillHoleController : IDAssign
{
    public TextMeshPro TxtCount;
    public bool Open = true;

    public int Amount = 0;
    public int RealAmount = 0;

    public void AddTempo()
    {
        /*Amount++;
        if (Amount == GameManager.Instance.MaxHoleIntake)
        {
            Open = false;
        }*/
    }

    public void FillIn(PeopleController person)
    {
        /*RealAmount++;
        TxtCount.text = "" + (GameManager.Instance.MaxHoleIntake - RealAmount);
        person.gameObject.SetActive(false);

        if (RealAmount == GameManager.Instance.MaxHoleIntake)
        {
            EventDispatcher.Instance.SendEvent(new HoleScan { fillhole = this });
            gameObject.SetActive(false);
        } */
    }
}
