using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace AiToSimulateHumanBehaviourInPoker
{
    //=================================================================================================================
    // Card Class
    //=================================================================================================================
    public enum CardRanks
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public enum CardSuits
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public class Card
    {
        public CardRanks Rank { get; }
        public CardSuits Suit { get; }

        // Dictionaries for nicer card formatting
        private static readonly Dictionary<CardSuits, string> SuitUnicode = new Dictionary<CardSuits, string>
        {
            { CardSuits.Clubs, "\u2663" },    // ♣
            { CardSuits.Diamonds, "\u2666" }, // ♦
            { CardSuits.Hearts, "\u2665" },   // ♥
            { CardSuits.Spades, "\u2660" }    // ♠
        };

        private static readonly Dictionary<CardRanks, string> RankDict = new Dictionary<CardRanks, string>
        {
            { CardRanks.Ace, "A"},
            { CardRanks.Two, "2" },
            { CardRanks.Three, "3" },
            { CardRanks.Four, "4" },
            { CardRanks.Five, "5" },
            { CardRanks.Six, "6"},
            { CardRanks.Seven, "7"},
            { CardRanks.Eight, "8"},
            { CardRanks.Nine, "9"},
            { CardRanks.Ten, "10"},
            { CardRanks.Jack, "J"},
            { CardRanks.Queen, "Q" },
            { CardRanks.King, "K"}
        };

        public Card(CardRanks rank, CardSuits suit)
        {
            Rank = rank;
            Suit = suit;
        }

        // Polymorphism so cards can be printed in a specific way
        public override string ToString()
        {
            return $"{RankDict[Rank]}{SuitUnicode[Suit]}";
        }
    }


    //=================================================================================================================
    // Deck Class
    //=================================================================================================================
    public class Deck
    {
        protected Card[] DeckOfCards;

        public Deck()
        {
            const int DECKOFCARDS_SIZE = 52;
            DeckOfCards = new Card[DECKOFCARDS_SIZE];
        }

        public void GenerateDeck()
        {
            int index = 0;

            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
            {
                foreach (CardRanks rank in Enum.GetValues(typeof(CardRanks)))
                {
                    DeckOfCards[index++] = new Card(rank, suit);
                }
            }
        }

        public void DisplayDeck()
        {
            foreach (Card card in DeckOfCards)
            {
                Console.WriteLine(card);
            }
        }

        public void ShuffleDeck()
        {
            // Fisher-Yates Shuffle
            Random random = new Random();

            for (int Pos = DeckOfCards.Length - 1; Pos > 0; Pos--)
            {
                int RandomPos = random.Next(0, Pos + 1);

                Card temp = DeckOfCards[Pos];
                DeckOfCards[Pos] = DeckOfCards[RandomPos];
                DeckOfCards[RandomPos] = temp;
            }
        }

        public void ResetDeck()
        {
            GenerateDeck();
        }

        public void DealCard(Player player)
        {
            for (int i = 0; i < DeckOfCards.Length; i++)
            {
                if (DeckOfCards[i] != null)
                {
                    for (int j = 0; j < player.Hand.Length; j++)
                    {
                        if (player.Hand[j] == null)
                        {
                            player.Hand[j] = DeckOfCards[i];
                            DeckOfCards[i] = null;
                        }
                    }
                }
            }
        }

        public void AddCardsToTable(Table Gametable, int NumberOfCards)
        {
            int cardsDealt = 0;

            for (int i = 0; i < DeckOfCards.Length && cardsDealt < NumberOfCards; i++)
            {
                if (DeckOfCards[i] != null)
                {
                    for (int j = 0; j < Gametable.PokerTable.Length; j++)
                    {
                        if (Gametable.PokerTable[j] == null)
                        {
                            Gametable.PokerTable[j] = DeckOfCards[i];
                            DeckOfCards[i] = null; // Mark the dealt card as null in the deck
                            cardsDealt++;
                            break; // Move to the next card after dealing
                        }
                    }
                }
            }
        }
    }

    //=================================================================================================================
    // Table Class
    //=================================================================================================================
    public class Table
    {
        public int PotValue;
        public Card[] PokerTable;

        public Table()
        {
            const int TABLE_SIZE = 5;
            PokerTable = new Card[TABLE_SIZE];
            PotValue = 0;
        }

        public void Show()
        {
            Console.WriteLine($"Pot Value: {PotValue}");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"{i + 1} - {PokerTable[i]}");
            }
        }

        public void ResetPotValue()
        {
            PotValue = 0;
        }

        public void ResetCards()
        {
            for (int i = 0; i < PokerTable.Length; i++)
            {
                PokerTable[i] = null;
            }
        }
    }

    //=================================================================================================================
    // Player Class
    //=================================================================================================================
    public class Player
    {
        public string Name { get; set; }
        public int ChipCount { get; set; }
        public int WinCount { get; set; }
        public int CurrentBet;
        public Card[] Hand;
        public bool Folded;
        public bool Checked;

        public Player()
        {
            const int HAND_SIZE = 2;
            Hand = new Card[HAND_SIZE];

            CurrentBet = 0;
            Folded = false;
            Checked = false;
        }

        public void Show()
        {
            Console.WriteLine($"{Name}");
            Console.WriteLine($"Chip Count: {ChipCount}");
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine(Hand[i]);
            }
        }

        public void ResetHand()
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                Hand[i] = null; 
            }
        }

        public virtual void PlaceBet(int BetAmount)
        {
            if (ChipCount >= BetAmount)
            {
                ChipCount -= BetAmount;
                CurrentBet += BetAmount;
                Console.WriteLine($"{Name} placed a bet of {BetAmount} chips");
            }
            else
            {
                Console.WriteLine($"{Name} doesn't have enough chips");
            }
        }

        public bool Fold()
        {
            Console.WriteLine($"{Name} folds");
            Folded = true;
            return Folded;
        }

        public bool Check()
        {
            Console.WriteLine($"{Name} checks");
            Checked = true;
            return Checked;
        }
    }

    //=================================================================================================================
    // AI Class
    //=================================================================================================================
    public class AI : Player
    {
        public AI()
        {
            ChipCount = 10000; // Default ChipCount
            Name = "Ai";
        }

        public override void PlaceBet(int MainPlayerBet)
        {
            Random random = new Random();
           
            bool BetLastRound = false;

            PokerHand AiHandStength = HandEvaluator.EvaluateHoleCards(Hand);

            if (ChipCount != 0)
            {
                if (MainPlayerBet > ChipCount)
                {
                    Console.WriteLine($"{Name} goes all in!");
                    base.PlaceBet(ChipCount);
                }
                else
                {
                    if (MainPlayerBet != CurrentBet)
                    {
                        base.PlaceBet(MainPlayerBet);
                    }
                    else if (AiHandStength == PokerHand.Pair && random.Next(1, 11) <= 8 && BetLastRound == false) // 80% chance of betting if pair
                    {
                        int BetAmount = random.Next(1, ChipCount / 2);
                        base.PlaceBet(BetAmount);
                        BetLastRound = true;
                    }
                    else if (AiHandStength == PokerHand.HighCard && random.Next(1, 11) <= 9) // 90% chance of checking
                    {
                        Check();
                    }
                    else
                    {
                        Fold(); // 10% chance of folding 
                    }
                }
            }
            else
            {
                Console.WriteLine($"{Name} is out of chips, continue playing");
                return;
            }
            
        }
    }

    //=================================================================================================================
    // Poker Hand Evaluator Class
    //=================================================================================================================
    public enum PokerHand
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public class HandEvaluator
    {
        private static bool HasPair(Card[] Cards)
        {
            return Cards.GroupBy(c => c.Rank).Any(g => g.Count() == 2);
        }

        private static bool HasTwoPair(Card[] Cards)
        {
            return Cards.GroupBy(c => c.Rank).Count(g => g.Count() == 2) == 2;
        }

        private static bool HasThreeOfAKind(Card[] Cards)
        {
            return Cards.GroupBy(c => c.Rank).Any(g => g.Count() == 3);
        }

        private static bool HasStraight(Card[] Cards)
        {
            var distinctRanks = Cards.Select(c => c.Rank).Distinct().OrderByDescending(c => c).ToList();

            // Check for Ace-low straight
            if (distinctRanks.Contains(CardRanks.Ace) && distinctRanks.Contains(CardRanks.Two) && distinctRanks.Contains(CardRanks.Three)
                && distinctRanks.Contains(CardRanks.Four) && distinctRanks.Contains(CardRanks.Five))
            {
                return true;
            }

            // Check for Ace-high straight
            if (distinctRanks.Contains(CardRanks.Ace) && distinctRanks.Contains(CardRanks.King) && distinctRanks.Contains(CardRanks.Queen)
                && distinctRanks.Contains(CardRanks.Jack) && distinctRanks.Contains(CardRanks.Ten))
            {
                return true;
            }

            // Check for other straights
            int consecutiveCount = 1;
            for (int i = 0; i < distinctRanks.Count - 1; i++)
            {
                if (distinctRanks[i] - 1 == distinctRanks[i + 1])
                {
                    consecutiveCount++;
                    if (consecutiveCount >= 5)
                    {
                        return true;
                    }
                }
                else
                {
                    consecutiveCount = 1;
                }
            }

            return false;
        }

        private static bool HasFlush(Card[] Cards)
        {
            return Cards.GroupBy(c => c.Suit).Any(g => g.Count() == 5);
        }

        private static bool HasFullHouse(Card[] Cards)
        {
            return HasPair(Cards) && HasThreeOfAKind(Cards);
        }

        private static bool HasFourOfAKind(Card[] Cards)
        {
            return Cards.GroupBy(c => c.Rank).Any(g => g.Count() == 4);
        }

        private static bool HasStraightFlush(Card[] Cards)
        {
            return HasStraight(Cards) && HasFlush(Cards);
        }

        private static bool HasRoyalFlush(Card[] Cards)
        {
            return HasFlush(Cards) &&
           Cards.Any(c => c.Rank == CardRanks.Ten) &&
           Cards.Any(c => c.Rank == CardRanks.Jack) &&
           Cards.Any(c => c.Rank == CardRanks.Queen) &&
           Cards.Any(c => c.Rank == CardRanks.King) &&
           Cards.Any(c => c.Rank == CardRanks.Ace);
        }

        public static PokerHand EvaluateHand(Card[] Hand, Card[] CommunityCards)
        {
            const int MAX_SIZE = 7;
            Card[] Cards = new Card[MAX_SIZE];

            for (int i = 0; i < Hand.Length; i++)
            {
                Cards[i] = Hand[i];
            }

            for (int i = 0; i < CommunityCards.Length; i++)
            {
                Cards[Hand.Length + i] = CommunityCards[i];
            }


            if (HasRoyalFlush(Cards))
            {
                return PokerHand.RoyalFlush;
            }


            if (HasStraightFlush(Cards))
            {
                return PokerHand.StraightFlush;
            }


            if (HasFourOfAKind(Cards))
            {
                return PokerHand.FourOfAKind;
            }

            if (HasFullHouse(Cards))
            {
                return PokerHand.FullHouse;
            }

            if (HasFlush(Cards))
            {
                return PokerHand.Flush;
            }

            if (HasStraight(Cards))
            {
                return PokerHand.Straight;
            }

            if (HasThreeOfAKind(Cards))
            {
                return PokerHand.ThreeOfAKind;
            }

            if (HasTwoPair(Cards))
            {
                return PokerHand.TwoPair;
            }


            if (HasPair(Cards))
            {
                return PokerHand.Pair;
            }

            return PokerHand.HighCard;
        }

        public static PokerHand EvaluateHoleCards(Card[] Hand)
        {
            if (HasPair(Hand))
            {
                return PokerHand.Pair;
            }
            else
            {
                return PokerHand.HighCard;
            }
        }

        public static int FindCardRankSum(Card[] Hand)
        {
            int sum = Hand.Sum(card => (int)card.Rank);
            return sum;
        }
    }

    //=================================================================================================================
    // Main Program Class
    //=================================================================================================================
    internal class Program
    {
        //=================================================================================================================
        // Play Poker Functions
        //=================================================================================================================
        static void PrintSeparator()
        {
            Console.WriteLine("-----------------");
        }

        static void DetermineWinner(Table GameTable, Player MainPlayer, PokerHand MainPlayerHand, Player Ai, PokerHand AiPlayerHand)
        {
            if (MainPlayerHand > AiPlayerHand)
            {
                Console.WriteLine($"{MainPlayer.Name} wins!");
                MainPlayer.ChipCount += GameTable.PotValue;
                MainPlayer.WinCount += 1;
            }
            else if (AiPlayerHand > MainPlayerHand)
            {
                Console.WriteLine($"{Ai.Name} wins!");
                Ai.ChipCount += GameTable.PotValue;
            }
            else
            {
                int PlayerSum = HandEvaluator.FindCardRankSum(MainPlayer.Hand);
                int AiSum = HandEvaluator.FindCardRankSum(Ai.Hand);

                if (PlayerSum > AiSum)
                {
                    Console.WriteLine($"{MainPlayer.Name} wins!");
                    MainPlayer.ChipCount += GameTable.PotValue;
                    MainPlayer.WinCount += 1;
                }
                else if (AiSum > PlayerSum)
                {
                    Console.WriteLine($"{Ai.Name} wins!");
                    Ai.ChipCount += GameTable.PotValue;
                }
                else
                {
                    Console.WriteLine("Both players tied!");
                    Ai.ChipCount += (GameTable.PotValue) / 2;
                    MainPlayer.ChipCount += (GameTable.PotValue) / 2;
                }
            }
        }

        static int GetValidBettingAmount(Player player)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int amount))
                {
                    if (amount <= player.ChipCount && amount > 0)
                    {
                        return amount;
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid amount within your chip count.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer amount.");
                }
            }
        }

        static bool BettingRoundComplete(Player MainPlayer, Player Ai1)
        {
            return (MainPlayer.Folded || Ai1.Folded || MainPlayer.CurrentBet == Ai1.CurrentBet || 
                MainPlayer.ChipCount == 0 || Ai1.ChipCount == 0);
        }
        
        static void SimulateBettingRound(Table GameTable, Player MainPlayer, Player Ai1)
        {
            MainPlayer.Show();
            PrintSeparator();
            GameTable.Show();

            MainPlayer.CurrentBet = 0;
            Ai1.CurrentBet = 0;

            do
            {
                if (MainPlayer.Folded != true)
                {
                    PrintSeparator();
                    Console.WriteLine("1. Check 2. Bet 3. Fold 4: Call");
                    PrintSeparator();
                    
                    if (int.TryParse(Console.ReadLine(), out int PlayerChoice))
                    {
                        switch (PlayerChoice)
                        {
                            // Check
                            case 1:
                                if (MainPlayer.CurrentBet == Ai1.CurrentBet)
                                {
                                    MainPlayer.Check();
                                }
                                else
                                {
                                    Console.WriteLine("You can't check, because there is a bet that needs to be called");
                                }
                                break;
                            case 2:
                                Console.WriteLine("Please enter the amount of chips you want to bet");
                                int BetAmount = GetValidBettingAmount(MainPlayer);
                                MainPlayer.PlaceBet(BetAmount);
                                break;
                            case 3:
                                MainPlayer.Fold();
                                break;
                            case 4:
                                MainPlayer.PlaceBet(Ai1.CurrentBet);
                                break;
                            default:
                                Console.WriteLine("Enter an integer between 1 and 4 inclusive");
                                break;
                        }
                    }
                }

                if (Ai1.Folded != true)
                {
                    Ai1.PlaceBet(MainPlayer.CurrentBet);
                }

                GameTable.PotValue += MainPlayer.CurrentBet + Ai1.CurrentBet;
                PrintSeparator();
            }
            while (!BettingRoundComplete(MainPlayer, Ai1));
        }

        static void LetsPlayPoker(Player MainPlayer, Player Ai1, Deck DeckOfCards, Table GameTable)
        {
            // Preflop stage
            MainPlayer.Folded = false;
            Ai1.Folded = false;
            MainPlayer.Checked = false;
            Ai1.Checked = false;
            
            DeckOfCards.GenerateDeck();
            DeckOfCards.ShuffleDeck();

            GameTable.ResetPotValue();
            GameTable.ResetCards();

            MainPlayer.ResetHand();
            Ai1.ResetHand();

            DeckOfCards.DealCard(MainPlayer);
            DeckOfCards.DealCard(Ai1);

            DeckOfCards.DealCard(MainPlayer);
            DeckOfCards.DealCard(Ai1);

            SimulateBettingRound(GameTable, MainPlayer, Ai1);

            // Flop stage
            DeckOfCards.AddCardsToTable(GameTable, 3);
            if (MainPlayer.Folded != true || Ai1.Folded != true)
            {
                SimulateBettingRound(GameTable, MainPlayer, Ai1);
            }

            // Turn stage
            DeckOfCards.AddCardsToTable(GameTable, 1);
            if (MainPlayer.Folded != true || Ai1.Folded != true)
            {
                SimulateBettingRound(GameTable, MainPlayer, Ai1);
            }

            // River stage
            DeckOfCards.AddCardsToTable(GameTable, 1);
            if (MainPlayer.Folded != true || Ai1.Folded != true)
            {
                SimulateBettingRound(GameTable, MainPlayer, Ai1);
            }

            // End Game
            MainPlayer.Show();
            PrintSeparator();

            Ai1.Show();
            PrintSeparator();

            GameTable.Show();
            PrintSeparator();

            PokerHand MainPlayerHand = HandEvaluator.EvaluateHand(MainPlayer.Hand, GameTable.PokerTable);
            Console.WriteLine($"{MainPlayer.Name} you got a {MainPlayerHand}");

            PokerHand AiHand = HandEvaluator.EvaluateHand(Ai1.Hand, GameTable.PokerTable);
            Console.WriteLine($"{Ai1.Name} you got a {AiHand}");

            // Determine Winner
            PrintSeparator();
            
            if (MainPlayer.Folded != true && Ai1.Folded != true)
            {
                DetermineWinner(GameTable, MainPlayer, MainPlayerHand, Ai1, AiHand);
            }
            else if (MainPlayer.Folded != true && Ai1.Folded == true)
            {
                Console.WriteLine($"{MainPlayer.Name} wins!");
                MainPlayer.ChipCount += GameTable.PotValue;
                MainPlayer.WinCount += 1;
            }
            else if (MainPlayer.Folded == true && Ai1.Folded != true)
            {
                Console.WriteLine($"{Ai1.Name} wins!");
                Ai1.ChipCount += GameTable.PotValue;
            }
            else
            {
                Console.WriteLine("No winners");
                Ai1.ChipCount += (GameTable.PotValue) / 2;
                MainPlayer.ChipCount += (GameTable.PotValue) / 2;
            }
            
            PrintSeparator();

            Console.WriteLine("Would you like to play again? y/n");
            string choice = Console.ReadLine().ToLower();

            if (choice == "y")
            {
                Console.Clear();
                LetsPlayPoker(MainPlayer, Ai1, DeckOfCards, GameTable);
            }
            else
            {
                Console.WriteLine($"{MainPlayer.Name} has a total win count of {MainPlayer.WinCount}");
                SaveWinCount(MainPlayer);
                Console.WriteLine("Thank you for playing!");
            }
        }

        //=============================================================================================================
        // User Setup Functions
        //=============================================================================================================
        static void LoadProfile(Player MainPlayer)
        {
            const string USERFILE = "USERFILE.txt";

            // To be read from USERFILE
            string username;
            int winCount;
            int chipCount;

            if (!File.Exists(USERFILE))
            {
                Console.WriteLine($"{USERFILE} does not exist, please create it.");
            }
            else
            {
                try
                {
                    using (StreamReader InputFile = new StreamReader(USERFILE))
                    {
                        while (!InputFile.EndOfStream)
                        {
                            string line = InputFile.ReadLine();
                            line = line.Trim();                     
                            string[] values = line.Split(',');      // Data is separated by commas.

                            if (values.Length == 3)
                            {
                                username = values[0];
                                MainPlayer.Name = username;

                                if (int.TryParse(values[1], out winCount))
                                {
                                    MainPlayer.WinCount = winCount;
                                }
                                else
                                {
                                    Console.WriteLine("No valid winCount, setting winCount to 0");

                                    MainPlayer.WinCount = 0;
                                }

                                if (int.TryParse(values[2], out chipCount))
                                {
                                    MainPlayer.ChipCount = chipCount;
                                }
                                else
                                {
                                    Console.WriteLine("No valid chipCount, setting chipCount to 10000.");

                                    chipCount = 10000;
                                    MainPlayer.ChipCount = chipCount;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Invalid Format on Line: {line}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void SaveWinCount(Player MainPlayer)
        {
            const string USERFILE = "USERFILE.txt";

            try
            {
                using (StreamWriter InputFile = new StreamWriter(USERFILE))
                {
                    InputFile.WriteLine($"{MainPlayer.Name},{MainPlayer.WinCount},{MainPlayer.ChipCount}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //=============================================================================================================
        // Main Function
        //=============================================================================================================
        static void Main()
        {
            // For Unicode Console Output (Windows 11)
            Console.OutputEncoding = Encoding.UTF8;

            // Initialising Classes
            Player MainPlayer = new Player();
            Player Ai1 = new AI();
            Deck DeckOfCards = new Deck();
            Table GameTable = new Table();

            // Loading Player Profile
            LoadProfile(MainPlayer);

            // Playing Game
            LetsPlayPoker(MainPlayer, Ai1, DeckOfCards, GameTable);

            Console.Read();
        }
    }
}