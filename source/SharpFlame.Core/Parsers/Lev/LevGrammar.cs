using Sprache;

namespace SharpFlame.Core.Parsers.Lev
{
    public class LevGrammar
    {
        //level   test_flame-T1
        public static readonly Parser<string> Level =
            from level in Parse.String("level")
            from name in Parse.AnyChar.AtLeastOnce().Token().Text()
            select name;

        //players 2
        public static readonly Parser<int> Players =
            from players in Parse.String("players")
            from number in Parse.Number.Token()
            let n = int.Parse(number)
            select n;
    }
}