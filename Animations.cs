using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS
{
    class Animations
    {
        private static List<List<Vector2>> animationDimensions =
                                        new List<List<Vector2>>();

        private void InitializeAnimationDimensionsList()
        {
            List<Vector2> lCur = new List<Vector2>();

            lCur.Add(new Vector2(0.85f, 40.0f));//1
            lCur.Add(new Vector2(0.75f, 75.0f));
            lCur.Add(new Vector2(1.0f, 68.0f));
            lCur.Add(new Vector2(0.6f, 118.0f));
            lCur.Add(new Vector2(1.0f, 113.0f));
            lCur.Add(new Vector2(0.8f, 136.0f));//6
            lCur.Add(new Vector2(0.45f, 154.0f));
            lCur.Add(new Vector2(0.7f, 210.0f));
            lCur.Add(new Vector2(0.75f, 248.0f));
            lCur.Add(new Vector2(0.35f, 273.0f));//10
            lCur.Add(new Vector2(1.0f, 313.0f));
            lCur.Add(new Vector2(0.5f, 343.0f));//12
            animationDimensions.Add(lCur);
        }
        public enum ANIMTYPE
        {
            GENERAL = 0
        }

        public ANIMTYPE animType;
        public float radius = 0.0f;
        
        public Animations( ANIMTYPE anim )
        {
            InitializeAnimationDimensionsList();
            animType = anim;
        }

        public List<Vector2> sequence() { return animationDimensions[(int) animType]; }
    }
}
