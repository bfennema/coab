namespace Classes.Secret
{
    /// <summary>
    /// Summary description for Affect.
    /// </summary>
    public class Affect
    {
        [DataOffset(0x00, DataType.IByte)]
        Affects type;
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

            map.Add(Affects.none, Classes.Affects.none);
            map.Add(Affects.bless, Classes.Affects.bless);
            map.Add(Affects.cursed, Classes.Affects.cursed);
            map.Add(Affects.sticks_to_snakes, Classes.Affects.sticks_to_snakes);
            map.Add(Affects.dispel_evil, Classes.Affects.dispel_evil);
            map.Add(Affects.detect_magic, Classes.Affects.detect_magic);
            map.Add(Affects.weap_flame_tongue, Classes.Affects.weap_flame_tongue);
            map.Add(Affects.faerie_fire, Classes.Affects.faerie_fire);
            map.Add(Affects.protection_from_evil, Classes.Affects.protection_from_evil);
            map.Add(Affects.protection_from_good, Classes.Affects.protection_from_good);
            map.Add(Affects.spell_resist_cold, Classes.Affects.spell_resist_cold);
            map.Add(Affects.charm_person, Classes.Affects.charm_person);
            map.Add(Affects.enlarge, Classes.Affects.enlarge);
            map.Add(Affects.reduce, Classes.Affects.reduce);
            map.Add(Affects.friends, Classes.Affects.friends);
            map.Add(Affects.poison_damage, Classes.Affects.poison_damage);
            map.Add(Affects.read_magic, Classes.Affects.read_magic);
            map.Add(Affects.shield, Classes.Affects.shield);
            map.Add(Affects.gnome_vs_goblin_kobold, Classes.Affects.gnome_vs_goblin_kobold);
            map.Add(Affects.find_traps, Classes.Affects.find_traps);
            map.Add(Affects.spell_resist_fire, Classes.Affects.spell_resist_fire);
            map.Add(Affects.silence_15_radius, Classes.Affects.silence_15_radius);
            map.Add(Affects.slow_poison, Classes.Affects.slow_poison);
            map.Add(Affects.spiritual_hammer, Classes.Affects.spiritual_hammer);
            map.Add(Affects.detect_invisibility, Classes.Affects.detect_invisibility);
            map.Add(Affects.invisibility, Classes.Affects.invisibility);
            map.Add(Affects.dwarf_vs_orc_goblin, Classes.Affects.dwarf_vs_orc_goblin);
            map.Add(Affects.fumbling, Classes.Affects.fumbling);
            map.Add(Affects.mirror_image, Classes.Affects.mirror_image);
            map.Add(Affects.ray_of_enfeeblement, Classes.Affects.ray_of_enfeeblement);
            map.Add(Affects.stinking_cloud, Classes.Affects.stinking_cloud);
            map.Add(Affects.helpless, Classes.Affects.helpless);
            map.Add(Affects.animate_dead, Classes.Affects.animate_dead);
            map.Add(Affects.blinded, Classes.Affects.blinded);
            map.Add(Affects.cause_disease_1, Classes.Affects.cause_disease_1);
            map.Add(Affects.confuse, Classes.Affects.confuse);
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
            map.Add(Affects.dwarf_and_gnome_vs_giants, Classes.Affects.giant_vs_dwarf_gnome);
            map.Add(Affects.gnoll_bugbear_vs_gnome, Classes.Affects.gnoll_bugbear_vs_gnome);
            map.Add(Affects.prayer, Classes.Affects.prayer);
            map.Add(Affects.hot_fire_shield, Classes.Affects.hot_fire_shield);
            map.Add(Affects.snake_charm, Classes.Affects.snake_charm);
            map.Add(Affects.paralyze, Classes.Affects.paralyze);
            map.Add(Affects.sleep, Classes.Affects.sleep);
            map.Add(Affects.cold_fire_shield, Classes.Affects.cold_fire_shield);
            map.Add(Affects.poisoned, Classes.Affects.poisoned);
            map.Add(Affects.item_invisibility, Classes.Affects.item_invisibility);
            map.Add(Affects.engulf, Classes.Affects.engulf);
            map.Add(Affects.clear_movement, Classes.Affects.clear_movement);
            map.Add(Affects.regenerate, Classes.Affects.regenerate);
            map.Add(Affects.resist_normal_weapons, Classes.Affects.resist_normal_weapons);
            map.Add(Affects.item_fire_resist, Classes.Affects.item_fire_resist);
            map.Add(Affects.highConRegen, Classes.Affects.highConRegen);
            map.Add(Affects.minor_globe_of_invulnerability, Classes.Affects.minor_globe_of_invulnerability);
            map.Add(Affects.poison_plus_0, Classes.Affects.poison_plus_0);
            map.Add(Affects.poison_plus_4, Classes.Affects.poison_plus_4);
            map.Add(Affects.poison_plus_2, Classes.Affects.poison_plus_2);
            map.Add(Affects.thri_kreen_paralyze, Classes.Affects.thri_kreen_paralyze);
            map.Add(Affects.feeblemind, Classes.Affects.feeblemind);
            map.Add(Affects.invisible_to_animals, Classes.Affects.invisible_to_animals);
            map.Add(Affects.poison_neg_2, Classes.Affects.poison_neg_2);
            map.Add(Affects.invisible, Classes.Affects.invisible);
            map.Add(Affects.camouflage, Classes.Affects.camouflage);
            map.Add(Affects.prot_drag_breath, Classes.Affects.prot_drag_breath);
            map.Add(Affects.affect_4a, Classes.Affects.affect_4a);
            map.Add(Affects.weap_dragon_slayer, Classes.Affects.weap_dragon_slayer);
            //map.Add(Affects.periapt_health, Classes.Affects.periapt_health);
            map.Add(Affects.berserk, Classes.Affects.berserk);
            //map.Add(Affects.stone_good_luck, Classes.Affects.stone_good_luck);
            map.Add(Affects.fireAttack_2d10, Classes.Affects.fireAttack_2d10);
            map.Add(Affects.ankheg_melee_acid_attack, Classes.Affects.ankheg_melee_acid_attack);
            map.Add(Affects.half_damage, Classes.Affects.half_damage);
            map.Add(Affects.resist_fire_and_cold, Classes.Affects.resist_fire_and_cold);
            map.Add(Affects.petrifying_gaze, Classes.Affects.petrifying_gaze);
            map.Add(Affects.shambling_absorb_lightning, Classes.Affects.shambling_absorb_lightning);
            map.Add(Affects.resist_piercing, Classes.Affects.resist_piercing);
            map.Add(Affects.spit_acid, Classes.Affects.spit_acid);
            map.Add(Affects.beholder_eyestalk, Classes.Affects.beholder_eyestalk);
            map.Add(Affects.breath_elec, Classes.Affects.breath_elec);
            map.Add(Affects.displace, Classes.Affects.displace);
            map.Add(Affects.breath_acid, Classes.Affects.breath_acid);
            map.Add(Affects.affect_in_cloud_kill, Classes.Affects.affect_in_cloud_kill);
            map.Add(Affects.affect_5c, Classes.Affects.affect_5c);
            map.Add(Affects.half_fire, Classes.Affects.half_fire);
            map.Add(Affects.resist_blunt_pierce, Classes.Affects.resist_blunt_pierce);
            map.Add(Affects.delay_death, Classes.Affects.delay_death);
            map.Add(Affects.owlbear_hug_check, Classes.Affects.owlbear_hug_check);
            map.Add(Affects.con_saving_bonus, Classes.Affects.con_saving_bonus);
            map.Add(Affects.regen_3_hp, Classes.Affects.regen_3_hp);
            map.Add(Affects.fight_unconscious, Classes.Affects.fight_unconscious);
            map.Add(Affects.troll_fire_or_acid, Classes.Affects.troll_fire_or_acid);
            map.Add(Affects.troll_regen, Classes.Affects.troll_regen);
            map.Add(Affects.TrollRegen, Classes.Affects.TrollRegen);
            map.Add(Affects.salamander_heat_damage, Classes.Affects.salamander_heat_damage);
            map.Add(Affects.thri_kreen_dodge_missile, Classes.Affects.thri_kreen_dodge_missile);
            map.Add(Affects.resist_magic_50_percent, Classes.Affects.resist_magic_50_percent);
            map.Add(Affects.resist_magic_15_percent, Classes.Affects.resist_magic_15_percent);
            map.Add(Affects.elf_resist_sleep, Classes.Affects.elf_resist_sleep);
            map.Add(Affects.protect_charm_sleep, Classes.Affects.protect_charm_sleep);
            map.Add(Affects.prot_paralysis_poison, Classes.Affects.prot_paralysis_poison);
            map.Add(Affects.immune_to_fire, Classes.Affects.immune_to_fire);
            map.Add(Affects.efreeti_fire_resist, Classes.Affects.efreeti_fire_resist);
            map.Add(Affects.half_elec, Classes.Affects.half_elec);
            map.Add(Affects.resist_pierce_slash, Classes.Affects.resist_pierce_slash);
            map.Add(Affects.resist_magic_weapon, Classes.Affects.resist_magic_weapon);
            map.Add(Affects.vuln_holy_water, Classes.Affects.vuln_holy_water);
            map.Add(Affects.half_cold, Classes.Affects.half_cold);
            map.Add(Affects.protect_non_magic_weapons, Classes.Affects.protect_non_magic_weapons);
            map.Add(Affects.boulder_evasion, Classes.Affects.boulder_evasion);
            map.Add(Affects.ankheg_ranged_acid_attack, Classes.Affects.ankheg_ranged_acid_attack);
            map.Add(Affects.dracolich_paralysis, Classes.Affects.dracolich_paralysis);
            map.Add(Affects.dracolich_cold_damage, Classes.Affects.dracolich_cold_damage);
            map.Add(Affects.halfelf_resistance, Classes.Affects.halfelf_resistance);
            map.Add(Affects.prot_sleep_charm_paralysis_poison, Classes.Affects.prot_sleep_charm_paralysis_poison);
            map.Add(Affects.dracolich_paralytic_gaze, Classes.Affects.dracolich_paralytic_gaze);
            map.Add(Affects.reflectable_gaze, Classes.Affects.reflectable_gaze);
            map.Add(Affects.breath_fire, Classes.Affects.breath_fire);
            map.Add(Affects.item_affect, Classes.Affects.item_affect);
            map.Add(Affects.protect_magic, Classes.Affects.protect_magic);
            map.Add(Affects.vuln_blessed_quarrel, Classes.Affects.vuln_blessed_quarrel);
            map.Add(Affects.cast_breath_fire, Classes.Affects.cast_breath_fire);
            map.Add(Affects.cast_throw_lightening, Classes.Affects.cast_throw_lightening);
            map.Add(Affects.dracolich_protection, Classes.Affects.dracolich_protection);
            map.Add(Affects.ranger_vs_giant, Classes.Affects.ranger_vs_giant);
            map.Add(Affects.protect_elec, Classes.Affects.protect_elec);
            map.Add(Affects.entangle, Classes.Affects.entangle);
            map.Add(Affects.confuse_berserk, Classes.Affects.confuse_berserk);
            map.Add(Affects.add_invisibility, Classes.Affects.add_invisibility);
            map.Add(Affects.affect_8b, Classes.Affects.affect_8b);
            map.Add(Affects.paladinDailyHealCast, Classes.Affects.paladinDailyHealCast);
            map.Add(Affects.paladinDailyCureRefresh, Classes.Affects.paladinDailyCureRefresh);
            map.Add(Affects.fear, Classes.Affects.fear);
            map.Add(Affects.fire_shield_damage, Classes.Affects.fire_shield_damage);
            map.Add(Affects.owlbear_hug_round_attack, Classes.Affects.owlbear_hug_round_attack);
            map.Add(Affects.dispel_evil_banish, Classes.Affects.dispel_evil_banish);
            map.Add(Affects.strength_spell, Classes.Affects.strength_spell);
            map.Add(Affects.do_items_affect, Classes.Affects.do_items_affect);

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
            type = mapping[affect.type][0];
            minutes = affect.minutes;
            affect_data = affect.affect_data;
            callAffectTable = affect.callAffectTable;
        }

        public void Load(Classes.Player player)
        {
            Classes.Affect affect = new Classes.Affect(mapping[type][0], minutes, affect_data, callAffectTable);

            player.affects.Add(affect);
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }

        public enum Affects
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
            spell_resist_cold = 0xa,
            charm_person = 0xb,
            enlarge = 0xc,
            reduce = 0xd,
            friends = 0xe,
            poison_damage = 0xf,
            read_magic = 0x10,
            shield = 0x11,
            gnome_vs_goblin_kobold = 0x12,
            find_traps = 0x13,
            spell_resist_fire = 0x14,
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
            item_fire_resist = 0x3d,
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
            periapt_health = 0x4c,
            berserk = 0x4d,
            stone_good_luck = 0x4e,
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
            paladinDailyHealCast = 0x6d,
            paladinDailyCureRefresh = 0x6e,
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
            item_affect = 0x80,
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
            affect_8c = 0x8c,
            affect_8d = 0x8d,
            fear = 0x8e,
            fire_shield_damage = 0x8f,
            owlbear_hug_round_attack = 0x90,
            dispel_evil_banish = 0x91,
            strength_spell = 0x92,
            do_items_affect = 0x93
        }
    }
}
