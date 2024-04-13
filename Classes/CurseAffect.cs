namespace Classes
{
    public enum CurseAffects
    {
        none = 0,
        bless = 0x1,
        cursed = 0x2,
        sticks_to_snakes = 0x3,
        dispel_evil = 0x4,
        detect_magic = 0x5,
        weap_flame_tongue = 0x6,
        faerie_fire = 0x7,
        protection_from_evil = 0x8,
        protection_from_good = 0x9,
        resist_cold = 0xa,
        charm_person = 0xb,
        enlarge = 0xc,
        reduce = 0xd,
        friends = 0xe,
        poison_damage = 0xf,
        read_magic = 0x10,
        shield = 0x11,
        gnome_vs_goblin_kobold = 0x12,
        find_traps = 0x13,
        resist_fire = 0x14,
        silence_15_radius = 0x15,
        slow_poison = 0x16,
        spiritual_hammer = 0x17,
        detect_invisibility = 0x18,
        invisibility = 0x19,
        dwarf_vs_orc_goblin = 0x1a,
        fumbling = 0x1b,
        mirror_image = 0x1c,
        ray_of_enfeeblement = 0x1d,
        stinking_cloud = 0x1e,
        helpless = 0x1f,
        animate_dead = 0x20,
        blinded = 0x21,
        cause_disease_1 = 0x22,
        confuse = 0x23,
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
        gnoll_bugbear_vs_gnome = 0x30,
        prayer = 0x31,
        hot_fire_shield = 0x32,
        snake_charm = 0x33,
        paralyze = 0x34,
        sleep = 0x35,
        cold_fire_shield = 0x36,
        poisoned = 0x37,
        item_invisibility = 0x38,
        engulf = 0x39,
        clear_movement = 0x3a,
        regenerate = 0x3b,
        resist_normal_weapons = 0x3c,
        fire_resist = 0x3d,
        highConRegen = 0x3e,
        minor_globe_of_invulnerability = 0x3f,
        poison_plus_0 = 0x40,
        poison_plus_4 = 0x41,
        poison_plus_2 = 0x42,
        thri_kreen_paralyze = 0x43,
        feeblemind = 0x44,
        invisible_to_animals = 0x45,
        poison_neg_2 = 0x46,
        invisible = 0x47,
        camouflage = 0x48,
        prot_drag_breath = 0x49,
        affect_4a = 0x4a,
        weap_dragon_slayer = 0x4b,
        weap_frost_brand = 0x4c,
        berserk = 0x4d,
        affect_4e = 0x4e,
        fireAttack_2d10 = 0x4f,
        ankheg_melee_acid_attack = 0x50,
        half_damage = 0x51,
        resist_fire_and_cold = 0x52,
        petrifying_gaze = 0x53,
        shambling_absorb_lightning = 0x54,
        resist_piercing = 0x55,
        spit_acid = 0x56,
        beholder_eyestalk = 0x57,
        breath_elec = 0x58,
        displace = 0x59,
        breath_acid = 0x5a,
        affect_in_cloud_kill = 0x5b,
        affect_5c = 0x5c,
        half_fire = 0x5d,
        resist_blunt_pierce = 0x5e,
        delay_death = 0x5f,
        owlbear_hug_check = 0x60,
        con_saving_bonus = 0x61,
        regen_3_hp = 0x62,
        fight_unconscious = 0x63,
        troll_fire_or_acid = 0x64,
        troll_regen = 0x65,
        TrollRegen = 0x66,
        salamander_heat_damage = 0x67,
        thri_kreen_dodge_missile = 0x68,
        resist_magic_50_percent = 0x69,
        resist_magic_15_percent = 0x6a,
        elf_resist_sleep = 0x6b,
        protect_charm_sleep = 0x6c,
        resist_paralyze = 0x6d,
        immune_to_cold = 0x6e,
        prot_paralysis_poison = 0x6f,
        immune_to_fire = 0x70,
        efreeti_fire_resist = 0x71,
        half_elec = 0x72,
        resist_pierce_slash = 0x73,
        resist_magic_weapon = 0x74,
        vuln_holy_water = 0x75,
        half_cold = 0x76,
        protect_non_magic_weapons = 0x77,
        boulder_evasion = 0x78,
        ankheg_ranged_acid_attack = 0x79,
        dracolich_paralysis = 0x7a,
        dracolich_cold_damage = 0x7b,
        halfelf_resistance = 0x7c,
        prot_sleep_charm_paralysis_poison = 0x7d,
        dracolich_paralytic_gaze = 0x7e,
        reflectable_gaze = 0x7f,
        breath_fire = 0x80,
        item_affect_0 = 0x80,
        protect_magic = 0x81,
        vuln_blessed_quarrel = 0x82,
        cast_breath_fire = 0x83,
        cast_throw_lightening = 0x84,
        dracolich_protection = 0x85,
        ranger_vs_giant = 0x86,
        girdle_of_dwarves = 0x86,
        protect_elec = 0x87,
        entangle = 0x88,
        confuse_berserk = 0x89,
        add_invisibility = 0x8a,
        affect_8b = 0x8b,
        paladinDailyHealCast = 0x8c,
        paladinDailyCureRefresh = 0x8d,
        fear = 0x8e,
        fire_shield_damage = 0x8f,
        owlbear_hug_round_attack = 0x90,
        dispel_evil_banish = 0x91,
        strength_spell = 0x92,
        do_items_affect = 0x93
    }

    /// <summary>
    /// Summary description for Affect.
    /// </summary>
    public class CurseAffect
    {
        public const int StructSize = 9;

        static BiLookup<CurseAffects, Affects> mapping;

        static void InitMapping()
        {
            if (mapping != null) { return; }

            mapping = new BiLookup<CurseAffects, Affects>();

            mapping.Add(CurseAffects.none, Affects.none);
            mapping.Add(CurseAffects.bless, Affects.bless);
            mapping.Add(CurseAffects.cursed, Affects.cursed);
            mapping.Add(CurseAffects.sticks_to_snakes, Affects.sticks_to_snakes);
            mapping.Add(CurseAffects.dispel_evil, Affects.dispel_evil);
            mapping.Add(CurseAffects.detect_magic, Affects.detect_magic);
            mapping.Add(CurseAffects.weap_flame_tongue, Affects.weap_flame_tongue);
            mapping.Add(CurseAffects.faerie_fire, Affects.faerie_fire);
            mapping.Add(CurseAffects.protection_from_evil, Affects.protection_from_evil);
            mapping.Add(CurseAffects.protection_from_good, Affects.protection_from_good);
            mapping.Add(CurseAffects.resist_cold, Affects.resist_cold);
            mapping.Add(CurseAffects.charm_person, Affects.charm_person);
            mapping.Add(CurseAffects.enlarge, Affects.enlarge);
            mapping.Add(CurseAffects.reduce, Affects.reduce);
            mapping.Add(CurseAffects.friends, Affects.friends);
            mapping.Add(CurseAffects.poison_damage, Affects.poison_damage);
            mapping.Add(CurseAffects.read_magic, Affects.read_magic);
            mapping.Add(CurseAffects.shield, Affects.shield);
            mapping.Add(CurseAffects.gnome_vs_goblin_kobold, Affects.gnome_vs_goblin_kobold);
            mapping.Add(CurseAffects.find_traps, Affects.find_traps);
            mapping.Add(CurseAffects.resist_fire, Affects.resist_fire);
            mapping.Add(CurseAffects.silence_15_radius, Affects.silence_15_radius);
            mapping.Add(CurseAffects.slow_poison, Affects.slow_poison);
            mapping.Add(CurseAffects.spiritual_hammer, Affects.spiritual_hammer);
            mapping.Add(CurseAffects.detect_invisibility, Affects.detect_invisibility);
            mapping.Add(CurseAffects.invisibility, Affects.invisibility);
            mapping.Add(CurseAffects.dwarf_vs_orc_goblin, Affects.dwarf_vs_orc_goblin);
            mapping.Add(CurseAffects.fumbling, Affects.fumbling);
            mapping.Add(CurseAffects.mirror_image, Affects.mirror_image);
            mapping.Add(CurseAffects.ray_of_enfeeblement, Affects.ray_of_enfeeblement);
            mapping.Add(CurseAffects.stinking_cloud, Affects.stinking_cloud);
            mapping.Add(CurseAffects.helpless, Affects.helpless);
            mapping.Add(CurseAffects.animate_dead, Affects.animate_dead);
            mapping.Add(CurseAffects.blinded, Affects.blinded);
            mapping.Add(CurseAffects.cause_disease_1, Affects.cause_disease_1);
            mapping.Add(CurseAffects.confuse, Affects.confuse);
            mapping.Add(CurseAffects.bestow_curse, Affects.bestow_curse);
            mapping.Add(CurseAffects.blink, Affects.blink);
            mapping.Add(CurseAffects.strength, Affects.strength);
            mapping.Add(CurseAffects.haste, Affects.haste);
            mapping.Add(CurseAffects.affect_in_stinking_cloud, Affects.affect_in_stinking_cloud);
            mapping.Add(CurseAffects.prot_from_normal_missiles, Affects.prot_from_normal_missiles);
            mapping.Add(CurseAffects.slow, Affects.slow);
            mapping.Add(CurseAffects.weaken, Affects.weaken);
            mapping.Add(CurseAffects.cause_disease_2, Affects.cause_disease_2);
            mapping.Add(CurseAffects.prot_from_evil_10_radius, Affects.prot_from_evil_10_radius);
            mapping.Add(CurseAffects.prot_from_good_10_radius, Affects.prot_from_good_10_radius);
            mapping.Add(CurseAffects.dwarf_and_gnome_vs_giants, Affects.giant_vs_dwarf_gnome);
            mapping.Add(CurseAffects.gnoll_bugbear_vs_gnome, Affects.gnoll_bugbear_vs_gnome);
            mapping.Add(CurseAffects.prayer, Affects.prayer);
            mapping.Add(CurseAffects.hot_fire_shield, Affects.hot_fire_shield);
            mapping.Add(CurseAffects.snake_charm, Affects.snake_charm);
            mapping.Add(CurseAffects.paralyze, Affects.paralyze);
            mapping.Add(CurseAffects.sleep, Affects.sleep);
            mapping.Add(CurseAffects.cold_fire_shield, Affects.cold_fire_shield);
            mapping.Add(CurseAffects.poisoned, Affects.poisoned);
            mapping.Add(CurseAffects.item_invisibility, Affects.item_invisibility);
            mapping.Add(CurseAffects.engulf, Affects.engulf);
            mapping.Add(CurseAffects.clear_movement, Affects.clear_movement);
            mapping.Add(CurseAffects.regenerate, Affects.regenerate);
            mapping.Add(CurseAffects.resist_normal_weapons, Affects.resist_normal_weapons);
            mapping.Add(CurseAffects.fire_resist, Affects.fire_resist);
            mapping.Add(CurseAffects.highConRegen, Affects.highConRegen);
            mapping.Add(CurseAffects.minor_globe_of_invulnerability, Affects.minor_globe_of_invulnerability);
            mapping.Add(CurseAffects.poison_plus_0, Affects.poison_plus_0);
            mapping.Add(CurseAffects.poison_plus_4, Affects.poison_plus_4);
            mapping.Add(CurseAffects.poison_plus_2, Affects.poison_plus_2);
            mapping.Add(CurseAffects.thri_kreen_paralyze, Affects.thri_kreen_paralyze);
            mapping.Add(CurseAffects.feeblemind, Affects.feeblemind);
            mapping.Add(CurseAffects.invisible_to_animals, Affects.invisible_to_animals);
            mapping.Add(CurseAffects.poison_neg_2, Affects.poison_neg_2);
            mapping.Add(CurseAffects.invisible, Affects.invisible);
            mapping.Add(CurseAffects.camouflage, Affects.camouflage);
            mapping.Add(CurseAffects.prot_drag_breath, Affects.prot_drag_breath);
            mapping.Add(CurseAffects.affect_4a, Affects.affect_4a);
            mapping.Add(CurseAffects.weap_dragon_slayer, Affects.weap_dragon_slayer);
            mapping.Add(CurseAffects.weap_frost_brand, Affects.weap_frost_brand);
            mapping.Add(CurseAffects.berserk, Affects.berserk);
            mapping.Add(CurseAffects.affect_4e, Affects.affect_4e);
            mapping.Add(CurseAffects.fireAttack_2d10, Affects.fireAttack_2d10);
            mapping.Add(CurseAffects.ankheg_melee_acid_attack, Affects.ankheg_melee_acid_attack);
            mapping.Add(CurseAffects.half_damage, Affects.half_damage);
            mapping.Add(CurseAffects.resist_fire_and_cold, Affects.resist_fire_and_cold);
            mapping.Add(CurseAffects.petrifying_gaze, Affects.petrifying_gaze);
            mapping.Add(CurseAffects.shambling_absorb_lightning, Affects.shambling_absorb_lightning);
            mapping.Add(CurseAffects.resist_piercing, Affects.resist_piercing);
            mapping.Add(CurseAffects.spit_acid, Affects.spit_acid);
            mapping.Add(CurseAffects.beholder_eyestalk, Affects.beholder_eyestalk);
            mapping.Add(CurseAffects.breath_elec, Affects.breath_elec);
            mapping.Add(CurseAffects.displace, Affects.displace);
            mapping.Add(CurseAffects.breath_acid, Affects.breath_acid);
            mapping.Add(CurseAffects.affect_in_cloud_kill, Affects.affect_in_cloud_kill);
            mapping.Add(CurseAffects.affect_5c, Affects.affect_5c);
            mapping.Add(CurseAffects.half_fire, Affects.half_fire);
            mapping.Add(CurseAffects.resist_blunt_pierce, Affects.resist_blunt_pierce);
            mapping.Add(CurseAffects.delay_death, Affects.delay_death);
            mapping.Add(CurseAffects.owlbear_hug_check, Affects.owlbear_hug_check);
            mapping.Add(CurseAffects.con_saving_bonus, Affects.con_saving_bonus);
            mapping.Add(CurseAffects.regen_3_hp, Affects.regen_3_hp);
            mapping.Add(CurseAffects.fight_unconscious, Affects.fight_unconscious);
            mapping.Add(CurseAffects.troll_fire_or_acid, Affects.troll_fire_or_acid);
            mapping.Add(CurseAffects.troll_regen, Affects.troll_regen);
            mapping.Add(CurseAffects.TrollRegen, Affects.TrollRegen);
            mapping.Add(CurseAffects.salamander_heat_damage, Affects.salamander_heat_damage);
            mapping.Add(CurseAffects.thri_kreen_dodge_missile, Affects.thri_kreen_dodge_missile);
            mapping.Add(CurseAffects.resist_magic_50_percent, Affects.resist_magic_50_percent);
            mapping.Add(CurseAffects.resist_magic_15_percent, Affects.resist_magic_15_percent);
            mapping.Add(CurseAffects.elf_resist_sleep, Affects.elf_resist_sleep);
            mapping.Add(CurseAffects.protect_charm_sleep, Affects.protect_charm_sleep);
            mapping.Add(CurseAffects.resist_paralyze, Affects.resist_paralyze);
            mapping.Add(CurseAffects.immune_to_cold, Affects.immune_to_cold);
            mapping.Add(CurseAffects.prot_paralysis_poison, Affects.prot_paralysis_poison);
            mapping.Add(CurseAffects.immune_to_fire, Affects.immune_to_fire);
            mapping.Add(CurseAffects.efreeti_fire_resist, Affects.efreeti_fire_resist);
            mapping.Add(CurseAffects.half_elec, Affects.half_elec);
            mapping.Add(CurseAffects.resist_pierce_slash, Affects.resist_pierce_slash);
            mapping.Add(CurseAffects.resist_magic_weapon, Affects.resist_magic_weapon);
            mapping.Add(CurseAffects.vuln_holy_water, Affects.vuln_holy_water);
            mapping.Add(CurseAffects.half_cold, Affects.half_cold);
            mapping.Add(CurseAffects.protect_non_magic_weapons, Affects.protect_non_magic_weapons);
            mapping.Add(CurseAffects.boulder_evasion, Affects.boulder_evasion);
            mapping.Add(CurseAffects.ankheg_ranged_acid_attack, Affects.ankheg_ranged_acid_attack);
            mapping.Add(CurseAffects.dracolich_paralysis, Affects.dracolich_paralysis);
            mapping.Add(CurseAffects.dracolich_cold_damage, Affects.dracolich_cold_damage);
            mapping.Add(CurseAffects.halfelf_resistance, Affects.halfelf_resistance);
            mapping.Add(CurseAffects.prot_sleep_charm_paralysis_poison, Affects.prot_sleep_charm_paralysis_poison);
            mapping.Add(CurseAffects.dracolich_paralytic_gaze, Affects.dracolich_paralytic_gaze);
            mapping.Add(CurseAffects.reflectable_gaze, Affects.reflectable_gaze);
            mapping.Add(CurseAffects.breath_fire, Affects.breath_fire);
            mapping.Add(CurseAffects.item_affect_0, Affects.item_affect_0);
            mapping.Add(CurseAffects.protect_magic, Affects.protect_magic);
            mapping.Add(CurseAffects.vuln_blessed_quarrel, Affects.vuln_blessed_quarrel);
            mapping.Add(CurseAffects.cast_breath_fire, Affects.cast_breath_fire);
            mapping.Add(CurseAffects.cast_throw_lightening, Affects.cast_throw_lightening);
            mapping.Add(CurseAffects.dracolich_protection, Affects.dracolich_protection);
            mapping.Add(CurseAffects.ranger_vs_giant, Affects.ranger_vs_giant);
            mapping.Add(CurseAffects.protect_elec, Affects.protect_elec);
            mapping.Add(CurseAffects.entangle, Affects.entangle);
            mapping.Add(CurseAffects.confuse_berserk, Affects.confuse_berserk);
            mapping.Add(CurseAffects.add_invisibility, Affects.add_invisibility);
            mapping.Add(CurseAffects.affect_8b, Affects.affect_8b);
            mapping.Add(CurseAffects.paladinDailyHealCast, Affects.paladinDailyHealCast);
            mapping.Add(CurseAffects.paladinDailyCureRefresh, Affects.paladinDailyCureRefresh);
            mapping.Add(CurseAffects.fear, Affects.fear);
            mapping.Add(CurseAffects.fire_shield_damage, Affects.fire_shield_damage);
            mapping.Add(CurseAffects.owlbear_hug_round_attack, Affects.owlbear_hug_round_attack);
            mapping.Add(CurseAffects.dispel_evil_banish, Affects.dispel_evil_banish);
            mapping.Add(CurseAffects.strength_spell, Affects.strength_spell);
            mapping.Add(CurseAffects.do_items_affect, Affects.do_items_affect);
        }

        public CurseAffect(CurseAffects _type, ushort _minutes, byte _affect_data, bool _call_spell_jump_list)
        {
            type = _type;
            minutes = _minutes;
            affect_data = _affect_data;
            callAffectTable = _call_spell_jump_list;
        }

        public CurseAffect(byte[] data, int offset)
        {
            type = (CurseAffects)data[offset + 0x0];
            minutes = Sys.ArrayToUshort(data, offset + 0x1);
            affect_data = data[offset + 0x3];
            callAffectTable = (data[offset + 0x4] != 0);
        }

        public CurseAffect(Affect affect, Player player)
        {
            if (mapping == null) { InitMapping(); }

            type = mapping[affect.type][0];
            minutes = affect.minutes;
            affect_data = affect.affect_data;
            callAffectTable = affect.callAffectTable;
        }

        public void Load(Player player)
        {
            if (mapping == null) { InitMapping(); }

            Affect affect = new Affect(mapping[type][0], minutes, affect_data, callAffectTable);

            player.affects.Add(affect);
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }

        [DataOffset(0x00, DataType.IByte)]
        public CurseAffects type;
        [DataOffset(0x01, DataType.Word)]
        public ushort minutes;
        [DataOffset(0x03, DataType.Byte)]
        public byte affect_data;
        [DataOffset(0x04, DataType.Bool)]
        public bool callAffectTable;
    }
}
