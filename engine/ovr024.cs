using Classes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace engine
{
    internal enum CheckType
    {
        None = 0,
        Visibility = 1,
        PostHit1_Damage = 2,
        PostHit2_Damage = 3,
        SpecialAttacks = 4,
        Type_5 = 5,
        PreDamage = 6,
        PlayerRestrained = 7,
        BattleSetup1 = 8,
        MagicResistance = 9,
        CanHitAttacker = 10,
        Type_11 = 11,
        SavingThrow = 12,
        Death = 13,
        Type_14 = 14,
        Type_15 = 15,
        CanHitTarget = 16,
        Morale = 17,
        Movement = 18,
        BattleRound = 19,
        FireShield = 20,
        Confusion = 21,
        BattleSetup2 = 22,
        Type_23 = 23
    }

    public class ovr024
    {
        internal static async Task<bool> KillPlayer(string text, Status new_health_status, Player player) // sub_63014
        {
            ovr025.DisplayPlayerStatusString(false, 10, text, player);

            if (player.health_status != Status.stoned &&
                player.health_status != Status.dead &&
                player.health_status != Status.gone)
            {
                player.health_status = new_health_status;
                player.in_combat = false;
                player.hit_point_current = 0;

                await RemoveCombatAffects(player);
                await Affects.Effect.Check(player, CheckType.Death);

                if (player.in_combat == false)
                {
                    ovr033.CombatantKilled(player);
                }

                seg041.GameDelay();
                ovr025.ClearPlayerTextArea();

                if (gbl.game_state != GameState.Combat)
                {
                    ovr025.PartySummary(gbl.SelectedPlayer);
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        internal static async Task<bool> remove_affect(Affect? affect, Classes.Affects affect_id, Player player)
        {
            if (affect == null)
            {
                affect = player.GetAffect(affect_id);
            }

            if (affect != null)
            {
                if (affect.callAffectTable == true)
                {
                    await Affects.Effect.Call(Effect.Remove, affect, player, affect_id);
                }

                player.affects.Remove(affect);

                if (affect_id == Classes.Affects.spell_resist_fire)
                {
                    await CalcStatBonuses(Stat.CHA, player);
                }

                if (affect_id == Classes.Affects.enlarge ||
                    affect_id == Classes.Affects.strength ||
                    affect_id == Classes.Affects.strength_spell)
                {
                    await CalcStatBonuses(Stat.STR, player);
                }
            }

            return true;
        }

        static Classes.Affects[] unk_6325A = { Classes.Affects.silence_15_radius, Classes.Affects.prot_from_evil_10_radius, Classes.Affects.prot_from_good_10_radius, Classes.Affects.prayer };

        internal static async Task<bool> calc_affect_effect(Classes.Affects affect_type, Player player)
        {
            bool found = false;

            Affect affect;
            if (ovr025.FindAffect(out affect, affect_type, player) == true)
            {
                found = true;
            }
            else if (System.Array.Exists(unk_6325A, vv => vv ==affect_type) == true)
            {
                foreach (Player team_member in gbl.TeamList)
                {
                    if (found) break;

                    if (ovr025.FindAffect(out affect, affect_type, team_member) == true)
                    {
                        if (gbl.game_state == GameState.Combat)
                        {
                            int max_range = (affect_type == Classes.Affects.prayer) ? 6 : 1;

                            var scl = ovr032.Rebuild_SortedCombatantList(team_member, max_range, p => p == player);

                            found = scl.Count > 0;
                        }
                        else
                        {
                            found = true;
                        }
                    }
                }
            }

            if (found == true)
            {
                await Affects.Effect.Call(Effect.Add, affect, player, affect_type);
            }

            return found;
        }

        static Player sub_63D03(byte[] directions, int arraySize, List<GasCloud> list, Point mapPos) // sub_63D03
        {
            var arg_6 = list.Find(cell =>
            {
                for (int i = 0; i < arraySize; i++)
                {
                    if (cell.present[i] == true &&
                        cell.targetPos + gbl.MapDirectionDelta[directions[i]] == mapPos)
                    {
                        return true;
                    }
                }

                return false;
            });

            Player player_base = (arg_6 != null) ? arg_6.player : gbl.TeamList[0];

            return player_base;
        }


        internal static async Task<bool> in_poison_cloud(byte arg_0, Player player)
        {
            if (player.in_combat == true)
            {
                bool isPoisonousCloud;
                bool isNoxiouxCloud;
                int dummyGroundTile;
                int dummyPlayerIndex;
                ovr033.getGroundInformation(out isPoisonousCloud, out isNoxiouxCloud, out dummyGroundTile, out dummyPlayerIndex, 8, player);

                Affect affect;

                if (isNoxiouxCloud && arg_0 != 0 &&
                    ovr025.FindAffect(out affect, Classes.Affects.helpless, player) == false &&
                    ovr025.FindAffect(out affect, Classes.Affects.animate_dead, player) == false &&
                    ovr025.FindAffect(out affect, Classes.Affects.prot_paralysis_poison, player) == false &&
                    ovr025.FindAffect(out affect, Classes.Affects.prot_sleep_charm_paralysis_poison, player) == false)
                {
                    bool save_passed = await RollSavingThrow(0, 0, player);

                    if (save_passed == true)
                    {
                        Player tmp_player_ptr = gbl.SelectedPlayer;

                        gbl.SelectedPlayer = sub_63D03(gbl.unk_18AEA, 4, gbl.StinkingCloud, ovr033.PlayerMapPos(player));

                        await ApplyAttackSpellAffect("starts to cough", save_passed, 0, false, 0xff, 1, Classes.Affects.stinking_cloud, player);

                        if (player.HasAffect(Classes.Affects.stinking_cloud) == true)
                        {
                            await Affects.Effect.Call(Effect.Add, affect, player, Classes.Affects.stinking_cloud);
                        }

                        gbl.SelectedPlayer = tmp_player_ptr;
                    }
                    else
                    {
                        Player tmp_player_ptr = gbl.SelectedPlayer;

                        gbl.SelectedPlayer = sub_63D03(gbl.unk_18AEA, 4, gbl.StinkingCloud, ovr033.PlayerMapPos(player));

                        await ApplyAttackSpellAffect("chokes and gags from nausea", save_passed, 0, false, 0xff, (ushort)(roll_dice(4, 1) + 1), Classes.Affects.helpless, player);

                        if (ovr025.FindAffect(out affect, Classes.Affects.helpless, player) == true)
                        {
                            await Affects.Effect.Call(Effect.Add, affect, player, Classes.Affects.helpless);
                        }

                        gbl.SelectedPlayer = tmp_player_ptr;
                    }
                }

                if (isPoisonousCloud == true &&
                    player.in_combat == true)
                {
                    if (player.HitDice >= 0 && player.HitDice <= 4)
                    {
                        ovr025.DisplayPlayerStatusString(false, 10, "is Poisoned", player);
                        seg041.GameDelay();
                        add_affect(false, 0xff, 0, Classes.Affects.minor_globe_of_invulnerability, player);
                        await KillPlayer("is killed", Status.dead, player);
                    }
                    else if (player.HitDice == 5)
                    {
                        if (await RollSavingThrow(-4, 0, player) == false)
                        {
                            ovr025.DisplayPlayerStatusString(false, 10, "is Poisoned", player);
                            seg041.GameDelay();
                            add_affect(false, 0xff, 0, Classes.Affects.poisoned, player);
                            await KillPlayer("is killed", Status.dead, player);
                        }
                    }
                    else if (player.HitDice == 6)
                    {
                        if (await RollSavingThrow(0, 0, player) == false)
                        {
                            ovr025.DisplayPlayerStatusString(false, 10, "is Poisoned", player);
                            seg041.GameDelay();
                            add_affect(false, 0xff, 0, Classes.Affects.poisoned, player);
                            await KillPlayer("is killed", Status.dead, player);
                        }
                    }
                }
            }

            return true;
        }


        internal static async Task<bool> CanHitTarget(int bonus, Player target) // sub_641DD
        {
            bool hit = false;
            gbl.attack_roll = roll_dice(20, 1);

            if (gbl.attack_roll > 1)
            {
                // natural 20, always hits, so make it huge, so it always beats the AC.
                if (gbl.attack_roll == 20)
                {
                    gbl.attack_roll = 100;
                }

                await Affects.Effect.Check(target, CheckType.CanHitTarget);

                if (gbl.attack_roll >= 0)
                {
                    if ((gbl.attack_roll + bonus) > target.ac)
                    {
                        hit = true;
                    }
                }
            }

            return hit;
        }


        internal static async Task<bool> PC_CanHitTarget(int target_ac, Player target, Player attacker) /* sub_64245 */
        {
            bool hit = false;

            await remove_invisibility(attacker);
            gbl.attack_roll = roll_dice(20, 1);

            if (gbl.attack_roll > 1)
            {
                if (gbl.attack_roll == 20)
                {
                    gbl.attack_roll = 100;
                }

                await Affects.Effect.Check(attacker, CheckType.CanHitAttacker);
                await Affects.Effect.Check(target, CheckType.CanHitTarget);

                int team_bonus;
                if (attacker.combat_team == CombatTeam.Ours)
                {
                    team_bonus = gbl.area2_ptr.field_6E2;
                }
                else
                {
                    team_bonus = gbl.area2_ptr.field_6E0;
                }

                if (gbl.attack_roll >= 0)
                {
                    if ((gbl.attack_roll + attacker.hitBonus + team_bonus) >= target_ac)
                    {
                        hit = true;
                    }
                }
            }
            return hit;
        }


        internal static async Task<bool> RollSavingThrow(int saveBonus, SaveVerseType saveType, Player player) // do_saving_throw
        {
            gbl.savingThrowMade = true;
            gbl.savingThrowRoll = roll_dice(20, 1);

            if (Cheats.player_always_saves && player.combat_team == 0)
            {
                gbl.savingThrowRoll = 20;
            }

            if (gbl.savingThrowRoll == 1)
            {
                gbl.savingThrowMade = false;
            }
            else if (gbl.savingThrowRoll == 20)
            {
                gbl.savingThrowMade = true;
            }
            else
            {
                gbl.savingThrowRoll += saveBonus + player.field_186;
                gbl.saveVerseType = saveType;

                await Affects.Effect.Check(player, CheckType.SavingThrow);

                gbl.savingThrowMade = gbl.savingThrowRoll >= player.saveVerse[(int)saveType];
            }

            return gbl.savingThrowMade;
        }


        internal static byte roll_dice(int dice_size, int dice_count, int min_roll = 1)
        {
            int roll_total = 0;

            for (int i = 0; i < dice_count; i++)
            {
                int roll_current;

                // don't bother rolling if we can't roll over the min_roll
                if (min_roll >= dice_size)
                {
                    roll_current = dice_size;
                }
                else
                {
                    roll_current = seg051.Random(dice_size) + 1;
                    if (roll_current < min_roll)
                        roll_current = min_roll;
                }
                roll_total += roll_current;
            }

            byte byte_total = (byte)roll_total;

            return byte_total;
        }


        internal static int roll_dice_save(int dice_size, int dice_count)
        {
            gbl.dice_count = dice_count;

            return roll_dice(dice_size, dice_count);
        }


        internal static void add_affect(bool call_spell_jump_list, int data, ushort minutes, Classes.Affects type, Player player)
        {
            Affect affect = new Affect(type, minutes, (byte)data, call_spell_jump_list);

            player.affects.Add(affect);
            //TODO simplify this funcation.
        }


        internal static async Task<bool> RemoveFromCombat(string msg, Status health_status, Player player) // sub_644A7
        {
            if (player.in_combat == true)
            {
                int player_index = ovr033.GetPlayerIndex(player);

                ovr033.RedrawCombatIfFocusOn(false, 3, player);

                ovr025.DisplayPlayerStatusString(true, 10, msg, player);

                player.in_combat = false;

                player.health_status = health_status;

                if (player.health_status != Status.running)
                {
                    player.hit_point_current = 0;
                }

                ovr033.RedrawPlayerBackground(player_index);
                seg040.DrawOverlay();

                gbl.CombatMap[player_index].size = 0;

                ovr033.setup_mapToPlayerIndex_and_playerScreen();

                ovr025.clear_actions(player);
                await RemoveCombatAffects(player);
            }
            return true;
        }


        internal static async Task<bool> remove_invisibility(Player player)
        {
            Affect affect;

            while (ovr025.FindAffect(out affect, Classes.Affects.invisibility, player) == true)
            {
                await remove_affect(affect, Classes.Affects.invisibility, player);
            }
            return true;
        }


        internal static async Task<bool> RemoveCombatAffects(Player player) // sub_645AB
        {
            Classes.Affects[] table = {
                Classes.Affects.faerie_fire,
                Classes.Affects.charm_person,
                Classes.Affects.reduce,
                Classes.Affects.silence_15_radius,
                Classes.Affects.spiritual_hammer,
                Classes.Affects.stinking_cloud,
                Classes.Affects.helpless,
                Classes.Affects.animate_dead,
                Classes.Affects.snake_charm,
                Classes.Affects.paralyze,
                Classes.Affects.sleep,
                Classes.Affects.clear_movement,
                Classes.Affects.regenerate,
                Classes.Affects.delay_death,
                Classes.Affects.regen_3_hp,
                Classes.Affects.entangle,
                Classes.Affects.confuse_berserk,
                Classes.Affects.affect_8b,
                Classes.Affects.owlbear_hug_round_attack
            };

            await RemoveAffects(table, player);

            if (player.HasAffect(Classes.Affects.berserk) == true && player.control_morale == Control.PC_Berserk)
            {
                player.combat_team = CombatTeam.Ours;
            }

            return true;
        }
        internal static async Task<bool> RemoveAttackersAffects(Player player)
        {
            Classes.Affects[] table = {
                Classes.Affects.reduce,
                Classes.Affects.clear_movement,
                Classes.Affects.affect_8b,
                Classes.Affects.owlbear_hug_round_attack
            };

            return await RemoveAffects(table, player);
        }


        internal static async Task<bool> RemoveAffects(Classes.Affects[] table, Player player) // sub_6460D
        {
            CancellationTokenSource cts = new();
            ParallelOptions options = new() { CancellationToken = cts.Token };

            try
            {
                await Parallel.ForEachAsync(table, options, async (affect, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    await remove_affect(null, affect, player);
                    ct.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            return true;

        }


        internal static async Task<bool> cure_affect(Classes.Affects affectId, Player player) /* is_cured */
        {
            Affect affect = player.GetAffect(affectId);
            if (affect != null)
            {
                ovr025.DisplayPlayerStatusString(true, 10, "is Cured", player);

                await remove_affect(affect, affectId, player);

                return true;
            }

            return false;
        }


        internal static int encode_strength(int str_00, int str) /* odd_math */
        {
            int ret_val = str + 100;

            if (str == 18)
            {
                ret_val = str_00 + 1;
            }

            return ret_val;
        }


        internal static void decode_strength(out int str_00, out int str, Affect arg_8) /* sub_646D9 */
        {
            str_00 = 0;
            str = (byte)(arg_8.affect_data & 0x7F);

            if (str <= 101)
            {
                str_00 = (byte)(str - 1);
                str = 18;
            }
            else
            {
                str -= 100;
            }
        }


        internal static bool TryEncodeStrength(out int encoded_str, int str_100, int str, Player player) // sub_64728
        {
            bool encoded;

            if (str > player.stats.Str.Base ||
                (str == 18 && str_100 > player.stats.Str00.Base))
            {
                encoded = true;
                encoded_str = encode_strength(str_100, str);
            }
            else
            {
                encoded = false;
                encoded_str = 0;
            }

            return encoded;
        }


        internal static void max_strength(ref int str_a, int str_b, ref int str_00_a, int str_00_b) /* sub_64771 */
        {
            if (str_b > str_a ||
                (str_a == str_b && str_b == 18 && str_00_b > str_00_a))
            {
                str_a = str_b;
                str_00_a = str_00_b;
            }
        }


        internal static int ConHitPointBonus(int classLvl, SkillType class_index, int cons, Player player) // sub_647BE
        {
            int returnVal = 0;

            if (ovr018.hp_calc_table[class_index].max_hit_die < classLvl)
            {
                classLvl = ovr018.hp_calc_table[class_index].max_hit_die;
            }

            if (class_index == SkillType.Ranger &&
                (player.multiclassLevel == 0 || player.ranger_old_lvl == player.multiclassLevel))
            {
                classLvl += 1;
            }

            if (class_index == SkillType.Fighter || 
                class_index == SkillType.Paladin ||
                class_index == SkillType.Knight ||
                class_index == SkillType.Ranger)
            {
                if (cons >= 15 && cons <= 19)
                {
                    returnVal = classLvl * (cons - 14);
                }
                else if (cons == 20)
                {
                    returnVal = classLvl * 5;
                }
                else if (cons >= 21 && cons <= 23)
                {
                    returnVal = classLvl * 6;
                }
                else if (cons >= 24 && cons <= 25)
                {
                    returnVal = classLvl * 7;
                }
            }
            else
            {
                if (cons > 15)
                {
                    returnVal = classLvl * 2;
                }
                else if (cons == 15)
                {
                    returnVal = classLvl;
                }
            }

            return returnVal;
        }


        internal static async Task<bool> CalcStatBonuses(Stat stat_index, Player player) // sub_648D9
        {
            int stat_b = 0;
            int str_00_b = 0;
            int var_11 = 0x0FF;

            int stat_a = player.stats[(int)stat_index].Base;
            int str_00_a = player.stats.Str00.Base;

            foreach (Item item in player.items)
            {
                if ((int)item.affect_3 > 0x80 && item.readied == true)
                {
                    int var_12 = (int)item.affect_3 & 0x7F;

                    if (stat_index == Stat.STR)
                    {
                        if (var_12 == 3 && (int)item.affect_2 == 0) // Gauntlets of Ogre Power
                        {
                            stat_b = 18;
                            str_00_b = 100;
                        }
                        else if (var_12 == 5) // Girdle of Giant Strength
                        {
                            switch ((int)item.affect_2)
                            {
                                case 0:
                                    stat_b = 18;
                                    str_00_b = 100;
                                    break;

                                case 1:
                                    stat_b = 19;
                                    str_00_b = 0;
                                    break;

                                case 2:
                                    stat_b = 20;
                                    str_00_b = 0;
                                    break;

                                case 3:
                                    stat_b = 21;
                                    str_00_b = 0;
                                    break;

                                case 4:
                                    stat_b = 22;
                                    str_00_b = 0;
                                    break;

                                case 5:
                                    stat_b = 23;
                                    str_00_b = 0;
                                    break;

                                case 6:
                                    stat_b = 24;
                                    str_00_b = 0;
                                    break;
                            }
                        }
                        else if (var_12 == 8) // pale blue ioun stone
                        {
                            if (player.stats.Str.Base < 18 &&
                                item.affect_2 == 0)
                            {
                                stat_b = (byte)(player.stats.Str.Base + 1);
                                str_00_b = 0;
                            }
                        }
                        else if (var_12 == 13)
                        {
                            var_11 = 3;
                        }

                        max_strength(ref stat_a, stat_b, ref str_00_a, str_00_b);
                    }
                    else if (stat_index == Stat.CON)
                    {
                        if (var_12 == 6)
                        {
                            stat_a++;
                        }
                        else if (var_12 == 8 &&
                            player.stats.Con.Base < 18 &&
                            (int)item.affect_2 == 4) // pink ioun stone
                        {
                            stat_a++;
                        }
                    }
                    else if (stat_index == Stat.INT)
                    {
                        if (var_12 == 8) // scarlet and blue ioun stone
                        {
                            if (player.stats.Int.Base < 18 &&
                                (int)item.affect_2 == 1)
                            {
                                stat_a++;
                            }
                        }
                        else if (var_12 == 12)
                        {
                            var_11 = 7;
                        }
                        else if (var_12 == 13)
                        {
                            var_11 = 3;
                        }
                    }
                    else if (stat_index == Stat.WIS)
                    {
                        if (var_12 == 8 &&
                            (int)item.affect_2 == 2 &&
                            player.stats.Wis.Base < 18) // incandescent blue ioun stone
                        {
                            stat_a++;
                        }
                    }
                    else if (stat_index == Stat.DEX)
                    {
                        if (var_12 == 2)
                        {
                            if (player.stats.Dex.Base >= 0 && player.stats.Dex.Base <= 6)
                            {
                                stat_a += 4;
                            }
                            else if (player.stats.Dex.Base >= 7 && player.stats.Dex.Base <= 13)
                            {
                                stat_a += 2;
                            }
                            else
                            {
                                stat_a++;
                            }
                        }
                        else if (var_12 == 8) // deep red ioun stone
                        {
                            if (player.stats.Dex.Base < 18 &&
                                (int)item.affect_2 == 3)
                            {
                                stat_a++;
                            }
                        }
                        else if (var_12 == 10)
                        {
                            stat_a -= 2;
                        }
                    }
                    else if (stat_index == Stat.CHA)
                    {
                        if (var_12 == 6)
                        {
                            stat_a -= 1;
                        }
                        else if (var_12 == 8 &&
                            player.stats.Cha.Base < 18 &&
                            (int)item.affect_2 == 5) // ping and green ioun stone
                        {
                            stat_a += 1;
                        }
                    }
                }
            }

            if (stat_index == Stat.STR) 
            {
                Affect affect_ptr;

                if (ovr025.FindAffect(out affect_ptr, Classes.Affects.strength, player) == true)
                {
                    decode_strength(out str_00_b, out stat_b, affect_ptr);

                    if (stat_a <= 18 &&
                        str_00_a < 100)
                    {
                        stat_b += stat_a;

                        if (stat_b > 18)
                        {
                            if (player.fighter_lvl > 0 ||
                                player.fighter_old_lvl > 0 ||
                                player.paladin_lvl > 0 ||
                                player.paladin_old_lvl > 0 ||
                                player.ranger_lvl > 0 ||
                                player.ranger_old_lvl > 0)
                            {
                                str_00_b = (byte)(player.stats.Str00.Base + ((stat_b - 18) * 10));

                                if (str_00_b > 100)
                                {
                                    str_00_b = 100;
                                }

                                stat_b = 18;
                            }
                            else
                            {
                                stat_b = 18;
                                str_00_b = 0;
                            }
                        }
                    }


                    max_strength(ref stat_a, stat_b, ref str_00_a, str_00_b);
                }

                if (ovr025.FindAffect(out affect_ptr, Classes.Affects.strength_spell, player) == true)
                {
                    decode_strength(out str_00_b, out stat_b, affect_ptr);
                    max_strength(ref stat_a, stat_b, ref str_00_a, str_00_b);
                }

                if (ovr025.FindAffect(out affect_ptr, Classes.Affects.enlarge, player) == true)
                {
                    decode_strength(out str_00_b, out stat_b, affect_ptr);
                    max_strength(ref stat_a, stat_b, ref str_00_a, str_00_b);
                }

                if (var_11 != 0xff)
                {
                    player.stats.Str.Current = var_11;
                    player.stats.Str00.Current = 0;
                }
                else
                {
                    player.stats.Str.Current = stat_a;
                    player.stats.Str00.Current = str_00_a;
                }
            }
            else if (stat_index == Stat.CON)
            {
                byte orig_max_hp = player.hit_point_max;
                player.hit_point_max = player.hit_point_rolled;

                int hitPointBonus = ovr018.calc_fixed_hp_bonus(player, stat_a);

                player.hit_point_max += (byte)hitPointBonus;

                if (player.hit_point_max > orig_max_hp)
                {
                    player.hit_point_current += (byte)(player.hit_point_max - orig_max_hp);
                }
                else if (player.hit_point_max < orig_max_hp)
                {
                    if (player.hit_point_current > (orig_max_hp - player.hit_point_max))
                    {
                        player.hit_point_current -= (byte)(orig_max_hp - player.hit_point_max);
                    }
                    else
                    {
                        player.hit_point_current = 0;
                    }
                }

                player.stats.Con.Current = stat_a;

                if (player.stats.Con.Current >= 20)
                {
                    if (player.HasAffect(Classes.Affects.highConRegen) == false)
                    {
                        // Per 1e, healing is 1/6 turns at 20, 1/5 turns at 21, ... 1/1 turn at 25
                        ushort rounds = (ushort)((26 - player.stats.Con.Current) * 10);
                        add_affect(true, 0xff, rounds, Classes.Affects.highConRegen, player);
                    }
                }
                else
                {
                    await remove_affect(null, Classes.Affects.highConRegen, player);
                }
                ovr026.recalc_saving_throws(player);
            }
            else if (stat_index == Stat.INT)
            {
                if (player.HasAffect(Classes.Affects.feeblemind) == true && var_11 > 7)
                {
                    var_11 = 3;
                }

                if (var_11 != 0xff)
                {
                    player.stats.Int.Current = var_11;
                }
                else
                {
                    player.stats.Int.Current = stat_a;
                }
            }
            else if (stat_index == Stat.WIS)
            {
                if (player.HasAffect(Classes.Affects.feeblemind) == true &&
                    var_11 > 7)
                {
                    var_11 = 3;
                }

                if (var_11 != 0xff)
                {
                    player.stats.Wis.Current = var_11;
                }
                else
                {
                    player.stats.Wis.Current = stat_a;
                }
            }
            else if (stat_index == Stat.DEX)
            {
                if (var_11 != 0xff)
                {
                    player.stats.Dex.Current = var_11;
                }
                else
                {
                    player.stats.Dex.Current = stat_a;
                }
                ovr026.recalc_thief_skills(player);
            }
            else if (stat_index == Stat.CHA)
            {
                Affect affect;
                if (ovr025.FindAffect(out affect, Classes.Affects.friends, player) == true)
                {
                    stat_a = affect.affect_data;
                }

                player.stats.Cha.Current = stat_a;
            }

            return true;
        }

        internal static async Task<bool> damage_person(bool change_damage, DamageOnSave arg_2, int damage, Player player)
        {
            string text;

            gbl.damage = damage;

            await Affects.Effect.Check(player, CheckType.PreDamage);

            if (change_damage == true)
            {
                if (arg_2 == DamageOnSave.Zero)
                {
                    gbl.damage = 0;
                }
                else if (arg_2 == DamageOnSave.Half)
                {
                    gbl.damage /= 2;
                }
            }
            else
            {
                await Affects.Effect.Check(player, CheckType.FireShield);
            }

            if (gbl.damage > 0)
            {
                if (gbl.damage != 1)
                {
                    text = "takes " + gbl.damage.ToString() + " points of damage ";
                }
                else
                {
                    text = "takes 1 point of damage ";
                }

                int mask = (int)(gbl.damage_flags & ~DamageType.Magic);
                if (mask == 0x01)
                {
                    text += "from Fire";
                }
                else if (mask == 0x02)
                {
                    text += "from Cold";
                }
                else if (mask == 0x04)
                {
                    text += "from Electricity";
                }
                else if (mask == 0x10)
                {
                    text += "from Acid";
                }

                if ((gbl.damage_flags & DamageType.Magic) == gbl.damage_flags)
                {
                    text += "from Magic";
                }

                ovr025.MagicAttackDisplay(text, false, player);
                ovr025.damage_player(gbl.damage, player);

                if (gbl.game_state == GameState.Combat)
                {
                    TryLooseSpell(player);
                }

                if (player.in_combat == false)
                {
                    text = "Goes Down";

                    if (player.health_status == Status.dying)
                    {
                        text = text + ", and is Dying";
                    }

                    if (player.health_status == Status.dead ||
                        player.health_status == Status.stoned ||
                        player.health_status == Status.gone )
                    {
                        text = "is killed";
                    }

                    ovr025.DisplayPlayerStatusString(false, gbl.textYCol + 1, text, player);

                    if (gbl.game_state != GameState.Combat)
                    {
                        seg041.GameDelay();
                    }
                    else
                    {
                        await RemoveCombatAffects(player);

                        await Affects.Effect.Check(player, CheckType.Death);

                        if (player.in_combat == false)
                        {
                            ovr033.CombatantKilled(player);
                        }
                        else
                        {
                            seg041.GameDelay();
                        }
                    }
                }

                ovr025.ClearPlayerTextArea();

            }
            return true;
        }

        internal static void TryLooseSpell(Player player)
        {
            player.actions.can_cast = false;

            if (player.actions.spell_id > 0)
            {
                ovr025.DisplayPlayerStatusString(true, 12, "lost a spell", player);

                player.spellList.ClearSpell((Spells)player.actions.spell_id);
                player.actions.spell_id = 0;
            }
        }


        internal static async Task<bool> ApplyAttackSpellAffect(string text, bool saved, DamageOnSave can_save, bool call_affect_table, int data, ushort time, Classes.Affects affect_id, Player target) // is_unaffected
		{
            gbl.current_affect = affect_id;

            await Affects.Effect.Check(target, CheckType.MagicResistance);

            if (gbl.current_affect == 0 ||
                (saved == true && can_save == DamageOnSave.Zero))
            {
                ovr025.DisplayPlayerStatusString(true, 10, "is Unaffected", target);
            }
            else
            {
                Affect found_affect;

                if (ovr025.FindAffect(out found_affect, affect_id, target) == true &&
                    found_affect.minutes > 0)
                {
                    await remove_affect(found_affect, affect_id, target);
                }

                add_affect(call_affect_table, data, time, affect_id, target);

                if (text.Length != 0)
                {
                    ovr025.MagicAttackDisplay(text, true, target);
                    ovr025.ClearPlayerTextArea();
                }
            }

            return true;
        }


        internal static async Task<bool> heal_player(byte arg_0, int amount_healed, Player player)
        {
            if (player.health_status == Status.okey ||
                player.health_status == Status.animated ||
                player.health_status == Status.unconscious ||
                player.health_status == Status.dying )
            {
                if (player.hit_point_current < player.hit_point_max ||
                    (player.hit_point_current >= player.hit_point_max &&
                    arg_0 == 0))
                {
                    player.hit_point_current = (byte)(amount_healed + player.hit_point_current);

                    if (player.hit_point_current > player.hit_point_max)
                    {
                        player.hit_point_current = player.hit_point_max;
                    }

                    if (player.in_combat == false)
                    {
                        if (player.health_status == Status.dying)
                        {
                            player.health_status = Status.unconscious;
                        }

                        if (player.health_status == Status.unconscious &&
                            gbl.game_state != GameState.Combat)
                        {
                            await Affects.Effect.Call(Effect.Remove, null, player, Classes.Affects.affect_4e);
                        }
                    }

                    return true;
                }
            }

            return false;
        }


        internal static bool combat_heal(byte arg_0, Player player)
        {
            if (ovr033.sub_7515A(true, ovr033.PlayerMapPos(player), player) == true)
            {
                player.health_status = Status.okey;
                player.in_combat = true;
                player.hit_point_current = arg_0;

                if (gbl.game_state == GameState.Combat)
                {
                    ovr033.RedrawCombatIfFocusOn(false, 3, player);
                }

                string text;
                if (player.combat_team == CombatTeam.Enemy)
                {
                    text = "stands up and grins";
                }
                else
                {
                    text = "gets back up";
                }

                ovr025.DisplayPlayerStatusString(true, 10, text, player);

                ovr025.CountCombatTeamMembers();

                return true;
            }

            return false;
        }
    }
}
