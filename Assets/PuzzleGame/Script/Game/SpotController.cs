using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SpotController : IDAssign
{
   /* public TextMeshProUGUI TxtCount;
    public bool Open = true;
    public Transform spotParent;

    public List<PeopleController> peoples = new List<PeopleController>();
    [HideInInspector] public List<Transform> spots = new List<Transform>();

    public int Amount = 0;
    public int RealAmount = 0;

    private void Awake()
    {
        foreach(Transform spotgroup in spotParent)
            foreach(Transform spot in spotgroup)
                spots.Add(spot);
    }

    public void AddTempo(PeopleController person)
    {
        if (ID != person.ID)
            ID = person.ID;

        Amount++;
        peoples.Add(person);

        if (Amount == GameManager.Instance.MaxSpotCount)
        {
            Open = false;
        }
    }

    public void FillIn(PeopleController person)
    {
        Transform trans = person.transform;
        Sequence tween = DOTween.Sequence()
            .SetEase(Ease.OutQuad);
        tween.Join(trans.DOMove(spots[RealAmount].position + Vector3.up * 0.1f, 0.25f * GameManager.Instance.gameSpeed));
        tween.Join(trans.DOLocalRotateQuaternion(spots[RealAmount].rotation, 0.25f * GameManager.Instance.gameSpeed));
        tween.Join(trans.DOScale(person.transform.localScale * 0.75f, 0.25f * GameManager.Instance.gameSpeed));
        tween.OnComplete(() =>
        {
            TxtCount.text = "" + (GameManager.Instance.MaxSpotCount - RealAmount);
        });
        RealAmount++;
    }

    public PeopleController GetPeople()
    {
        foreach (var person in peoples)
        {
            if (person != null)
                return person;
        }
        return null;
    }

    public void RemovePeople(PeopleController p, FillHoleController hole)
    {
        Vector3 endpos = hole.transform.position + new Vector3(Random.Range(-0.75f,0.75f), 0, Random.Range(-0.75f,0.75f));
        peoples.Remove(p);
        Amount--;
        RealAmount--;
        if (RealAmount < GameManager.Instance.MaxSpotCount && !Open)
        {
            Open = true;
        }
        if (RealAmount == 0)
        {
            ID = 0;
        }
        p.transform.DOJump(endpos, 2f, 1, 1f * GameManager.Instance.gameSpeed)
            .OnComplete(() =>
            {
                hole.FillIn(p);
                TxtCount.text = "" + (GameManager.Instance.MaxSpotCount - Amount);
            }
        );
       
    }

    private void ChangeColor()
    {

    }*/
}
