using System.Collections.Generic;

namespace Classes
{
    public class SpellBook
    {
        public List<Spells> spellBook = new List<Spells>();

        public SpellBook() { }

        public SpellBook(SpellBook spellBook)
        {
            spellBook.spellBook.ForEach(spell => LearnSpell(spell));
        }
        public SpellBook(List<Spells> spells)
        {
            spells.ForEach(spell => LearnSpell(spell));
        }
        // overload operator +
        public static SpellBook operator +(SpellBook a, SpellBook b)
        {
            var c = new SpellBook(a);

            b.spellBook.ForEach(spell => c.LearnSpell(spell));

            return c;
        }

        public void Clear()
        {
            spellBook.Clear();
        }

        public IEnumerable<Spells> LearntList()
        {
            foreach (var sp in spellBook)
            {
                yield return sp;
            }
        }

        public bool KnowsSpell(Spells spell)
        {
            return spellBook.Contains(spell);
        }

        public void LearnSpell(Spells spell)
        {
            if (!spellBook.Contains(spell))
            {
                spellBook.Add(spell);
            }
        }

        public void UnlearnSpell(Spells spell)
        {
            spellBook.Remove(spell);
        }
    }
}
