using System;

namespace theorycraft
{
	public enum AbilityType {
		Melee,
		DirectHealing,
		DirectDamage,
		DoT,
		Buff,
		GroupBuff,
		GroupHealing,
		GroupDamage,
		Custom
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
		Poison,
		Aegis
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
		Constitution
	}

    public enum Slot
    {
        Held,
        Armor,
        Accessory,
        Tome
    }
}

