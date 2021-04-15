using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace ASTEROIDS
{
    public class HighScores
    {
        private int MAXHIGHSCORES = 10;
        private List<HighScore> m_lstHighScores = new List<HighScore>();
        private String HighScoresPath = Directory.GetCurrentDirectory() + "\\HighScores.DAT";

        public HighScores()
        {
            Load();
        }

        public List<HighScore> list
        {
            get { return m_lstHighScores; }
        }

        public void Save()
        {
            List<string> lines = new List<string>();
            foreach (HighScore hs in m_lstHighScores)
            {
                lines.Add(String.Format("{0}{1}", hs.Initials, hs.Score));
            }
            File.WriteAllLines(@HighScoresPath, lines.ToArray(), Encoding.UTF8);

            return;
        }

        public bool Load()
        {
            m_lstHighScores.Clear();
            if (File.Exists(HighScoresPath))
            {
                string[] lines = System.IO.File.ReadAllLines(@HighScoresPath);

                // There can be potentially 0 to n high scores in the high score file, if someone modified it... 
                // iterate through all of them, but.... 
                for (int nIndex = 0; nIndex < lines.Length; nIndex++)
                {
                    // Cap the high score list at the maximum number of scores listed as a constant above. 
                    if (nIndex >= MAXHIGHSCORES)
                        break;

                    HighScore hs = new HighScore();
                    hs.Initials = lines[nIndex].Substring(0, 3);
                    hs.Score = System.Convert.ToInt32(lines[nIndex].Substring(3, lines[nIndex].Length - 3));
                    m_lstHighScores.Add(hs);
                }
            }

            return true;
        }

        public bool isHighScore(int score)
        {
            bool bIshighScore = false;
            if (m_lstHighScores.Count < MAXHIGHSCORES) bIshighScore = true;
            else
            {
                for (int nIndex = 0; nIndex < m_lstHighScores.Count; nIndex++)
                {
                    if (m_lstHighScores[nIndex].Score < score)
                    {
                        bIshighScore = true;
                        break;
                    }
                }
            }


            return bIshighScore;
        }

        public void AddScore(HighScore hs)
        {
            bool bIshighScore = false;
            int nInsertAt = 0;
            for (int nIndex = 0; nIndex < m_lstHighScores.Count; nIndex++)
            {
                if (m_lstHighScores[nIndex].Score < hs.Score)
                {
                    bIshighScore = true;
                    nInsertAt = nIndex;
                    break;
                }
            }

            if (m_lstHighScores.Count < MAXHIGHSCORES)
            {
                m_lstHighScores.Add(hs);
            }
            else if (bIshighScore == true) // list of high scores is at capacity. 
            {
                m_lstHighScores.Insert(nInsertAt, hs);              // Insert at the position this is higher than
                m_lstHighScores.RemoveAt(m_lstHighScores.Count-1);    // then remove the score at the bottom of the list
            }
            Save(); Load();
        }

        public int GetHighScore()
        {
            int nScore = 0;
            if (m_lstHighScores.Count > 0)
                nScore = m_lstHighScores.OrderByDescending(hs => hs.Score).ToList()[0].Score;

            return nScore;
        }

    }

    public class HighScore
    {
        private int m_nCurInitialIndex = 0; // 0 based is first character

        private string m_sInitials = "A__";
        public string Initials
        {
            get { return m_sInitials; }
            set { m_sInitials = value; }
        }
        private int m_nScore;
        public int Score
        {
            get { return m_nScore; }
            set { m_nScore = value; }
        }

        public HighScore()
        {
        }
    }

    // TODO: Finish this class.... 
    public class EnterHighScoreScreen
    {
        public frmAsteroids canvas;
        private int m_CurIndex = 0; // Current index into the initial string, 0 starting offset
        public bool m_bDisplay = false;
        public HighScore newHighScore = new HighScore();

        public bool IsDone
        {
            get { return m_CurIndex>2;  }
        }

        public EnterHighScoreScreen(frmAsteroids frm)
        {
            canvas = frm;
            newHighScore.Initials = "A__";
        }

        public void Draw()
        {
            if (m_bDisplay)
            {
                // ASTEROIDS
                AsteroidsFontHandler afh = new AsteroidsFontHandler(canvas);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

                int topPosOfBottomRowOfHelpText = 485;

                // Since the bottom of this text is perfectly centered, and all other text is centered accordingly,
                // first I find the left for that text and then use that for the left on previous message text
                // SCOREBOARD LEFT!
                afh.FontSize = 32.0f; afh.Kerning = 10;
                afh.Text = "PUSH HYPERSPACE WHEN LETTER IS CORRECT";
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2), topPosOfBottomRowOfHelpText);
                afh.Draw();

                int left = afh.TextLeft;
                afh.Text = "PUSH ROTATE TO SELECT LETTER";
                afh.TextPosition = new Point(left, topPosOfBottomRowOfHelpText -= 50);
                afh.Draw();

                afh.Text = "PLEASE ENTER YOUR INITIALS";
                afh.TextPosition = new Point(left, topPosOfBottomRowOfHelpText -= 50);
                afh.Draw();

                afh.Text = "YOUR SCORE IS ONE OF THE TEN BEST";
                afh.TextPosition = new Point(left, topPosOfBottomRowOfHelpText -= 50);
                afh.Draw();

                // Now draw the high score area.... 
                afh.FontSize = 50.0f; afh.Kerning = 20;
                afh.Text = newHighScore.Initials;
                // For the following, the minus 28 off center is to match the sleightly off center the original arcade version has
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2) - 28, canvas.Height - 215);
                afh.Draw();
            }
        }

        public bool ProcessKey(Keys key )
        {
            bool bProcessed = false;
            if (!IsDone)
            {
                char cur = newHighScore.Initials[m_CurIndex];
                char[] allInitials = newHighScore.Initials.ToCharArray();
                string allChars = "_ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                int indexofCurChar = allChars.IndexOf(cur);

                switch (key)
                {
                    case Keys.Left:
                        if (indexofCurChar == 0)
                            indexofCurChar = allChars.Length;
                        cur = allChars.ToCharArray()[indexofCurChar - 1];
                        allInitials[m_CurIndex] = cur;
                        newHighScore.Initials = string.Join("", allInitials);
                        bProcessed = true;
                        break;
                    case Keys.Right:
                        if (indexofCurChar == allChars.Length - 1)
                            indexofCurChar = -1;
                        cur = allChars.ToCharArray()[indexofCurChar + 1];
                        allInitials[m_CurIndex] = cur;
                        newHighScore.Initials = string.Join("", allInitials);
                        bProcessed = true;
                        break;
                    case Keys.Down:
                        m_CurIndex += 1;
                        bProcessed = true;
                        break;
                }
            }

            return bProcessed;
        }
    }
}