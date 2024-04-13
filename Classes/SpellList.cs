﻿using System;
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
    }




    public class SpellList
    {
        public const int SpellListSize = 84;

        public List<SpellItem> spells = new List<SpellItem>();

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
            found.Learning = true;
            //spells.Remove(found);
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

        public void AddLearnt(int id)
        {
            if (gbl.game != Game.CurseOfTheAzureBonds || (id & 0x7F) != (int)Spells.animate_dead)
            {
                spells.Add(new SpellItem((Spells)(id & 0x7F), id > 0x7f));
            }
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

        public void Load(byte[] data, int offset, int size = SpellListSize)
        {
            for (int i = 0; i < size; i++)
            {
                if (data[offset + i] > 0)
                {
                    AddLearnt(data[offset + i]);
                }
            }
        }

        public void Save(byte[] data, int offset, int size = SpellListSize)
        {
            for (int i = 0; i < size; i++)
            {
                data[offset + i] = 0;
            }

            int idx = size - 1;
            foreach (var sp in spells)
            {
                if (sp.Learning == false)
                {
                    data[offset + idx] = (byte)sp.Id;
                    idx -= 1;
                }
            }
        }
    }
}
