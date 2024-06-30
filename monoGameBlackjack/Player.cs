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
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

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
        private bool win;
        private bool lost;
        private bool doubBrk;

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
            win = false;
            lost = false;
            holdingAlt1 = false ;
            holdingAlt2 = false;
        }

        // reset 
        public void reset()
        {
            holdingAlt1 = false;
            holdingAlt2 = false;
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
            win = false;
            lost = false;
            aceCount = 0;
            aceCountAlt1 = 0;
            aceCountAlt2 = 0;
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
                    dealerHandTot += 1;
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
                    hitRect.Contains(cur.Position)&& aceCount==0)
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

                        if (mainHandTot > 21)
                        {
                            lost = true;
                            
                        }
                    }

                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    holdRect.Contains(cur.Position) && aceCount == 0)
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
                case handState.doubled:////////////////////////////////////////////////////////////////////////

                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    (cur.X > 600))
                    {
                        intialize();
                    }

                    if (doubleDown == false)
                    {
                        if (cur.LeftButton == ButtonState.Released &&
                        last.LeftButton == ButtonState.Pressed &&
                        yesRect.Contains(cur.Position))
                        {
                            doubleDown = true;
                        }

                        if (cur.LeftButton == ButtonState.Released &&
                        last.LeftButton == ButtonState.Pressed &&
                        noRect.Contains(cur.Position))
                        {
                            hState=handState.single;
                        }
                    }


                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    hitRectAlt1.Contains(cur.Position) && aceCountAlt1 == 0 && aceCountAlt2 == 0 && doubleDown==true && altHand1Tot<=21)
                    {
                        Card temp = (deck.ShuffDeck.Pop());

                        if (temp.number <= 10)
                        {
                            altHand1Tot += temp.number;
                        }
                        else if (temp.number < 14)
                        {
                            altHand1Tot += 10;
                        }
                        else
                        {
                            //deal with ace
                            aceCountAlt1++;
                        }
                        altHand1.Add(temp);

                        
                    }

                    if (altHand1Tot > 21 && altHand2Tot >21)
                    {
                        lost = true;

                    }

                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    holdRectAlt1.Contains(cur.Position) && aceCountAlt1 == 0 && aceCountAlt2 == 0 && doubleDown == true)
                    {
                        holdingAlt1 = true;
                    }

                    /////////////////////////////////////////////////////////////
                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    hitRectAlt2.Contains(cur.Position) && aceCountAlt2 == 0 && aceCountAlt1 == 0 && doubleDown == true && altHand2Tot <= 21)
                    {
                        Card temp = (deck.ShuffDeck.Pop());

                        if (temp.number <= 10)
                        {
                            altHand2Tot += temp.number;
                        }
                        else if (temp.number < 14)
                        {
                            altHand2Tot += 10;
                        }
                        else
                        {
                            //deal with ace
                            aceCountAlt2++;
                        }
                        altHand2.Add(temp);


                    }


                    if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    holdRectAlt1.Contains(cur.Position) && aceCountAlt2 == 0 && aceCountAlt1 == 0 && doubleDown == true)
                    {
                        holdingAlt2 = true;
                    }

                    if (holdingAlt2 && holdingAlt1)
                    {
                        holding = true;
                    }

                    if(altHand1Tot > 21 && holdingAlt2)
                    {
                        holding = true;
                    }

                    if (altHand2Tot > 21 && holdingAlt1)
                    {
                        holding = true;
                    }

                    break;
            }

            if (aceCountAlt2 > 0)
            {
                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    yesRect.Contains(cur.Position))
                {
                    altHand2Tot+=11;
                    aceCountAlt2--;
                    
                }

                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    noRect.Contains(cur.Position))
                {
                    altHand2Tot++;
                    aceCountAlt2--;
                    
                }
            }
            else if(aceCountAlt1 > 0)
            {
                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    yesRect.Contains(cur.Position))
                {
                    altHand1Tot += 11;
                    aceCountAlt1--;

                }

                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    noRect.Contains(cur.Position))
                {
                    altHand1Tot++;
                    aceCountAlt1--;

                }
            }
            else if(aceCount > 0)
            {
                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    yesRect.Contains(cur.Position))
                {
                    mainHandTot+=11;
                    aceCount--;
                    if (mainHandTot > 21)
                    {
                        lost = true;

                    }
                }

                if (cur.LeftButton == ButtonState.Released &&
                    last.LeftButton == ButtonState.Pressed &&
                    noRect.Contains(cur.Position))
                {
                    mainHandTot++;
                    aceCount--;
                    if (mainHandTot > 21)
                    {
                        lost = true;

                    }
                }
                
            }

            // dealer logic
            if(holding == true)
            {
                if (dealerHandTot < 17)
                {
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
                        dealerHandTot += 1;
                    }
                    dealerHand.Add(dtemp);
                    if (dealerHandTot > 21)
                    {
                        win= true;
                    }
                }

                if(lost == false && win == false)
                {
                    if(dealerHandTot < mainHandTot)
                    {
                        win = true;
                    }
                    else if((dealerHandTot < altHand1Tot) && altHand1Tot<= 21)
                    {
                        win = true;
                    }
                    else if((dealerHandTot < altHand2Tot) && altHand1Tot <= 21)
                    {
                        win = true;
                    }

                    

                    if (win == false)
                    {
                        lost=true;
                    }
                }
                
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
                    sb.Draw(cardImgs[51], holdRect, Color.White);


                    if (aceCount > 0)
                    {
                        sb.Draw(cardImgs[5], yesRect, Color.White);
                        sb.Draw(cardImgs[34], noRect, Color.White);
                    }

                    string text = "total " + mainHandTot;
                    sb.DrawString(Arial24,text,new Vector2 (200, 200), Color.White);
                    break;
                case handState.doubled:

                    // draw double down yes and no
                    if (doubleDown == false)
                    {
                        sb.Draw(cardImgs[5], yesRect, Color.White);
                        sb.Draw(cardImgs[34], noRect, Color.White);
                    }

                    if (altHand1Tot <= 21)
                    {
                        for (int i = 0; i < altHand1.Count; i++)
                        {
                            // systen.debug.writeline to figure out what position    // altHand1[i].position - 1
                            sb.Draw(cardImgs[altHand1[i].position - 1], new Rectangle(100 + (i * 125), 500, 100, 100), Color.White);

                            
                        }
                        // draw hit and hold buttons
                        sb.Draw(cardImgs[0], hitRectAlt1, Color.White);
                        sb.Draw(cardImgs[51], holdRectAlt1, Color.White);


                        // yes and no for ace count
                        if (aceCountAlt1 > 0)
                        {
                            sb.Draw(cardImgs[5], yesRect, Color.White);
                            sb.Draw(cardImgs[34], noRect, Color.White);
                        }
                    }

                    if (altHand2Tot <= 21)
                    {
                        for (int i = 0; i < altHand2.Count; i++)
                        {
                            //altHand2[i].position - 1
                            sb.Draw(cardImgs[altHand2[i].position - 1], new Rectangle(100 + (i * 125), 650, 100, 100), Color.White);
                        }
                        // draw hit and hold buttons
                        sb.Draw(cardImgs[0], hitRectAlt2, Color.White);
                        sb.Draw(cardImgs[51], holdRectAlt2, Color.White);


                        // yes and no for ace count
                        if (aceCountAlt2 > 0)
                        {
                            sb.Draw(cardImgs[5], yesRect, Color.White);
                            sb.Draw(cardImgs[34], noRect, Color.White);
                        }
                    }

                    break;
            }



            if (lost ==true)
            {
                sb.DrawString(Arial24, " you lose", new Vector2(300, 300), Color.White);
                sb.DrawString(Arial24, "reset:", new Vector2(310, 360), Color.White);
            }
            else if (win == true)
            {
                sb.DrawString(Arial24, " you win", new Vector2(300, 300), Color.White);
                sb.DrawString(Arial24, " reset:", new Vector2(320, 360), Color.White);
            }


            if (holding == true)
            {
                for (int i = 0; i < dealerHand.Count; i++)
                {
                    sb.Draw(cardImgs[dealerHand[i].position - 1], new Rectangle(100 + (i * 125), 100, 100, 100), Color.White);
                }
                string text = "dtotal " + dealerHandTot;
                sb.DrawString(Arial24, text, new Vector2(600, 200), Color.White);
            }
            else
            {
                sb.Draw(cardBack, new Rectangle(100, 100, 100, 100), Color.White);
                sb.Draw(cardImgs[dealerHand[1].position-1], new Rectangle(225, 100, 100, 100),Color.White);
            }

            //sb.Draw(cardBack,new Rectangle(270,50,100,100),Color.White);
        }
    }
}
