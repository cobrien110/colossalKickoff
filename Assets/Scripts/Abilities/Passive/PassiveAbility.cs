using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbility : MonoBehaviour
{
    [Header("Basic Info")]
    public string abilityName = "NoName";
    protected float timer;
    public bool hasCooldown = false;
    public float cooldown;
    public float counterMax;
    public float counterAmount;
    protected bool isActive;

    [Header("Visuals")]
    public GameObject visualizer;

    [HideInInspector] public MonsterController MC;
    [HideInInspector] public AudioPlayer audioPlayer;
    [HideInInspector] public Animator ANIM;
    [HideInInspector] public UIManager UM;
    protected GameplayManager GM;
    protected BallProperties BP;

    void Start()
    {
        Setup();
    }

    protected void Setup()
    {
        MC = GetComponent<MonsterController>();
        audioPlayer = GetComponent<AudioPlayer>();
        ANIM = MC.GetAnimator();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        timer = cooldown;
    }

    public virtual void Deactivate()
    {
        isActive = false;
    }

    public bool GetActive()
    {
        return isActive;
    }

    public void SetActive(bool a)
    {
        isActive = a;
    }

    public void SetCounter(float x)
    {
        counterAmount = x;
    }
}