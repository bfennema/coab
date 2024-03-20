namespace Classes
{
    public enum PoolRadAffects
    {
        bless = 0x1,
        cursed = 0x2,
        sword_vs_undead = 0x3,
        studying_manual_bodily_health = 0x4,
        detect_magic = 0x5,
        weap_flame_tongue = 0x6,
        training_manual_bodily_health = 0x7,
        protection_from_evil = 0x8,
        protection_from_good = 0x9,
        resist_cold = 0xa,
        charm_person = 0xb,
        enlarge = 0xc,
        reduce = 0xd,
        friends = 0xe,
        slow_poison = 0xf,
        read_magic = 0x10,
        shield = 0x11,
        gnome_vs_goblin_kobold = 0x12,
        find_traps = 0x13,
        resist_fire = 0x14,
        silence_15_radius = 0x15,
        slow_poison_end = 0x16,
        spiritual_hammer = 0x17,
        detect_invisibility = 0x18,
        invisibility = 0x19,
        dwarf_vs_orc = 0x1a,
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
        dwarf_and_gnome_vs_giants = 0x2f,
        gnome_vs_gnoll = 0x30,
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
        fire_resist = 0x3d,
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

    /// <summary>
    /// Summary description for Affect.
    /// </summary>
    public class PoolRadAffect
    {
        public const int StructSize = 9;

        static BiLookup<PoolRadAffects, Affects> mapping;

        static void InitMapping()
        {
            if (mapping != null) { return; }

            mapping = new BiLookup<PoolRadAffects, Affects>();

            mapping.Add(PoolRadAffects.bless, Affects.bless);
            mapping.Add(PoolRadAffects.cursed, Affects.cursed);
            //mapping.Add(PoolRadAffects.sword_vs_undead, Affects.sword_vs_undead);
            //mapping.Add(PoolRadAffects.studying_manual_bodily_health, Affects.studying_manual_bodily_health);
            mapping.Add(PoolRadAffects.detect_magic, Affects.detect_magic);
            mapping.Add(PoolRadAffects.weap_flame_tongue, Affects.weap_flame_tongue);
            //mapping.Add(PoolRadAffects.training_manual_bodily_health, Affects.training_manual_bodily_health);
            mapping.Add(PoolRadAffects.protection_from_evil, Affects.protection_from_evil);
            mapping.Add(PoolRadAffects.protection_from_good, Affects.protection_from_good);
            mapping.Add(PoolRadAffects.resist_cold, Affects.resist_cold);
            mapping.Add(PoolRadAffects.charm_person, Affects.charm_person);
            mapping.Add(PoolRadAffects.enlarge, Affects.enlarge);
            mapping.Add(PoolRadAffects.reduce, Affects.reduce);
            mapping.Add(PoolRadAffects.friends, Affects.friends);
            //mapping.Add(PoolRadAffects.slow_poison, Affects.slow_poison);
            mapping.Add(PoolRadAffects.read_magic, Affects.read_magic);
            mapping.Add(PoolRadAffects.shield, Affects.shield);
            mapping.Add(PoolRadAffects.gnome_vs_goblin_kobold, Affects.gnome_vs_goblin_kobold);
            mapping.Add(PoolRadAffects.find_traps, Affects.find_traps);
            mapping.Add(PoolRadAffects.resist_fire, Affects.resist_fire);
            mapping.Add(PoolRadAffects.silence_15_radius, Affects.silence_15_radius);
            mapping.Add(PoolRadAffects.slow_poison_end, Affects.slow_poison);
            mapping.Add(PoolRadAffects.spiritual_hammer, Affects.spiritual_hammer);
            mapping.Add(PoolRadAffects.detect_invisibility, Affects.detect_invisibility);
            mapping.Add(PoolRadAffects.invisibility, Affects.invisibility);
            mapping.Add(PoolRadAffects.dwarf_vs_orc, Affects.dwarf_vs_orc);
            //mapping.Add(PoolRadAffects.feather_fall, Affects.feather_fall);
            mapping.Add(PoolRadAffects.mirror_image, Affects.mirror_image);
            mapping.Add(PoolRadAffects.ray_of_enfeeblement, Affects.ray_of_enfeeblement);
            mapping.Add(PoolRadAffects.stinking_cloud, Affects.stinking_cloud);
            mapping.Add(PoolRadAffects.helpless, Affects.helpless);
            mapping.Add(PoolRadAffects.animate_dead, Affects.animate_dead);
            mapping.Add(PoolRadAffects.blinded, Affects.blinded);
            mapping.Add(PoolRadAffects.cause_disease_1, Affects.cause_disease_1);
            //mapping.Add(PoolRadAffects.prayer_2, Affects.prayer_2);
            mapping.Add(PoolRadAffects.bestow_curse, Affects.bestow_curse);
            mapping.Add(PoolRadAffects.blink, Affects.blink);
            mapping.Add(PoolRadAffects.strength, Affects.strength);
            mapping.Add(PoolRadAffects.haste, Affects.haste);
            mapping.Add(PoolRadAffects.affect_in_stinking_cloud, Affects.affect_in_stinking_cloud);
            mapping.Add(PoolRadAffects.prot_from_normal_missiles, Affects.prot_from_normal_missiles);
            mapping.Add(PoolRadAffects.slow, Affects.slow);
            mapping.Add(PoolRadAffects.weaken, Affects.weaken);
            mapping.Add(PoolRadAffects.cause_disease_2, Affects.cause_disease_2);
            mapping.Add(PoolRadAffects.prot_from_evil_10_radius, Affects.prot_from_evil_10_radius);
            mapping.Add(PoolRadAffects.prot_from_good_10_radius, Affects.prot_from_good_10_radius);
            mapping.Add(PoolRadAffects.dwarf_and_gnome_vs_giants, Affects.dwarf_and_gnome_vs_giants);
            mapping.Add(PoolRadAffects.gnome_vs_gnoll, Affects.gnome_vs_gnoll);
            mapping.Add(PoolRadAffects.prayer, Affects.prayer);
            mapping.Add(PoolRadAffects.mummy_disease_healing, Affects.mummy_disease_healing);
            mapping.Add(PoolRadAffects.snake_charm, Affects.snake_charm);
            mapping.Add(PoolRadAffects.paralyze, Affects.paralyze);
            mapping.Add(PoolRadAffects.sleep, Affects.sleep);
            //mapping.Add(PoolRadAffects.repulsed, Affects.repulsed);
            mapping.Add(PoolRadAffects.poisoned, Affects.poisoned);
            mapping.Add(PoolRadAffects.item_invisibility, Affects.item_invisibility);
            mapping.Add(PoolRadAffects.mummy_disease_rot, Affects.mummy_disease_rot);
            mapping.Add(PoolRadAffects.clear_movement, Affects.clear_movement);
            mapping.Add(PoolRadAffects.regenerate, Affects.regenerate);
            mapping.Add(PoolRadAffects.resist_normal_weapons, Affects.resist_normal_weapons);
            mapping.Add(PoolRadAffects.fire_resist, Affects.fire_resist);
            mapping.Add(PoolRadAffects.high_con_regen, Affects.highConRegen);
            // mapping.Add(PoolRadAffects.affect_3f, Affects.affect_3f);
            mapping.Add(PoolRadAffects.poison_plus_0, Affects.poison_plus_0);
            mapping.Add(PoolRadAffects.poison_plus_4, Affects.poison_plus_4);
            mapping.Add(PoolRadAffects.poison_plus_2, Affects.poison_plus_2);
            //mapping.Add(PoolRadAffects.thri_kreen_paralyze, Affects.thri_kreen_paralyze);
            //mapping.Add(PoolRadAffects.feeblemind, Affects.feeblemind);
            //mapping.Add(PoolRadAffects.invisible_to_animals, Affects.invisible_to_animals);
            mapping.Add(PoolRadAffects.poison_neg_2, Affects.poison_neg_2);
            mapping.Add(PoolRadAffects.invisible, Affects.invisible);
            mapping.Add(PoolRadAffects.camouflage, Affects.camouflage);
            //mapping.Add(PoolRadAffects.prot_drag_breath, Affects.prot_drag_breath);
            mapping.Add(PoolRadAffects.affect_4a, Affects.affect_4a);
            //mapping.Add(PoolRadAffects.weap_dragon_slayer, Affects.weap_dragon_slayer);
            //mapping.Add(PoolRadAffects.weap_frost_brand, Affects.weap_frost_brand);
            //mapping.Add(PoolRadAffects.berserk, Affects.berserk);
            mapping.Add(PoolRadAffects.affect_4e, Affects.affect_4e);
            mapping.Add(PoolRadAffects.fireAttack_2d10, Affects.fireAttack_2d10);
            mapping.Add(PoolRadAffects.ankheg_melee_acid_attack, Affects.ankheg_melee_acid_attack);
            //mapping.Add(PoolRadAffects.dragon_fear_aura, Affects.dragon_fear_aura);
            mapping.Add(PoolRadAffects.mummy_fear, Affects.mummy_fear);
            mapping.Add(PoolRadAffects.petrifying_gaze, Affects.petrifying_gaze);
            //mapping.Add(PoolRadAffects.charming_gaze, Affects.charming_gaze);
            //mapping.Add(PoolRadAffects.energy_drain_1, Affects.energy_drain_1);
            //mapping.Add(PoolRadAffects.energy_drain_2, Affects.energy_drain_2);
            mapping.Add(PoolRadAffects.mummy_rot_attack, Affects.mummy_rot_attack);
            mapping.Add(PoolRadAffects.breath_elec, Affects.breath_elec);
            mapping.Add(PoolRadAffects.displace, Affects.displace);
            //mapping.Add(PoolRadAffects.breath_acid, Affects.breath_acid);
            mapping.Add(PoolRadAffects.immune_to_electricity, Affects.protect_elec);
            mapping.Add(PoolRadAffects.affect_5c, Affects.affect_5c);
            mapping.Add(PoolRadAffects.half_fire, Affects.half_fire);
            //mapping.Add(PoolRadAffects.resist_blunt_pierce, Affects.resist_blunt_pierce);
            mapping.Add(PoolRadAffects.delay_death, Affects.delay_death);
            //mapping.Add(PoolRadAffects.owlbear_hug_check, Affects.owlbear_hug_check);
            mapping.Add(PoolRadAffects.con_saving_bonus, Affects.con_saving_bonus);
            mapping.Add(PoolRadAffects.regen_3_hp, Affects.regen_3_hp);
            mapping.Add(PoolRadAffects.fight_unconscious, Affects.fight_unconscious);
            mapping.Add(PoolRadAffects.troll_fire_or_acid, Affects.troll_fire_or_acid);
            mapping.Add(PoolRadAffects.troll_regen, Affects.troll_regen);
            mapping.Add(PoolRadAffects.TrollRegen, Affects.TrollRegen);
            //mapping.Add(PoolRadAffects.salamander_heat_damage, Affects.salamander_heat_damage);
            mapping.Add(PoolRadAffects.thri_kreen_dodge_missile, Affects.thri_kreen_dodge_missile);
            mapping.Add(PoolRadAffects.resist_magic_50_percent, Affects.resist_magic_50_percent);
            //mapping.Add(PoolRadAffects.resist_magic_100_percent, Affects.resist_magic_100_percent);
            mapping.Add(PoolRadAffects.elf_resist_sleep, Affects.elf_resist_sleep);
            //mapping.Add(PoolRadAffects.protect_charm_sleep, Affects.protect_charm_sleep);
            //mapping.Add(PoolRadAffects.resist_paralyze, Affects.resist_paralyze);
            //mapping.Add(PoolRadAffects.immune_to_cold, Affects.immune_to_cold);
            //mapping.Add(PoolRadAffects.prot_paralysis_poison, Affects.prot_paralysis_poison);
            //mapping.Add(PoolRadAffects.immune_to_fire, Affects.immune_to_fire);
            mapping.Add(PoolRadAffects.efreeti_fire_resist, Affects.efreeti_fire_resist);
            //mapping.Add(PoolRadAffects.resist_electricity, Affects.resist_electricity);
            //mapping.Add(PoolRadAffects.resist_pierce_slash, Affects.resist_pierce_slash);
            //mapping.Add(PoolRadAffects.resist_magic_weapon, Affects.resist_magic_weapon);
            mapping.Add(PoolRadAffects.vuln_holy_water, Affects.vuln_holy_water);
            mapping.Add(PoolRadAffects.half_cold, Affects.half_cold);
            //mapping.Add(PoolRadAffects.protect_non_magic_weapons, Affects.protect_non_magic_weapons);
            mapping.Add(PoolRadAffects.boulder_evasion, Affects.boulder_evasion);
            mapping.Add(PoolRadAffects.ankheg_ranged_acid_attack, Affects.ankheg_ranged_acid_attack);
            mapping.Add(PoolRadAffects.vuln_fire, Affects.vuln_fire);
            //mapping.Add(PoolRadAffects.resist_silver, Affects.resist_silver);
            mapping.Add(PoolRadAffects.halfelf_resistance, Affects.halfelf_resistance);
            //mapping.Add(PoolRadAffects.prot_sleep_charm_paralysis_poison, Affects.prot_sleep_charm_paralysis_poison);
            //mapping.Add(PoolRadAffects.immune_gaze_attacks, Affects.immune_gaze_attacks);
            mapping.Add(PoolRadAffects.reflectable_gaze, Affects.reflectable_gaze);
        }

        public PoolRadAffect(PoolRadAffects _type, ushort _minutes, byte _affect_data, bool _call_spell_jump_list)
        {
            type = _type;
            minutes = _minutes;
            affect_data = _affect_data;
            callAffectTable = _call_spell_jump_list;
        }

        public PoolRadAffect(byte[] data, int offset)
        {
            type = (PoolRadAffects)data[offset + 0x0];
            minutes = Sys.ArrayToUshort(data, offset + 0x1);
            affect_data = data[offset + 0x3];
            callAffectTable = (data[offset + 0x4] != 0);
        }

        public PoolRadAffect(Affect affect, Player player)
        {
            if (mapping == null) { InitMapping(); }

            type = mapping[affect.type][0];
            minutes = affect.minutes;
            if (type == PoolRadAffects.enlarge)
            {
                if (player.stats2.Str.cur == 18)
                {
                    affect_data = (byte)(player.stats2.Str00.cur + 1);
                }
                else
                {
                    affect_data = (byte)(player.stats2.Str.cur + 100);
                }
            }
            else if (type == PoolRadAffects.friends)
            {
                affect_data = (byte)player.stats2.Cha.cur;
            }
            affect_data = affect.affect_data;
            callAffectTable = affect.callAffectTable;

        }

        public void Load(Player player)
        {
            if (mapping == null) { InitMapping(); }

            if (type == PoolRadAffects.strength)
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

                player.stats2.Str.cur = str;
                player.stats2.Str00.cur = str_00;
            }
            else if (gbl.game == Game.CurseOfTheAzureBonds)
            {
                if (type == PoolRadAffects.gnome_vs_goblin_kobold ||
                    type == PoolRadAffects.dwarf_vs_orc ||
                    type == PoolRadAffects.dwarf_and_gnome_vs_giants ||
                    type == PoolRadAffects.gnome_vs_gnoll ||
                    type == PoolRadAffects.con_saving_bonus ||
                    type == PoolRadAffects.elf_resist_sleep ||
                    type == PoolRadAffects.halfelf_resistance)
                {
                    Affect affect = new Affect(mapping[type][0], minutes, affect_data, callAffectTable);
                    player.affects.Add(affect);
                }
                else if (type == PoolRadAffects.friends)
                {
                    player.stats2.Cha.cur = affect_data;
                }
                else if (type == PoolRadAffects.enlarge)
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

                    player.stats2.Str.cur = str;
                    player.stats2.Str00.cur = str_00;
                }
            }
            else if (mapping[type].Count == 1)
            {
                Affect affect = new Affect(mapping[type][0], minutes, affect_data, callAffectTable);
                player.affects.Add(affect);
            }
        }

        [DataOffset(0x00, DataType.IByte)]
        public PoolRadAffects type;
        [DataOffset(0x01, DataType.Word)]
        public ushort minutes;
        [DataOffset(0x03, DataType.Byte)]
        public byte affect_data;
        [DataOffset(0x04, DataType.Bool)]
        public bool callAffectTable;

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }
    }
}
