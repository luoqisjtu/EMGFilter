using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Program
{
    static void Main(string[] args)
    {
    }
}
public class BayesFilter
{
    public int PROBPOINTS = 128;
	public float dEmgThreshold = 0.02f;
	public float dMaxv = 0.8f;
	public float dSwitchVal = 0.1f;//1e-1f
	public float dDriftVal = 1.5e-4f;//1e-4f
	public float dCurrEst = -99f;
	public List<float> prior = new List<float>();
	public List<float> expx = new List<float>();
    public BayesFilter()
    {

        int i;
        for (i = 0; i < PROBPOINTS; i++)
        {
			prior.Add((float)(1.0 / PROBPOINTS));
			expx.Add((float) Math.Exp(-(double)i / PROBPOINTS));
        }
    }
	public BayesFilter(float thresh, float maxv, float switchv, float drif)
    {
        int i;
        for (i = 0; i < PROBPOINTS; i++)
        {
			prior.Add((float)(1.0 / PROBPOINTS));
			expx.Add((float) Math.Exp(-(double)i / PROBPOINTS));
        }

    }
    ~BayesFilter() { }
    // Updating estimate
    public float UpdateEst(float samp)
    // Updates the filter with new measurement
    {
        int i = 0;
		float v = 0.0f;
		float total_pdf = 0.0f;
		float max_pdf_val = 0.0f;
        int max_pdf_index = 0;


        // Normalize or zero the value
        v = Math.Abs(samp);
        if (v < dEmgThreshold)
            v = 0.0f;
        v /= dMaxv;
        v *= 4;

        // Do the propagation steps
        // blurring NECESSARY FOR SMOOTH MOVEMENT
        for (i = 0; i < PROBPOINTS; i++)
            if (i > 0 && i < PROBPOINTS - 1)
                prior[i] += dDriftVal * (prior[i - 1] + prior[i + 1]) / 100.0f;

        //constant shift   NECESSARY FOR JUMPS
        for (i = 0; i < PROBPOINTS; i++)
            prior[i] += dSwitchVal * 1.0E-12f;

        // Do estimation step, get sum	
        for (i = 0; i < PROBPOINTS; i++)
        {
			prior[i] *= (float) Math.Pow(((float)i) / PROBPOINTS, v) * expx[i];  //poisson
            total_pdf += prior[i];
        }

        // normalize
        for (i = 0; i < PROBPOINTS; i++)
            prior[i] /= total_pdf;

        //make prediction by finding highest point of pdf
        for (i = 0; i < PROBPOINTS; i++)
            if (prior[i] > max_pdf_val)
            {
                max_pdf_val = prior[i];
                max_pdf_index = i;
            }

        // for some reason, it never becomes 0, so drop down by one
        max_pdf_index = max_pdf_index - 1;

        // Get new value, store in current value
		dCurrEst = ((float)max_pdf_index) / PROBPOINTS;
        //dCurrEst = samp;
        return dCurrEst;
    }


}



