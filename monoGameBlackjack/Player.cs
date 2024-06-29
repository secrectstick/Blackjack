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
using static monoGameBlackjack.Game1;

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
        private bool holdingAlt1;
        private bool holdingAlt2;
        private bool doubleDown;
        private int aceCount;
        private int aceCountAlt1;
        private int aceCountAlt2;

        // dealer info
        private List<Card> dealerHand;
        private int dealerHandTot;

        // texture info
        private Texture2D[] cardImgs;
        private Texture2D cardBack;

        //deck
        Deck deck;

        // input info
        private MouseState cur;
        private MouseState last;

        // button rects
        private Rectangle yesRect;
        private Rectangle noRect;
        private Rectangle hitRect;
        private Rectangle holdRect;
        private Rectangle hitRectAlt1;
        private Rectangle holdRectAlt1;
        private Rectangle hitRectAlt2;
        private Rectangle holdRectAlt2;

        // font
        private SpriteFont Arial24;

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


        // second reset for redealing hands // just call intialize



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

            yesRect = new Rectangle(300,300,50,50);
            noRect = new Rectangle(375,300,50,50);
            hitRect = new Rectangle(100,500,50,50);
            holdRect = new Rectangle(100, 570, 50, 50);
            holdRectAlt1 = new Rectangle(100, 400, 50, 50);
            hitRectAlt1 = new Rectangle(100, 470, 50, 50);
            holdRectAlt2 = new Rectangle(100, 540, 50, 50);
            hitRectAlt2 = new Rectangle(100,610,50,50);

            Arial24 = Content.Load<SpriteFont>("Arial");

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
                    aceCount++;
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
                    altHand1Tot += mainHand[0].number;
                }
                else if (mainHand[0].number < 14)
                {
                    altHand1Tot += 10;
                }
                else
                {
                    //deal with ace
                    aceCountAlt1++;
                }

                altHand2.Add(mainHand[1]);

                if (mainHand[1].number <= 10)
                {
                    altHand2Tot += mainHand[1].number;
                }
                else if (mainHand[1].number < 14)
                {
                    altHand2Tot += 10;
                }
                else
                {
                    //deal with ace
                    aceCountAlt2++;
                }
            }

        }



        // update

        public void update()
        {
            cur = Mouse.GetState();

            switch (hState)
            {
                case handState.single:

                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    hitRect.Contains(cur.Position))
                    {
                        Card temp = (deck.ShuffDeck.Pop());

                        if (temp.number <= 10)
                        {
                            mainHandTot += temp.number;
                        }
                        else if (temp.number < 14)
                        {
                            mainHandTot += 10;
                        }
                        else
                        {
                            //deal with ace
                            aceCount++;
                        }
                        mainHand.Add(temp);
                    }

                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    holdRect.Contains(cur.Position))
                    {
                        holding = true;
                    }


                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    (cur.X > 600))
                    {
                        intialize();
                    }

                    break;
                case handState.doubled:
                    
                    break;
            }

            if (aceCountAlt2 > 0)
            {

            }
            else if(aceCountAlt1 > 0)
            {

            }
            else if(aceCount > 0)
            {
                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    yesRect.Contains(cur.Position))
                {
                    mainHandTot+=11;
                    aceCount--;
                }

                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    noRect.Contains(cur.Position))
                {
                    mainHandTot++;
                    aceCount--;
                }
                
            }

            if(holding == true)
            {

            }

            last = cur;
        }




        //draw

        public void draw(SpriteBatch sb)
        {

            switch (hState)
            {
                case handState.single:
                    for(int i =0; i< mainHand.Count;i++)
                    {
                        sb.Draw(cardImgs[mainHand[i].position -1], new Rectangle(100 + (i * 125), 600 , 100, 100), Color.White);
                    }

                    sb.Draw(cardImgs[0],hitRect, Color.White);

                    

                    if (aceCount > 0)
                    {
                        sb.Draw(cardImgs[5], yesRect, Color.White);
                        sb.Draw(cardImgs[34], noRect, Color.White);
                    }

                    string text = "total " + mainHandTot;
                    //sb.DrawString(Arial24,text,new Vector2 (200, 200), Color.White);
                    break;
                case handState.doubled: 
                    break;
            }


            //sb.Draw(cardBack,new Rectangle(270,50,100,100),Color.White);
        }
    }
}
