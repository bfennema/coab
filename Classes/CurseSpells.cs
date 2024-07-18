namespace Classes
{
    public class CurseSpells
    {
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

        static BiLookup<Spells, Classes.Spells> mapping;

        static void InitMapping()
        {
            if (mapping != null) { return; }

            mapping = new BiLookup<Spells, Classes.Spells>();

            mapping.Add(Spells.bless, Classes.Spells.bless);
            mapping.Add(Spells.curse, Classes.Spells.curse);
            mapping.Add(Spells.cure_light_wounds, Classes.Spells.cure_light_wounds_CL);
            mapping.Add(Spells.cause_light_wounds, Classes.Spells.cause_light_wounds_CL);
            mapping.Add(Spells.detect_magic_CL, Classes.Spells.detect_magic_CL);
            mapping.Add(Spells.protect_from_evil_CL, Classes.Spells.protect_from_evil_CL);
            mapping.Add(Spells.protect_from_good_CL, Classes.Spells.protect_from_good_CL);
            mapping.Add(Spells.resist_cold, Classes.Spells.resist_cold);
            mapping.Add(Spells.burning_hands, Classes.Spells.burning_hands);
            mapping.Add(Spells.charm_person, Classes.Spells.charm_person);
            mapping.Add(Spells.detect_magic_MU, Classes.Spells.detect_magic_MU);
            mapping.Add(Spells.enlarge, Classes.Spells.enlarge);
            mapping.Add(Spells.reduce, Classes.Spells.reduce);
            mapping.Add(Spells.friends, Classes.Spells.friends);
            mapping.Add(Spells.magic_missile, Classes.Spells.magic_missile);
            mapping.Add(Spells.protect_from_evil_MU, Classes.Spells.protect_from_evil_MU);
            mapping.Add(Spells.protect_from_good_MU, Classes.Spells.protect_from_good_MU);
            mapping.Add(Spells.read_magic, Classes.Spells.read_magic);
            mapping.Add(Spells.shield, Classes.Spells.shield);
            mapping.Add(Spells.shocking_grasp, Classes.Spells.shocking_grasp);
            mapping.Add(Spells.sleep, Classes.Spells.sleep);
            mapping.Add(Spells.find_traps, Classes.Spells.find_traps);
            mapping.Add(Spells.hold_person_CL, Classes.Spells.hold_person_CL);
            mapping.Add(Spells.resist_fire, Classes.Spells.resist_fire);
            mapping.Add(Spells.silence_15_radius, Classes.Spells.silence_15_radius);
            mapping.Add(Spells.slow_poison, Classes.Spells.slow_poison);
            mapping.Add(Spells.snake_charm, Classes.Spells.snake_charm);
            mapping.Add(Spells.spiritual_hammer, Classes.Spells.spiritual_hammer);
            mapping.Add(Spells.detect_invisibility, Classes.Spells.detect_invisibility);
            mapping.Add(Spells.invisibility, Classes.Spells.invisibility);
            mapping.Add(Spells.knock, Classes.Spells.knock);
            mapping.Add(Spells.mirror_image, Classes.Spells.mirror_image);
            mapping.Add(Spells.ray_of_enfeeblement, Classes.Spells.ray_of_enfeeblement);
            mapping.Add(Spells.stinking_cloud, Classes.Spells.stinking_cloud);
            mapping.Add(Spells.strength, Classes.Spells.strength);
            mapping.Add(Spells.animate_dead, Classes.Spells.animate_dead);
            mapping.Add(Spells.cure_blindness, Classes.Spells.cure_blindness);
            mapping.Add(Spells.cause_blindness, Classes.Spells.cause_blindness);
            mapping.Add(Spells.cure_disease, Classes.Spells.cure_disease_CL);
            mapping.Add(Spells.cause_disease, Classes.Spells.cause_disease_CL);
            mapping.Add(Spells.dispel_magic_CL, Classes.Spells.dispel_magic_CL);
            mapping.Add(Spells.prayer, Classes.Spells.prayer);
            mapping.Add(Spells.remove_curse_CL, Classes.Spells.remove_curse_CL);
            mapping.Add(Spells.bestow_curse_CL, Classes.Spells.bestow_curse_CL);
            mapping.Add(Spells.blink, Classes.Spells.blink);
            mapping.Add(Spells.dispel_magic_MU, Classes.Spells.dispel_magic_MU);
            mapping.Add(Spells.fireball, Classes.Spells.fireball);
            mapping.Add(Spells.haste, Classes.Spells.haste);
            mapping.Add(Spells.hold_person_MU, Classes.Spells.hold_person_MU);
            mapping.Add(Spells.invisibility_10_radius, Classes.Spells.invisibility_10_radius);
            mapping.Add(Spells.lightning_bolt, Classes.Spells.lightning_bolt);
            mapping.Add(Spells.protect_from_evil_10_rad, Classes.Spells.protect_from_evil_10_rad);
            mapping.Add(Spells.protect_from_good_10_rad, Classes.Spells.protect_from_good_10_rad);
            mapping.Add(Spells.protect_from_normal_missiles, Classes.Spells.protect_from_normal_missiles);
            mapping.Add(Spells.slow, Classes.Spells.slow);
            mapping.Add(Spells.restoration, Classes.Spells.restoration);
            mapping.Add(Spells.potion_of_speed, Classes.Spells.potion_of_speed);
            mapping.Add(Spells.cure_serious_wounds_CL, Classes.Spells.cure_serious_wounds_CL);
            mapping.Add(Spells.potion_giant_strength, Classes.Spells.potion_giant_strength);
            mapping.Add(Spells.spell_3c, Classes.Spells.spell_3c);
            mapping.Add(Spells.wand_of_paralyzation, Classes.Spells.wand_of_paralyzation);
            mapping.Add(Spells.spell_3e, Classes.Spells.spell_3e);
            mapping.Add(Spells.dust_of_disappearance, Classes.Spells.dust_of_disappearance);
            mapping.Add(Spells.necklace_of_missiles, Classes.Spells.necklace_of_missiles);
            mapping.Add(Spells.wand_of_magic_missiles, Classes.Spells.wand_of_magic_missiles);
            mapping.Add(Spells.cause_serious_wounds, Classes.Spells.cause_serious_wounds_CL);
            mapping.Add(Spells.neutralize_poison, Classes.Spells.neutralize_poison_CL);
            mapping.Add(Spells.poison, Classes.Spells.poison);
            mapping.Add(Spells.protect_evil_10_rad, Classes.Spells.protect_evil_10_rad);
            mapping.Add(Spells.sticks_to_snakes, Classes.Spells.sticks_to_snakes_CL);
            mapping.Add(Spells.cure_critical_wounds, Classes.Spells.cure_critical_wounds);
            mapping.Add(Spells.cause_critical_wounds, Classes.Spells.cause_critical_wounds);
            mapping.Add(Spells.dispel_evil, Classes.Spells.dispel_evil);
            mapping.Add(Spells.flame_strike, Classes.Spells.flame_strike);
            mapping.Add(Spells.raise_dead, Classes.Spells.raise_dead);
            mapping.Add(Spells.slay_living, Classes.Spells.slay_living);
            mapping.Add(Spells.detect_magic_DR, Classes.Spells.detect_magic_DR);
            mapping.Add(Spells.entangle, Classes.Spells.entangle);
            mapping.Add(Spells.faerie_fire, Classes.Spells.faerie_fire);
            mapping.Add(Spells.invisibility_to_animals, Classes.Spells.invisibility_to_animals);
            mapping.Add(Spells.charm_monsters, Classes.Spells.charm_monsters);
            mapping.Add(Spells.confusion, Classes.Spells.confusion);
            mapping.Add(Spells.dimension_door, Classes.Spells.dimension_door);
            mapping.Add(Spells.fear, Classes.Spells.fear);
            mapping.Add(Spells.fire_shield, Classes.Spells.fire_shield);
            mapping.Add(Spells.fumble, Classes.Spells.fumble);
            mapping.Add(Spells.ice_storm, Classes.Spells.ice_storm);
            mapping.Add(Spells.minor_globe_of_invuln, Classes.Spells.minor_globe_of_invuln);
            mapping.Add(Spells.remove_curse_MU, Classes.Spells.remove_curse_MU);
            mapping.Add(Spells.spell_5a, Classes.Spells.spell_5a);
            mapping.Add(Spells.cloud_kill, Classes.Spells.cloud_kill);
            mapping.Add(Spells.cone_of_cold, Classes.Spells.cone_of_cold);
            mapping.Add(Spells.feeblemind, Classes.Spells.feeblemind);
            mapping.Add(Spells.hold_monsters, Classes.Spells.hold_monsters);
            mapping.Add(Spells.prot_dragon_breath, Classes.Spells.prot_dragon_breath);
            mapping.Add(Spells.prot_paralyzation, Classes.Spells.prot_paralyzation);
            mapping.Add(Spells.potion_of_invisibility, Classes.Spells.potion_of_invisibility);
            mapping.Add(Spells.wand_of_defoliation, Classes.Spells.wand_of_defoliation);
            mapping.Add(Spells.potion_extra_healing, Classes.Spells.potion_extra_healing);
            mapping.Add(Spells.bestow_curse_MU, Classes.Spells.bestow_curse_MU);
            mapping.Add(Spells.unknown_10, Classes.Spells.unknown_10);
        }

        static public void Load(SpellList spellList, byte[] data, int size)
        {
            if (mapping == null) { InitMapping(); }

            for (int i = 0; i < size; i++)
            {
                if (data[i] > 0)
                {
                    Spells id = (Spells)(data[i] & 0x7F);
                    bool learning = data[i] > 0x7F;
                    if (id > Spells.bless && id != Spells.animate_dead)
                    {
                        if (mapping[id].Count > 0)
                        {
                            spellList.AddLearnt(mapping[id][0], learning);
                        }
                    }
                }
            }
        }

        static public void Load(SpellBook spellBook, byte[] data, int size)
        {
            if (mapping == null) { InitMapping(); }

            for (int i = 0; i < size; i++)
            {
                if (data[i] != 0)
                {
                    Spells id = (Spells)(i+1);
                    if (mapping[id].Count > 0)
                    {
                        spellBook.LearnSpell(mapping[id][0]);
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
                if (mapping[id].Count > 0) {
                    data[idx] = (byte)mapping[id][0];
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
                if (mapping[id].Count > 0)
                {
                    data[(byte)mapping[id][0] - 1] = 1;
                }
            }
        }
    }
}