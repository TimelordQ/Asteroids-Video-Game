using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASTEROIDS
{
    class UFO
    {
        private frmAsteroids canvas;

        private bool m_bLeftToRight;
        public float m_myAngle = 0.0f;
        
        //public float velocity;
        public const float SMALLSPEED = 5.0f;
        public const float LARGESPEED = 4.5f; // Bigger and slower. 
        private const long TICKSPERSECOND = 10000000;
        public const float MAXVELOCITY = 0.8f;
        public const int RADIUS = 27; // effectively the maxium distance drawn from center
        public const int XMARGIN = 200; // DEAD ZONE preventing further changes in direction. 

        public Point collisionLocation;
        public Animations destructionAnimation = new Animations(Animations.ANIMTYPE.GENERAL);


        private const int MAXDIRECTIONALCHANGES = 5;
        private const long MININTERVALFORDIRECTIONALCHANGE = TICKSPERSECOND * 2;
        private int m_nDirectionalChanges = 0;
        private const float PROBABILITYOFDIRECTIONCHANGESTART = 5.0f;
        private const float PROBABILITYOFDIRECTIONCHANGEINCREASEPERTICK = 0.1f;

        private float m_fProbabilityOfDirectionChange = PROBABILITYOFDIRECTIONCHANGESTART;
        private long nextPossibleDirectionalChange;

        private float m_sa;
        private float m_ca;
        private float m_nForAngularSpeedAdjustmentHack = 90.0f;

        public enum UFOSIZE
        {
            LARGE = 0,
            SMALL = 1
        }

        public UFOSIZE m_ufoType = UFOSIZE.SMALL;

        public Point position = new Point();
        private bool m_bIsActive = false;
        public bool IsActive
        {
            get { return m_bIsActive;  }
            set { m_bIsActive = value; }
        }

        public bool bIsHyperSpace = false;

        private long m_DestructionAnimationTickStart;
        private bool m_bUFOIsDestroyed = false;
        public bool bUFOIsDestroyed
        {
            get { return m_bUFOIsDestroyed; }
            set
            {
                if (value)
                    m_DestructionAnimationTickStart = DateTime.Now.Ticks;

                m_bUFOIsDestroyed = value;
            }
        }

        public UFO(frmAsteroids frm)
        {
            canvas = frm;
        }

        public void Draw()
        {
            Pen pen = new Pen(Color.White);

            if (m_bUFOIsDestroyed)
            {
                if (destructionAnimation.radius < RADIUS)
                    destructionAnimation.radius += 2.25f;
                else
                {
                    IsActive = false;
                    m_bUFOIsDestroyed = false;
                }

                Brush curBrush = (Brush)Brushes.White;
                List<Point> daPoints = new List<Point>();
                foreach (Vector2 vec in destructionAnimation.sequence())
                {
                    float ND = -(float)((float)Math.PI / 2.0f);
                    int nX = (int)(Math.Cos((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius);
                    int nY = (int)(Math.Sin((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius);

                    canvas.g.FillRectangle(curBrush, collisionLocation.X + nX, collisionLocation.Y + nY, 2, 2);
                }
            }
            else if (m_bIsActive) 
            {
                bool bShouldIChangeDirection = (canvas.randomizer.Next(1,(int)m_fProbabilityOfDirectionChange) ==1)?true:false;
                // The probability of direction change emulates the seemingly random aspect of the ship's directional changes
                // in the arcade gameas it moves across the screen. Basically, for every tick the direction doesn't change, 
                // the probability  of a directional change increases ever so slightly, making it so there's no single direction 
                // the ship maintains for too long before it changes direction, just like the original arcade game. 
                if (bShouldIChangeDirection )
                    m_fProbabilityOfDirectionChange = PROBABILITYOFDIRECTIONCHANGESTART;
                else if(m_fProbabilityOfDirectionChange > 2)
                    m_fProbabilityOfDirectionChange -= PROBABILITYOFDIRECTIONCHANGEINCREASEPERTICK;

                // only change direction if the time elapsed since a last directional change has occured
                // AND the number of times this has changed direction doesn't exceed the max threshold
                if ((nextPossibleDirectionalChange < DateTime.Now.Ticks) &&
                     (bShouldIChangeDirection) &&
                     (m_nDirectionalChanges < MAXDIRECTIONALCHANGES))
                {
                    m_nDirectionalChanges += 1;
                // Account for left to right in the movement.
                weirdShitWithRandomNumbers:
                    int nDirection = canvas.randomizer.Next(1, 4);

                    if (nDirection < 1 || nDirection > 3) goto weirdShitWithRandomNumbers;

                    m_nForAngularSpeedAdjustmentHack = ( 45 * (int)nDirection);

                    m_myAngle = ( ( m_bLeftToRight )?1: -1 ) * (float) ( (float)m_nForAngularSpeedAdjustmentHack * ( Math.PI / 180.0f) );

                    m_sa = (float) Math.Sin(m_myAngle);
                    m_ca = (float) Math.Cos(m_myAngle);

                    nextPossibleDirectionalChange = DateTime.Now.Ticks + MININTERVALFORDIRECTIONALCHANGE;
                }
                // Another hack. The UFO always seems to be going in a straight line at the left and right most 
                // edges of the screen revert any directional changes to 
                if (   ( ( ( ( position.X + XMARGIN ) > canvas.Width ) && m_bLeftToRight ) 
                    ||   ( ( ( position.X - XMARGIN ) < 0) ) && !m_bLeftToRight )  &&
                         ( Math.Abs(m_myAngle) != 90.0f ) )
                {
                    m_myAngle = ((m_bLeftToRight) ? 1 : -1) * (float)((float)90.0f * (Math.PI / 180.0f));

                    m_sa = (float)Math.Sin(m_myAngle);
                    m_ca = (float)Math.Cos(m_myAngle);
                }

                position.X += (int) ( (float) ( (m_bLeftToRight) ? 1 : -1)  *
                                    ((m_ufoType == UFOSIZE.LARGE) ? LARGESPEED : SMALLSPEED ) );
                position.Y += (int)(m_ca *
                                   ((float)((m_ufoType == UFOSIZE.LARGE) ? LARGESPEED : SMALLSPEED)));

                if (position.Y < 0) position.Y = canvas.Height + position.Y;
                if (position.Y > canvas.Height) position.Y = position.Y - canvas.Height;

                if (position.X - RADIUS > canvas.Width || position.X + RADIUS < 0)
                {
                    m_bIsActive = false;
                    canvas.onUFOExit();
                }
                else
                {
                    List<Point> points = new List<Point>();

                    int px = position.X;
                    int py = position.Y;

                    if (m_ufoType == UFOSIZE.LARGE)
                    {
                        points.AddRange(new[]
                                            {
                                        new Point(px + 27, py),
                                        new Point(px + 10, py-9),
                                        new Point(px + 5, py-17),
                                        new Point(px - 5, py-17),
                                        new Point(px - 10, py-9),
                                        new Point(px + 10, py-9),
                                        new Point(px - 10, py-9),
                                        new Point(px - 27, py),
                                        new Point(px + 27, py),
                                        new Point(px + 10, py +9),
                                        new Point(px - 10, py +9),
                                        new Point(px - 27, py),
                                    });
                    }
                    else // Small one
                    {
                        points.AddRange(new[]
                                            {
                                        new Point(px + 27/2, py),
                                        new Point(px + 10/2, py-9/2),
                                        new Point(px + 5/2, py-17/2),
                                        new Point(px - 5/2, py-17/2),
                                        new Point(px - 10/2, py-9/2),
                                        new Point(px + 10/2, py-9/2),
                                        new Point(px - 10/2, py-9/2),
                                        new Point(px - 27/2, py),
                                        new Point(px + 27/2, py),
                                        new Point(px + 10/2, py +9/2),
                                        new Point(px - 10/2, py +9/2),
                                        new Point(px - 27/2, py),
                                    });
                    }

                    canvas.g.DrawLines(pen, points.ToArray());
                }
                // doPhysics();
            }
        }

        private void doPhysics()
        {

        }

        // Level & time in this level are both used to determine the type of UFO to spawn. 
        public void spawnUFO(int level, long levelTime)
        {
            if ( !m_bIsActive ) // only spawn a "new one" when the old one is gone
            {
                // My observations of the game have resulted in lower levels having a much higher initial incidence of larger 
                // UFOs, and the longer time spent on a level the higher the chance of a smaller and faster one. As the player
                // level increases, the less likely I was seeing the larger ships. Still not altogether gone, but what stood
                // true was the higher the likelihood of smaller and faster and deadlier ones more quickly. 
                // This simplistic algorithm reflects those observations. 
                m_ufoType = (((levelTime / TICKSPERSECOND) * level) < 45) ? UFOSIZE.LARGE : UFOSIZE.SMALL;

                // Does the UFO move from left to right or right to left?
                m_bLeftToRight = ( canvas.randomizer.Next(1,100) < 50 )? true: false;

                // Sets the beginning position at a random point on the Y axis just off screen on the left or right
                position = new Point((m_bLeftToRight) ? -RADIUS : canvas.Width + RADIUS, RADIUS + canvas.randomizer.Next(canvas.Height - (2*RADIUS)));

                nextPossibleDirectionalChange = DateTime.Now.Ticks + MININTERVALFORDIRECTIONALCHANGE;
                m_nDirectionalChanges = 0;

                // set the initial direction as being directly left or right, like the game.... 
                m_myAngle = ((m_bLeftToRight) ? 1 : -1) * (float)( Math.PI/2 );

                // Initialize the sin and cosine
                m_sa = (float)Math.Sin(m_myAngle);
                m_ca = (float)Math.Cos(m_myAngle);

                // Good to go... UFO is active..
                m_bIsActive = true;
            }

        }

        public bool doesObjectCollide(Point p, float objectradius)
        {
            float checkVal = (float)Math.Sqrt(Math.Pow(position.X - p.X, 2) + Math.Pow(position.Y - p.Y, 2));
            if (checkVal < objectradius)
            {
                collisionLocation = position;
                destructionAnimation.radius = 0.0f;
                return true;
            }

            return false;
        }

        public void triggerCollisionSequence()
        {
            collisionLocation = position;
            destructionAnimation.radius = 0.0f;
            m_bIsActive = false;
            m_bUFOIsDestroyed = true;
        }
    }
}
