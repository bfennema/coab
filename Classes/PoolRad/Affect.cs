using System;

namespace Classes.PoolRad
{
    /// <summary>
    /// Summary description for Affect.
    /// </summary>
    public class Affect
    {
        [DataOffset(0x00, DataType.IByte)]
        public Affects type;
        [DataOffset(0x01, DataType.Word)]
        public ushort minutes;
        [DataOffset(0x03, DataType.Byte)]
        public byte affect_data;
        [DataOffset(0x04, DataType.Bool)]
        public bool callAffectTable;

        public const int StructSize = 9;

        readonly static BiLookup<Affects, Classes.Affects> mapping = InitMapping();

        private static BiLookup<Affects, Classes.Affects> InitMapping()
        {
            BiLookup<Affects, Classes.Affects> map = new();

            map.Add(Affects.bless, Classes.Affects.bless);
            map.Add(Affects.cursed, Classes.Affects.cursed);
            map.Add(Affects.weap_undead_slayer, Classes.Affects.weap_undead_slayer);
            //map.Add(Affects.studying_manual_bodily_health, Classes.Affects.studying_manual_bodily_health);
            map.Add(Affects.detect_magic, Classes.Affects.detect_magic);
            map.Add(Affects.weap_flame_tongue, Classes.Affects.weap_flame_tongue);
            //map.Add(Affects.training_manual_bodily_health, Classes.Affects.training_manual_bodily_health);
            map.Add(Affects.protection_from_evil, Classes.Affects.protection_from_evil);
            map.Add(Affects.protection_from_good, Classes.Affects.protection_from_good);
            map.Add(Affects.spell_resist_cold, Classes.Affects.spell_resist_cold);
            map.Add(Affects.charm_person, Classes.Affects.charm_person);
            map.Add(Affects.enlarge, Classes.Affects.enlarge);
            map.Add(Affects.reduce, Classes.Affects.reduce);
            map.Add(Affects.friends, Classes.Affects.friends);
            //map.Add(Affects.slow_poison, Classes.Affects.slow_poison);
            map.Add(Affects.read_magic, Classes.Affects.read_magic);
            map.Add(Affects.shield, Classes.Affects.shield);
            map.Add(Affects.gnome_vs_goblin_kobold, Classes.Affects.gnome_vs_goblin_kobold);
            map.Add(Affects.find_traps, Classes.Affects.find_traps);
            map.Add(Affects.spell_resist_fire, Classes.Affects.spell_resist_fire);
            map.Add(Affects.silence_15_radius, Classes.Affects.silence_15_radius);
            map.Add(Affects.slow_poison_end, Classes.Affects.slow_poison);
            map.Add(Affects.spiritual_hammer, Classes.Affects.spiritual_hammer);
            map.Add(Affects.detect_invisibility, Classes.Affects.detect_invisibility);
            map.Add(Affects.invisibility, Classes.Affects.invisibility);
            map.Add(Affects.dwarf_vs_orc_goblin, Classes.Affects.dwarf_vs_orc_goblin);
            //map.Add(Affects.feather_fall, Classes.Affects.feather_fall);
            map.Add(Affects.mirror_image, Classes.Affects.mirror_image);
            map.Add(Affects.ray_of_enfeeblement, Classes.Affects.ray_of_enfeeblement);
            map.Add(Affects.stinking_cloud, Classes.Affects.stinking_cloud);
            map.Add(Affects.helpless, Classes.Affects.helpless);
            map.Add(Affects.animate_dead, Classes.Affects.animate_dead);
            map.Add(Affects.blinded, Classes.Affects.blinded);
            map.Add(Affects.cause_disease_1, Classes.Affects.cause_disease_1);
            //map.Add(Affects.prayer_2, Classes.Affects.prayer_2);
            map.Add(Affects.bestow_curse, Classes.Affects.bestow_curse);
            map.Add(Affects.blink, Classes.Affects.blink);
            map.Add(Affects.strength, Classes.Affects.strength);
            map.Add(Affects.haste, Classes.Affects.haste);
            map.Add(Affects.affect_in_stinking_cloud, Classes.Affects.affect_in_stinking_cloud);
            map.Add(Affects.prot_from_normal_missiles, Classes.Affects.prot_from_normal_missiles);
            map.Add(Affects.slow, Classes.Affects.slow);
            map.Add(Affects.weaken, Classes.Affects.weaken);
            map.Add(Affects.cause_disease_2, Classes.Affects.cause_disease_2);
            map.Add(Affects.prot_from_evil_10_radius, Classes.Affects.prot_from_evil_10_radius);
            map.Add(Affects.prot_from_good_10_radius, Classes.Affects.prot_from_good_10_radius);
            map.Add(Affects.giant_vs_dwarf_gnome, Classes.Affects.giant_vs_dwarf_gnome);
            map.Add(Affects.gnoll_bugbear_vs_gnome, Classes.Affects.gnoll_bugbear_vs_gnome);
            map.Add(Affects.prayer, Classes.Affects.prayer);
            //map.Add(Affects.mummy_disease_healing, Classes.Affects.mummy_disease_healing);
            map.Add(Affects.snake_charm, Classes.Affects.snake_charm);
            map.Add(Affects.paralyze, Classes.Affects.paralyze);
            map.Add(Affects.sleep, Classes.Affects.sleep);
            //map.Add(Affects.repulsed, Classes.Affects.repulsed);
            map.Add(Affects.poisoned, Classes.Affects.poisoned);
            map.Add(Affects.item_invisibility, Classes.Affects.item_invisibility);
            //map.Add(Affects.mummy_disease_rot, Classes.Affects.mummy_disease_rot);
            map.Add(Affects.clear_movement, Classes.Affects.clear_movement);
            map.Add(Affects.regenerate, Classes.Affects.regenerate);
            map.Add(Affects.resist_normal_weapons, Classes.Affects.resist_normal_weapons);
            map.Add(Affects.item_fire_resist, Classes.Affects.item_fire_resist);
            map.Add(Affects.high_con_regen, Classes.Affects.highConRegen);
            // map.Add(Affects.affect_3f, Classes.Affects.affect_3f);
            map.Add(Affects.poison_plus_0, Classes.Affects.poison_plus_0);
            map.Add(Affects.poison_plus_4, Classes.Affects.poison_plus_4);
            map.Add(Affects.poison_plus_2, Classes.Affects.poison_plus_2);
            //map.Add(Affects.thri_kreen_paralyze, Classes.Affects.thri_kreen_paralyze);
            //map.Add(Affects.feeblemind, Classes.Affects.feeblemind);
            //map.Add(Affects.invisible_to_animals, Classes.Affects.invisible_to_animals);
            map.Add(Affects.poison_neg_2, Classes.Affects.poison_neg_2);
            map.Add(Affects.invisible, Classes.Affects.invisible);
            map.Add(Affects.camouflage, Classes.Affects.camouflage);
            //map.Add(Affects.prot_drag_breath, Classes.Affects.prot_drag_breath);
            map.Add(Affects.affect_4a, Classes.Affects.affect_4a);
            //map.Add(Affects.weap_dragon_slayer, Classes.Affects.weap_dragon_slayer);
            //map.Add(Affects.weap_frost_brand, Classes.Affects.weap_frost_brand);
            //map.Add(Affects.berserk, Classes.Affects.berserk);
            map.Add(Affects.affect_4e, Classes.Affects.affect_4e);
            map.Add(Affects.fireAttack_2d10, Classes.Affects.fireAttack_2d10);
            map.Add(Affects.ankheg_melee_acid_attack, Classes.Affects.ankheg_melee_acid_attack);
            //map.Add(Affects.dragon_fear_aura, Classes.Affects.dragon_fear_aura);
            //map.Add(Affects.mummy_fear, Classes.Affects.mummy_fear);
            map.Add(Affects.petrifying_gaze, Classes.Affects.petrifying_gaze);
            //map.Add(Affects.charming_gaze, Classes.Affects.charming_gaze);
            //map.Add(Affects.energy_drain_1, Classes.Affects.energy_drain_1);
            //map.Add(Affects.energy_drain_2, Classes.Affects.energy_drain_2);
            //map.Add(Affects.mummy_rot_attack, Classes.Affects.mummy_rot_attack);
            map.Add(Affects.breath_elec, Classes.Affects.breath_elec);
            map.Add(Affects.displace, Classes.Affects.displace);
            //map.Add(Affects.breath_acid, Classes.Affects.breath_acid);
            map.Add(Affects.immune_to_electricity, Classes.Affects.protect_elec);
            map.Add(Affects.affect_5c, Classes.Affects.affect_5c);
            map.Add(Affects.half_fire, Classes.Affects.half_fire);
            //map.Add(Affects.resist_blunt_pierce, Classes.Affects.resist_blunt_pierce);
            map.Add(Affects.delay_death, Classes.Affects.delay_death);
            //map.Add(Affects.owlbear_hug_check, Classes.Affects.owlbear_hug_check);
            map.Add(Affects.con_saving_bonus, Classes.Affects.con_saving_bonus);
            map.Add(Affects.regen_3_hp, Classes.Affects.regen_3_hp);
            map.Add(Affects.fight_unconscious, Classes.Affects.fight_unconscious);
            map.Add(Affects.troll_fire_or_acid, Classes.Affects.troll_fire_or_acid);
            map.Add(Affects.troll_regen, Classes.Affects.troll_regen);
            map.Add(Affects.TrollRegen, Classes.Affects.TrollRegen);
            //map.Add(Affects.salamander_heat_damage, Classes.Affects.salamander_heat_damage);
            map.Add(Affects.thri_kreen_dodge_missile, Classes.Affects.thri_kreen_dodge_missile);
            map.Add(Affects.resist_magic_50_percent, Classes.Affects.resist_magic_50_percent);
            //map.Add(Affects.resist_magic_100_percent, Classes.Affects.resist_magic_100_percent);
            map.Add(Affects.elf_resist_sleep, Classes.Affects.elf_resist_sleep);
            //map.Add(Affects.protect_charm_sleep, Classes.Affects.protect_charm_sleep);
            //map.Add(Affects.resist_paralyze, Classes.Affects.resist_paralyze);
            //map.Add(Affects.immune_to_cold, Classes.Affects.immune_to_cold);
            //map.Add(Affects.prot_paralysis_poison, Classes.Affects.prot_paralysis_poison);
            //map.Add(Affects.immune_to_fire, Classes.Affects.immune_to_fire);
            map.Add(Affects.efreeti_fire_resist, Classes.Affects.efreeti_fire_resist);
            //map.Add(Affects.resist_electricity, Classes.Affects.resist_electricity);
            //map.Add(Affects.resist_pierce_slash, Classes.Affects.resist_pierce_slash);
            //map.Add(Affects.resist_magic_weapon, Classes.Affects.resist_magic_weapon);
            map.Add(Affects.vuln_holy_water, Classes.Affects.vuln_holy_water);
            map.Add(Affects.half_cold, Classes.Affects.half_cold);
            //map.Add(Affects.protect_non_magic_weapons, Classes.Affects.protect_non_magic_weapons);
            map.Add(Affects.boulder_evasion, Classes.Affects.boulder_evasion);
            map.Add(Affects.ankheg_ranged_acid_attack, Classes.Affects.ankheg_ranged_acid_attack);
            //map.Add(Affects.vuln_fire, Classes.Affects.vuln_fire);
            //map.Add(Affects.resist_silver, Classes.Affects.resist_silver);
            map.Add(Affects.halfelf_resistance, Classes.Affects.halfelf_resistance);
            //map.Add(Affects.prot_sleep_charm_paralysis_poison, Classes.Affects.prot_sleep_charm_paralysis_poison);
            //map.Add(Affects.immune_gaze_attacks, Classes.Affects.immune_gaze_attacks);
            map.Add(Affects.reflectable_gaze, Classes.Affects.reflectable_gaze);

            return map;
        }

        public Affect(Affects _type, ushort _minutes, byte _affect_data, bool _call_spell_jump_list)
        {
            type = _type;
            minutes = _minutes;
            affect_data = _affect_data;
            callAffectTable = _call_spell_jump_list;
        }

        public Affect(byte[] data, int offset)
        {
            type = (Affects)data[offset + 0x0];
            minutes = Sys.ArrayToUshort(data, offset + 0x1);
            affect_data = data[offset + 0x3];
            callAffectTable = (data[offset + 0x4] != 0);
        }

        public Affect(Classes.Affect affect, Classes.Player player)
        {
            if (mapping[affect.type].Count == 0)
            {
                throw new ArgumentException("Unable to map affect.type", "affect");
            }
            else
            {
                type = mapping[affect.type][0];
                minutes = affect.minutes;
                if (type == Affects.enlarge)
                {
                    if (player.stats.Str.cur == 18)
                    {
                        affect_data = (byte)(player.stats.Str00.cur + 1);
                    }
                    else
                    {
                        affect_data = (byte)(player.stats.Str.cur + 100);
                    }
                }
                else if (type == Affects.friends)
                {
                    affect_data = (byte)player.stats.Cha.cur;
                }
                affect_data = affect.affect_data;
                callAffectTable = affect.callAffectTable;
            }
        }

        public void Load(Classes.Player player)
        {
            if (type == Affects.strength)
            {
                int str_00 = 0;
                int str = (int)(affect_data & 0x7F);

                if (str <= 101)
                {
                    str_00 = str - 1;
                    str = 18;
                }
                else
                {
                    str -= 100;
                    str_00 = 0;
                }

                player.stats.Str.cur = str;
                player.stats.Str00.cur = str_00;
            }
            else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
            {
                if (type == Affects.gnome_vs_goblin_kobold ||
                    type == Affects.dwarf_vs_orc_goblin ||
                    type == Affects.giant_vs_dwarf_gnome ||
                    type == Affects.gnoll_bugbear_vs_gnome ||
                    type == Affects.con_saving_bonus ||
                    type == Affects.elf_resist_sleep ||
                    type == Affects.halfelf_resistance)
                {
                    Classes.Affect affect = new(mapping[type][0], minutes, affect_data, callAffectTable);
                    player.affects.Add(affect);
                }
                else if (type == Affects.friends)
                {
                    player.stats.Cha.cur = affect_data;
                }
                else if (type == Affects.enlarge)
                {
                    int str_00 = 0;
                    int str = (int)(affect_data & 0x7F);

                    if (str <= 101)
                    {
                        str_00 = str - 1;
                        str = 18;
                    }
                    else
                    {
                        str -= 100;
                        str_00 = 0;
                    }

                    player.stats.Str.cur = str;
                    player.stats.Str00.cur = str_00;
                }
            }
            else if (mapping[type].Count == 1)
            {
                Classes.Affect affect = new Classes.Affect(mapping[type][0], minutes, affect_data, callAffectTable);
                player.affects.Add(affect);
            }
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }

        public enum Affects
        {
            bless = 0x1,
            cursed = 0x2,
            weap_undead_slayer = 0x3,
            studying_manual_bodily_health = 0x4,
            detect_magic = 0x5,
            weap_flame_tongue = 0x6,
            training_manual_bodily_health = 0x7,
            protection_from_evil = 0x8,
            protection_from_good = 0x9,
            spell_resist_cold = 0xa,
            charm_person = 0xb,
            enlarge = 0xc,
            reduce = 0xd,
            friends = 0xe,
            slow_poison = 0xf,
            read_magic = 0x10,
            shield = 0x11,
            gnome_vs_goblin_kobold = 0x12,
            find_traps = 0x13,
            spell_resist_fire = 0x14,
            silence_15_radius = 0x15,
            slow_poison_end = 0x16,
            spiritual_hammer = 0x17,
            detect_invisibility = 0x18,
            invisibility = 0x19,
            dwarf_vs_orc_goblin = 0x1a,
            feather_fall = 0x1b,
            mirror_image = 0x1c,
            ray_of_enfeeblement = 0x1d,
            stinking_cloud = 0x1e,
            helpless = 0x1f,
            animate_dead = 0x20,
            blinded = 0x21,
            cause_disease_1 = 0x22,
            prayer_2 = 0x23, // ?? - 0x31
            bestow_curse = 0x24,
            blink = 0x25,
            strength = 0x26,
            haste = 0x27,
            affect_in_stinking_cloud = 0x28,
            prot_from_normal_missiles = 0x29,
            slow = 0x2a,
            weaken = 0x2b,
            cause_disease_2 = 0x2c,
            prot_from_evil_10_radius = 0x2d,
            prot_from_good_10_radius = 0x2e,
            giant_vs_dwarf_gnome = 0x2f,
            gnoll_bugbear_vs_gnome = 0x30,
            prayer = 0x31,
            mummy_disease_healing = 0x32,
            snake_charm = 0x33,
            paralyze = 0x34,
            sleep = 0x35,
            repulsed = 0x36,
            poisoned = 0x37,
            item_invisibility = 0x38,
            mummy_disease_rot = 0x39,
            clear_movement = 0x3a,
            regenerate = 0x3b,
            resist_normal_weapons = 0x3c,
            item_fire_resist = 0x3d,
            high_con_regen = 0x3e,
            affect_3f,
            poison_plus_0 = 0x40,
            poison_plus_4 = 0x41,
            poison_plus_2 = 0x42,
            paralyze_plus_0 = 0x43,
            paralyze_elves_immune = 0x44,
            paralyze_neg_2 = 0x45,
            poison_neg_2 = 0x46,
            invisible = 0x47,
            camouflage = 0x48,
            rake = 0x49,
            affect_4a = 0x4a,
            affect_4b = 0x4b,
            blood_drain = 0x4c,
            bite_and_hold = 0x4d,
            affect_4e = 0x4e,
            fireAttack_2d10 = 0x4f,
            ankheg_melee_acid_attack = 0x50,
            dragon_fear_aura = 0x51,
            mummy_fear = 0x52,
            petrifying_gaze = 0x53,
            charming_gaze = 0x54,
            energy_drain_1 = 0x55,
            energy_drain_2 = 0x56,
            mummy_rot_attack = 0x57,
            breath_elec = 0x58,
            displace = 0x59,
            halfling_poison_bonus = 0x5a,
            immune_to_electricity = 0x5b,
            affect_5c = 0x5c,
            half_fire = 0x5d,
            half_blunt_pierce = 0x5e,
            delay_death = 0x5f,
            immune_non_silver_magic = 0x60,
            con_saving_bonus = 0x61,
            regen_3_hp = 0x62,
            fight_unconscious = 0x63,
            troll_fire_or_acid = 0x64,
            troll_regen = 0x65,
            TrollRegen = 0x66,
            immune_non_magic_weapons_2 = 0x67,
            thri_kreen_dodge_missile = 0x68,
            resist_magic_50_percent = 0x69,
            resist_magic_100_percent = 0x6a,
            elf_resist_sleep = 0x6b,
            immune_charm_sleep = 0x6c,
            immune_paralyze = 0x6d,
            immune_cold = 0x6e,
            immune_paralysis_poison = 0x6f,
            immune_fire = 0x70,
            efreeti_fire_resist = 0x71,
            half_electricity = 0x72,
            half_pierce_slash = 0x73,
            half_magic_weapon = 0x74,
            vuln_holy_water = 0x75,
            half_cold = 0x76,
            immune_non_magic_weapons = 0x77,
            boulder_evasion = 0x78,
            ankheg_ranged_acid_attack = 0x79,
            vuln_fire = 0x7a,
            resist_silver = 0x7b,
            halfelf_resistance = 0x7c,
            immune_sleep_charm_paralysis_poison = 0x7d,
            immune_gaze_attacks = 0x7e,
            reflectable_gaze = 0x7f,
        }
    }
}
