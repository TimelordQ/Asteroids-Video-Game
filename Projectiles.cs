using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS
{
    class Projectiles
    {
        public List<Projectile> list = new List<Projectile>();
        private frmAsteroids canvas;
        private long lastTick = 0;

        public Projectiles(frmAsteroids frm)
        {
            canvas = frm;
            lastTick = System.DateTime.Now.Ticks;
        }

        public void Draw()
        {
            foreach (Projectile p in list)
            {
                float sa = (float)Math.Sin(p.firingAngle - ( Math.PI /2 ));
                float ca = (float)Math.Cos(p.firingAngle - (Math.PI / 2));
                float psa = (float)Math.Sin(p.angleOfShipMomentum - (Math.PI / 2));
                float pca = (float)Math.Cos(p.angleOfShipMomentum - (Math.PI / 2));
                double sox = psa * p.shipVelocity;
                double soy = pca * p.shipVelocity;

                Brush curBrush = (Brush)Brushes.White ;

                // Calculate the new position
                  Point position = new Point( p.position.X + (int)(ca * Projectile.SPEED),
                                             p.position.Y + (int)(sa * Projectile.SPEED  ));
                if (position.X < 0)
                    position = new Point(canvas.Width + position.X, position.Y);
                else if (position.X > canvas.Width)
                    position = new Point(position.X - canvas.Width, position.Y);
                if (position.Y < 0)
                    position = new Point(position.X, canvas.Height + position.Y);
                else if (position.Y > canvas.Height)
                    position = new Point(position.X, position.Y - canvas.Height);

                // Draw the laser
                canvas.g.FillRectangle(curBrush, position.X, position.Y, 2, 2);

                // Update the Projectile Information
                p.position = position;
                p.decay += System.DateTime.Now.Ticks - lastTick;
            }

            lastTick = System.DateTime.Now.Ticks;

            // Remove Decayed Projectils
            if (list.Count > 0)
            {
                for (int nCur = list.Count; nCur > 0; nCur--)
                {
                    if ( ( list[nCur-1].decay / 10000000) > Projectile.DECAYTIME)
                        list.RemoveAt(nCur-1);
                }
            }
        }

        private long ticksSinceLastFire = 0;
        public void Fire(Ship player, Point value, float curAngle, double av )
        {
            long curTicks = System.DateTime.Now.Ticks;
            // The time calculation is used to limit the firing speed to one projectile every 1/10th of a second, roughly
            // the same limitations as imposed on the actual video game itself. 
            if (list.Count < 4  && ( ( curTicks - ticksSinceLastFire ) > 1000000 ) )
            {
                ticksSinceLastFire = curTicks;
                list.Add(new Projectile() { decay = 0.0f, firingAngle = curAngle, position = value, angulerVelocity = av, shipVelocity = player.velocity, angleOfShipMomentum = player.m_myAngle });
            }
        }

        public void RandomFire(Point origin)
        {
            long curTicks = System.DateTime.Now.Ticks;
            float curAngle = (float)canvas.randomizer.Next(1, 360);
            float av = (float)canvas.randomizer.Next(1, 360);

            // The time calculation is used to limit the firing speed to one projectile every 1/10th of a second, roughly
            // the same limitations as imposed on the actual video game itself. 
            if (list.Count < 4 && ((curTicks - ticksSinceLastFire) > 1000000))
            {
                ticksSinceLastFire = curTicks;
                list.Add(new Projectile() { decay = 0.0f, firingAngle = curAngle, position = origin, angulerVelocity = av });
            }
        }

        public void NotSoRandomFire(Point origin, float angleOfPlayerFromUFO )
        {
            long curTicks = System.DateTime.Now.Ticks;
            float curAngle = angleOfPlayerFromUFO, av = curAngle;

            // The time calculation is used to limit the firing speed to one projectile every 1/10th of a second, roughly
            // the same limitations as imposed on the actual video game itself. 
            if (list.Count < 4 && ((curTicks - ticksSinceLastFire) > 1000000))
            {
                ticksSinceLastFire = curTicks;
                list.Add(new Projectile() { decay = 0.0f, firingAngle = curAngle, position = origin, angulerVelocity = av });
            }
        }
    }

    public class Projectile
        {
            public const float DECAYTIME = 1.2f; // in seconds
            public const float SPEED = 19.0f; // Pixels per second

            public float decay = 0.0f;
            public float firingAngle = 0.0f;
            public double angulerVelocity = 0.0f; // Used to determine +/- offset for speed
            public Point position = new Point(0, 0);

            // Ship offset information to retain based on momentum changes of a projectile in motion
            public double shipVelocity = 0.0f;
            public float angleOfShipMomentum = 0.0f;

        }
}
