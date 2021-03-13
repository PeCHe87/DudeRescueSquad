using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DudeResqueSquad;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealthBarHUD : MonoBehaviour
{
    #region Inspector properties

    [SerializeField] private Image _imgFillProgress = null;
    [SerializeField] private TextMeshProUGUI _txtValue = null;
    [SerializeField] private GameObject _entity = null;

    #endregion

    private IDamageable _damageable = null;
    
    #region Private methods

    private void Awake()
    {
        _damageable = _entity.GetComponent<IDamageable>();
        
        // Connect with on value changed
        _damageable.PropertyChanged += UpdateValue;
    }

    private void OnDestroy()
    {
        _damageable.PropertyChanged -= UpdateValue;
    }

    private void UpdateValue(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(_damageable.Health)))
        {
            _txtValue.text = $"{Mathf.CeilToInt(_damageable.Health)}/{Mathf.CeilToInt(_damageable.MaxHealth)}";

            _imgFillProgress.fillAmount = _damageable.Health / _damageable.MaxHealth;
        }
    }

    #endregion
}
