using System.Collections.Generic;

namespace Classes
{
    public class Exp
    {
        internal class Entry
        {
            int[] _table;
            int _additional;
            internal protected Entry(int[] table, int additional)
            {
                _table = table;
                _additional = additional;
            }
            internal int Cost(int level)
            {
                if (level >= _table.Length)
                {
                    return _table[^1] + (1 + level - _table.Length) * _additional;
                }
                else
                {
                    return _table[level];
                }
            }
        }
        private enum classList
        {
            Cleric,
            Druid,
            Fighter,
            Paladin,
            Ranger,
            MagicUser,
            Thief,
            Monk,
        }
        private readonly static Dictionary<classList, Entry> expTable = new Dictionary<classList, Entry>()
        {
            [classList.Cleric] = new Entry([0, 1501, 3001, 6001, 13001, 27501, 55001, 110001, 225001, 450001], 225000),
            [classList.Druid] = new Entry([0, 2001, 4001, 7501, 12501, 20001, 35001, 60001, 90001, 125001, 200001, 300001, 750001, 1500001], 0),
            [classList.Fighter] = new Entry([0, 2001, 4001, 8001, 18001, 35001, 70001, 125001, 250001], 225000),
            [classList.Paladin] = new Entry([0, 2751, 5501, 12001, 24001, 45001, 95001, 175001, 350001], 350000),
            [classList.Ranger] = new Entry([0, 2251, 4501, 10001, 20001, 40001, 90001, 150001, 225001, 325001], 325000),
            [classList.MagicUser] = new Entry([0, 2501, 5001, 10001, 22501, 40001, 60001, 90001, 135001, 250001, 375001], 375000),
            [classList.Thief] = new Entry([0, 1251, 2501, 5001, 10001, 20001, 42501, 70000, 110001, 160001, 220001], 220000),
            [classList.Monk] = new Entry([0, 2251, 4751, 10001, 22501, 47501, 98001, 200001, 350001, 500001, 700001, 950001, 1250001], 500000),
        };

        public static int Cost(Player player, SkillType skill, int level)
        {
            classList index;
            switch (skill)
            {
                case SkillType.Cleric: index = classList.Cleric; break;
                case SkillType.Druid: index = classList.Druid; break;
                case SkillType.Fighter: index = classList.Fighter; break;
                case SkillType.Paladin: index = classList.Paladin; break;
                case SkillType.Ranger: index = classList.Ranger; break;
                case SkillType.MagicUser: index = classList.MagicUser; break;
                case SkillType.Thief: index = classList.Thief; break;
                case SkillType.Monk: index = classList.Monk; break;
                default: index = classList.Fighter; break;
            }
            return expTable[index].Cost(level);
        }
    }
}
