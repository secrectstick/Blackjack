using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace monoGameBlackjack
{
    internal class Player
    {
        public enum handState
        {
            single,
            doubled
        }

        // fields
        //player info
        private List<Card> mainHand;
        private int mainHandTot;
        private List<Card> altHand1;
        private int altHand1Tot;
        private List<Card> altHand2;
        private int altHand2Tot;
        private Random rng;

        //fsm info
        private handState hState;
        private bool holding;
        private bool doubleDown;

        // dealer info
        private List<Card> dealerHand;
        private int dealerHandTot;

        // texture info
        private Texture2D[] cardImgs;
        private Texture2D cardBack;

        //deck
        Deck deck;

        public Player() 
        {
            mainHandTot = 0;
            mainHand = new List<Card>();
            altHand1 = new List<Card>();
            altHand2 = new List<Card>();
            altHand1Tot = 0;
            altHand2Tot = 0;
            rng = new Random();
            hState = handState.single;
            holding = false;
            doubleDown = false;
            dealerHand = new List<Card>();
            dealerHandTot = 0;
            cardImgs = new Texture2D[52];
            deck = new Deck();
        }

        // reset 
        public void reset()
        {
            altHand1Tot = 0;
            altHand2Tot = 0;
            dealerHandTot = 0;
            hState = handState.single;
            holding = false;
            doubleDown = false;
            mainHandTot = 0;
            deck.shuffle();
            if(mainHand != null)
            {
                mainHand.Clear();
            }
            if (altHand1 != null)
            {
                altHand1.Clear();
            }
            if (altHand2 != null)
            {
                altHand2.Clear();
            }
            if(dealerHand != null) 
            {
                dealerHand.Clear();
            }

        }


        // second reset for redealing hands



        //load content
        public void LoadContent(ContentManager Content)
        {
            for (int i = 2; i <= 14; i++)
            {
                Texture2D newCard = Content.Load<Texture2D>($"{i}_spades");
                cardImgs[i-2] = newCard;
            }
            for (int i = 15; i <= 27; i++)
            {
                Texture2D newCard = Content.Load<Texture2D>($"{i-13}_clubs");
                cardImgs[i - 2] = newCard;
            }
            for (int i = 28; i <= 40; i++)
            {
                Texture2D newCard = Content.Load<Texture2D>($"{i-26}_diamonds");
                cardImgs[i - 2] = newCard;
            }
            for (int i = 41; i <= 53; i++)
            {
                Texture2D newCard = Content.Load<Texture2D>($"{i-39}_hearts");
                cardImgs[i - 2] = newCard;
            }
            cardBack = Content.Load<Texture2D>("back_red_basic");
        }


        //intialize
        public void intialize()
        {
            reset();
            for (int i = 0; i < 2; i++)
            {
                Card temp = (deck.ShuffDeck.Pop());
                
                if(temp.number <= 10)
                {
                    mainHandTot += temp.number;
                }
                else if(temp.number < 14)
                {
                    mainHandTot += 10;
                }
                else
                {
                    //deal with ace
                }
                mainHand.Add(temp);

                Card dtemp = (deck.ShuffDeck.Pop());

                if (dtemp.number <= 10)
                {
                    dealerHandTot += dtemp.number;
                }
                else if (dtemp.number < 14)
                {
                    dealerHandTot += 10;
                }
                else
                {
                    //deal with ace
                }
                dealerHand.Add(dtemp);
            }

            if (mainHand[0].number == mainHand[1].number)
            {
                hState = handState.doubled;
                altHand1.Add(mainHand[0]);

                if (mainHand[0].number <= 10)
                {
                    mainHandTot += mainHand[0].number;
                }
                else if (mainHand[0].number < 14)
                {
                    mainHandTot += 10;
                }
                else
                {
                    //deal with ace
                }

                altHand2.Add(mainHand[1]);

                if (mainHand[1].number <= 10)
                {
                    mainHandTot += mainHand[1].number;
                }
                else if (mainHand[1].number < 14)
                {
                    mainHandTot += 10;
                }
                else
                {
                    //deal with ace
                }
            }

        }



        // update

        public void update()
        {

        }




        //draw

        public void draw(SpriteBatch sb)
        {
            sb.Draw(cardImgs[33],new Rectangle(270,50,100,100),Color.White);
        }
    }
}
