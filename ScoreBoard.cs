using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS
{
    public class ScoreBoard
    {
        public int SCORE = 0;
        public int HIGHSCORE = 1000000;
        private int m_nShipsLeft = 0;
        private List<Ship> m_ShipsLeft = new List<Ship>();
        public int SHIPSLEFT
        {
            get { return m_nShipsLeft;  }
            set {
                m_nShipsLeft = value;
                if(m_ShipsLeft.Count != m_nShipsLeft )
                {
                    m_ShipsLeft.Clear();
                    for (int x = 0; x < m_nShipsLeft; x++)
                    {
                        Ship cur = new Ship(canvas);
                        cur.position = new Point(445 + (x * 23), 168);
                        m_ShipsLeft.Add( cur );
                    }
                }
                }
        }

        public int CURLEVEL = 0;

        public string COPYRIGHTMESSAGE = "©1979 ATARI INC";
        public string STARTMESSAGE = "PUSH START";
        public string GAMEOVERMESSAGE = "GAME OVER";
        public string PLAYERMESSAGE = "PLAYER 1";

        public long blinkTicks;
        public bool m_bIsPlaying = false;
        private bool m_bBlinkOn = true;

        // High Score Screen Toggle Information
        private const double DELAYBETWEENHIGHSCOREDISPLAY = 16.0f; // I timed it with a stopwatch. 16 seconds. 
        private bool m_bIsHighScoreScreenVisible = true;
        public void displayHighScoreScreen()
        {
            m_bIsHighScoreScreenVisible = true;
            m_lNextToggle = System.DateTime.Now.AddTicks((long)(DELAYBETWEENHIGHSCOREDISPLAY * (double)frmAsteroids.TICKSPERSECOND)).Ticks;
        }
        private long m_lNextToggle;

        private frmAsteroids canvas;

        private HighScores m_highScores = new HighScores();
        public HighScores highScores
        {
            get { return m_highScores;  } 
        }

        public ScoreBoard( frmAsteroids frm)
        {
            canvas = frm;
            m_lNextToggle = System.DateTime.Now.AddTicks( (long) ( DELAYBETWEENHIGHSCOREDISPLAY * (double) frmAsteroids.TICKSPERSECOND ) ).Ticks;
            blinkTicks = System.DateTime.Now.AddTicks(6000000).Ticks;
        }

        private Point m_retainStartMsgLeft = new Point();

        public void Draw()
        {
            // ASTEROIDS
            AsteroidsFontHandler afh = new AsteroidsFontHandler(canvas);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            // SCOREBOARD LEFT!
            afh.FontSize = 32.0f; afh.Kerning = 7;
            afh.Text = SCORE.ToString("D2");
            afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2) - 410 - (int)afh.TextWidth, 60);
            afh.Draw();

            // SCOREBOARD HIGH SCORE
            afh.FontSize = 18.0f;
            afh.Kerning = 3;
            afh.Text = m_highScores.GetHighScore().ToString();
            afh.TextPosition = new Point((int)((canvas.Width - afh.TextWidth) / 2), 70);
            afh.Draw();

            afh.FontSize = 32.0f; afh.Kerning = 10;
            afh.Text = SCORE.ToString("D2");

            /*
             * afh.Text = canvas.DEBUGOUTPUT; //  SCORE.ToString("D2");
            afh.TextPosition = new Point(524 - (int) afh.TextWidth, 111);
            afh.Draw();

            TEST DRAW FOR CHARACTERS
            afh.Text = "NOPQRSTUVWXYZ"; //  SCORE.ToString("D2");
            afh.TextPosition = new Point(524 - (int)afh.TextWidth, 150);
            afh.Draw();

            afh.Text = "0123456789._©"; //  SCORE.ToString("D2");
            afh.TextPosition = new Point(524 - (int)afh.TextWidth, 189);
            afh.Draw();
            */

            if (System.DateTime.Now.Ticks > blinkTicks)
            {
                blinkTicks = System.DateTime.Now.AddTicks(4750000).Ticks;
                m_bBlinkOn = !m_bBlinkOn;
            }

            if (m_bBlinkOn && !m_bIsPlaying)
            {
                // STATUS MESSAGE
                afh.Kerning = 10;
                afh.Text = STARTMESSAGE; //  SCORE.ToString("D2");
                m_retainStartMsgLeft = new Point(((canvas.Width - afh.TextWidth) / 2), 175);
                afh.TextPosition = m_retainStartMsgLeft;

                afh.Draw();
            }
            if( !m_bIsPlaying)
            {
                if (m_lNextToggle <= System.DateTime.Now.Ticks)
                {
                    m_bIsHighScoreScreenVisible = !m_bIsHighScoreScreenVisible;

                    m_lNextToggle = System.DateTime.Now.AddTicks((long)(DELAYBETWEENHIGHSCOREDISPLAY *
                                                                       (double)frmAsteroids.TICKSPERSECOND)).Ticks;
                }

                if (m_bIsHighScoreScreenVisible)
                {
                    afh.Kerning = 10;
                    afh.Text = "HIGH SCORES"; //  SCORE.ToString("D2");
                    afh.TextPosition = new Point( m_retainStartMsgLeft.X, m_retainStartMsgLeft.Y + 90 );
                    afh.Draw();

                    int nX = 0;
                    foreach( HighScore hs in m_highScores.list.OrderByDescending( hs => hs.Score ) )
                    {
                        String replaceUnderscores = hs.Initials.Replace('_', ' ');
                        afh.Text = String.Format("{0}{1}.{2,5} {3}",
                                            ( ( nX + 1 ).ToString().Length == 1 )?" ":"",
                                            nX + 1,
                                            hs.Score,
                                            replaceUnderscores);
                        afh.TextPosition = new Point(m_retainStartMsgLeft.X-25, m_retainStartMsgLeft.Y + 175 + ( nX * 40) );
                        afh.Draw();
                        nX += 1;
                    }
                }

            }
            else if( m_bIsPlaying )
            {
                // STATUS MESSAGE
                afh.Text = PLAYERMESSAGE; //  SCORE.ToString("D2");
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2), 225);
                afh.Draw();

                foreach (Ship s in m_ShipsLeft)
                    s.Draw();
            }

            // GAME OVER MESSAGE
            if (canvas.m_bGameOver && canvas.m_EnterHighScoreScreen.m_bDisplay != true )
            {
                afh.Text = GAMEOVERMESSAGE; //  SCORE.ToString("D2");
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2), (canvas.Height / 2) - 100);
                afh.Draw();
            }


            // Copyright Message
            afh.FontSize = 18.0f;
            afh.Kerning = 3;
            afh.Text = COPYRIGHTMESSAGE; //  SCORE.ToString("D2");
            afh.TextPosition = new Point( (int)((canvas.Width - afh.TextWidth) / 2), canvas.Height - 50);
            afh.Draw();

            // Draw the number of ships the player currently has
        }

    }
}
