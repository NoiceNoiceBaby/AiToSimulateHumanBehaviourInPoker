using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public Card(CardRanks rank, CardSuits suit)
        {
            Rank = rank;
            Suit = suit;
        }

        // Dictionaries for nicer card formatting
        private static readonly Dictionary<CardSuits, string> SuitUnicode = new Dictionary<CardSuits, string>
        {
            { CardSuits.Clubs, "\u2663" },    // ♣
            { CardSuits.Diamonds, "\u2666" }, // ♦
            { CardSuits.Hearts, "\u2665" },   // ♥
            { CardSuits.Spades, "\u2660" }    // ♠
        };

        private static readonly Dictionary<CardRanks, string> RankUnicode = new Dictionary<CardRanks, string>
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

        public override string ToString()
        {
            return $"{RankUnicode[Rank]}{SuitUnicode[Suit]}";
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
            
            foreach(CardSuits suit in Enum.GetValues(typeof(CardSuits)))
            {
                foreach(CardRanks rank in Enum.GetValues(typeof(CardRanks)))
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
            Random RandInt = new Random();


            for (int i = 0; i < DeckOfCards.Length; i++)
            {
                int RandomIndex = RandInt.Next(0, DeckOfCards.Length);

                Card TempCard = DeckOfCards[i];
                DeckOfCards[i] = DeckOfCards[RandomIndex];
                DeckOfCards[RandomIndex] = TempCard;
            }
        }

        public void ResetDeck()
        {
            GenerateDeck();
        }

        public void DealHand(Player player)
        {
            Random RandomInt = new Random();
            int RandomIndex;

            for (int i = 0; i < 2; i++)
            {
                do { RandomIndex = RandomInt.Next(0, DeckOfCards.Length); }
                while (DeckOfCards[RandomIndex] == null);
                {
                    Card SelectedCard = DeckOfCards[RandomIndex];
                    DeckOfCards[RandomIndex] = null;

                    player.Hand[i] = SelectedCard;
                }
            }
        }  

        public void AddCardsToTable(Table Gametable)
        {
            Random RandomInt = new Random();
            int RandomIndex;

            for (int i = 0; i < 3; i++)
            {
                do { RandomIndex = RandomInt.Next(0, DeckOfCards.Length); }
                while (DeckOfCards[RandomIndex] == null);
                {
                    Card SelectedCard = DeckOfCards[RandomIndex];
                    DeckOfCards[RandomIndex] = null;

                    Gametable.PokerTable[i] = SelectedCard;
                }
            }
        }

        public void AddTurnToTable(Table GameTable)
        {
            Random RandomInt = new Random();
            int RandomIndex;

            do { RandomIndex = RandomInt.Next(0, DeckOfCards.Length); }
            while (DeckOfCards[RandomIndex] == null);
            {
                Card card = DeckOfCards[RandomIndex];
                DeckOfCards[RandomIndex] = null;
                GameTable.PokerTable[3] = card;
            }
        }

        public void AddRiverToTable(Table GameTable)
        {
            Random RandomInt = new Random();
            int RandomIndex;

            do { RandomIndex = RandomInt.Next(0, DeckOfCards.Length); }
            while (DeckOfCards[RandomIndex] == null);
            {
                Card card = DeckOfCards[RandomIndex];
                DeckOfCards[RandomIndex] = null;
                GameTable.PokerTable[4] = card;
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
           
            for(int i = 0; i < 5; i++)
            {
                Console.WriteLine($"{i+1} - {PokerTable[i]}");
            }
        }

        public void ResetPotValue()
        {
            PotValue = 0;
        }

        public void ResetCards()
        { 
            for (int i = 0; i < PokerTable.Length; i ++)
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
        public Card[] Hand;
        public bool Folded;
        public bool Checked;

        public Player()
        {
            const int HAND_SIZE = 2;
            Hand = new Card[HAND_SIZE];
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

        public void Bet(Table GameTable, int Chips)
        {
            GameTable.PotValue = GameTable.PotValue + Chips;
            ChipCount = ChipCount - Chips;
        }

        public void Call(Table GameTable, int Chips)
        {
            GameTable.PotValue = GameTable.PotValue + Chips;
            ChipCount = ChipCount - Chips;
        }

        public void Raise(Table GameTable, int Chips)
        {
            GameTable.PotValue = GameTable.PotValue + Chips;
            ChipCount = ChipCount - Chips;
        }

        public bool Fold()
        {
            Folded = true;
            return Folded;
        }

        public bool Check()
        {
            Checked = true;
            return Checked;
        }
    }

    //=================================================================================================================
    // AI Class
    //=================================================================================================================
    public class AI : Player
    {
        //=============================================================================================================
        // Probability Matrices
        //=============================================================================================================

        //=============================================================================================================
        // Default ChipCount
        //=============================================================================================================
        public AI()
        {
            ChipCount = 10000;
            Name = "AI";
        }
    }

    //=================================================================================================================
    // Poker Hand Evualtor Class
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

    public class HandEvaultor
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
                if (HasFullHouse(Cards))
                {
                    return PokerHand.FullHouse;
                }
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

        static void SimulateBettingRound(Deck DeckOfCards, Table GameTable, Player MainPlayer, Player Ai1)
        {
            int CurrentBet = GameTable.PotValue;
            bool isMainPlayerTurn = true;

            MainPlayer.Show();
            PrintSeparator();
            GameTable.Show();

            while (true)
            {
                if (isMainPlayerTurn)
                {
                    PrintSeparator();
                    Console.WriteLine($"{MainPlayer.Name}'s turn");
                    Console.WriteLine("1: Bet, 2: Call, 3: Raise, 4: Fold, 5: Check");
                    //int playerChoice = int.Parse(Console.ReadLine());
                    if (int.TryParse(Console.ReadLine(), out int playerChoice))
                    {
                        if (playerChoice >= 1 & playerChoice <= 5)
                        {
                            switch (playerChoice)
                            {
                                // Bet
                                case 1:
                                    Console.WriteLine("Enter how many chips you want to bet.");
                                    int betAmount = GetValidBettingAmount(MainPlayer);
                                    MainPlayer.Bet(GameTable, betAmount);
                                    CurrentBet = betAmount;
                                    break;
                                // Call
                                case 2:
                                    MainPlayer.Call(GameTable, CurrentBet);
                                    break;
                                // Raise
                                case 3:
                                    Console.WriteLine("Enter how many chips you would like to raise.");
                                    int raiseAmount = GetValidBettingAmount(MainPlayer);
                                    MainPlayer.Raise(GameTable, raiseAmount);
                                    CurrentBet = raiseAmount;
                                    break;
                                // Fold
                                case 4:
                                    MainPlayer.Fold();
                                    break;
                                // Check
                                case 5:
                                    Console.WriteLine($"{MainPlayer.Name} checked.");
                                    break;
                                // Input validation
                                default:
                                    Console.WriteLine("Please enter a valid number.");
                                    continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please enter a number between 1 and 5 inclusive.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter an integer.");
                    }   
                }
                else if (MainPlayer.Checked == true)
                {
                    Ai1.Check();
                    isMainPlayerTurn = !isMainPlayerTurn;
                }
                else
                {
                    Ai1.Call(GameTable, CurrentBet);
                    GameTable.PotValue += CurrentBet;
                    isMainPlayerTurn = !isMainPlayerTurn;
                }
                
                if (MainPlayer.Folded || Ai1.Folded || MainPlayer.Checked && Ai1.Checked || MainPlayer.ChipCount == 0 || Ai1.ChipCount == 0)
                {
                    break;
                }

                break;
            }

            Console.WriteLine("Betting round ended.");
            GameTable.PotValue = CurrentBet;
            Console.Clear();
        }

        static void LetsPlayPoker(Player MainPlayer, Player Ai1, Deck DeckOfCards, Table GameTable)
        {
            // Preflop stage
            DeckOfCards.GenerateDeck();
            DeckOfCards.ShuffleDeck();
            DeckOfCards.DealHand(MainPlayer);
            DeckOfCards.DealHand(Ai1);
            SimulateBettingRound(DeckOfCards, GameTable, MainPlayer, Ai1);

            // Flop stage
            DeckOfCards.AddCardsToTable(GameTable);
            SimulateBettingRound(DeckOfCards, GameTable, MainPlayer, Ai1);

            // Turn stage
            DeckOfCards.AddTurnToTable(GameTable);
            SimulateBettingRound(DeckOfCards, GameTable, MainPlayer, Ai1);

            // River stage
            DeckOfCards.AddRiverToTable(GameTable);
            SimulateBettingRound(DeckOfCards, GameTable, MainPlayer, Ai1);

            // Determine winner
            MainPlayer.Show();
            PrintSeparator();
            Ai1.Show();
            PrintSeparator();
            GameTable.Show();
            PrintSeparator();
            PokerHand MainPlayerHand = HandEvaultor.EvaluateHand(MainPlayer.Hand, GameTable.PokerTable);
            Console.WriteLine($"{MainPlayer.Name} you got a {MainPlayerHand}");
            PokerHand AiHand = HandEvaultor.EvaluateHand(Ai1.Hand, GameTable.PokerTable);
            Console.WriteLine($"{Ai1.Name} you got a {AiHand}");
            Console.Read();
        }

        //=============================================================================================================
        // User Setup Functions
        //=============================================================================================================
        static void LoadProfile(Player MainPlayer)
        {
            const string USERFILE = "USERFILE.txt";
            // To be read from USERFILE
            string username;
            string password;
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
                            line = line.Trim();                     // Removes potential whitespaces in the line.
                            string[] values = line.Split(',');      // Data is separated by commas.

                            if (values.Length == 3)
                            {
                                username = values[0];
                                password = values[1];

                                MainPlayer.Name = username;

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

        //=============================================================================================================
        // Main Function
        //=============================================================================================================
        static void Main(string[] args)
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