using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class HealthBarHandler : MonoBehaviour {
    private Slider healthBar;
    [SerializeField]
    private BaseAtributesSO baseAtributes;

    void Awake()
    {
        healthBar = GetComponent<Slider>();
    }

    void Start()
    {
        healthBar.maxValue = baseAtributes.HealthPointsMax;
        healthBar.value = baseAtributes.HealthPointsMax;
    }

    public void OnEnable()
    {
        baseAtributes.OnLifeChange += OnLifeChange;
    }

    public void OnDisable()
    {
        baseAtributes.OnLifeChange -= OnLifeChange;
    }

    private void OnLifeChange(int healthPoint)
    {
        healthBar.value = healthPoint;
    }

}
