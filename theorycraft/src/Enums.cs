using System;

namespace theorycraft
{
	public enum TraitType {
		Melee,
		DirectHealing,
		DirectDamage,
		DoT,
		Buff,
		GroupBuff,
		GroupHealing,
		GroupDamage,
		Custom,
        StatChange
	}

	public enum Sizes
	{
		Tiny,
		Small,
		Medium,
		Large,
		Huge,
		Gargantuan
	}

	public enum Effect
	{
		Poison
	}

    public enum Buff
	{
		Aegis,
		PoisonedWeapon
	}

	public enum Row
	{
		Front,
		Middle,
		Back
	}

	public enum Resist
	{
		Magic,
		Fire,
		Cold,
		Holy,
		Necrotic,
		Psionic,
		Poison
	}

	public enum Stat
	{
		Strength,
		Dexterity,
		Wisdom,
		Charisma,
		Intelligence,
		Constitution,
        AC
	}

    public enum Slot
    {
        Held,
        Armor,
        Accessory,
        Tome
    }
}

