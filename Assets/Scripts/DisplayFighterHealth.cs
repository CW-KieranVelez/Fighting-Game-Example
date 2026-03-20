using FightTest.Systems;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFighterHealth : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private CharacterHealth _characterHealth;

    private float _originalLength;
    
    private void Start()
    {
        _characterHealth.OnHealthChange += CharacterHealthOnOnHealthChange;
        _originalLength = _healthBar.rectTransform.rect.width;
    }

    private void OnDestroy()
    {
        _characterHealth.OnHealthChange -= CharacterHealthOnOnHealthChange;
    }

    private void CharacterHealthOnOnHealthChange(int currentHealth, int maxHealth)
    {
        _healthBar.rectTransform.sizeDelta = new Vector2(
            _originalLength * ((float)currentHealth / maxHealth),
            _healthBar.rectTransform.sizeDelta.y
        );
    }
}