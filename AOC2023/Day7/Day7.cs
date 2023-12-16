
using System.Text;
using Xunit.Abstractions;

namespace AOC2023.Day7;

public class Day7 : Day
{
    public Day7(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "5905";

    public override async Task<string> Run(StreamReader inputData)
    {
        var s = "";
        var allHands = new List<Hand>();
        while((s = inputData.ReadLine()) != null)
        {
            var data = s.Split(' ');
            allHands.Add(new Hand(data[0], long.Parse(data[1])));
        }

        allHands.Sort();
        var sum = 0L;
        for(int i = 0; i < allHands.Count; i++)
        {
            sum += (i + 1) * allHands[i].Bid;
        }
        return sum.ToString();
    }

    public enum HandType : int
    {
        HighCard = 1,
        OnePair = 2,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    public class Hand : IComparable<Hand>
    {
        public string HandString { get; }
        public long Bid { get; set; }
        public HandType HandType { get; }

        public Hand(string hand, long bid)
        {
            HandString = hand;
            Bid = bid;
            if (HandString.All(h => h == 'J'))
                HandType = HandType.FiveOfAKind;
            else if (HandString.Contains('J'))
            {
                var permuteTo = HandString.Where(c => c != 'J').ToList();
                var permutations = new Dictionary<int, List<char>>(); //joker pos, permutations to test
                for(int i = 0; i < hand.Length; i++)
                {
                    if (hand[i] == 'J')
                        permutations.Add(i, permuteTo);
                }
                HandType = GetHandType(hand, permutations).OrderByDescending(c => c).First();
            }
            else HandType = GetHandType(hand);
        }

        private IEnumerable<HandType> GetHandType(string hand, Dictionary<int, List<char>> permutations)
        {
            if (permutations.Count == 0)
            {
                yield return GetHandType(hand);
                yield break;
            }

            var kv = permutations.First();
            foreach(var v in kv.Value)
            {
                var sb = new StringBuilder(hand);
                sb[kv.Key] = v;
                var newHand = sb.ToString();
                foreach (var ht in GetHandType(newHand, permutations.Skip(1).ToDictionary(k => k.Key, v => v.Value)))
                    yield return ht;
            }
        }

        private HandType GetHandType(string hand)
        {
            var gHandCardCount = hand.GroupBy(h => h).Select(h => h.Count()).ToList();
            if (gHandCardCount.Count == 1)
                return HandType.FiveOfAKind;
            else if (gHandCardCount.Count == 2)
            {
                if (gHandCardCount.Contains(4))
                    return HandType.FourOfAKind;
                else
                    return HandType.FullHouse;
            }
            else if (gHandCardCount.Count == 3)
            {
                if (gHandCardCount.Contains(3))
                    return HandType.ThreeOfAKind;
                else return HandType.TwoPair;
            }
            else if (gHandCardCount.Count == 4)
            {
                return HandType.OnePair;
            }
            else if (gHandCardCount.Count == 5)
            {
                return HandType.HighCard;
            }
            throw new Exception("Unable to find a hand type");
        }

        public int CompareTo(Hand? other)
        {
            if(HandType != other.HandType)
                return HandType.CompareTo(other.HandType);
            for(int i = 0; i < HandString.Length; i++)
            {
                var c1 = HandString[i];
                var c2 = other.HandString[i];
                if (c1 == c2)
                    continue;
                var c1v = GetValue(c1); 
                var c2v = GetValue(c2);
                return c1v.CompareTo(c2v);
            }
            return 0;
        }

        private static int GetValue(char card)
        {
            return card switch
            {
                'T' => 10,
                'J' => 1,
                'Q' => 12,
                'K' => 13,
                'A' => 14,
                _ when char.IsDigit(card) => card - '0',
                _ => throw new ArgumentOutOfRangeException(nameof(card)),
            };
        }
    }
}
