namespace engine.Affects
{
    internal class Effect
    {
        delegate void affectDelegate(Classes.Effect arg_0, object affect, Classes.Player player);
        readonly static System.Collections.Generic.Dictionary<Classes.Affects, affectDelegate> affect_table = [];

        static Effect()
        {
            affect_table.Add(Classes.Affects.bless, ovr013.Bless);
            affect_table.Add(Classes.Affects.cursed, ovr013.Curse);
            affect_table.Add(Classes.Affects.sticks_to_snakes, ovr013.SticksToSnakes);
            affect_table.Add(Classes.Affects.dispel_evil, ovr013.DispelEvil);
            affect_table.Add(Classes.Affects.detect_magic, ovr013.empty);
            affect_table.Add(Classes.Affects.weap_flame_tongue, ovr013.AffectFlameTongue);
            affect_table.Add(Classes.Affects.faerie_fire, ovr013.FaerieFire);
            affect_table.Add(Classes.Affects.protection_from_evil, ovr013.affect_protect_evil);
            affect_table.Add(Classes.Affects.protection_from_good, ovr013.affect_protect_good);
            affect_table.Add(Classes.Affects.spell_resist_cold, ovr013.affect_resist_cold);
            affect_table.Add(Classes.Affects.charm_person, ovr013.affect_charm_person);
            affect_table.Add(Classes.Affects.enlarge, ovr013.empty);
            affect_table.Add(Classes.Affects.reduce, ovr013.Suffocates);
            affect_table.Add(Classes.Affects.friends, ovr013.empty);
            affect_table.Add(Classes.Affects.poison_damage, ovr013.AffectPoisonDamage);
            affect_table.Add(Classes.Affects.read_magic, ovr013.empty);
            affect_table.Add(Classes.Affects.shield, ovr013.Affectshield);
            affect_table.Add(Classes.Affects.gnome_vs_goblin_kobold, ovr013.AffectGnomeVsGoblinKobold);
            affect_table.Add(Classes.Affects.find_traps, ovr013.empty);
            affect_table.Add(Classes.Affects.spell_resist_fire, ovr013.AffectResistFire);
            affect_table.Add(Classes.Affects.silence_15_radius, ovr013.is_silenced1);
            affect_table.Add(Classes.Affects.slow_poison, ovr013.AffectslowPoison);
            affect_table.Add(Classes.Affects.spiritual_hammer, ovr013.affect_spiritual_hammer);
            affect_table.Add(Classes.Affects.detect_invisibility, ovr013.empty);
            affect_table.Add(Classes.Affects.invisibility, ovr013.sub_3A6C6);
            affect_table.Add(Classes.Affects.dwarf_vs_orc_goblin, ovr013.AffectDwarfVsOrcGoblin);
            affect_table.Add(Classes.Affects.fumbling, ovr013.sub_3A071);
            affect_table.Add(Classes.Affects.mirror_image, ovr013.MirrorImage);
            affect_table.Add(Classes.Affects.ray_of_enfeeblement, ovr013.three_quarters_damage);
            affect_table.Add(Classes.Affects.stinking_cloud, ovr013.StinkingCloud);
            affect_table.Add(Classes.Affects.helpless, ovr013.sub_3A071);
            affect_table.Add(Classes.Affects.animate_dead, ovr013.sub_3A89E);
            affect_table.Add(Classes.Affects.blinded, ovr013.AffectBlinded);
            affect_table.Add(Classes.Affects.cause_disease_1, ovr013.AffectCauseDisease);
            affect_table.Add(Classes.Affects.confuse, ovr013.AffectConfuse);
            affect_table.Add(Classes.Affects.bestow_curse, ovr013.affect_curse);
            affect_table.Add(Classes.Affects.blink, ovr013.AffectBlink);
            affect_table.Add(Classes.Affects.strength, ovr013.empty);
            affect_table.Add(Classes.Affects.haste, ovr013.AffectHaste);
            affect_table.Add(Classes.Affects.affect_in_stinking_cloud, ovr013.StinkingCloudAffect);
            affect_table.Add(Classes.Affects.prot_from_normal_missiles, ovr013.AffectProtNormalMissles);
            affect_table.Add(Classes.Affects.slow, ovr013.Affectslow);
            affect_table.Add(Classes.Affects.weaken, ovr013.weaken);
            affect_table.Add(Classes.Affects.cause_disease_2, ovr013.sub_3B0C2);
            affect_table.Add(Classes.Affects.prot_from_evil_10_radius, ovr013.affect_protect_evil);
            affect_table.Add(Classes.Affects.prot_from_good_10_radius, ovr013.affect_protect_good);
            affect_table.Add(Classes.Affects.giant_vs_dwarf_gnome, ovr013.AffectGiantVsDwarfGnome);
            affect_table.Add(Classes.Affects.gnoll_bugbear_vs_gnome, ovr013.AffectGnollBugbearVsGnome);
            affect_table.Add(Classes.Affects.prayer, ovr013.AffectPrayer);
            affect_table.Add(Classes.Affects.hot_fire_shield, ovr013.HotFireShield);
            affect_table.Add(Classes.Affects.snake_charm, ovr013.sub_3A071);
            affect_table.Add(Classes.Affects.paralyze, ovr013.sub_3A071);
            affect_table.Add(Classes.Affects.sleep, ovr013.sub_3A071);
            affect_table.Add(Classes.Affects.cold_fire_shield, ovr013.ColdFireShield);
            affect_table.Add(Classes.Affects.poisoned, ovr013.empty);
            affect_table.Add(Classes.Affects.item_invisibility, ovr013.sub_3B27B);
            affect_table.Add(Classes.Affects.engulf, ovr014.AffectEngulf);
            affect_table.Add(Classes.Affects.clear_movement, ovr013.AffectClearMovement);
            affect_table.Add(Classes.Affects.regenerate, ovr013.AffectRegenration);
            affect_table.Add(Classes.Affects.resist_normal_weapons, ovr013.AffectResistWeapons);
            affect_table.Add(Classes.Affects.item_fire_resist, ovr013.AffectFireResist);
            affect_table.Add(Classes.Affects.highConRegen, ovr013.AffectHighConRegen);
            affect_table.Add(Classes.Affects.minor_globe_of_invulnerability, ovr013.AffectMinorGlobeOfInvulnerability);
            affect_table.Add(Classes.Affects.poison_plus_0, ovr013.AffectPoisonPlus0);
            affect_table.Add(Classes.Affects.poison_plus_4, ovr013.AffectPoisonPlus4);
            affect_table.Add(Classes.Affects.poison_plus_2, ovr013.AffectPoisonPlus2);
            affect_table.Add(Classes.Affects.thri_kreen_paralyze, ovr013.ThriKreenParalyze);
            affect_table.Add(Classes.Affects.feeblemind, ovr013.AffectFeebleMind);
            affect_table.Add(Classes.Affects.invisible_to_animals, ovr013.AffectInvisToAnimals);
            affect_table.Add(Classes.Affects.poison_neg_2, ovr013.AffectPoisonNeg2);
            affect_table.Add(Classes.Affects.invisible, ovr013.AffectInvisible);
            affect_table.Add(Classes.Affects.camouflage, ovr013.AffectCamouflage);
            affect_table.Add(Classes.Affects.prot_drag_breath, ovr013.ProtDragonsBreath);
            affect_table.Add(Classes.Affects.affect_4a, ovr013.empty);
            affect_table.Add(Classes.Affects.weap_dragon_slayer, ovr013.AffectDragonSlayer);
            affect_table.Add(Classes.Affects.weap_frost_brand, ovr013.AffectFrostBrand);
            affect_table.Add(Classes.Affects.berserk, ovr013.AffectBerzerk);
            affect_table.Add(Classes.Affects.affect_4e, ovr013.sub_3B8D9);
            affect_table.Add(Classes.Affects.fireAttack_2d10, ovr013.MagicFireAttack_2d10);
            affect_table.Add(Classes.Affects.ankheg_melee_acid_attack, ovr013.AnkhegMeleeAcidAttack);
            affect_table.Add(Classes.Affects.half_damage, ovr013.half_damage);
            affect_table.Add(Classes.Affects.resist_fire_and_cold, ovr013.AffectResistFireAndCold);
            affect_table.Add(Classes.Affects.petrifying_gaze, ovr023.AffectPetrifyingGaze);
            affect_table.Add(Classes.Affects.shambling_absorb_lightning, ovr013.AffectshamblerAbsorbLightning);
            affect_table.Add(Classes.Affects.resist_piercing, ovr013.AffectResistPiercing);
            affect_table.Add(Classes.Affects.spit_acid, ovr023.AffectSpitAcid);
            affect_table.Add(Classes.Affects.beholder_eyestalk, ovr014.beholder_eyestalk);
            affect_table.Add(Classes.Affects.breath_elec, ovr023.DragonBreathElec);
            affect_table.Add(Classes.Affects.displace, ovr013.AffectDisplace);
            affect_table.Add(Classes.Affects.breath_acid, ovr023.DragonBreathAcid);
            affect_table.Add(Classes.Affects.affect_in_cloud_kill, ovr013.CloudKillAffect);
            affect_table.Add(Classes.Affects.affect_5c, ovr013.empty);
            affect_table.Add(Classes.Affects.half_fire, ovr013.half_fire_damage);
            affect_table.Add(Classes.Affects.resist_blunt_pierce, ovr013.AffectResistBluntPierce);
            affect_table.Add(Classes.Affects.delay_death, ovr013.AffectDelayDeath);
            affect_table.Add(Classes.Affects.owlbear_hug_check, ovr014.AffectOwlbearHugAttackCheck);
            affect_table.Add(Classes.Affects.con_saving_bonus, ovr013.con_saving_bonus);
            affect_table.Add(Classes.Affects.regen_3_hp, ovr013.AffectRegen3Hp);
            affect_table.Add(Classes.Affects.fight_unconscious, ovr013.AffectFightUnconscious);
            affect_table.Add(Classes.Affects.troll_fire_or_acid, ovr013.AffectTrollFireOrAcid);
            affect_table.Add(Classes.Affects.troll_regen, ovr013.AffectTrollRegenerate);
            affect_table.Add(Classes.Affects.TrollRegen, ovr013.AffectTrollRegen);
            affect_table.Add(Classes.Affects.salamander_heat_damage, ovr013.AffectsalamanderHeatDamage);
            affect_table.Add(Classes.Affects.thri_kreen_dodge_missile, ovr013.sub_3C0DA);
            affect_table.Add(Classes.Affects.resist_magic_50_percent, ovr013.ResistMagic50Percent);
            affect_table.Add(Classes.Affects.resist_magic_15_percent, ovr013.ResistMagic15Percent);
            affect_table.Add(Classes.Affects.elf_resist_sleep, ovr013.AffectElfRisistSleep);
            affect_table.Add(Classes.Affects.protect_charm_sleep, ovr013.AffectProtCharmSleep);
            affect_table.Add(Classes.Affects.resist_paralyze, ovr013.ResistParalyze);
            affect_table.Add(Classes.Affects.immune_to_cold, ovr013.AffectImmuneToCold);
            affect_table.Add(Classes.Affects.prot_paralysis_poison, ovr013.AffectProtParalysisPoison);
            affect_table.Add(Classes.Affects.immune_to_fire, ovr013.AffectImmuneToFire);
            affect_table.Add(Classes.Affects.efreeti_fire_resist, ovr013.AffectEfreetiFireResist);
            affect_table.Add(Classes.Affects.half_elec, ovr013.AffectProtectionFromElectricity);
            affect_table.Add(Classes.Affects.resist_pierce_slash, ovr013.AffectResistPierceSlash);
            affect_table.Add(Classes.Affects.resist_magic_weapon, ovr013.half_damage_if_weap_magic);
            affect_table.Add(Classes.Affects.vuln_holy_water, ovr013.AffectVulnHolyWater);
            affect_table.Add(Classes.Affects.half_cold, ovr013.AffectProtCold);
            affect_table.Add(Classes.Affects.protect_non_magic_weapons, ovr013.AffectProtNonMagicWeapons);
            affect_table.Add(Classes.Affects.boulder_evasion, ovr013.AffectBoulderEvasion);
            affect_table.Add(Classes.Affects.ankheg_ranged_acid_attack, ovr013.AffectAnkhedRangedAcidAttack);
            affect_table.Add(Classes.Affects.dracolich_paralysis, ovr013.AffectDracolichParalysis);
            affect_table.Add(Classes.Affects.dracolich_cold_damage, ovr013.AffectDracolichColdDamage);
            affect_table.Add(Classes.Affects.halfelf_resistance, ovr013.AffectHalfElfResistance);
            affect_table.Add(Classes.Affects.prot_sleep_charm_paralysis_poison, ovr013.AffectProtSleepCharmParalysisPoison);
            affect_table.Add(Classes.Affects.dracolich_paralytic_gaze, ovr023.cast_gaze_paralyze);
            affect_table.Add(Classes.Affects.reflectable_gaze, ovr013.empty);
            affect_table.Add(Classes.Affects.breath_fire, ovr023.DragonBreathFire);
            affect_table.Add(Classes.Affects.protect_magic, ovr013.AffectProtMagic);
            affect_table.Add(Classes.Affects.vuln_blessed_quarrel, ovr013.AffectVulnBlessedQuarrel);
            affect_table.Add(Classes.Affects.cast_breath_fire, ovr023.cast_breath_fire);
            affect_table.Add(Classes.Affects.cast_throw_lightening, ovr023.cast_throw_lightening);
            affect_table.Add(Classes.Affects.dracolich_protection, ovr013.AffectDracolichProtection);
            affect_table.Add(Classes.Affects.ranger_vs_giant, ovr013.AffectRangerVsGiant);
            affect_table.Add(Classes.Affects.protect_elec, ovr013.AffectProtElec);
            affect_table.Add(Classes.Affects.entangle, ovr013.AffectEntangle);
            affect_table.Add(Classes.Affects.confuse_berserk, ovr013.AffectConfuseBerserk);
            affect_table.Add(Classes.Affects.add_invisibility, ovr013.AffectAddInvisibility);
            affect_table.Add(Classes.Affects.affect_8b, ovr014.sub_425C6);
            affect_table.Add(Classes.Affects.paladinDailyHealCast, ovr013.empty);
            affect_table.Add(Classes.Affects.paladinDailyCureRefresh, ovr013.PaladinCastCureRefresh);
            affect_table.Add(Classes.Affects.fear, ovr013.AffectFear);
            affect_table.Add(Classes.Affects.fire_shield_damage, ovr013.AffectFireShieldDamage);
            affect_table.Add(Classes.Affects.owlbear_hug_round_attack, ovr014.AffectOwlbearHugRoundAttack);
            affect_table.Add(Classes.Affects.dispel_evil_banish, ovr013.AffectDispelEvilBanish);
            affect_table.Add(Classes.Affects.strength_spell, ovr013.empty);
            affect_table.Add(Classes.Affects.do_items_affect, ovr013.do_items_affect);
            affect_table.Add(Classes.Affects.weap_undead_slayer, Weapon.UndeadSlayer);
        }
        internal static void Setup() // setup_spells2
        {

        }
        internal static void Call(Classes.Effect add_remove, object parameter, Classes.Player player, Classes.Affects affect) /* sub_630C7 */
        {
            if (Classes.gbl.applyItemAffect == true)
            {
                affect = Classes.Affects.do_items_affect;
            }

            affectDelegate func;
            if (affect_table.TryGetValue(affect, out func))
            {
                func(add_remove, parameter, player);
            }
        }
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
                    ovr024.calc_affect_effect(Classes.Affects.weap_undead_slayer, player);
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
                    ovr024.calc_affect_effect(Classes.Affects.weap_undead_slayer, player);
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
