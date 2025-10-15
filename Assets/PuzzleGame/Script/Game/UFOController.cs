using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq.Expressions;

public class UFOController : MonoBehaviour
{
    public Queue<PeopleController> peoples = new Queue<PeopleController>();
    public int maxCarry = 32;
    public int assignID = 0;
    public bool Maxed = false;
    public bool Running = false;

    public SpotController spot = null;
    public FillHoleController hole = null;

    [SerializeField] private int currentCarry = 0;
    public void AddPeople(PeopleController p)
    {
        p.gameObject.transform.position = transform.position;
        peoples.Enqueue(p);
        currentCarry++;
        
        if (currentCarry >= maxCarry)
        {
            Maxed = true;
        }
    }

    public void StartAnimation(Vector3 pos)
    {
        Running = true; 
        transform.position = pos;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(Vector3.one, 0.5f * GameManager.Instance.gameSpeed);

        StartCoroutine(ReturnPeople());

    }

    public IEnumerator ReturnPeople()
    {
        yield return new WaitForSeconds(1f * GameManager.Instance.gameSpeed);
        /*while (peoples.Count > 0)
        {
            if (spot != null)
            {
                peoples.Peek().FireAnim(spot);
            }
            else if (hole != null)
            {
                peoples.Peek().FireAnim(hole);
            }
            else // Drop in the center (Indicate as lose)
            {
                peoples.Peek().FireAnim();
            }
            yield return new WaitForSeconds(0.2f * GameManager.Instance.gameSpeed);
            peoples.Dequeue();
        }
        yield return new WaitForSeconds(0.5f * GameManager.Instance.gameSpeed);
        if (peoples.Count > 0) // Run the UFO again due to delayed movement
        {
            StartCoroutine(ReturnPeople());
            yield break;
        }
        StartCoroutine(EndAnimation());*/
    }

    public IEnumerator EndAnimation()
    {
        transform.DOScale(Vector3.zero, 0.5f * GameManager.Instance.gameSpeed);
        yield return new WaitForSeconds(0.5f * GameManager.Instance.gameSpeed);

        assignID = 0;
        spot = null;
        hole = null;
        Running = false;
        Maxed = false;
        currentCarry = 0;
        peoples.Clear();

        Pooling.Despawn("UFO", gameObject);
        gameObject.SetActive(false);
    }
}
