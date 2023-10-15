using UnityEngine;

using System;

using Mirror;

//typical use case of scriptable object, this can't really be serialized.
[CreateAssetMenu(menuName = "Items/Weapon Data")]
public class Weapon : ScriptableObject
{
    [Space, TextArea(3, 10), SerializeField]
    string description = "Item Description";

    [Range(0, 1000), SerializeField]
    int value = 300;

    [Range(0, 100), Space, SerializeField]
    int damage = 20;

    [Space, SerializeField]
    GameObject model = null;

    [Space, SerializeField]
    Sprite icon = null;

    public string Name => name;
    public string Description => description;

    public int Value => value;
    public int Damage => damage;

    public GameObject Model => model;

    public Sprite Icon => icon;
}
//static container class for all weapons in our game, loaded once at game startup
public static class Weapons
{
    static Weapon[] all = null;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        all = Resources.LoadAll<Weapon>("");

        //sort once to ensure the order is the same across all clients
        Array.Sort(all);
    }

    public static Weapon Get(int index)
    {
        if (index < 0 || index >= all.Length)
            return null;

        return all[index];
    }
    //returns -1 if weapon doesn't exist in array
    public static int IndexOf(Weapon weapon)
    {
        if (weapon == null)
            return -1;

        return Array.IndexOf(all, weapon);
    }
}
//mirror provides the ability to write custom serialization
public static class WeaponSerializer
{
    public static void WriteWeapon(this NetworkWriter writer, Weapon weapon)
    {
        //we could write our entire weapon here, but that is incredibly wasteful and requires a gameobject serialization aswell.
        //instead we can just send the INDEX of our weapon in our weapon container
        int index = Weapons.IndexOf(weapon);

        writer.Write<int>(index);
    }
    public static Weapon ReadWeapon(this NetworkReader reader)
    {
        int index = reader.Read<int>();

        if(index == -1)
            return null;

        return Weapons.Get(index);
    }
}
