namespace engine.Affects
{
    internal class Effect
    {
        static internal void Check(Classes.Player player, CheckType type) // work_on_00
        {
            switch (type)
            {
                case CheckType.None:
                    break;

                case CheckType.Visibility:
                    ovr024.calc_affect_effect(Classes.Affects.blink, player);
                    ovr024.calc_affect_effect(Classes.Affects.invisibility, player);
                    ovr024.calc_affect_effect(Classes.Affects.invisible, player);
                    ovr024.calc_affect_effect(Classes.Affects.invisible_to_animals, player);
                    break;

                case CheckType.PostHit1_Damage:
                    ovr024.calc_affect_effect(Classes.Affects.fireAttack_2d10, player);
                    ovr024.calc_affect_effect(Classes.Affects.ankheg_melee_acid_attack, player);
                    ovr024.calc_affect_effect(Classes.Affects.dispel_evil_banish, player);
                    ovr024.calc_affect_effect(Classes.Affects.engulf, player);
                    ovr024.calc_affect_effect(Classes.Affects.owlbear_hug_check, player);
                    ovr024.calc_affect_effect(Classes.Affects.dracolich_paralysis, player);
                    ovr024.calc_affect_effect(Classes.Affects.dracolich_cold_damage, player);
                    break;

                case CheckType.PostHit2_Damage:
                    ovr024.calc_affect_effect(Classes.Affects.poison_plus_0, player);
                    ovr024.calc_affect_effect(Classes.Affects.poison_plus_4, player);
                    ovr024.calc_affect_effect(Classes.Affects.poison_plus_2, player);
                    ovr024.calc_affect_effect(Classes.Affects.thri_kreen_paralyze, player);
                    ovr024.calc_affect_effect(Classes.Affects.poison_neg_2, player);
                    ovr024.calc_affect_effect(Classes.Affects.fireAttack_2d10, player);
                    ovr024.calc_affect_effect(Classes.Affects.beholder_eyestalk, player);
                    break;

                case CheckType.SpecialAttacks:
                    ovr024.calc_affect_effect(Classes.Affects.ray_of_enfeeblement, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_flame_tongue, player);
                    ovr024.calc_affect_effect(Classes.Affects.salamander_heat_damage, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_dragon_slayer, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_frost_brand, player);
                    ovr024.calc_affect_effect(Classes.Affects.ranger_vs_giant, player);
                    break;

                case CheckType.Type_5:
                    ovr024.calc_affect_effect(Classes.Affects.mirror_image, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_from_normal_missiles, player);
                    ovr024.calc_affect_effect(Classes.Affects.thri_kreen_dodge_missile, player);
                    ovr024.calc_affect_effect(Classes.Affects.boulder_evasion, player);
                    ovr024.calc_affect_effect(Classes.Affects.troll_regen, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_pierce_slash, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_magic_weapon, player);
                    ovr024.calc_affect_effect(Classes.Affects.protect_non_magic_weapons, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_blunt_pierce, player);
                    ovr024.calc_affect_effect(Classes.Affects.vuln_holy_water, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_normal_weapons, player);
                    ovr024.calc_affect_effect(Classes.Affects.half_damage, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_fire_and_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_piercing, player);
                    ovr024.calc_affect_effect(Classes.Affects.vuln_blessed_quarrel, player);
                    ovr024.calc_affect_effect(Classes.Affects.fire_shield_damage, player);
                    break;

                case CheckType.PreDamage:
                    ovr024.calc_affect_effect(Classes.Affects.efreeti_fire_resist, player);
                    ovr024.calc_affect_effect(Classes.Affects.item_fire_resist, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_frost_brand, player);
                    ovr024.calc_affect_effect(Classes.Affects.spell_resist_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.spell_resist_fire, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_magic_50_percent, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_magic_15_percent, player);
                    ovr024.calc_affect_effect(Classes.Affects.immune_to_fire, player);
                    ovr024.calc_affect_effect(Classes.Affects.half_elec, player);
                    ovr024.calc_affect_effect(Classes.Affects.half_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.shield, player);
                    ovr024.calc_affect_effect(Classes.Affects.half_fire, player);
                    ovr024.calc_affect_effect(Classes.Affects.troll_regen, player);
                    ovr024.calc_affect_effect(Classes.Affects.mirror_image, player);
                    ovr024.calc_affect_effect(Classes.Affects.immune_to_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_drag_breath, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_fire_and_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.shambling_absorb_lightning, player);
                    ovr024.calc_affect_effect(Classes.Affects.protect_magic, player);
                    ovr024.calc_affect_effect(Classes.Affects.dracolich_protection, player);
                    ovr024.calc_affect_effect(Classes.Affects.protect_elec, player);
                    ovr024.calc_affect_effect(Classes.Affects.minor_globe_of_invulnerability, player);
                    break;

                case CheckType.PlayerRestrained:
                    ovr024.calc_affect_effect(Classes.Affects.snake_charm, player);
                    ovr024.calc_affect_effect(Classes.Affects.paralyze, player);
                    ovr024.calc_affect_effect(Classes.Affects.sleep, player);
                    ovr024.calc_affect_effect(Classes.Affects.helpless, player);
                    ovr024.calc_affect_effect(Classes.Affects.sticks_to_snakes, player);
                    ovr024.calc_affect_effect(Classes.Affects.fumbling, player);
                    ovr024.calc_affect_effect(Classes.Affects.entangle, player);
                    break;

                case CheckType.BattleSetup1:
                    ovr024.calc_affect_effect(Classes.Affects.fight_unconscious, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_fire_and_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.displace, player);
                    ovr024.calc_affect_effect(Classes.Affects.camouflage, player);
                    ovr024.calc_affect_effect(Classes.Affects.item_invisibility, player);
                    break;

                case CheckType.MagicResistance:
                    ovr024.calc_affect_effect(Classes.Affects.resist_magic_50_percent, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_magic_15_percent, player);
                    ovr024.calc_affect_effect(Classes.Affects.elf_resist_sleep, player);
                    ovr024.calc_affect_effect(Classes.Affects.protect_charm_sleep, player);
                    ovr024.calc_affect_effect(Classes.Affects.resist_paralyze, player);
                    ovr024.calc_affect_effect(Classes.Affects.immune_to_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_paralysis_poison, player);
                    ovr024.calc_affect_effect(Classes.Affects.immune_to_fire, player);
                    ovr024.calc_affect_effect(Classes.Affects.halfelf_resistance, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_sleep_charm_paralysis_poison, player);
                    ovr024.calc_affect_effect(Classes.Affects.minor_globe_of_invulnerability, player);
                    ovr024.calc_affect_effect(Classes.Affects.protect_magic, player);
                    break;

                case CheckType.CanHitAttacker:
                    ovr024.calc_affect_effect(Classes.Affects.bless, player);
                    ovr024.calc_affect_effect(Classes.Affects.cursed, player);
                    ovr024.calc_affect_effect(Classes.Affects.blinded, player);
                    ovr024.calc_affect_effect(Classes.Affects.bestow_curse, player);
                    ovr024.calc_affect_effect(Classes.Affects.prayer, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_flame_tongue, player);
                    ovr024.calc_affect_effect(Classes.Affects.gnome_vs_goblin_kobold, player);
                    ovr024.calc_affect_effect(Classes.Affects.dwarf_vs_orc_goblin, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_dragon_slayer, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_frost_brand, player);
                    break;

                case CheckType.Type_11:
                    ovr024.calc_affect_effect(Classes.Affects.blinded, player);
                    ovr024.calc_affect_effect(Classes.Affects.shield, player);
                    ovr024.calc_affect_effect(Classes.Affects.protection_from_evil, player);
                    ovr024.calc_affect_effect(Classes.Affects.protection_from_good, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_from_evil_10_radius, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_from_good_10_radius, player);
                    ovr024.calc_affect_effect(Classes.Affects.stinking_cloud, player);
                    ovr024.calc_affect_effect(Classes.Affects.faerie_fire, player);
                    break;

                case CheckType.SavingThrow:
                    ovr024.calc_affect_effect(Classes.Affects.protection_from_evil, player);
                    ovr024.calc_affect_effect(Classes.Affects.protection_from_good, player);
                    ovr024.calc_affect_effect(Classes.Affects.spell_resist_cold, player);
                    ovr024.calc_affect_effect(Classes.Affects.shield, player);
                    ovr024.calc_affect_effect(Classes.Affects.spell_resist_fire, player);
                    ovr024.calc_affect_effect(Classes.Affects.blinded, player);
                    ovr024.calc_affect_effect(Classes.Affects.bestow_curse, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_from_evil_10_radius, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_from_good_10_radius, player);
                    ovr024.calc_affect_effect(Classes.Affects.prayer, player);
                    ovr024.calc_affect_effect(Classes.Affects.item_fire_resist, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_frost_brand, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_paralysis_poison, player);
                    ovr024.calc_affect_effect(Classes.Affects.prot_sleep_charm_paralysis_poison, player);
                    ovr024.calc_affect_effect(Classes.Affects.con_saving_bonus, player);
                    ovr024.calc_affect_effect(Classes.Affects.hot_fire_shield, player);
                    ovr024.calc_affect_effect(Classes.Affects.cold_fire_shield, player);
                    break;

                case CheckType.Death:
                    ovr024.calc_affect_effect(Classes.Affects.fight_unconscious, player);
                    ovr024.calc_affect_effect(Classes.Affects.troll_fire_or_acid, player);
                    ovr024.calc_affect_effect(Classes.Affects.weap_dragon_slayer, player);
                    break;

                case CheckType.Type_14:
                    ovr024.calc_affect_effect(Classes.Affects.petrifying_gaze, player);
                    ovr024.calc_affect_effect(Classes.Affects.breath_elec, player);
                    ovr024.calc_affect_effect(Classes.Affects.ankheg_ranged_acid_attack, player);
                    ovr024.calc_affect_effect(Classes.Affects.spit_acid, player);
                    ovr024.calc_affect_effect(Classes.Affects.beholder_eyestalk, player);
                    ovr024.calc_affect_effect(Classes.Affects.breath_acid, player);
                    ovr024.calc_affect_effect(Classes.Affects.dracolich_paralytic_gaze, player);
                    ovr024.calc_affect_effect(Classes.Affects.breath_fire, player);
                    ovr024.calc_affect_effect(Classes.Affects.cast_breath_fire, player);
                    ovr024.calc_affect_effect(Classes.Affects.cast_throw_lightening, player);
                    ovr024.calc_affect_effect(Classes.Affects.affect_8b, player);
                    break;

                case CheckType.Type_15:
                    ovr024.calc_affect_effect(Classes.Affects.silence_15_radius, player);
                    ovr024.calc_affect_effect(Classes.Affects.stinking_cloud, player);
                    ovr024.calc_affect_effect(Classes.Affects.charm_person, player);
                    ovr024.calc_affect_effect(Classes.Affects.reduce, player);
                    ovr024.calc_affect_effect(Classes.Affects.berserk, player);
                    break;

                case CheckType.CanHitTarget:
                    ovr024.calc_affect_effect(Classes.Affects.invisibility, player);
                    ovr024.calc_affect_effect(Classes.Affects.invisible, player);
                    ovr024.calc_affect_effect(Classes.Affects.blink, player);
                    ovr024.calc_affect_effect(Classes.Affects.giant_vs_dwarf_gnome, player);
                    ovr024.calc_affect_effect(Classes.Affects.gnoll_bugbear_vs_gnome, player);
                    ovr024.calc_affect_effect(Classes.Affects.displace, player);
                    ovr024.calc_affect_effect(Classes.Affects.dispel_evil, player);
                    break;

                case CheckType.Morale:
                    ovr024.calc_affect_effect(Classes.Affects.bless, player);
                    ovr024.calc_affect_effect(Classes.Affects.cursed, player);
                    ovr024.calc_affect_effect(Classes.Affects.charm_person, player);
                    break;

                case CheckType.Movement:
                    ovr024.calc_affect_effect(Classes.Affects.haste, player);
                    ovr024.calc_affect_effect(Classes.Affects.slow, player);
                    ovr024.calc_affect_effect(Classes.Affects.clear_movement, player);
                    break;

                case CheckType.BattleRound:
                    ovr024.calc_affect_effect(Classes.Affects.regen_3_hp, player);
                    ovr024.calc_affect_effect(Classes.Affects.spiritual_hammer, player);
                    ovr024.calc_affect_effect(Classes.Affects.camouflage, player);
                    ovr024.calc_affect_effect(Classes.Affects.item_invisibility, player);
                    ovr024.calc_affect_effect(Classes.Affects.charm_person, player);
                    break;

                case CheckType.FireShield:
                    ovr024.calc_affect_effect(Classes.Affects.hot_fire_shield, player);
                    ovr024.calc_affect_effect(Classes.Affects.cold_fire_shield, player);
                    break;

                case CheckType.Confusion:
                    ovr024.calc_affect_effect(Classes.Affects.confuse, player);
                    break;

                case CheckType.BattleSetup2:
                    ovr024.calc_affect_effect(Classes.Affects.add_invisibility, player);
                    break;

                case CheckType.Type_23:
                    ovr024.calc_affect_effect(Classes.Affects.affect_4a, player);
                    break;
            }
        }
    }
}
