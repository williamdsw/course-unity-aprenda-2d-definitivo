using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class DadosJogadorRevisao
{
    private int languageID;
    private int numberGold;
    private int playerID;
    private int weaponID;
    private int equippedArrowID;
    private int[] arrowQuantities;
    private int[] potionQuantities;
    private List<string> inventaryItems;
    private List<int> weaponImprovements;

    // PROPERTIES

    public int LanguageID
    {
        get { return this.languageID; }
        set { this.languageID = value; }
    }

    public int NumberGold
    {
        get { return this.numberGold; }
        set { this.numberGold = value; }
    }

    public int PlayerID
    {
        get { return this.playerID; }
        set { this.playerID = value; }
    }

    public int WeaponID
    {
        get { return this.weaponID; }
        set { this.weaponID = value; }
    }

    public int EquippedArrowID
    {
        get { return this.equippedArrowID; }
        set { this.equippedArrowID = value; }
    }

    public int[] ArrowQuantities
    {
        get { return this.arrowQuantities; }
        set { this.arrowQuantities = value; }
    }

    public int[] PotionQuantities
    {
        get { return this.potionQuantities; }
        set { this.potionQuantities = value; }
    }

    public List<string> InventaryItems
    {
        get { return this.inventaryItems; }
        set { this.inventaryItems = value; }
    }

    public List<int> WeaponImprovements
    {
        get { return this.weaponImprovements; }
        set { this.weaponImprovements = value; }
    }
}