using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShootingModeUI : MonoBehaviour
{
    public TMP_Text text;

    public static ShootingModeUI main;

    private void Awake()
    {
        main = this;
    }

    public void UpdateShootingMode(ShootingModes _shootingMode)
    {
        text.text = _shootingMode.ToString();
    }

}
