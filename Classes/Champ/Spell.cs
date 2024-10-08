namespace Classes.Champ
{
    public class Spell
    {
        readonly static BiLookup<Spells, Classes.Spells> spells_map = InitSpellsMap();

        private static BiLookup<Spells, Classes.Spells> InitSpellsMap()
        {
            BiLookup<Spells, Classes.Spells> map = new();

            map.Add(Spells.bless, Classes.Spells.bless);
            map.Add(Spells.curse, Classes.Spells.curse);
            map.Add(Spells.cure_light_wounds, Classes.Spells.cure_light_wounds_CL);
            map.Add(Spells.cause_light_wounds, Classes.Spells.cause_light_wounds_CL);
            map.Add(Spells.detect_magic_CL, Classes.Spells.detect_magic_CL);
            map.Add(Spells.protect_from_evil_CL, Classes.Spells.protect_from_evil_CL);
            map.Add(Spells.protect_from_good_CL, Classes.Spells.protect_from_good_CL);
            map.Add(Spells.resist_cold, Classes.Spells.resist_cold);
            map.Add(Spells.burning_hands, Classes.Spells.burning_hands);
            map.Add(Spells.charm_person, Classes.Spells.charm_person);
            map.Add(Spells.detect_magic_MU, Classes.Spells.detect_magic_MU);
            map.Add(Spells.enlarge, Classes.Spells.enlarge);
            map.Add(Spells.reduce, Classes.Spells.reduce);
            map.Add(Spells.friends, Classes.Spells.friends);
            map.Add(Spells.magic_missile, Classes.Spells.magic_missile);
            map.Add(Spells.protect_from_evil_MU, Classes.Spells.protect_from_evil_MU);
            map.Add(Spells.protect_from_good_MU, Classes.Spells.protect_from_good_MU);
            map.Add(Spells.read_magic, Classes.Spells.read_magic);
            map.Add(Spells.shield, Classes.Spells.shield);
            map.Add(Spells.shocking_grasp, Classes.Spells.shocking_grasp);
            map.Add(Spells.sleep, Classes.Spells.sleep);
            map.Add(Spells.find_traps, Classes.Spells.find_traps);
            map.Add(Spells.hold_person_CL, Classes.Spells.hold_person_CL);
            map.Add(Spells.resist_fire, Classes.Spells.resist_fire);
            map.Add(Spells.silence_15_radius, Classes.Spells.silence_15_radius);
            map.Add(Spells.slow_poison, Classes.Spells.slow_poison);
            map.Add(Spells.snake_charm, Classes.Spells.snake_charm);
            map.Add(Spells.spiritual_hammer, Classes.Spells.spiritual_hammer);
            map.Add(Spells.detect_invisibility, Classes.Spells.detect_invisibility);
            map.Add(Spells.invisibility, Classes.Spells.invisibility);
            map.Add(Spells.knock, Classes.Spells.knock);
            map.Add(Spells.mirror_image, Classes.Spells.mirror_image);
            map.Add(Spells.ray_of_enfeeblement, Classes.Spells.ray_of_enfeeblement);
            map.Add(Spells.stinking_cloud, Classes.Spells.stinking_cloud);
            map.Add(Spells.strength, Classes.Spells.strength);
            map.Add(Spells.animate_dead, Classes.Spells.animate_dead);
            map.Add(Spells.cure_blindness, Classes.Spells.cure_blindness);
            map.Add(Spells.cause_blindness, Classes.Spells.cause_blindness);
            map.Add(Spells.cure_disease, Classes.Spells.cure_disease);
            map.Add(Spells.cause_disease, Classes.Spells.cause_disease);
            map.Add(Spells.dispel_magic_CL, Classes.Spells.dispel_magic_CL);
            map.Add(Spells.prayer, Classes.Spells.prayer);
            map.Add(Spells.remove_curse_CL, Classes.Spells.remove_curse_CL);
            map.Add(Spells.bestow_curse_CL, Classes.Spells.bestow_curse_CL);
            map.Add(Spells.blink, Classes.Spells.blink);
            map.Add(Spells.dispel_magic_MU, Classes.Spells.dispel_magic_MU);
            map.Add(Spells.fireball, Classes.Spells.fireball);
            map.Add(Spells.haste, Classes.Spells.haste);
            map.Add(Spells.hold_person_MU, Classes.Spells.hold_person_MU);
            map.Add(Spells.invisibility_10_radius, Classes.Spells.invisibility_10_radius);
            map.Add(Spells.lightning_bolt, Classes.Spells.lightning_bolt);
            map.Add(Spells.protect_from_evil_10_rad, Classes.Spells.protect_from_evil_10_rad);
            map.Add(Spells.protect_from_good_10_rad, Classes.Spells.protect_from_good_10_rad);
            map.Add(Spells.protect_from_normal_missiles, Classes.Spells.protect_from_normal_missiles);
            map.Add(Spells.slow, Classes.Spells.slow);
            map.Add(Spells.restoration, Classes.Spells.restoration);
            map.Add(Spells.potion_of_speed, Classes.Spells.potion_of_speed);
            map.Add(Spells.cure_serious_wounds_CL, Classes.Spells.cure_serious_wounds_CL);
            map.Add(Spells.potion_giant_strength, Classes.Spells.potion_giant_strength);
            map.Add(Spells.spell_3c, Classes.Spells.spell_3c);
            map.Add(Spells.wand_of_paralyzation, Classes.Spells.wand_of_paralyzation);
            map.Add(Spells.spell_3e, Classes.Spells.spell_3e);
            map.Add(Spells.dust_of_disappearance, Classes.Spells.dust_of_disappearance);
            map.Add(Spells.necklace_of_missiles, Classes.Spells.necklace_of_missiles);
            map.Add(Spells.wand_of_magic_missiles, Classes.Spells.wand_of_magic_missiles);
            map.Add(Spells.cause_serious_wounds, Classes.Spells.cause_serious_wounds_CL);
            map.Add(Spells.neutralize_poison, Classes.Spells.neutralize_poison_CL);
            map.Add(Spells.poison, Classes.Spells.poison);
            map.Add(Spells.protect_evil_10_rad, Classes.Spells.protect_evil_10_rad);
            map.Add(Spells.sticks_to_snakes, Classes.Spells.sticks_to_snakes_CL);
            map.Add(Spells.cure_critical_wounds, Classes.Spells.cure_critical_wounds);
            map.Add(Spells.cause_critical_wounds, Classes.Spells.cause_critical_wounds);
            map.Add(Spells.dispel_evil, Classes.Spells.dispel_evil);
            map.Add(Spells.flame_strike, Classes.Spells.flame_strike);
            map.Add(Spells.raise_dead, Classes.Spells.raise_dead);
            map.Add(Spells.slay_living, Classes.Spells.slay_living);
            map.Add(Spells.detect_magic_DR, Classes.Spells.detect_magic_DR);
            map.Add(Spells.entangle, Classes.Spells.entangle);
            map.Add(Spells.faerie_fire, Classes.Spells.faerie_fire);
            map.Add(Spells.invisibility_to_animals, Classes.Spells.invisibility_to_animals);
            map.Add(Spells.charm_monsters, Classes.Spells.charm_monsters);
            map.Add(Spells.confusion, Classes.Spells.confusion);
            map.Add(Spells.dimension_door, Classes.Spells.dimension_door);
            map.Add(Spells.fear, Classes.Spells.fear);
            map.Add(Spells.fire_shield, Classes.Spells.fire_shield);
            map.Add(Spells.fumble, Classes.Spells.fumble);
            map.Add(Spells.ice_storm, Classes.Spells.ice_storm);
            map.Add(Spells.minor_globe_of_invuln, Classes.Spells.minor_globe_of_invuln);
            map.Add(Spells.remove_curse_MU, Classes.Spells.remove_curse_MU);
            map.Add(Spells.spell_5a, Classes.Spells.spell_5a);
            map.Add(Spells.cloud_kill, Classes.Spells.cloud_kill);
            map.Add(Spells.cone_of_cold, Classes.Spells.cone_of_cold);
            map.Add(Spells.feeblemind, Classes.Spells.feeblemind);
            map.Add(Spells.hold_monsters, Classes.Spells.hold_monsters);
            map.Add(Spells.prot_dragon_breath, Classes.Spells.prot_dragon_breath);
            map.Add(Spells.prot_paralyzation, Classes.Spells.prot_paralyzation);
            map.Add(Spells.potion_of_invisibility, Classes.Spells.potion_of_invisibility);
            map.Add(Spells.wand_of_defoliation, Classes.Spells.wand_of_defoliation);
            map.Add(Spells.potion_extra_healing, Classes.Spells.potion_extra_healing);
            map.Add(Spells.bestow_curse_MU, Classes.Spells.bestow_curse_MU);
            map.Add(Spells.unknown_10, Classes.Spells.unknown_10);

            return map;
        }

        static public void Load(SpellList spellList, byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (data[i] > 0)
                {
                    Spells id = (Spells)(data[i] & 0x7F);
                    bool learning = data[i] > 0x7F;
                    if (id > Spells.bless && id != Spells.animate_dead)
                    {
                        if (spells_map[id].Count > 0)
                        {
                            spellList.AddLearnt(spells_map[id][0], learning);
                        }
                    }
                }
            }
        }

        static public void Load(SpellBook spellBook, byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (data[i] != 0)
                {
                    Spells id = (Spells)(i+1);
                    if (spells_map[id].Count > 0)
                    {
                        spellBook.LearnSpell(spells_map[id][0]);
                    }
                }
            }
        }

        static public void Save(SpellList spellList, byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
            {
                data[i] = 0;
            }

            int idx = size - 1;
            foreach (var id in spellList.LearntList())
            {
                if (spells_map[id].Count > 0) {
                    data[idx] = (byte)spells_map[id][0];
                    idx -= 1;
                }
            }
        }
        static public void Save(SpellBook spellBook, byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
            {
                data[i] = 0;
            }

            foreach (var id in spellBook.LearntList())
            {
                if (spells_map[id].Count > 0)
                {
                    data[(byte)spells_map[id][0] - 1] = 1;
                }
            }
        }
        enum Spells
        {
            bless = 0x01,
            curse = 0x02,
            cure_light_wounds = 0x03,
            cause_light_wounds = 0x04,
            detect_magic_CL = 0x05,
            protect_from_evil_CL = 0x06,
            protect_from_good_CL = 0x07,
            resist_cold = 0x08,
            burning_hands = 0x09,
            charm_person = 0x0a,
            detect_magic_MU = 0x0b,
            enlarge = 0x0c,
            reduce = 0x0d,
            friends = 0x0e,
            magic_missile = 0x0f,
            protect_from_evil_MU = 0x10,
            protect_from_good_MU = 0x11,
            read_magic = 0x12,
            shield = 0x13,
            shocking_grasp = 0x14,
            sleep = 0x15,
            find_traps = 0x16,
            hold_person_CL = 0x17,
            resist_fire = 0x18,
            silence_15_radius = 0x19,
            slow_poison = 0x1a,
            snake_charm = 0x1b,
            spiritual_hammer = 0x1c,
            detect_invisibility = 0x1d,
            invisibility = 0x1e,
            knock = 0x1f,
            mirror_image = 0x20,
            ray_of_enfeeblement = 0x21,
            stinking_cloud = 0x22,
            strength = 0x23,
            animate_dead = 0x24,
            cure_blindness = 0x25,
            cause_blindness = 0x26,
            cure_disease = 0x27,
            cause_disease = 0x28,
            dispel_magic_CL = 0x29,
            prayer = 0x2a,
            remove_curse_CL = 0x2b,
            bestow_curse_CL = 0x2c,
            blink = 0x2d,
            dispel_magic_MU = 0x2e,
            fireball = 0x2f,
            haste = 0x30,
            hold_person_MU = 0x31,
            invisibility_10_radius = 0x32,
            lightning_bolt = 0x33,
            protect_from_evil_10_rad = 0x34,
            protect_from_good_10_rad = 0x35,
            protect_from_normal_missiles = 0x36,
            slow = 0x37,
            restoration = 0x38,
            potion_of_speed = 0x39,
            cure_serious_wounds_CL = 0x3a,
            potion_giant_strength = 0x3b,
            spell_3c = 0x3c,
            wand_of_paralyzation = 0x3d,
            spell_3e = 0x3e,
            dust_of_disappearance = 0x3f,
            necklace_of_missiles = 0x40,
            wand_of_magic_missiles = 0x41,
            cause_serious_wounds = 0x42,
            neutralize_poison = 0x43,
            poison = 0x44,
            protect_evil_10_rad = 0x45,
            sticks_to_snakes = 0x46,
            cure_critical_wounds = 0x47,
            cause_critical_wounds = 0x48,
            dispel_evil = 0x49,
            flame_strike = 0x4a,
            raise_dead = 0x4b,
            slay_living = 0x4c,
            detect_magic_DR = 0x4d,
            entangle = 0x4e,
            faerie_fire = 0x4f,
            invisibility_to_animals = 0x50,
            charm_monsters = 0x51,
            confusion = 0x52,
            dimension_door = 0x53,
            fear = 0x54,
            fire_shield = 0x55,
            fumble = 0x56,
            ice_storm = 0x57,
            minor_globe_of_invuln = 0x58,
            remove_curse_MU = 0x59,
            spell_5a = 0x5a,
            cloud_kill = 0x5b,
            cone_of_cold = 0x5c,
            feeblemind = 0x5d,
            hold_monsters = 0x5e,
            prot_dragon_breath = 0x5f,
            prot_paralyzation = 0x60,
            potion_of_invisibility = 0x61,
            wand_of_defoliation = 0x62,
            potion_extra_healing = 0x63,
            bestow_curse_MU = 0x64,
            unknown_10 = 0x65,
            //spell_88 = 0x88,
        }
    }
}