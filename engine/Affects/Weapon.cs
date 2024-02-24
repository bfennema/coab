using Classes;

namespace engine.Affects
{
    internal class Weapon
    {
        internal static void UndeadSlayer(Classes.Effect arg_0, object param, Player player)
        {
            int bonus = 0;

            if (player.actions != null && player.actions.target != null)
            {
                gbl.spell_target = player.actions.target;

                if (gbl.spell_target.flags.HasFlag(Flags.Undead))
                {
                    bonus = 2;
                }
                else
                {
                    bonus = 0;
                }
            }
            gbl.attack_roll += bonus;
            gbl.damage += bonus;
        }
    }
}
