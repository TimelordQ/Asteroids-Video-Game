using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS
{
    class Asteroid
    {
        public Point Position;
        public List<Point> points = new List<Point>();
        public float velocity;
        private frmAsteroids canvas;
        
        public float m_fMoveAngle;

        // Size constants
        private const float NUMPOINTS = 11.0f;
        private const float LA_WIDTHLARGE = 55;
        private const float MA_WIDTHLARGE = 28;
        private const float SA_WIDTHLARGE = 13;

        // Speed constants
        private const int SLOWSPEED = 4;
        private const int FASTSPEEDBIG = 5;
        private const int FASTSPEEDMEDIUM = 6;
        private const int FASTSPEEDSMALL = 7;

        public enum SIZEOFASTEROID
        {
            SMALL = 0,
            MEDIUM = 1,
            LARGE = 2
        } ;

        public List<int> ASTEROIDPOINTVALUES = new List<int>() { 100, 50, 20 };

        public SIZEOFASTEROID mySize;
        public float myRadius = 0.0f;

        public bool bDestroyed = false;
        public bool bPlayerCollided = false;

        public Point collisionLocation;
        public Animations destructionAnimation = new Animations( Animations.ANIMTYPE.GENERAL );
        public float MAXANIMRADIUS = LA_WIDTHLARGE;

        public Asteroid(frmAsteroids frm, SIZEOFASTEROID asteroidSize, Point? p = null)
        {
            int fs;
            canvas = frm;
            mySize = asteroidSize;

            if (asteroidDimensions.Count == 0)
                InitializeAsteroidDimensionsList();

            if (mySize == SIZEOFASTEROID.SMALL)
                fs = FASTSPEEDSMALL;
            else if (mySize == SIZEOFASTEROID.MEDIUM)
                fs = FASTSPEEDMEDIUM;
            else
                fs = FASTSPEEDBIG;

            if (p == null)
                Position = new Point(canvas.randomizer.Next((int)LA_WIDTHLARGE, canvas.Width - (int)LA_WIDTHLARGE),
                                    canvas.randomizer.Next((int)LA_WIDTHLARGE, canvas.Height - (int)LA_WIDTHLARGE));
            else
                Position = (Point) p;

            Generate(mySize);
            velocity = canvas.randomizer.Next(SLOWSPEED, fs);
            m_fMoveAngle = ((float)canvas.randomizer.Next(0, 360));
        }

        private static List<List<Vector2>> asteroidDimensions = 
                                        new List<List<Vector2>>();

        private void InitializeAsteroidDimensionsList()
        {
            List<Vector2> lCur = new List<Vector2>();

            // Big Asteroid #1
            lCur.Add(new Vector2(0.4f, 5.0f));//1
            lCur.Add(new Vector2(0.9f, 30.0f));
            lCur.Add(new Vector2(1.0f, 68.0f));
            lCur.Add(new Vector2(0.65f, 89.0f));
            lCur.Add(new Vector2(1.0f, 113.0f));
            lCur.Add(new Vector2(0.8f, 160.0f));//6
            lCur.Add(new Vector2(0.85f, 208.0f));
            lCur.Add(new Vector2(0.9f, 246.0f));
            lCur.Add(new Vector2(0.95f, 295.0f));
            lCur.Add(new Vector2(0.87f, 333.0f));//10
            asteroidDimensions.Add(lCur);

            // Big Asteroid #2
            lCur = new List<Vector2>();
            lCur.Add(new Vector2(0.9f, 26.0f));//1
            lCur.Add(new Vector2(0.95f, 65.0f));
            lCur.Add(new Vector2(0.45f, 65.0f));
            lCur.Add(new Vector2(0.92f, 103.0f));
            lCur.Add(new Vector2(0.92f, 153.0f));
            lCur.Add(new Vector2(0.7f, 202.0f));//6
            lCur.Add(new Vector2(0.95f, 210.0f));
            lCur.Add(new Vector2(1.0f, 247.0f));
            lCur.Add(new Vector2(1.0f, 293.0f));
            lCur.Add(new Vector2(0.95f, 331.0f));
            lCur.Add(new Vector2(0.6f, 354.0f));//11
            asteroidDimensions.Add(lCur);

            // Big Asteroid #3
            lCur = new List<Vector2>();
            lCur.Add(new Vector2(0.7f, 19.0f));//1
            lCur.Add(new Vector2(0.92f, 72.0f));
            lCur.Add(new Vector2(0.88f, 84.0f));
            lCur.Add(new Vector2(0.2f, 111.0f));
            lCur.Add(new Vector2(1.0f, 117.0f));
            lCur.Add(new Vector2(0.96f, 152.0f));//6
            lCur.Add(new Vector2(0.65f, 160.0f));
            lCur.Add(new Vector2(0.96f, 205.0f));
            lCur.Add(new Vector2(0.88f, 252.0f));
            lCur.Add(new Vector2(0.89f, 290.0f));
            lCur.Add(new Vector2(0.35f, 329.0f));
            lCur.Add(new Vector2(0.75f, 330.0f));//12
            asteroidDimensions.Add(lCur);
        }

        public void Draw()
        {
            Pen penColor = new Pen(Color.White);
            if (!bDestroyed)
            {
                for (int nPoint = 0; nPoint < points.Count; nPoint++)
                {
                    Point from, to;
                    if (nPoint < (points.Count - 1))
                    {
                        from = new Point(Position.X + points[nPoint].X, Position.Y + points[nPoint].Y);
                        to = new Point(Position.X + points[nPoint + 1].X, Position.Y + points[nPoint + 1].Y);
                    }
                    else
                    {
                        from = new Point(Position.X + points[nPoint].X, Position.Y + points[nPoint].Y);
                        to = new Point(Position.X + points[0].X, Position.Y + points[0].Y);
                    }
                    canvas.g.DrawLine(penColor, from, to);
                }
            }
            else
            {
                Brush curBrush = (Brush)Brushes.White;
                List<Point> daPoints = new List<Point>();
                foreach (Vector2 vec in destructionAnimation.sequence() )
                {
                    float ND = -(float)((float)Math.PI / 2.0f);
                    int nX = (int)(Math.Cos((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius );
                    int nY = (int)(Math.Sin((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius);

                    canvas.g.FillRectangle(curBrush, collisionLocation.X + nX, collisionLocation.Y + nY , 2, 2);
                    //daPoints.Add(new Point(nX, nY));
                }
            }
        }
        public void Move()
        {
            this.Position.X += (int)(Math.Sin((m_fMoveAngle * Math.PI) / 180.0f) * velocity);
            this.Position.Y += (int)(Math.Cos((m_fMoveAngle * Math.PI) / 180.0f) * velocity);

            if (this.Position.X > canvas.Width)
                this.Position.X = this.Position.X - canvas.Width;
            else if (this.Position.Y > canvas.Height)
                this.Position.Y = this.Position.Y - canvas.Height;
            else if (this.Position.X < 0)
                this.Position.X = canvas.Width + this.Position.X;
            else if (this.Position.Y < 0)
                this.Position.Y = canvas.Height  + this.Position.Y;
        }


        private void Generate( SIZEOFASTEROID asteroidSize )
        {
            float rotationAngle = (float)(Math.PI * 2.0f) / (float)(NUMPOINTS);
            float curAngle = 0;

            switch (asteroidSize)
            {
                case SIZEOFASTEROID.LARGE:
                    myRadius = LA_WIDTHLARGE;
                    break;
                case SIZEOFASTEROID.MEDIUM:
                    myRadius = MA_WIDTHLARGE;
                    break;
                case SIZEOFASTEROID.SMALL:
                    myRadius = SA_WIDTHLARGE;
                    break;
            }

            int nCurAsteroid = canvas.randomizer.Next(0, asteroidDimensions.Count);
            foreach (Vector2 vec in asteroidDimensions[nCurAsteroid])
            {
                float ND = -(float)((float)Math.PI / 2.0f);
                int nX = (int)(Math.Cos((vec.Y / 180) * Math.PI + ND) * vec.X * myRadius);
                int nY = (int)(Math.Sin((vec.Y / 180) * Math.PI + ND) * vec.X * myRadius);
                points.Add(new Point(nX, nY));
            }
        }

        public bool doesPointCollide( Point p )
        {
            float checkVal = (float)Math.Sqrt(Math.Pow(Position.X - p.X, 2) + Math.Pow(Position.Y - p.Y, 2));

            // myRadius + X, where X is the fudge factor leniency for a hit, extending the radius in this case 
            // by 8 pixels to  accomodate for higher speed projectile calculation misses. This won't catch 
            // everything that should register as a hit, but does a lot better than requiring a direct hit 
            // for a projectile that moves right through (and past) the target without registering a hit because
            // for one frame it's on one side of the target and the next it's on the other. 
            if (checkVal < ( myRadius + 2)) 
            {
                collisionLocation = p;
                return true;
            }

            return false;
        }

        public void newPseudoRandomVelocity( Asteroid a )
        {
            float lb = (a.velocity - 5 > SLOWSPEED) ? a.velocity - 5 : SLOWSPEED;
            float ub;
            if (mySize == SIZEOFASTEROID.MEDIUM)
                ub = (a.velocity + 5 > FASTSPEEDMEDIUM) ? a.velocity + 5 : FASTSPEEDMEDIUM;
            else
                ub = (a.velocity + 5 > FASTSPEEDSMALL) ? a.velocity + 5 : FASTSPEEDSMALL;

            velocity = canvas.randomizer.Next((int)lb, (int)ub);
        }
    }
}
