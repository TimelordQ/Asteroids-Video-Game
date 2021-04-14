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
    class Ship
    {
        private frmAsteroids canvas;
        public float m_myAngle = 0.0f;
        public float m_accelerationAngle = 0.0f;
        public float rotationAngle;
        public double velocity = 0.0f;

        //public float velocity;
        public const double ACCELERATIONSPEED = 0.015f;
        public const double DECELERATIONSPEED = 0.0002f;
        public const double MAXVELOCITY = 0.8f;
        public const float ROTSPEED = ((float)Math.PI / 90.0f) * 2.8f;
        public const int RADIUS = 0;

        private bool m_bAccelerating;

        private bool m_bIsRotating;
        private Keys m_keyDepressed;

        public Point position = new Point();
        public bool m_bIsActive = false;
        public bool bIsHyperSpace = false;

        private long m_DestructionAnimationTickStart;
        private bool m_bPlayerIsDestroyed = false;

        public bool bPlayerIsDestroyed
        {
            get { return m_bPlayerIsDestroyed;  }
            set {
                if (value)
                    m_DestructionAnimationTickStart = DateTime.Now.Ticks;

                m_bPlayerIsDestroyed = value;
                }
        }

        public Ship( frmAsteroids frm )
        {
            canvas = frm;
            resetToCenter();
        }

        public void resetToCenter()
        {
            position.X = (canvas.Width - 15) / 2;
            position.Y = (canvas.Height + 25) / 2;
        }

        public void Draw()
        {
            if (bIsHyperSpace)
                return; // No draw, no physics in hyperspace

            if (m_bIsRotating)
            {
                if (m_keyDepressed == Keys.Left)
                    m_accelerationAngle -= ((m_accelerationAngle>0.0f)?ROTSPEED:-((float)(Math.PI*2.0f)-ROTSPEED ));
                
                else
                    m_accelerationAngle += ((m_accelerationAngle < (Math.PI*2.0f)) ? ROTSPEED : -(m_accelerationAngle-ROTSPEED));

                rotationAngle = m_accelerationAngle;
            }

            Pen pen = new Pen(Color.White);
            if (m_bPlayerIsDestroyed)
            {
                int offset = (int)((DateTime.Now.Ticks - m_DestructionAnimationTickStart) / 500000);

                // SIX (6) lines for the destruction animation sequence. NEVER taking into account the rotation of the ship. 
                if (offset < 26)
                    canvas.g.DrawLine(pen,
                                        new Point(position.X - 7,
                                                  position.Y - 9 - offset),
                                        new Point(position.X + 9,
                                                  position.Y - 6 - offset));

                if (offset < 32)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X - 2 + (int)(Math.Sin(Math.PI / 3) * offset),
                                              position.Y - 9 - (int)(Math.Cos(Math.PI / 3) * offset)),
                                    new Point(position.X + 15 + (int)(Math.Sin(Math.PI / 3) * offset),
                                              position.Y - (int)(Math.Cos(Math.PI / 3) * offset)));

                float sa = (float)Math.Sin(Math.PI/2);
                float ca = (float)Math.Cos(Math.PI/2);

                if (offset < 38)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X + 7 + (int)(sa * offset),
                                              position.Y + (int)(ca * offset)),
                                    new Point(position.X + 15 + (int)(sa * offset),
                                              position.Y + 10 + (int)(ca * offset)));

                sa = (float)Math.Sin(Math.PI / 6);
                ca = (float)Math.Cos(Math.PI / 6);

                if (offset < 30)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(sa * offset),
                                              position.Y + 7 + (int)(ca * offset)),
                                    new Point(position.X + 10 + (int)(sa * offset),
                                              position.Y + 4 + (int)(ca * offset)));

                sa = (float)Math.Sin(Math.PI / 6);
                ca = (float)Math.Cos(Math.PI / 6);

                if (offset < 12)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X - 7 - (int)(sa * offset),
                                              position.Y + 2 + (int)(ca * offset)),
                                    new Point(position.X - (int)(sa * offset),
                                              position.Y + 8 + (int)(ca * offset)));

                sa = (float)Math.Sin(Math.PI / 6);
                ca = (float)Math.Cos(Math.PI / 6);

                if (offset < 50)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X - 3 - (int)(sa * offset),
                                              position.Y - 3 - (int)(ca * offset)),
                                    new Point(position.X - 10 - (int)(sa * offset),
                                              position.Y + 9 - (int)(ca * offset)));

            }
            else
            {
                float sa = (float)Math.Sin(m_accelerationAngle);
                float ca = (float)Math.Cos(m_accelerationAngle);

                // Draw the ship
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(sa * 13),
                                              position.Y - (int)(ca * 13)),
                                    new Point(position.X + (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) + (int)(sa * 10)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) + (int)(sa * 10)),
                                    new Point(position.X + (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) + (int)(sa * 4)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) + (int)(sa * 4)),
                                    new Point(position.X - (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) - (int)(sa * 4)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X - (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) - (int)(sa * 10)),
                                    new Point(position.X - (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) - (int)(sa * 4)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(sa * 13),
                                              position.Y - (int)(ca * 13)),
                                    new Point(position.X - (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) - (int)(sa * 10)));

                // Draw the Jet Flume ONLY if it's being accelerated....
                if (m_bAccelerating)
                {
                    canvas.g.DrawLine(pen,
                                        new Point(position.X - (int)(ca * 4) - (int)(sa * 14),
                                                  position.Y + (int)(ca * 14) - (int)(sa * 4)),
                                        new Point(position.X - (int)(sa * 23),
                                                  position.Y + (int)(ca * 23)));
                    canvas.g.DrawLine(pen,
                                        new Point(position.X + (int)(ca * 4) - (int)(sa * 14),
                                                  position.Y + (int)(ca * 14) + (int)(sa * 4)),
                                        new Point(position.X - (int)(sa * 23),
                                                  position.Y + (int)(ca * 23)));
                }

                doPhysics();
            }
        }

        public void Rotate(Keys key, bool bIsRotating )
        {
            m_bIsRotating = bIsRotating;
            m_keyDepressed = key;
        }

        public void Accelerate(Keys key, bool bAccelerate)
        {
            m_bAccelerating = bAccelerate;
            //m_accelerationAngle = m_myAngle;
        }

        private void doPhysics()
        {
            // Step 1: Calculate the new acceleration, no change in trajectory cases
            if (!m_bAccelerating)
            {
                if (velocity > 0.0f)
                    velocity -= DECELERATIONSPEED;
                else
                    velocity = 0.0f;
            }
            // Step 2: Calculate acceleration changes, positive acceleration...
            // Increase velocity from full stop...
            else if (velocity == 0.0f) 
            {
                velocity += ACCELERATIONSPEED;
                m_myAngle = m_accelerationAngle;
            }
            // Constant Increase in Velocity, Same Direction.
            else if ( ((velocity + ACCELERATIONSPEED) < MAXVELOCITY)
                      && ( m_myAngle == m_accelerationAngle ) )
            {
                velocity += ACCELERATIONSPEED;
            }
            else /// Increase velocity, mid flight, could include trajectory change....
            {
                // Calculate existing expected offsets from current position
                double vfCX = velocity * Math.Cos(m_myAngle);
                double vfCY = velocity * Math.Sin(m_myAngle);
                // Calculate the change in position based on the thrust direction
                double vfNX = ACCELERATIONSPEED * Math.Cos(m_accelerationAngle);
                double vfNY = ACCELERATIONSPEED * Math.Sin(m_accelerationAngle);
                // Make sure the accelertaion adjustment does not exceed maximum acceleration, and then set it if it won't. 
                if ((Math.Sqrt(Math.Pow(vfCX + vfNX, 2) + Math.Pow(vfCY + vfNY, 2)) < MAXVELOCITY))
                    velocity = Math.Sqrt(Math.Pow(vfCX + vfNX, 2) + Math.Pow(vfCY + vfNY, 2));

                // calculate the new angle...
                // I was using ATAN at first, which was causing glitchiness, changing it to ATAN2 resolves that glitch.
                // This StackOverflow question details the different in quadrant calculations that each returns
                // https://stackoverflow.com/questions/283406/what-is-the-difference-between-atan-and-atan2-in-c
                m_myAngle =  (float)Math.Atan2((vfCY + vfNY),(vfCX + vfNX));
                // Reference this wonderful fuckin video for how this was obtained! 
                // From: Michel van Biezen  https://www.youtube.com/watch?v=l53G_Y1kLc4  
            }

            float sa = (float)Math.Sin(m_myAngle);
            float ca = (float)Math.Cos(m_myAngle);

            position = new Point(position.X + (int)(sa * velocity * 15),
                                 position.Y - (int)(ca * velocity * 15));
            if (position.X < 0)
                position = new Point(canvas.Width + position.X, position.Y);
            else if (position.X > canvas.Width)
                position = new Point(position.X - canvas.Width, position.Y);
            if (position.Y < 0)
                position = new Point(position.X, canvas.Height + position.Y);
            else if (position.Y > canvas.Height)
                position = new Point(position.X, position.Y - canvas.Height);

            /*canvas.DEBUGOUTPUT = String.Format("ROT={0:0.00},ACC={1:0.00},AA={2:0.00}",
                                                ((m_myAngle * 180.0f) / Math.PI),
                                                m_Acceleration,
                                                ((m_accelerationAngle * 180.0f) / Math.PI) ); */
        }

        public bool doesObjectCollide(Point p, float objectradius )
        {
            float checkVal = (float)Math.Sqrt(Math.Pow(position.X - p.X, 2) + Math.Pow(position.Y - p.Y, 2));
            if (checkVal < (objectradius + RADIUS + 10))
                return true;
            else if (objectradius < 10)
                throw new Exception("SHOULD NEVER BE HERE!");

            return false;
        }
    }
}
