using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS
{
    class AsteroidsFontHandler
    {
        private frmAsteroids canvas;

        private int m_nTextWidth;
        public int TextWidth { get { return m_nTextWidth; } }
        private int m_nTextHeight;
        public int TextHeight { get { return m_nTextHeight; } }
        private int m_nTextLeft;
        public int TextLeft { get { return m_nTextLeft; } }
        private int m_nTextTop;
        public int TextTop{ get { return m_nTextTop; } }

        private int m_nKerning;
        public int Kerning { get { return m_nKerning; } set { m_nKerning = value; }  }

        public Point TextPosition
        {
            set { m_nTextLeft = value.X; m_nTextTop = value.Y;  }
        }
        private string m_sText;

        private int m_nCharSize;
        public string Text
        {
            get { return m_sText; }
            set
            {
                m_sText = value;
                m_nTextWidth = (int) ( ( m_nCharSize + m_nKerning  ) * m_sText.Length );
            }
        }

        private float m_fFontSize;
        public float FontSize
        {
            get { return m_fFontSize; }
            set
            {
                m_fFontSize = value;
                m_nCharSize = (int) ( 0.7f * m_fFontSize );
            }
                 
        }

        public AsteroidsFontHandler( frmAsteroids frm )
        {
            canvas = frm;
        }

        private List<Point> m_secondaryPoints = new List<Point>(); // USED ONLY FOR COPYRIGHT SYMBOL or any non-detached lines
        public void Draw()
        {
            Pen p = new Pen(Color.White);
            var ca = m_sText.ToCharArray();

            for ( int x=0;x< ca.Length; x++ )
            {
                char s = ca[x];
                if (s != '.' && s != ' ')
                {
                    canvas.g.DrawLines(p, GetPoints(s, x));
                    if (m_secondaryPoints.Count > 0)
                    {
                        canvas.g.DrawLines(p, m_secondaryPoints.ToArray());
                        m_secondaryPoints.Clear();
                    }

                }
                else if(s == '.')
                {
                    calcOffsetsAndStoreInSecondaryPoints(x);
                    canvas.g.FillRectangle( p.Brush, m_secondaryPoints[0].X, m_secondaryPoints[0].Y, 1, 1);
                    m_secondaryPoints.Clear();
                }
            }
            return;
        }

        private void calcOffsetsAndStoreInSecondaryPoints( int ox )
        {
            int lx = ox * (m_nCharSize + m_nKerning);
            int fs = (int)FontSize;

            m_secondaryPoints.Add(new Point(m_nTextLeft + lx, m_nTextTop + fs));
        }

        private Point[] GetPoints(char cur, int ox)
        {
            List<Point> points = new List<Point>();
            // int lx = ox * (m_nCharSize + (int) ( (float)m_nCharSize / (float)2.5f ) );
            int lx = ox * (m_nCharSize + m_nKerning);
//                 (m_nCharSize + (int)((float)m_nCharSize / (float)2.5f));
            
            int fs = (int)FontSize;
            switch ( cur )
            {
                case 'A':
                case 'a':
                    points.AddRange(new[]
                                        {   
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + ((2*fs)/3)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + ((2*fs)/3))
                                    });
                    break;

                case 'B':
                case 'b':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + ((2*fs)/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + ((5*fs)/6)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (fs/2))

                                    });
                    break;

                case 'C':
                case 'c':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case 'D':
                case 'd':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop +(fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop +(2*fs/3)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop)
                                    });
                    break;

                case 'E':
                case 'e':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4) , m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });

                    break;
                case 'F':
                case 'f':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4) , m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs)
                                    });
                    break;

                case 'G':
                case 'g':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (2*fs/3))
                                    });
                    break;

                case 'H':
                case 'h':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs/2),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop + fs)
                                    });
                    break;

                case 'I':
                case 'i':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop +fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop +fs)
                                    });
                    break;


                case 'J':
                case 'j':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (2*fs/3))
                                    });
                    break;

                case 'K':
                case 'k':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case 'L':
                case 'l':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop + fs)
                                    });
                    break;

                case 'M':
                case 'm':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;


                case 'N':
                case 'n':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs)
                                    });
                    break;

                case 'P':
                case 'p':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2))
                                    });
                    break;
                case 'Q':
                case 'q':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (5*fs/6))
                                    });
                    break;
                case 'R':
                case 'r':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case 'S':
                case 's':
                case '5':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                    });
                    break;
                case 'T':
                case 't':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs)
                                    });
                    break;

                case 'U':
                case 'u':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                                    });
                    break;

                case 'V':
                case 'v':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                                    });
                    break;

                case 'W':
                case 'w':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                                    });
                    break;

                case 'X':
                case 'x':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs)
                                    });
                    break;

                case 'Y':
                case 'y':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs)
                                    });
                    break;

                case 'Z':
                case 'z':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                // NUMBERS
                case '0': 
                case 'O':
                case 'o':
                    points.AddRange(new[]
                                    {   new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop)
                                    });
                    break;

                case '1':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                    });
                    break;

                case '2':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                    });
                    break;

                case '3':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                    });
                    break;

                case '4':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case '6':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2))
                                    });
                    break;

                case '7':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case '8':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                    });
                    break;

                case '9':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2))
                                    });
                    break;

                case '©':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop)
                                    });

                    m_secondaryPoints.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (5*fs/6))
                                    });
                    break;

                case '_':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                default:
                    points.AddRange(new[]
                                    {   new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx+m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                                    });
                    break;

            }
            return points.ToArray();
        }
    }
}
