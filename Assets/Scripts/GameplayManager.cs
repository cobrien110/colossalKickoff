using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public bool isPlaying = false;
    [SerializeField] private UIManager UM = null;
    [SerializeField] private GameObject Ball = null;
    [SerializeField] private WarriorController WC = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Kickoff());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlaying()
    {
        isPlaying = true;
    }

    public void StopPlaying()
    {
        isPlaying = false;
    }

    private IEnumerator Kickoff()
    {
        StartCoroutine(UM.Countdown());
        yield return new WaitForSeconds(3f);
        StartPlaying();
    }

    public void Reset()
    {
        StopPlaying();
        StartCoroutine(Kickoff());
        Instantiate(Ball, new Vector3(0, 0, 2), Quaternion.identity);
        WC.Ball = GameObject.FindGameObjectWithTag("Ball");
        WC.BP = Ball.GetComponent<BallProperties>();
    }
}