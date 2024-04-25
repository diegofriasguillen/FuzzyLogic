using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyLogic
{
    //Innitial variables
    float _playerHealth;
    float _maxPlayerHealth;
    int _ammo;
    int _maxAmmo;
    float _distancePlayer;
    float _distanceAmmo;
    float _distanceHealth;
    float _health;
    float _maxHealth;

    //fuzzy variables
    private float fuzzyPlayerHealth;
    private float fuzzyAmmo;
    private float fuzzyHealth;
    private float fuzzyPlayerDistance;
    private float fuzzyAmmoDistance;
    private float fuzzyHealthDistance;

    public FuzzyLogic(float playerHealth, int ammo, float distancePlayer, float distanceAmmo, float distanceHealth, float health, float maxPlayerHealth, int maxAmmo, float maxHealth)
    {
        _playerHealth = playerHealth;
        _ammo = ammo;
        _distancePlayer = distancePlayer;
        _distanceAmmo = distanceAmmo;
        _distanceHealth = distanceHealth;
        _health = health;
        _maxPlayerHealth = maxPlayerHealth;
        _maxAmmo = maxAmmo;
        _maxHealth = maxHealth;
    }

    public void Fuzzify()
    {
        fuzzyPlayerHealth = (_playerHealth * 100) / _maxPlayerHealth;
        fuzzyAmmo = (_maxAmmo * 100) / _maxAmmo;
        fuzzyHealth = (_health * 100) / _health;

        List<float> SortedList = new List<float>();
        SortedList.Add(_distancePlayer);
        SortedList.Add(_distanceAmmo);
        SortedList.Add(_distanceHealth);
    }

    void SortList(List<float> List)
    {
        List<float> SortedList = new List<float>();
        SortedList.Add(List[0]);
        List.RemoveAt(0);
        int counter = 0;
        foreach (float var in List)
        {
            if (SortedList[0] > var)
            {
                SortedList.Add(var);
                List.Remove(var);    
            }
            counter++;
        }
    }
}
