using UnityEngine;
using System.Collections;

public static class SonicAssembler
{
    /// <summary>
    /// CalcAngle - Calculates the Archtangent of y/x (sub_364E)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Angle in original (360 Degrees = 256)</returns>
    public static float CalcAngle(float x, float y)
    {
        //moveq	#0,d3
        //moveq	#0,d4
        //move.w	d1,d3
        //move.w	d2,d4
        float d3 = x;
        float d4 = y;

        //or.w	d3,d4
        //beq.s	CalcAngle_Zero ; special case return if x and y are both 0
        if (d3 == 0f && d4 == 0f)
        {
            //CalcAngle_Zero : TODO
        }

        //absw.w	d3
        //absw.w	d4
        d3 = Mathf.Abs(d3);
        d4 = Mathf.Abs(d4);

        //cmp.w	d3,d4
        //bhs.w	+
        if (d3 >= d4)
        {

        }

        //lsl.l	#8,d4
        d4 = AsmMath.LSL(d4, 0x08);

        return 0f;
    }

}

public static class AsmMath
{
    /// <summary>
    /// Performs the Logical Shift Left Assembler Neumonic
    /// </summary>
    /// <param name="fIn"></param>
    /// <param name="Shift"></param>
    /// <returns></returns>
    public static float LSL(float fIn, byte Shift)
    {
        ulong d1 = (ulong)fIn;
        d1 = d1 << Shift;
        return (float)d1;
    }
    
}