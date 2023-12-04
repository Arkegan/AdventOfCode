using Xunit.Abstractions;

namespace AOC2023.Day4;

public class Day4 : Day
{
    public Day4(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "30";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        var allCards = new Dictionary<int,  Card>();
        int i = 0;
        while((l = await inputData.ReadLineAsync()) != null)
        {
            allCards.Add(i++, new Card(i, l, allCards));
        }

        var total = 0;
        foreach(var c in allCards.Values)
        {
            total += c.AdvancedScore.Value + 1;
        }

        return total.ToString();
    }

    private class Card
    {
        private readonly Dictionary<int, Card> board;

        public int Id { get; set; }
        public int Score { get; set; }
        public Lazy<int> AdvancedScore { get; }

        public Card(int id, string l, Dictionary<int, Card> board)
        {
            var winningNumbers = l
                .Split(new char[] { ':', '|' }, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(n => int.Parse(n)).ToList();
            var myNumbers = l
                .Split(new char[] { ':', '|' }, StringSplitOptions.RemoveEmptyEntries)[2]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(n => int.Parse(n))
                .ToList();

            var res = winningNumbers.Where(n => myNumbers.Contains(n)).ToList();
            Score = res.Count;
            Id = id;
            this.board = board;
            AdvancedScore = new Lazy<int>(CalculateAdvancedScore);
        }

        private int CalculateAdvancedScore()
        {
            int sum = Score;
            for(int i = 1; i <= Score; i++)
            {
                sum += board[Id + i - 1].AdvancedScore.Value;
            }
            return sum;
        }
    }
}
