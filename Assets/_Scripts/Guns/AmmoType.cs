using System;
[Serializable]
public class AmmoSupply
{
    public AmmoType ammoType = new AmmoType();

    public int currentAmount;

    public int maxAmount;

    public AmmoSupply()
    {
        
    }

    public void ConsumeAmmo()
    {
        if (currentAmount > 0)
            currentAmount--;
    }
    public int TakeAmmo(int _amount)
    {
        if (currentAmount < _amount)
        {
            var t = currentAmount;

            currentAmount = 0;

            return t;
        }
        else
        {
            currentAmount -= _amount;

            return _amount;
        }
            
    }
    public void AddAmmo(int _amount)
    {
        currentAmount += _amount;

        if (currentAmount > maxAmount)
        {
            currentAmount = maxAmount;
        }
    }
}


public enum AmmoType
{
    PistolAmmo,
    RifleAmmo,
    ShotgunAmmo
}


