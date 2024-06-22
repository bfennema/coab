using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class SpellItem
    {
        public Spells Id;
        public bool Learning;

        public SpellItem() { Id = 0; Learning = false; }
        public SpellItem(Spells id) { Id = id; Learning = false; }
        public SpellItem(Spells id, bool learning) { Id = id; Learning = learning; }
        public SpellItem(byte id, bool learning) { Id = (Spells)id; Learning = learning; }
        public SpellItem ShallowClone()
        {
            SpellItem s = (SpellItem)this.MemberwiseClone();
            return s;
        }
    }

    public class SpellList
    {
        public const int SpellListSize = 84;

        public List<SpellItem> spells = new List<SpellItem>();

        public SpellList() { }

        public SpellList(SpellList spellList)
        {
            spellList.spells.ForEach(spell => spells.Add(spell.ShallowClone()));
        }

        public void Clear()
        {
            spells.Clear();
        }

        public void ClearSpell(Spells spellId)
        {
            SpellItem found = null;

            foreach (var sp in spells)
            {
                if (sp.Id == spellId && sp.Learning == false)
                {
                    found = sp;
                    break;
                }
            }

            spells.Remove(found);
        }

        public void ClearSpell(int spellId)
        {
            ClearSpell((Spells)spellId);
        }

        public IEnumerable<Spells> IdList()
        {
            foreach (var sp in spells)
            {
                yield return sp.Id;
            }
        }

        public IEnumerable<Spells> LearntList()
        {
            foreach (var sp in spells)
            {
                if (sp.Learning == false)
                {
                    yield return sp.Id;
                }
            }
        }

        public IEnumerable<Spells> LearningList()
        {
            foreach (var sp in spells)
            {
                if (sp.Learning)
                {
                    yield return sp.Id;
                }
            }
        }

        public void AddLearn(Spells id)
        {
            spells.Add(new SpellItem(id, true));
        }

        public void AddLearn(byte id)
        {
            spells.Add(new SpellItem(id, true));
        }

        public void AddLearnt(Spells id, bool learning)
        {
            spells.Add(new SpellItem(id, learning));
        }

        public void AddLearnt(int id)
        {
            spells.Add(new SpellItem((Spells)(id & 0x7F), id > 0x7f));
        }

        public void MarkLearnt(Spells id)
        {
            var spell = spells.Find(sp => sp.Id == id && sp.Learning == true);

            if (spell != null)
            {
                spell.Learning = false;
            }
        }

        public bool HasSpells()
        {
            return spells.Count > 0;
        }

        public bool HasSpell(Spells id)
        {
            return spells.Exists(sp => sp.Id == id);
        }

        public void CancelLearning()
        {
            spells.RemoveAll(sp => sp.Learning == true);
        }
    }
}
