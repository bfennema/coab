using System.Collections.Generic;

namespace engine.Affects
{
    internal class Spells
    {
        delegate void spellDelegate();
        readonly static Dictionary<Classes.Spells, spellDelegate> spellTable = [];

        static Spells()
        {
            spellTable.Add(Classes.Spells.bless, ovr023.cleric_bless);
            spellTable.Add(Classes.Spells.curse, ovr023.cleric_curse);
            spellTable.Add(Classes.Spells.cure_light_wounds_CL, ovr023.SpellCureLight);
            spellTable.Add(Classes.Spells.cause_light_wounds_CL, ovr023.SpellCauseLight);
            spellTable.Add(Classes.Spells.detect_magic_CL, ovr023.is_affected);
            spellTable.Add(Classes.Spells.protect_from_evil_CL, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.protect_from_good_CL, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.resist_cold, ovr023.SpellResistCold);
            spellTable.Add(Classes.Spells.burning_hands, ovr023.SpellBuringHands);
            spellTable.Add(Classes.Spells.charm_person, ovr023.SpellCharm);
            spellTable.Add(Classes.Spells.detect_magic_MU, ovr023.is_affected);
            spellTable.Add(Classes.Spells.enlarge, ovr023.SpellEnlarge);
            spellTable.Add(Classes.Spells.reduce, ovr023.SpellReduce);
            spellTable.Add(Classes.Spells.friends, ovr023.SpellFriends);
            spellTable.Add(Classes.Spells.magic_missile, ovr023.SpellMagicMissile);
            spellTable.Add(Classes.Spells.protect_from_evil_MU, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.protect_from_good_MU, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.read_magic, ovr023.is_affected);
            spellTable.Add(Classes.Spells.shield, ovr023.SpellShield);
            spellTable.Add(Classes.Spells.shocking_grasp, ovr023.SpellShockingGrasp);
            spellTable.Add(Classes.Spells.sleep, ovr023.SpellSleep);
            spellTable.Add(Classes.Spells.find_traps, ovr023.is_affected);
            spellTable.Add(Classes.Spells.hold_person_CL, ovr023.SpellHoldX);
            spellTable.Add(Classes.Spells.resist_fire, ovr023.SpellFireResistant);
            spellTable.Add(Classes.Spells.silence_15_radius, ovr023.SpellSilence15Radius);
            spellTable.Add(Classes.Spells.slow_poison, ovr023.is_affected2);
            spellTable.Add(Classes.Spells.snake_charm, ovr023.SpellSnakeCharm);
            spellTable.Add(Classes.Spells.spiritual_hammer, ovr023.SpellSpiritualHammer);
            spellTable.Add(Classes.Spells.detect_invisibility, ovr023.is_affected);
            spellTable.Add(Classes.Spells.invisibility, ovr023.is_invisible);
            spellTable.Add(Classes.Spells.knock, ovr023.SpellKnock);
            spellTable.Add(Classes.Spells.mirror_image, ovr023.SpellMirrorImage);
            spellTable.Add(Classes.Spells.ray_of_enfeeblement, ovr023.SpellRayOfEnfeeblement);
            spellTable.Add(Classes.Spells.stinking_cloud, ovr023.SpellStinkingCloud);
            spellTable.Add(Classes.Spells.strength, ovr023.SpellStrength);
            spellTable.Add(Classes.Spells.animate_dead, ovr023.SpellAnimateDead);
            spellTable.Add(Classes.Spells.cure_blindness, ovr023.SpellCureBlindness);
            spellTable.Add(Classes.Spells.cause_blindness, ovr023.SpellCauseBlindness);
            spellTable.Add(Classes.Spells.cure_disease, ovr023.SpellCureDisease);
            spellTable.Add(Classes.Spells.cause_disease, ovr023.SpellCauseDisease);
            spellTable.Add(Classes.Spells.dispel_magic_CL, ovr023.SpellDispelMagic);
            spellTable.Add(Classes.Spells.prayer, ovr023.SpellPrayer);
            spellTable.Add(Classes.Spells.remove_curse_CL, ovr023.SpellRemoveCurse);
            spellTable.Add(Classes.Spells.bestow_curse_CL, ovr023.curse);
            spellTable.Add(Classes.Spells.blink, ovr023.spell_blinking);
            spellTable.Add(Classes.Spells.dispel_magic_MU, ovr023.SpellDispelMagic);
            spellTable.Add(Classes.Spells.fireball, ovr023.SpellFireball);
            spellTable.Add(Classes.Spells.haste, ovr023.cast_haste);
            spellTable.Add(Classes.Spells.hold_person_MU, ovr023.SpellHoldX);
            spellTable.Add(Classes.Spells.invisibility_10_radius, ovr023.is_invisible);
            spellTable.Add(Classes.Spells.lightning_bolt, ovr023.SpellLightningBolt);
            spellTable.Add(Classes.Spells.protect_from_evil_10_rad, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.protect_from_good_10_rad, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.protect_from_normal_missiles, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.slow, ovr023.SpellSlow);
            spellTable.Add(Classes.Spells.restoration, ovr023.SpellRestoration);
            spellTable.Add(Classes.Spells.potion_of_speed, ovr023.cast_speed);
            spellTable.Add(Classes.Spells.cure_serious_wounds_CL, ovr023.SpellCureSeriousWounds);
            spellTable.Add(Classes.Spells.potion_giant_strength, ovr023.cast_strength);
            spellTable.Add(Classes.Spells.spell_3c, ovr023.sub_6003C);
            spellTable.Add(Classes.Spells.wand_of_paralyzation, ovr023.cast_paralyzed);
            spellTable.Add(Classes.Spells.spell_3e, ovr023.cast_heal);
            spellTable.Add(Classes.Spells.dust_of_disappearance, ovr023.cast_invisible);
            spellTable.Add(Classes.Spells.necklace_of_missiles, ovr023.SpellFireball);
            spellTable.Add(Classes.Spells.wand_of_magic_missiles, ovr023.dam2d4plus2);
            spellTable.Add(Classes.Spells.cause_serious_wounds_CL, ovr023.SpellCauseSeriousWounds);
            spellTable.Add(Classes.Spells.neutralize_poison_CL, ovr023.SpellNeutralizePoison);
            spellTable.Add(Classes.Spells.poison, ovr023.SpellPoison);
            spellTable.Add(Classes.Spells.protect_evil_10_rad, ovr023.SpellProtectionFromX);
            spellTable.Add(Classes.Spells.sticks_to_snakes_CL, ovr023.SpellSticksToSnakes);
            spellTable.Add(Classes.Spells.cure_critical_wounds, ovr023.SpellCureCriticalWounds);
            spellTable.Add(Classes.Spells.cause_critical_wounds, ovr023.SpellCauseCriticalWounds);
            spellTable.Add(Classes.Spells.dispel_evil, ovr023.SpellDispelEvil);
            spellTable.Add(Classes.Spells.flame_strike, ovr023.SpellFlameStrike);
            spellTable.Add(Classes.Spells.raise_dead, ovr023.SpellRaiseDead);
            spellTable.Add(Classes.Spells.slay_living, ovr023.SpellSlayLiving);
            spellTable.Add(Classes.Spells.detect_magic_DR, ovr023.is_affected);
            spellTable.Add(Classes.Spells.entangle, ovr023.SpellEntangle);
            spellTable.Add(Classes.Spells.faerie_fire, ovr023.SpellFaerieFire);
            spellTable.Add(Classes.Spells.invisibility_to_animals, ovr023.SpellInvisToAnimals);
            spellTable.Add(Classes.Spells.charm_monsters, ovr023.SpellCharmMonsters);
            spellTable.Add(Classes.Spells.confusion, ovr023.SpellConfusion);
            spellTable.Add(Classes.Spells.dimension_door, ovr023.SpellDimensionDoor);
            spellTable.Add(Classes.Spells.fear, ovr023.SpellFear);
            spellTable.Add(Classes.Spells.fire_shield, ovr023.SpellFireProtection);
            spellTable.Add(Classes.Spells.fumble, ovr023.SpellFumble);
            spellTable.Add(Classes.Spells.ice_storm, ovr023.SpellIceStorm);
            spellTable.Add(Classes.Spells.minor_globe_of_invuln, ovr023.SpellMinorGlobeOfInvulnerability);
            spellTable.Add(Classes.Spells.remove_curse_MU, ovr023.SpellRemoveCurse);
            spellTable.Add(Classes.Spells.spell_5a, ovr023.SpellAnimateDead);
            spellTable.Add(Classes.Spells.cloud_kill, ovr023.SpellCloudKill);
            spellTable.Add(Classes.Spells.cone_of_cold, ovr023.SpellConeOfCold);
            spellTable.Add(Classes.Spells.feeblemind, ovr023.SpellFeeblemind);
            spellTable.Add(Classes.Spells.hold_monsters, ovr023.SpellHoldX);
            spellTable.Add(Classes.Spells.prot_dragon_breath, ovr023.SpellCastSpellIdAffect);
            spellTable.Add(Classes.Spells.prot_paralyzation, ovr023.SpellCastSpellIdAffect);
            spellTable.Add(Classes.Spells.potion_of_invisibility, ovr023.SpellCastSpellIdAffect);
            spellTable.Add(Classes.Spells.wand_of_defoliation, ovr023.SpellDefoliation);
            spellTable.Add(Classes.Spells.potion_extra_healing, ovr023.cast_heal2);
            spellTable.Add(Classes.Spells.bestow_curse_MU, ovr023.curse);
        }
        internal static void Setup()
        {
            Classes.gbl.cureSpell = false;
            Classes.gbl.spell_from_item = false;
            Classes.gbl.lastSelectetSpellTarget = null;
            Classes.gbl.byte_1D2C8 = true;

            Classes.gbl.SpellCastFunction = new Classes.spellDelegate(ovr023.NonCombatSpellCast);
        }
        internal static void Call(Classes.Spells spell_id)
        {
            Classes.gbl.spell_id = spell_id;
            spellDelegate func;
            if (spellTable.TryGetValue(spell_id, out func))
            {
                func();
            }
        }
    }
}
