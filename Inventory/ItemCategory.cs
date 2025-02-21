using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory
{
    None,                // Нет категории (может использоваться для предметов, которые должны быть видны всегда)
    PrimaryWeapon,       // Основное оружие
    SecondaryWeapon,     // Вспомогательное оружие
    MeleeWeapon,         // Оружие ближнего боя
    Gadget,              // Гаджеты
    Consumable,          // Расходники
    ThrowableWeapon,     // Метательное оружие
    Material             // Материалы (не отображаются в UI, используются для крафта)
}
