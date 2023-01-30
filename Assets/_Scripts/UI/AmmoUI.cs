using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    public TMP_Text text;

    public static AmmoUI main;

    private void Awake()
    {
        main = this;
    }


    public void ShowAmmo(Gun _gun)
    {
        text.text = _gun.curMagSize.ToString() + "/" + Inventory.main.GetAmmoSupply(_gun.ammoType).currentAmount.ToString();
    }

}
