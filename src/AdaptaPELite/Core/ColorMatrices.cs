using System;

namespace AdaptaPE.Core
{
    public static class ColorMatrices
    {
        public static readonly string[] Names={"ninguno","protanomalia","protanopia","deuteranomalia","deuteranopia","tritanomalia","tritanopia","acromatopsia"};
        public static readonly string[] Titles={"Sin filtro","Protanomalía","Protanopía","Deuteranomalía","Deuteranopía","Tritanomalía","Tritanopía","Acromatopsia"};
        static readonly float[][] rgb={
            new float[]{1,0,0,0,1,0,0,0,1},new float[]{.817f,.183f,0,.333f,.667f,0,0,.125f,.875f},
            new float[]{.567f,.433f,0,.558f,.442f,0,0,.242f,.758f},new float[]{.8f,.2f,0,.258f,.742f,0,0,.142f,.858f},
            new float[]{.625f,.375f,0,.7f,.3f,0,0,.3f,.7f},new float[]{.967f,.033f,0,0,.733f,.267f,0,.183f,.817f},
            new float[]{.95f,.05f,0,0,.433f,.567f,0,0,.475f,.525f},new float[]{.299f,.587f,.114f,.299f,.587f,.114f,.299f,.587f,.114f}
        };
        public static float[] Native(int index) {
            if(index<0||index>=rgb.Length)throw new ArgumentOutOfRangeException("index");
            var m=new float[25];for(int output=0;output<3;output++)for(int input=0;input<3;input++)m[input*5+output]=rgb[index][output*3+input];m[18]=1;m[24]=1;return m;
        }
    }
}
