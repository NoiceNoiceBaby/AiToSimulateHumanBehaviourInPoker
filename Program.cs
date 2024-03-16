using System;
using System.Collections.Generic;
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

        public virtual void PlaceBet(int BetAmount)
        {
            if (ChipCount > BetAmount)
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
            CurrentBet = 0;
            Name = "Ai";
        }

        public override void PlaceBet(int currentBet)
        {
            Random random = new Random();

            int CallAmount = currentBet - CurrentBet;
            int MaxRaiseAmount = Math.Min(ChipCount - CallAmount, random.Next(50, 1000));

            PokerHand AiHandStrength = HandEvaultor.EvaluateHoleCard(Hand);

            if (CallAmount > 0)
            {
                if (ChipCount < currentBet)
                {
                    Console.WriteLine($"{Name} calls the current bet of {currentBet}");
                    PlaceBet(currentBet);
                }
                else
                {
                    Fold();
                }
            }
            else
            {
                if (AiHandStrength == PokerHand.Pair && random.Next(1, 11) <= 8) // 80% chance of betting given a pair
                {
                    if (MaxRaiseAmount > 0)
                    {
                        int RaiseAmount = random.Next(1, MaxRaiseAmount + 1);
                        Console.WriteLine($"{Name} raise by {RaiseAmount} chips");
                        PlaceBet(CallAmount + RaiseAmount);
                    }
                    else
                    {
                        Console.WriteLine($"{Name} calls the bet of {CallAmount}");
                        PlaceBet(CallAmount);
                    }
                }
                else if (AiHandStrength == PokerHand.HighCard && random.Next(1, 11) <= 6) // 60% chance of calling given a high card
                {
                    Console.WriteLine($"{Name} calls the bet of {CallAmount}");
                    PlaceBet(CallAmount);
                }
                else
                {
                    if (random.Next(1, 11) <= 7) // 70% chance of checking
                    {
                        Check();
                    }
                    else
                    {
                        Fold();
                    }
                }
            }
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

        public static PokerHand EvaluateHoleCard(Card[] Hand)
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

        static void DetermineWinner(Player MainPlayer, PokerHand MainPlayerHand, Player Ai, PokerHand AiPlayerHand)
        {
            if (MainPlayerHand > AiPlayerHand)
            {
                Console.WriteLine($"{MainPlayer.Name} wins!");
            }
            else if (AiPlayerHand > MainPlayerHand)
            {
                Console.WriteLine($"{Ai.Name} wins!");
            }
            else
            {
                Console.WriteLine("You both tied!");
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

        static void SimulateBettingRound(Table GameTable, Player MainPlayer, Player Ai1)
        {
            int CurrentBet = GameTable.PotValue;

            MainPlayer.Show();
            PrintSeparator();
            GameTable.Show();

            while (true)
            {
                if (MainPlayer.Folded != true && MainPlayer.ChipCount > 0)
                {
                    // This is the main player's turn
                    PrintSeparator();
                    Console.WriteLine($"{MainPlayer.Name}'s turn");
                    PrintSeparator();
                    Console.WriteLine($"The current bet is {CurrentBet}");
                    Console.WriteLine("1: Call, 2: Raise, 3: Fold, 4: Bet, 5: Check");
                    PrintSeparator();

                    if (int.TryParse(Console.ReadLine(), out int PlayerChoice))
                    {
                        switch (PlayerChoice)
                        {
                            // Call
                            case 1:
                                int CallAmount = CurrentBet - MainPlayer.CurrentBet;
                                MainPlayer.PlaceBet(CallAmount);
                                GameTable.PotValue = CurrentBet;
                                PrintSeparator();
                                GameTable.Show();
                                break;
                            // Raise
                            case 2:
                                Console.WriteLine("How many chips would you like to raise?");
                                int RaiseAmount = GetValidBettingAmount(MainPlayer);
                                MainPlayer.PlaceBet(RaiseAmount);
                                CurrentBet += RaiseAmount;
                                GameTable.PotValue = CurrentBet;
                                PrintSeparator();
                                GameTable.Show();
                                break;
                            // Fold
                            case 3:
                                MainPlayer.Fold();
                                PrintSeparator();
                                GameTable.Show();
                                break;
                            // Bet
                            case 4:
                                Console.WriteLine("How many chips would you like to bet?");
                                int BetAmount = GetValidBettingAmount(MainPlayer);
                                MainPlayer.PlaceBet(BetAmount);
                                CurrentBet = BetAmount;
                                GameTable.PotValue = CurrentBet;
                                PrintSeparator();
                                GameTable.Show();
                                break;
                            // Check
                            case 5:
                                if (CurrentBet == MainPlayer.CurrentBet)
                                {
                                    MainPlayer.Check();
                                    PrintSeparator();
                                    GameTable.Show();
                                }
                                else
                                {
                                    Console.WriteLine($"{MainPlayer.Name} can't check, there is a bet of {CurrentBet} to call");
                                }
                                break;
                            // Error Check
                            default:
                                Console.WriteLine("Please enter an integer between 1 and 5 inclusive");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter an integer between 1 and 5 inclusive");
                    }
                }

                PrintSeparator();

                if (Ai1.Folded != true && Ai1.ChipCount > 0)
                {
                    // Ai's turn
                    int currentBet = Math.Abs(CurrentBet - Ai1.ChipCount);
                    Ai1.PlaceBet(currentBet);
                    PrintSeparator();
                }

                if (MainPlayer.Folded == true || Ai1.Folded == true || MainPlayer.ChipCount == 0 || Ai1.ChipCount == 0 
                    || MainPlayer.Checked && Ai1.Checked == true)
                {
                    break;
                }
            }
        }

        static void LetsPlayPoker(Player MainPlayer, Player Ai1, Deck DeckOfCards, Table GameTable)
        {
            // Preflop stage
            DeckOfCards.GenerateDeck();
            DeckOfCards.ShuffleDeck();

            GameTable.ResetPotValue();
            GameTable.ResetCards();

            DeckOfCards.DealCard(MainPlayer);
            DeckOfCards.DealCard(Ai1);

            DeckOfCards.DealCard(MainPlayer);
            DeckOfCards.DealCard(Ai1);

            SimulateBettingRound(GameTable, MainPlayer, Ai1);

            // Flop stage
            DeckOfCards.AddCardsToTable(GameTable, 3);
            SimulateBettingRound(GameTable, MainPlayer, Ai1);

            // Turn stage
            DeckOfCards.AddCardsToTable(GameTable, 1);
            SimulateBettingRound(GameTable, MainPlayer, Ai1);

            // River stage
            DeckOfCards.AddCardsToTable(GameTable, 1);
            SimulateBettingRound(GameTable, MainPlayer, Ai1);

            // End Game
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

            // Determine Winner
            PrintSeparator();
            DetermineWinner(MainPlayer, MainPlayerHand, Ai1, AiHand);
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
                            line = line.Trim();                     // Removes potential whitespaces in the line.
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

            //Console.WriteLine($"Name: {MainPlayer.Name}, WinCount: {MainPlayer.WinCount}, ChipCount: {MainPlayer.ChipCount}");

            // Playing Game
            LetsPlayPoker(MainPlayer, Ai1, DeckOfCards, GameTable);

            Console.Read();
        }
    }
}