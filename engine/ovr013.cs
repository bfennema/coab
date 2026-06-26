using Classes;
using System.Threading.Tasks;

namespace engine
{
	class ovr013
	{
		/// <summary>
		/// If same as current affect damage set to zero, or if affect is zero
		/// </summary>
		static void ProtectedIf(Classes.Affects affect) /* sub_3A019 */
		{
			if (gbl.current_affect == affect)
			{
				Protected();
			}
		}

		internal static void Protected()
		{
			gbl.damage = 0;
			gbl.current_affect = 0;
        }


        internal static bool addAffect(ushort time, int data, Classes.Affects affect_type, Player player)
		{
			if (gbl.cureSpell == true)
			{
				return false;
			}
			else
			{
				ovr024.add_affect(true, data, time, affect_type, player);
				return true;
			}
		}


		internal static Task<bool> sub_3A071(Effect arg_0, object param, Player player)
		{
			ovr025.clear_actions(player);

            return Task.FromResult(true);
		}


		internal static Task<bool> Bless(Effect add_remove, object param, Player player)
		{
			gbl.monster_morale += 5;
			gbl.attack_roll++;

            return Task.FromResult(true);
		}


		internal static Task<bool> Curse(Effect arg_0, object param, Player player)
		{
			if (gbl.monster_morale < 5)
			{
				gbl.monster_morale = 0;
			}
			else
			{
				gbl.monster_morale -= 5;
			}
			gbl.attack_roll--;

            return Task.FromResult(true);
        }


		internal static async Task<bool> SticksToSnakes(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;

			byte var_1 = (byte)(player.attack2_AttacksLeft + player.attack1_AttacksLeft);

			if (affect.affect_data > var_1)
			{
				affect.affect_data -= var_1;
			}
			else
			{
				await ovr024.remove_affect(null, Classes.Affects.sticks_to_snakes, player);
			}

			ovr025.MagicAttackDisplay("is fighting with snakes", true, player);
			ovr025.ClearPlayerTextArea();

			ovr025.clear_actions(player);

            return true;
		}


		internal static Task<bool> DispelEvil(Effect arg_0, object param, Player player)
		{
			if (gbl.SelectedPlayer.flags.HasFlag(Flags.EvilSummon))
			{
				gbl.attack_roll -= 7;
			}

            return Task.FromResult(true);
        }


		internal static Task<bool> AffectFlameTongue(Effect arg_0, object param, Player player) // sub_3A17A
		{
			int bonus = 0;

			if (player.actions != null &&
				player.actions.target != null)
			{
				gbl.spell_target = player.actions.target;

				if (gbl.spell_target.flags.HasFlag(Flags.Undead))
				{
					bonus = 3;
				}
				else if (gbl.spell_target.flags.HasFlag(Flags.Cold) || gbl.spell_target.flags.HasFlag(Flags.Avian))
				{
					bonus = 2;
				}
				else if (gbl.spell_target.flags.HasFlag(Flags.Regenerate))
				{
					bonus = 1;
				}
				else
				{
					bonus = 0;
				}
			}
			gbl.attack_roll += bonus;
			gbl.damage += bonus;
			gbl.damage_flags = DamageType.Magic | DamageType.Fire;

            return Task.FromResult(true);
        }


		internal static Task<bool> FaerieFire(Effect arg_0, object param, Player player)
		{
			gbl.attack_roll += 2;

            return Task.FromResult(true);
		}


		internal static Task<bool> affect_protect_evil(Effect arg_0, object param, Player player) /* sub_3A224 */
		{
			if (gbl.SelectedPlayer.alignment == 2 ||
				gbl.SelectedPlayer.alignment == 5 ||
				gbl.SelectedPlayer.alignment == 8)
			{
				gbl.savingThrowRoll += 2;
				gbl.attack_roll -= 2;

                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }


		internal static Task<bool> affect_protect_good(Effect arg_0, object param, Player player) /* sub_3A259 */
		{
			if (gbl.SelectedPlayer.alignment == 0 ||
				gbl.SelectedPlayer.alignment == 3 ||
				gbl.SelectedPlayer.alignment == 6)
			{
				gbl.savingThrowRoll += 2;
				gbl.attack_roll -= 2;

                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }


		internal static Task<bool> affect_resist_cold(Effect arg_0, object param, Player player) /* sub_3A28E */
		{
            if ((gbl.damage_flags.HasFlag(DamageType.Cold)))
            {
                gbl.damage /= 2;
                gbl.savingThrowRoll += 3;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> affect_charm_person(Effect arg_0, object param, Player player) /* sub_3A2AD */
		{
			Affect affect = (Affect)param;

			if (arg_0 == Effect.Remove)
			{
				player.combat_team = (CombatTeam)((affect.affect_data & 0x40) >> 6);

				if (player.control_morale == Control.PC_Berserk)
				{
					player.control_morale = Control.PC_Base;
				}
			}
			else
			{
				if ((affect.affect_data & 0x20) == 0)
				{
					affect.affect_data += (byte)(0x20 + (((int)player.combat_team) << 6));

					player.combat_team = (CombatTeam)(affect.affect_data >> 7);
					player.quick_fight = QuickFight.True;

					if (player.control_morale < Control.NPC_Base)
					{
						player.control_morale = Control.PC_Berserk;
					}

					player.actions.target = null;
					ovr025.CountCombatTeamMembers();
				}
				gbl.monster_morale = 100;
			}
            return Task.FromResult(true);
        }


		internal static async Task<bool> Suffocates(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;

			if (affect.affect_data == 0)
			{
				await ovr024.KillPlayer("Suffocates", Status.dead, player);
                return true;
			}
			else
			{
				affect.affect_data--;
                return false;
			}
		}


		internal static async Task<bool> AffectPoisonDamage(Effect arg_0, object param, Player player) // sub_3A3BC
		{
			Affect affect = (Affect)param;

			if (addAffect(10, affect.affect_data, Classes.Affects.poison_damage, player) == true &&
				player.hit_point_current > 1)
			{
				gbl.damage_flags = 0;

				await ovr024.damage_person(false, 0, 1, player);

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


		internal static Task<bool> Affectshield(Effect arg_0, object param, Player player) /* sub_3A41F */
		{
			if (player.ac < 57) // AC 3
			{
				player.ac = 57; // AC 3
			}

			gbl.savingThrowRoll += 1;

			if (gbl.spell_id == Spells.magic_missile || gbl.spell_id == Spells.wand_of_magic_missiles)
			{
				gbl.damage = 0;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectGnomeVsGoblinKobold(Effect arg_0, object param, Player player) // sub_3A44A
		{
			if (player.actions != null &&
				player.actions.target != null &&
				(player.actions.target.flags & Flags.GnomeBonus) != 0)
			{
				gbl.spell_target = player.actions.target;
				gbl.attack_roll++;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectResistFire(Effect add_remove, object param, Player player) /* sub_3A480 */
		{
			if (add_remove == Effect.Add &&
				(gbl.damage_flags & DamageType.Fire) != 0)
			{
				gbl.damage /= 2;
				gbl.savingThrowRoll += 3;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> is_silenced1(Effect arg_0, object param, Player player)
		{
			if (player.actions.can_use == true)
			{
				ovr025.DisplayPlayerStatusString(true, 10, "is silenced", player);
			}

			player.actions.can_use = false;
			player.actions.can_cast = false;

            return Task.FromResult(true);
        }


		internal static async Task<bool> AffectslowPoison(Effect arg_0, object param, Player player) // sub_3A517
		{
			if (player.HasAffect(Classes.Affects.poisoned) == true)
			{
				await ovr024.KillPlayer("dies from poison", Status.dead, player);
			}

			gbl.cureSpell = true;

			await ovr024.remove_affect(null, Classes.Affects.poison_damage, player);

			gbl.cureSpell = false;

            return true;
		}


		internal static async Task<bool> affect_spiritual_hammer(Effect add_remove, object param, Player player) /* sub_3A583 */
		{
			Item item = player.items.Find(i => i.type == Item.Type.Hammer && i.namenum[2] == Classes.Item.Names.Spiritual);
			bool item_found = item != null;

			if (add_remove == Effect.Remove && item != null)
			{
				ovr025.lose_item(item, player);
			}

			if (add_remove == Effect.Add &&
				item_found == false &&
				player.items.Count < Player.MaxItems)
			{
				item = new Item(0x80 | 0x09, Classes.Affects.spiritual_hammer, 0, 0, 0, 0, false, 0, false, 0, 1, Classes.Item.Names.Spiritual, Classes.Item.Names.WEAPONHammer, 0, Item.Type.Hammer, true);

				player.items.Add(item);
				if (gbl.SelectedPlayer.activeItems[ItemSlot.Weapon] != null)
				{
					await ovr020.ready_Item(gbl.SelectedPlayer.activeItems[ItemSlot.Weapon]);
					ovr025.reclac_player_values(player);
				}
				await ovr020.ready_Item(item);

				ovr025.DisplayPlayerStatusString(true, 10, "Gains an item", player);
			}

			ovr025.reclac_player_values(player);

            return true;
		}


		internal static Task<bool> sub_3A6C6(Effect arg_0, object param, Player player)
		{
			if (gbl.SelectedPlayer.HasAffect(Classes.Affects.detect_invisibility) == false &&
				player.HasAffect(Classes.Affects.faerie_fire) == false)
			{
				gbl.targetInvisible = true;
				gbl.attack_roll -= 4;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectDwarfVsOrcGoblin(Effect arg_0, object param, Player player) // sub_3A7E8
		{
			gbl.spell_target = player.actions.target;

            if (gbl.spell_target.flags.HasFlag(Flags.DwarfBonus))
            {
                gbl.attack_roll++;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> MirrorImage(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;

			if (ovr024.roll_dice((affect.affect_data >> 4) + 1, 1) > 1 &&
				gbl.spell_id > 0 &&
				gbl.byte_1D2C7 == false)
			{
				Protected();

				ovr025.DisplayPlayerStatusString(true, 10, "lost an image", player);

				affect.affect_data -= 1;

				if (affect.affect_data == 0)
				{
					await ovr024.remove_affect(null, Classes.Affects.mirror_image, player);
				}
                return true;
			}
            else
            {
                return false;
            }
		}


		internal static Task<bool> three_quarters_damage(Effect arg_0, object param, Player player)
		{
			gbl.damage -= gbl.damage / 4;
            return Task.FromResult(true);
        }


		internal static Task<bool> StinkingCloud(Effect arg_0, object param, Player player)
		{
			if (player.actions.can_use == true)
			{
				ovr025.DisplayPlayerStatusString(true, 10, "is coughing", player);
			}

			player.actions.can_use = false;
			player.actions.can_cast = false;

			ovr025.reclac_player_values(player);

			if (player.ac_behind > 0x34)
			{
				player.ac_behind -= 2;
			}
			else
			{
				player.ac_behind = 0x32;
			}

			player.ac = player.ac_behind;

			if (player == gbl.SelectedPlayer)
			{
				ovr025.CombatDisplayPlayerSummary(player);
			}
            return Task.FromResult(true);
        }


		internal static async Task<bool> sub_3A89E(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;

			affect.callAffectTable = false;

			if (gbl.cureSpell == false)
			{
				await ovr024.KillPlayer("collapses", Status.dead, player);
                return false;
			}

			player.combat_team = (CombatTeam)(affect.affect_data >> 4);
			player.quick_fight = QuickFight.True;
			player.level_undead = 0;

			player.attackLevel = (byte)player.SkillLevel(SkillType.Fighter, SkillType.Paladin, SkillType.Ranger);
			player.base_movement = 0x0C;

			if (player.control_morale == Control.PC_Berserk)
			{
				player.control_morale = Control.PC_Base;
			}

			player.flags |= Flags.Undead;
            return true;
		}


		internal static Task<bool> AffectBlinded(Effect arg_0, object param, Player player) // sub_3A951
		{
			gbl.attack_roll -= 4;

			player.ac -= 4;
			player.ac_behind -= 4;

			gbl.savingThrowRoll -= 4;
            return Task.FromResult(true);
        }


		internal static async Task<bool> AffectCauseDisease(Effect add_remove, object param, Player player) // sub_3A974
		{
			await Affects.Effect.Call(add_remove, param, player, Classes.Affects.weaken);
			await Affects.Effect.Call(add_remove, param, player, Classes.Affects.cause_disease_2);

            return true;
		}


		internal static async Task<bool> AffectConfuse(Effect arg_0, object arg_2, Player player) // sub_3A9D9
		{
			byte var_1 = ovr024.roll_dice(100, 1);

			if (var_1 >= 1 && var_1 <= 10)
			{
				await ovr024.remove_affect(null, Classes.Affects.confuse, player);
				player.actions.fleeing = true;
				player.quick_fight = QuickFight.True;

				if (player.control_morale < Control.NPC_Base)
				{
					player.control_morale = Control.PC_Berserk;
				}

				player.actions.target = null;

				await ovr024.ApplyAttackSpellAffect("runs away", false, DamageOnSave.Zero, true, 0, 10, Classes.Affects.fear, player);
			}
			else if (var_1 >= 11 && var_1 <= 60)
			{
				ovr025.MagicAttackDisplay("is confused", true, player);
				ovr025.ClearPlayerTextArea();
				await sub_3A071(0, arg_2, player);
			}
			else if (var_1 >= 61 && var_1 <= 80)
			{
				await ovr024.ApplyAttackSpellAffect("goes berserk", false, DamageOnSave.Zero, true, (byte)player.combat_team, 1, Classes.Affects.confuse_berserk, player);
				await Affects.Effect.Call(Effect.Add, null, player, Classes.Affects.confuse_berserk);
			}
			else if (var_1 >= 81 && var_1 <= 100)
			{
				ovr025.MagicAttackDisplay("is enraged", true, player);
				ovr025.ClearPlayerTextArea();
			}

			if (await ovr024.RollSavingThrow(-2, SaveVerseType.Spell, player) == true)
			{
				await ovr024.remove_affect(null, Classes.Affects.confuse, player);
			}

            return true;
		}


		internal static Task<bool> affect_curse(Effect arg_0, object param, Player player) /* sub_3AB6F */
		{
			gbl.attack_roll -= 4;
			gbl.savingThrowRoll -= 4;
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectBlink(Effect arg_0, object param, Player player) // has_action_timedout
		{
			if (player.actions.delay == 0)
			{
				gbl.targetInvisible = true;
				gbl.attack_roll = -1;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectHaste(Effect arg_0, object param, Player player) // spl_age
		{
			Affect affect = (Affect)param;

			if ((affect.affect_data & 0x10) == 0)
			{
				affect.affect_data += 0x10;

				ovr025.DisplayPlayerStatusString(true, 10, "ages", player);
				player.age++;
			}

			gbl.halfActionsLeft *= 2;

            return Task.FromResult(true);
        }


		internal static Task<bool> StinkingCloudAffect(Effect arg_0, object param, Player player) // sub_3AC1D
		{
			Affect affect = (Affect)param;

			var var_8 = gbl.StinkingCloud.Find(cell => cell.player == player && cell.field_1C == (affect.affect_data >> 4));

			if (var_8 != null)
			{
				ovr025.string_print01("The air clears a little...");

				for (int var_B = 0; var_B < 4; var_B++)
				{
					if (var_8.present[var_B] == true)
					{
						var tmp = var_8.targetPos + gbl.MapDirectionDelta[gbl.SmallCloudDirections[var_B]];

						bool var_9 = gbl.downedPlayers.Exists(cell => cell.target != null && cell.map == tmp);

						if (var_9 == true)
						{
							gbl.mapToBackGroundTile[tmp] = gbl.Tile_DownPlayer;
						}
						else
						{
							gbl.mapToBackGroundTile[tmp] = var_8.groundTile[var_B];
						}
					}
				}

				gbl.StinkingCloud.Remove(var_8);

				foreach (var var_4 in gbl.StinkingCloud)
				{
					for (int var_B = 0; var_B < 4; var_B++)
					{
						if (var_4.present[var_B] == true)
						{
							var tmp = gbl.MapDirectionDelta[gbl.SmallCloudDirections[var_B]] + var_4.targetPos;

							gbl.mapToBackGroundTile[tmp] = gbl.Tile_StinkingCloud;
						}
					}
				}
			}
            return Task.FromResult(true);
        }


		static void AvoidMissleAttack(int percentage, Player player) // sub_3AF06
		{
            if (gbl.SelectedPlayer.activeItems.primaryWeapon != null &&
				ovr025.getTargetRange(player, gbl.SelectedPlayer) == 0 &&
				ovr024.roll_dice(100, 1) <= percentage)
			{
				ovr025.DisplayPlayerStatusString(true, 10, "Avoids it", player);
				gbl.damage = 0;
				gbl.attack_roll = -1;
				gbl.attacksHit[1] -= 1;
			}
		}


		internal static Item get_primary_weapon(Player player) /* sub_3AF77 */
		{
			Item item = null;

            if (player.activeItems.primaryWeapon != null)
			{
				bool item_found = ovr025.GetCurrentAttackItem(out item, player);

				if (item_found == false || item == null)
				{
                    item = player.activeItems.primaryWeapon;
				}
			}

			return item;
		}


		internal static Task<bool> AffectProtNormalMissles(Effect arg_0, object param, Player player) // sub_3AFE0
		{
			Item item = get_primary_weapon(gbl.SelectedPlayer);

			if (item != null && item.plus == 0)
			{
				AvoidMissleAttack(100, player);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> Affectslow(Effect arg_0, object param, Player player) //sub_3B01B
		{
			gbl.halfActionsLeft /= 2;
            return Task.FromResult(true);
        }


		internal static Task<bool> weaken(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;

			if (addAffect(0x3c, affect.affect_data, Classes.Affects.weaken, player) == true)
			{
                if (player.stats.Str.Current > 3)
				{
					ovr025.DisplayPlayerStatusString(true, 10, "is weakened", player);
                    player.stats.Str.Current--;
				}
				else if (player.HasAffect(Classes.Affects.helpless) == true)
				{
					ovr024.add_affect(false, 0xff, 0, Classes.Affects.helpless, player);
				}
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }


		internal static async Task<bool> sub_3B0C2(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;

			if (addAffect(10, affect.affect_data, Classes.Affects.cause_disease_2, player) == true)
			{
				if (player.hit_point_current > 1)
				{
					gbl.damage_flags = 0;

					await ovr024.damage_person(false, 0, 1, player);

					if (gbl.game_state != GameState.Combat)
					{
						ovr025.PartySummary(gbl.SelectedPlayer);
					}
				}
				else if (player.HasAffect(Classes.Affects.helpless) == false)
				{
					ovr024.add_affect(false, 0xff, 0, Classes.Affects.helpless, player);
				}
                return true;
			}
            else
            {
                return false;
            }
		}


		internal static Task<bool> AffectGiantVsDwarfGnome(Effect arg_0, object param, Player player)
		{
			gbl.spell_target = player.actions.target;

			if (gbl.SelectedPlayer.flags.HasFlag(Flags.DwarfPenalty))
			{
				gbl.attack_roll -= 4;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectGnollBugbearVsGnome(Effect arg_0, object param, Player player)
		{
			if (gbl.SelectedPlayer.flags.HasFlag(Flags.GnomePenalty))
			{
				gbl.attack_roll -= 4;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectPrayer(Effect arg_0, object param, Player player) // sub_3B1C9
		{
			Affect affect = (Affect)param;

			CombatTeam team = (CombatTeam)((affect.affect_data & 0x10) >> 4);

			if (player.combat_team == team)
			{
                gbl.savingThrowRoll += 1;
                gbl.attack_roll += 1;
            }
			else
			{
				gbl.attack_roll -= 1;
				gbl.savingThrowRoll -= 1;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> HotFireShield(Effect arg_0, object param, Player player) // sub_3B212
		{
			if ((gbl.damage_flags & DamageType.Cold) != 0)
			{
				gbl.savingThrowRoll += 2;
			}
			else if ((gbl.damage_flags & DamageType.Fire) != 0 && gbl.savingThrowMade == false)
			{
				gbl.damage *= 2;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> ColdFireShield(Effect arg_0, object param, Player player) // sub_3B243
		{
			if ((gbl.damage_flags & DamageType.Fire) != 0)
			{
				gbl.savingThrowRoll += 2;
			}
			else if ((gbl.damage_flags & DamageType.Cold) != 0 && gbl.savingThrowMade == false)
			{
				gbl.damage *= 2;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> sub_3B27B(Effect arg_0, object param, Player player) // sub_3B27B
		{
			ovr024.add_affect(false, 12, 1, Classes.Affects.invisibility, player);
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectClearMovement(Effect arg_0, object param, Player player) //sub_3B29A
		{
			player.actions.move = 0;

			if (gbl.resetMovesLeft == true)
			{
				gbl.halfActionsLeft = 0;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectRegenration(Effect arg_0, object param, Player player)
		{
			ovr024.add_affect(false, 0xff, 0, Classes.Affects.regen_3_hp, player);
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectResistWeapons(Effect arg_0, object param, Player player) // sub_3B2D8
		{
			Item weapon = get_primary_weapon(gbl.SelectedPlayer);

			if (weapon == null ||
				weapon.plus == 0)
			{
				gbl.damage = 0;
			}
			else if (weapon.plus < 3)
			{
				gbl.damage /= 2;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectFireResist(Effect arg_0, object param, Player player)
		{
			if ((gbl.damage_flags & DamageType.Fire) != 0)
			{
				for (int i = 1; i <= gbl.dice_count; i++)
				{
					gbl.damage -= 2;

					if (gbl.damage < gbl.dice_count)
					{
						gbl.damage = gbl.dice_count;
					}
				}

				gbl.savingThrowRoll += 4;

				if ((gbl.damage_flags & DamageType.Magic) == 0)
				{
					Protected();
				}
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> AffectHighConRegen(Effect arg_0, object param, Player player) /* sub_3B386 */
		{
			Affect affect = (Affect)param;

			// BUGFIX: Only Regen when Con is high enough
			if (player.stats.Con.Current >= 20)
			{
				// Per 1e, healing is 1/6 turns at 20, 1/5 turns at 21, ... 1/1 turn at 25
				ushort rounds = (ushort)((26 - player.stats.Con.Current) * 10);
				if (addAffect(rounds, affect.affect_data, Classes.Affects.highConRegen, player) == true && 
					await ovr024.heal_player(1, 1, player) == true)
				{
					ovr025.DescribeHealing(player);
				}
                return true;
			}
            else
            {
                return false;
            }
		}


		internal static Task<bool> AffectMinorGlobeOfInvulnerability(Effect arg_0, object param, Player player) /* sub_3B3CA */
		{
			if (gbl.spell_id > 0 &&
				gbl.spellCastingTable[(byte)gbl.spell_id].spellLevel < 4)
			{
				Protected();
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> PoisonAttack(int save_bonus, Player player)
		{
			gbl.spell_target = player.actions.target;

            if (await ovr024.RollSavingThrow(save_bonus, SaveVerseType.Poison, gbl.spell_target) == false)
            {
                ovr025.DisplayPlayerStatusString(false, 10, "is Poisoned", gbl.spell_target);
                seg041.GameDelay();
                ovr024.add_affect(false, 0xff, 0, Classes.Affects.poisoned, gbl.spell_target);

                await ovr024.KillPlayer("is killed", Status.dead, gbl.spell_target);

                return true;
            }
            else
            {
                return false;
            }
		}


		internal static async Task<bool> AffectPoisonPlus0(Effect arg_0, object param, Player player) // sub_3B520
		{
			return await PoisonAttack(0, player);
		}


		internal static async Task<bool> AffectPoisonPlus4(Effect arg_0, object param, Player player) // sub_3B534
		{
			return await PoisonAttack(4, player);
		}


		internal static async Task<bool> AffectPoisonPlus2(Effect arg_0, object param, Player player) // sub_3B548
		{
			return await PoisonAttack(2, player);
		}


		internal static async Task<bool> ThriKreenParalyze(Effect arg_0, object param, Player player) // sub_3B55C
		{
			ushort time = ovr024.roll_dice(8, 2);

			gbl.spell_target = player.actions.target;

			if (await ovr024.RollSavingThrow(0, SaveVerseType.Poison, gbl.spell_target) == false)
			{
				ovr025.MagicAttackDisplay("is Paralyzed", true, gbl.spell_target);
				ovr024.add_affect(false, 12, time, Classes.Affects.paralyze, gbl.spell_target);

                return true;
			}
            else
            {
                return false;
            }
		}


		internal static Task<bool> AffectFeebleMind(Effect arg_0, object param, Player player) // spell_stupid
		{
            player.stats.Int.Current = 7;
            player.stats.Wis.Current = 7;

			ovr025.DisplayPlayerStatusString(true, 10, "is stupid", player);

			if (gbl.game_state == GameState.Combat)
			{
				ovr024.TryLooseSpell(player);
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectInvisToAnimals(Effect arg_0, object param, Player player) // sub_3B636
		{
			if (gbl.SelectedPlayer.flags.HasFlag(Flags.Animal))
			{
				if (gbl.SelectedPlayer.HasAffect(Classes.Affects.detect_invisibility) == false &&
					player.HasAffect(Classes.Affects.faerie_fire) == false)
				{
					gbl.targetInvisible = true;
					gbl.attack_roll -= 4;
                    return Task.FromResult(true);
                }
			}
            return Task.FromResult(false);
        }


		internal static async Task<bool> AffectPoisonNeg2(Effect arg_0, object param, Player player) // sub_3B671
		{
			return await PoisonAttack(-2, player);
		}


		internal static Task<bool> AffectInvisible(Effect arg_0, object param, Player player) // sub_3B685
		{
			gbl.targetInvisible = true;
			gbl.attack_roll -= 4;

            return Task.FromResult(true);
		}


		internal static Task<bool> AffectCamouflage(Effect arg_0, object param, Player player) // sub_3B696
		{
			if (ovr024.roll_dice(100, 1) <= 95)
			{
				ovr024.add_affect(false, 12, 1, Classes.Affects.invisibility, player);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> ProtDragonsBreath(Effect arg_0, object param, Player player)
		{
			if ((gbl.damage_flags & DamageType.DragonBreath) > 0)
			{
				Protected();
				ovr025.DisplayPlayerStatusString(true, 10, "is unaffected", player);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectDragonSlayer(Effect arg_0, object param, Player player) // sub_3B71A
		{
			if (player.actions != null &&
				player.actions.target != null)
			{
				gbl.spell_target = player.actions.target;

				if (gbl.spell_target.flags.HasFlag(Flags.Dragon))
				{
					gbl.damage = (ovr024.roll_dice(12, 1) * 3) + 4 + ovr025.strengthDamBonus(player);
					gbl.attack_roll += 2;
                    return Task.FromResult(true);
                }
			}
            return Task.FromResult(false);
        }


		internal static Task<bool> AffectFrostBrand(Effect arg_0, object param, Player player) // sub_3B772
		{
			if ((gbl.damage_flags & DamageType.Fire) != 0)
			{
				for (int i = 1; i <= gbl.dice_count; i++)
				{
					gbl.damage -= 2;

					if (gbl.damage < gbl.dice_count)
					{
						gbl.damage = gbl.dice_count;
					}
				}

				gbl.savingThrowRoll += 4;

				if ((gbl.damage_flags & DamageType.Magic) == 0)
				{
					Protected();
				}
			}
			if (player.actions != null)
			{
				gbl.spell_target = player.actions.target;

				if (gbl.spell_target != null &&
					gbl.spell_target.flags.HasFlag(Flags.Fire))
				{
					gbl.attack_roll += 3;
					gbl.damage += 3;
				}
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectBerzerk(Effect arg_0, object param, Player player)
		{
			if (arg_0 == Effect.Add)
			{
				player.quick_fight = QuickFight.True;

				if (player.control_morale < Control.NPC_Base ||
					player.control_morale == Control.PC_Berserk)
				{
					player.control_morale = Control.PC_Berserk;
				}
				else
				{
					player.control_morale = Control.NPC_Berserk;
				}

				if (gbl.game_state == GameState.Combat)
				{
					player.actions.target = null;

					var scl = ovr032.Rebuild_SortedCombatantList(player, 0xff, p => true);

					player.actions.target = scl[0].player;

					player.actions.can_cast = false;
					player.combat_team = player.actions.target.OppositeTeam();

					ovr025.DisplayPlayerStatusString(true, 10, "goes berserk", player);
				}
			}
			else
			{
				if (player.control_morale == Control.PC_Berserk)
				{
					player.control_morale = Control.PC_Base;
				}

				player.combat_team = CombatTeam.Ours;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> sub_3B8D9(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;

			if (ovr024.combat_heal(player.hit_point_current, player) == false)
			{
				addAffect(1, affect.affect_data, Classes.Affects.affect_4e, player);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> MagicFireAttack_2d10(Effect arg_0, object param, Player player) // sub_3B919
		{
			gbl.damage_flags = DamageType.Magic | DamageType.Fire;

			return await ovr024.damage_person(false, 0, ovr024.roll_dice_save(10, 2), player.actions.target);
		}


		internal static async Task<bool> AnkhegMeleeAcidAttack(Effect arg_0, object param, Player player) // sub_3B94C
		{
			gbl.damage_flags = DamageType.Acid;

			return await ovr024.damage_person(false, 0, ovr024.roll_dice_save(4, 1), player.actions.target);
		}


		internal static Task<bool> half_damage(Effect arg_0, object param, Player player) /* sub_3B97F */
		{
			gbl.damage /= 2;
            return Task.FromResult(true);
        }


		internal static async Task<bool> AffectResistFireAndCold(Effect arg_0, object param, Player player) // sub_3B990
		{
			if ((gbl.damage_flags & DamageType.Fire) != 0 ||
				(gbl.damage_flags & DamageType.Cold) != 0)
			{
				if (await ovr024.RollSavingThrow(0, SaveVerseType.Spell, player) == true &&
                    gbl.spell_id > 0 &&
					gbl.spellCastingTable[(byte)gbl.spell_id].damageOnSave != 0)
				{
					gbl.damage = 0;
				}
				else
				{
					gbl.damage /= 2;
				}
                return true;
			}
            else
            {
                return false;
            }
		}


		internal static Task<bool> AffectshamblerAbsorbLightning(Effect arg_0, object param, Player player) // sub_3B9E1
		{
			// Shambling Mounds absorb lighting and get more powerful.

			if ((gbl.damage_flags & DamageType.Electricity) != 0)
			{
				Protected();
                //byte var_1 = ovr024.roll_dice(8, 1);

                player.hit_point_current += 8;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectResistPiercing(Effect arg_0, object param, Player player) // sub_3BA14
		{
			Item item = get_primary_weapon(gbl.SelectedPlayer);

			if (item != null &&
				item.itemData.field_7 == 1)
			{
				gbl.damage = 1;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectDisplace(Effect arg_0, object param, Player player) /*sub_3BA55*/
		{
			Affect affect = (Affect)param;

			if (affect != null)
			{
				if (gbl.combat_round == 0 && gbl.attack_roll == 0)
				{
					affect.affect_data &= 0x0f;
				}
				else if ((affect.affect_data & 0x10) == 0)
				{
					gbl.attack_roll = -1;
					affect.affect_data |= 0x10;
				}
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> CloudKillAffect(Effect arg_0, object param, Player player) // sub_3BAB9
		{
			Affect affect = (Affect)param;

			GasCloud cell = gbl.CloudKillCloud.Find(c => c.player == player && c.field_1C == (affect.affect_data >> 4));

			if (cell != null)
			{
				ovr025.string_print01("The air clears a little...");

				for (int var_B = 0; var_B < 9; var_B++)
				{
					if (cell.present[var_B] == true)
					{
						var tmp = cell.targetPos + gbl.MapDirectionDelta[gbl.CloudDirections[var_B]];

						bool var_E = gbl.downedPlayers.Exists(c => c.target != null && c.map == tmp);

						if (var_E == true)
						{
							gbl.mapToBackGroundTile[tmp] = gbl.Tile_DownPlayer;
						}
						else
						{
							gbl.mapToBackGroundTile[tmp] = cell.groundTile[var_B];
						}
					}
				}


				gbl.CloudKillCloud.Remove(cell);

				foreach (var var_4 in gbl.CloudKillCloud)
				{
					for (int var_B = 0; var_B < 9; var_B++)
					{
						if (var_4.present[var_B] == true)
						{
							var tmp = var_4.targetPos + gbl.MapDirectionDelta[gbl.CloudDirections[var_B]];

							gbl.mapToBackGroundTile[tmp] = gbl.Tile_CloudKill;
						}
					}
				}
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> half_fire_damage(Effect arg_0, object param, Player arg_6) // sub_3BD98
		{
			if ((gbl.damage_flags & DamageType.Fire) != 0)
			{
				gbl.damage /= 2;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectResistBluntPierce(Effect arg_0, object param, Player arg_6) // sub_3BDB2
		{
			Item item = get_primary_weapon(gbl.SelectedPlayer);

			if (item != null &&
				(item.itemData.field_7 & 0x81) != 0)
			{
				gbl.damage /= 2;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> AffectDelayDeath(Effect arg_0, object param, Player player)
		{
			Affect affect = (Affect)param;
			affect.callAffectTable = false;

			if (player.in_combat == true)
			{
				await ovr024.KillPlayer("Falls dead", Status.dead, player);
                return true;
			}
            else
            {
                return false;
            }
		}


		internal static Task<bool> con_saving_bonus(Effect arg_0, object param, Player player) /* sub_3BE42 */
		{
			if (gbl.saveVerseType == SaveVerseType.Spell ||
				gbl.saveVerseType == SaveVerseType.RodStaffWand)
			{
				int save_bonus = 0;

                if (player.stats.Con.Current >= 4 && player.stats.Con.Current <= 6)
				{
					save_bonus = 1;
				}
                else if (player.stats.Con.Current >= 7 && player.stats.Con.Current <= 10)
				{
					save_bonus = 2;
				}
                else if (player.stats.Con.Current >= 11 && player.stats.Con.Current <= 13)
				{
					save_bonus = 3;
				}
                else if (player.stats.Con.Current >= 14 && player.stats.Con.Current <= 17)
				{
					save_bonus = 4;
				}
                else if (player.stats.Con.Current >= 18 && player.stats.Con.Current <= 20)
				{
					save_bonus = 5;
				}
				else if (player.stats.Con.Current >= 21 && player.stats.Con.Current <= 24)
				{
					save_bonus = 6;
				}
				else if (player.stats.Con.Current == 25)
				{
					save_bonus = 7;
				}

				gbl.savingThrowRoll += save_bonus;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectRegen3Hp(Effect arg_0, object param, Player player) // sub_3BEB8
		{
			player.hit_point_current += 3;

			if (player.hit_point_current > player.hit_point_max)
			{
				player.hit_point_current = player.hit_point_max;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> AffectFightUnconscious(Effect arg_0, object param, Player player) // sub_3BEE8
		{
			Affect arg_2 = (Affect)param;

			byte heal_amount = 0;

			if (player.health_status == Status.dying &&
				player.actions.bleeding < 6)
			{
				heal_amount = (byte)(6 - player.actions.bleeding);
			}

			if (player.health_status == Status.unconscious)
			{
				heal_amount = 6;
			}

			if (heal_amount > 0 &&
				ovr024.combat_heal(heal_amount, player) == true)
			{
				ovr024.add_affect(true, 0xff, (ushort)(ovr024.roll_dice(4, 1) + 1), Classes.Affects.delay_death, player);
				arg_2.callAffectTable = false;
				await ovr024.remove_affect(arg_2, Classes.Affects.fight_unconscious, player);
                return true;
			}
            else
            {
                return false;
            }
		}


		internal static Task<bool> AffectTrollFireOrAcid(Effect arg_0, object param, Player player)
		{
			if ((gbl.damage_flags & DamageType.Fire) == 0 &&
				(gbl.damage_flags & DamageType.Acid) == 0)
			{
				ovr024.add_affect(true, 0xff, ovr024.roll_dice(6, 3), Classes.Affects.TrollRegen, player);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectTrollRegenerate(Effect arg_0, object param, Player player) // sp_regenerate
		{
			if (player.HasAffect(Classes.Affects.regen_3_hp) == false &&
				player.HasAffect(Classes.Affects.regenerate) == false)
			{
				ovr024.add_affect(true, 0xff, 3, Classes.Affects.regenerate, player);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectTrollRegen(Effect arg_0, object param, Player player) // sub_3C01E
		{
			Affect affect = (Affect)param;

			if (ovr024.combat_heal(player.hit_point_max, player) == false)
			{
				addAffect(1, affect.affect_data, Classes.Affects.TrollRegen, player);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectsalamanderHeatDamage(Effect arg_0, object param, Player player) // sub_3C05D
		{
			gbl.spell_target = player.actions.target;

			if (gbl.spell_target.HasAffect(Classes.Affects.spell_resist_fire) == false &&
				gbl.spell_target.HasAffect(Classes.Affects.cold_fire_shield) == false &&
				gbl.spell_target.HasAffect(Classes.Affects.item_fire_resist) == false &&
				gbl.spell_target.HasAffect(Classes.Affects.weap_frost_brand) == false)
			{
				gbl.damage += ovr024.roll_dice(6, 1);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> sub_3C0DA(Effect arg_0, object param, Player player)
		{
			AvoidMissleAttack(60, player);
            return Task.FromResult(true);
        }


		internal static Task<bool> ResistMagicPercent(int rollBase) // sub_3C0EE
		{
			int target_count = ovr025.spellMaxTargetCount(gbl.spell_id);
			int rollNeeded = rollBase + ((11 - target_count) * 5);

			if (gbl.current_affect != 0 || (gbl.damage_flags & DamageType.Magic) != 0)
			{
				if (ovr024.roll_dice(100, 1) <= rollNeeded)
				{
					Protected();
                    return Task.FromResult(true);
                }
			}
            return Task.FromResult(false);
        }


		internal static Task<bool> ResistMagic50Percent(Effect arg_0, object param, Player arg_6) // sub_3C14F
		{
			ResistMagicPercent(50);
            return Task.FromResult(true);
        }


		internal static Task<bool> ResistMagic15Percent(Effect arg_0, object param, Player arg_6) // sub_3C15D
		{
			ResistMagicPercent(15);
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectElfRisistSleep(Effect arg_0, object param, Player arg_6) // sub_3C16B
		{
			if (ovr024.roll_dice(100, 1) <= 90)
			{
				ProtectedIf(Classes.Affects.sleep);
				ProtectedIf(Classes.Affects.charm_person);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectProtCharmSleep(Effect arg_0, object param, Player arg_6) // sub_3C18F
		{
			ProtectedIf(Classes.Affects.charm_person);
			ProtectedIf(Classes.Affects.sleep);
            return Task.FromResult(true);
        }


		internal static Task<bool> ResistParalyze(Effect arg_0, object param, Player arg_6) // sub_3C1A4
		{
			ProtectedIf(Classes.Affects.paralyze);
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectImmuneToCold(Effect arg_0, object param, Player arg_6) // sub_3C1B2
		{
			if ((gbl.damage_flags & DamageType.Cold) != 0)
			{
				Protected();
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectProtParalysisPoison(Effect arg_0, object param, Player arg_6) // sub_3C1C9
		{
			ProtectedIf(Classes.Affects.poisoned);
			ProtectedIf(Classes.Affects.paralyze);

			if (gbl.saveVerseType == SaveVerseType.Poison)
			{
				gbl.savingThrowRoll = 100;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }

		}


		internal static Task<bool> AffectImmuneToFire(Effect arg_0, object param, Player arg_6) // sub_3C1EA
		{
			if ((gbl.damage_flags & DamageType.Fire) != 0)
			{
				Protected();
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectEfreetiFireResist(Effect arg_0, object param, Player arg_6) // sub_3C201
		{
			if ((gbl.damage_flags & DamageType.Fire) != 0)
			{
				for (int i = 1; i <= gbl.dice_count; i++)
				{
					gbl.damage--;

					if (gbl.damage < gbl.dice_count)
					{
						gbl.damage = gbl.dice_count;
					}
                }
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }


		internal static Task<bool> AffectProtectionFromElectricity(Effect arg_0, object param, Player player) // sub_3C246
		{
			if ((gbl.damage_flags & DamageType.Electricity) != 0)
			{
				gbl.damage /= 2;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectResistPierceSlash(Effect arg_0, object param, Player player) // sub_3C260
		{
			Item weapon = get_primary_weapon(gbl.SelectedPlayer);

			if (weapon != null)
			{
				if (weapon.itemData.field_7 == 0 ||
					(weapon.itemData.field_7 & 1) != 0)
				{
					gbl.damage /= 2;
                    return Task.FromResult(true);
                }
			}
            return Task.FromResult(false);
        }


		internal static Task<bool> half_damage_if_weap_magic(Effect arg_0, object param, Player player) /* sub_3C2BF */
		{
			Item weapon = get_primary_weapon(gbl.SelectedPlayer);

			if (weapon != null &&
				weapon.plus > 0)
			{
				gbl.damage /= 2;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectVulnHolyWater(Effect arg_0, object param, Player player) // sub_3C2F9
		{
            Item item = gbl.SelectedPlayer.activeItems.primaryWeapon;

			if (item != null && item.type == Item.Type.HolyWater)
			{
				gbl.damage = ovr024.roll_dice_save(6, 1) + 1;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectProtCold(Effect arg_0, object param, Player player) // sub_3C33C
		{
			if ((gbl.damage_flags & DamageType.Cold) != 0)
			{
				gbl.damage /= 2;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectProtNonMagicWeapons(Effect arg_0, object param, Player player) // sub_3C356
		{
			Item weapon = get_primary_weapon(gbl.SelectedPlayer);

			if ((weapon == null || weapon.plus == 0) &&
				(gbl.SelectedPlayer.race > 0 || gbl.SelectedPlayer.HitDice < 4))
			{
				gbl.damage = 0;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectBoulderEvasion(Effect arg_0, object param, Player player) // sub_3C3A2
		{
			Item field_151 = player.activeItems.primaryWeapon;

			if (field_151 != null)
			{
				if (field_151.type == Item.Type.HillGiantBoulder || field_151.type == Item.Type.CloudGiantBoulder)
				{
					AvoidMissleAttack(50, player);
                    return Task.FromResult(true);
                }
			}
            return Task.FromResult(false);
        }


		internal static async Task<bool> AffectAnkhedRangedAcidAttack(Effect arg_0, object param, Player player) // sub_3C3F6
		{
			Affect affect = (Affect)param;

			gbl.spell_target = player.actions.target;

			if (ovr024.roll_dice(100, 1) <= 25)
			{
				if (ovr025.getTargetRange(gbl.spell_target, player) < 4)
				{
					ovr025.clear_actions(player);

					ovr025.DisplayPlayerStatusString(true, 10, "Spits Acid", player);

					ovr025.load_missile_icons(0x17);

					ovr025.draw_missile_attack(0x1e, 1, ovr033.PlayerMapPos(gbl.spell_target), ovr033.PlayerMapPos(player));

					int damage = ovr024.roll_dice_save(4, 8);
					bool saved = await ovr024.RollSavingThrow(0, SaveVerseType.BreathWeapon, gbl.spell_target);

					await ovr024.damage_person(saved, DamageOnSave.Half, damage, gbl.spell_target);

					await ovr024.remove_affect(affect, Classes.Affects.ankheg_ranged_acid_attack, player);
					await ovr024.remove_affect(null, Classes.Affects.ankheg_melee_acid_attack, player);

                    return true;
				}
			}
            return false;
		}


		internal static async Task<bool> AffectDracolichParalysis(Effect arg_0, object param, Player player) // spl_paralyze
		{
			gbl.spell_target = player.actions.target;

			if (await ovr024.RollSavingThrow(0, 0, gbl.spell_target) == false)
			{
				ovr024.add_affect(false, 0xff, 0, Classes.Affects.paralyze, gbl.spell_target);

				ovr025.DisplayPlayerStatusString(true, 10, "is paralyzed", gbl.spell_target);

                return true;
			}
            else
            {
                return false;
            }
		}


		internal static async Task<bool> AffectDracolichColdDamage(Effect arg_0, object param, Player player) // sub_3C59
		{
			gbl.damage_flags = DamageType.Cold; // was DamageType.Acid;

			await ovr024.damage_person(false, 0, ovr024.roll_dice_save(8, 2), player.actions.target);

            return true;
		}


		internal static Task<bool> AffectHalfElfResistance(Effect arg_0, object param, Player player) // sub_3C5D0
		{
			if (ovr024.roll_dice(100, 1) <= 30)
			{
				ProtectedIf(Classes.Affects.charm_person);
				ProtectedIf(Classes.Affects.sleep);
                return Task.FromResult(true);
			}
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectProtSleepCharmParalysisPoison(Effect arg_0, object param, Player player) // sub_3C5F4
		{
			ProtectedIf(Classes.Affects.charm_person);
			ProtectedIf(Classes.Affects.sleep);
			ProtectedIf(Classes.Affects.paralyze);
			ProtectedIf(Classes.Affects.poisoned);

			if (gbl.saveVerseType != SaveVerseType.Poison)
			{
				gbl.savingThrowRoll = 100;
			}

            return Task.FromResult(true);
		}


		internal static Task<bool> AffectProtMagic(Effect arg_0, object param, Player player) // sub_3C623
		{
			if (gbl.current_affect != 0 ||
				(gbl.damage_flags & DamageType.Magic) != 0)
			{
				Protected();

                return Task.FromResult(true);
			}
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> AffectVulnBlessedQuarrel(Effect arg_0, object arg_2, Player player) // sub_3C643
		{
			Item item;

			if (ovr025.GetCurrentAttackItem(out item, gbl.SelectedPlayer) == true &&
				item != null &&
				item.type == Item.Type.Quarrel &&
				item.namenum[2] == Item.Names.Blessed)
			{
				player.health_status = Status.gone;
				player.in_combat = false;
				player.hit_point_current = 0;
				await ovr024.RemoveCombatAffects(player);
				await Affects.Effect.Check(player, CheckType.Death);

				if (player.in_combat == true)
				{
					ovr033.CombatantKilled(player);
				}
			}
            return true;
		}


		internal static async Task<bool> do_items_affect(Effect remove_affect, object param, Player player) /* sub_3C6D3 */
		{
			Item item = (Item)param;

			gbl.applyItemAffect = false;

			if (remove_affect == Effect.Remove)
			{
				await ovr024.remove_affect(null, item.Affect_2, player);
			}
			else
			{
				ovr024.add_affect(true, 0xff, 0, item.Affect_2, player);

				if (gbl.game_state != GameState.Combat)
				{
					await Affects.Effect.Call(Effect.Add, null, player, item.Affect_2);
				}
			}
            return true;
		}


		internal static Task<bool> AffectDracolichProtection(Effect arg_0, object param, Player player) //sub_3C750
		{
			ProtectedIf(Classes.Affects.fear);
			ProtectedIf(Classes.Affects.ray_of_enfeeblement);
			ProtectedIf(Classes.Affects.feeblemind);

			if ((gbl.damage_flags & DamageType.Electricity) != 0)
			{
				Protected();
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectRangerVsGiant(Effect arg_0, object param, Player player) // sub_3C77C
		{
			gbl.spell_target = player.actions.target;

			if (gbl.spell_target.flags.HasFlag(Flags.RangerBonus))
			{
				gbl.damage += player.ranger_lvl;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectProtElec(Effect arg_0, object param, Player player)//sub_3C7B5
		{
			if ((gbl.damage_flags & DamageType.Electricity) != 0)
			{
				Protected();
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectEntangle(Effect arg_0, object param, Player player) // sub_3C7CC
		{
			player.actions.move = 0;
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectConfuseBerserk(Effect arg_0, object param, Player player) // sub_3C7E0
		{
			Affect affect = (Affect)param;

			if (arg_0 == Effect.Add)
			{
				player.quick_fight = QuickFight.True;

				if (player.control_morale < Control.NPC_Base ||
					player.control_morale == Control.PC_Berserk)
				{
					player.control_morale = Control.PC_Berserk;
				}
				else
				{
					player.control_morale = Control.NPC_Berserk;
				}

				player.actions.target = null;

				var scl = ovr032.Rebuild_SortedCombatantList(player, 0xff, p => true);

				player.actions.target = scl[0].player;
				player.combat_team = player.actions.target.OppositeTeam();
			}
			else
			{
				if (player.control_morale == Control.PC_Berserk)
				{
					player.control_morale = 0;
				}

				player.combat_team = (CombatTeam)affect.affect_data;
			}
            return Task.FromResult(true);
        }


		internal static Task<bool> AffectAddInvisibility(Effect arg_0, object param, Player player) // add_affect_19
		{
			ovr024.add_affect(false, 0xff, 0xff, Classes.Affects.invisibility, player);
            return Task.FromResult(true);
        }


		internal static Task<bool> PaladinCastCureRefresh(Effect add_remove, object param, Player player) // sub_3C8EF
		{
			if (add_remove == Effect.Remove)
			{
				player.paladinCuresLeft = (byte)(((player.SkillLevel(SkillType.Paladin) - 1) / 5) + 1);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static Task<bool> AffectFear(Effect add_remove, object param, Player player) /* sub_3C932 */
		{
			if (add_remove == Effect.Remove)
			{
				if (player.control_morale == Control.PC_Berserk)
				{
					player.control_morale = Control.PC_Base;
					player.quick_fight = QuickFight.False;
				}

				player.actions.fleeing = false;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
		}


		internal static async Task<bool> AffectFireShieldDamage(Effect arg_0, object arg_2, Player target)
		{
            if (ovr025.getTargetRange(target, gbl.SelectedPlayer) < 2)
            {
                int bkup_damage = gbl.damage;
                DamageType bkup_damage_flags = gbl.damage_flags;

                gbl.damage *= 2;
                gbl.damage_flags = DamageType.Magic;

                ovr025.DisplayPlayerStatusString(true, 10, "gets zapped", gbl.SelectedPlayer);

                await ovr024.damage_person(false, 0, gbl.damage, gbl.SelectedPlayer);
                gbl.damage = bkup_damage;
                gbl.damage_flags = bkup_damage_flags;
                return true;
            }
            else
            {
                return false;
            }
		}


		internal static async Task<bool> AffectDispelEvilBanish(Effect arg_0, object param, Player player)
		{
			gbl.spell_target = player.actions.target;

			if ((gbl.spell_target.flags & Flags.EvilSummon) != 0 &&
				await ovr024.RollSavingThrow(0, SaveVerseType.Spell, gbl.spell_target) == false)
			{
				await ovr024.KillPlayer("is dispelled", Status.gone, gbl.spell_target);

				await ovr024.remove_affect(null, Classes.Affects.dispel_evil, gbl.SelectedPlayer);
				await ovr024.remove_affect(null, Classes.Affects.dispel_evil_banish, gbl.SelectedPlayer);
                return true;
			}
			else
			{
				ovr025.DisplayPlayerStatusString(true, 10, "resists dispel evil", gbl.spell_target);
                return false;
			}
		}

		internal static Task<bool> empty(Effect arg_0, object param, Player player)
		{
            return Task.FromResult(false);
		}
	}
}
