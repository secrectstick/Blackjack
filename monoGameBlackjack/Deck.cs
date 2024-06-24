using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monoGameBlackjack
{
    internal class Deck
    {

        private List<Card> deck;
        private Stack<Card> shuffDeck;
        private Random rng;

        public Stack<Card> ShuffDeck
        {
            get { return shuffDeck; }
        }



        //ctor
        public Deck()
        {
            deck = new List<Card>();
            shuffDeck = new Stack<Card>();
            rng = new Random();

            NewDeck();
        }

        public void shuffle()
        {
            if (shuffDeck != null)
            {
                shuffDeck.Clear();
            }

            while (deck.Count > 0)
            {
                int select = rng.Next(0, deck.Count);
                shuffDeck.Push(deck[select]);
                deck.RemoveAt(select);
            }

            NewDeck();
        }


        public void NewDeck()
        {
            StreamReader reader = new StreamReader("../../../deckofcards.txt");

            try
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] splits = line.Split("|");
                    Card newCard = new Card(int.Parse(splits[0]), splits[1], int.Parse(splits[2]));
                    deck.Add(newCard);
                }
            }
            catch
            {
                Console.WriteLine("error");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        

    }
}
