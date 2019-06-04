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
    public double dEmgThreshold = 0.02;
    public double dMaxv = 0.8;
    public double dSwitchVal = 1e8;
    public double dDriftVal = 5;//27857
    public double dCurrEst = -99;
    public List<double> prior = new List<double>();
    public List<double> expx = new List<double>();
    public BayesFilter()
    {

        int i;
        for (i = 0; i < PROBPOINTS; i++)
        {
            prior.Add((double)(1.0 / PROBPOINTS));
            expx.Add(Math.Exp(-(double)i / PROBPOINTS));
        }
    }
    public BayesFilter(double thresh, double maxv, double switchv, double drif)
    {
        int i;
        for (i = 0; i < PROBPOINTS; i++)
        {
            prior.Add((double)(1.0 / PROBPOINTS));
            expx.Add(Math.Exp(-(double)i / PROBPOINTS));
        }

    }
    ~BayesFilter() { }
    // Updating estimate
    public double UpdateEst(float samp)
    // Updates the filter with new measurement
    {
        int i = 0;
        double v = 0.0;
        double total_pdf = 0.0;
        double max_pdf_val = 0.0;
        int max_pdf_index = 0;


        // Normalize or zero the value
        v = Math.Abs(samp);
        if (v < dEmgThreshold)
            v = 0.0;
        v /= dMaxv;
        v *= 4;

        // Do the propagation steps
        // blurring NECESSARY FOR SMOOTH MOVEMENT
        for (i = 0; i < PROBPOINTS; i++)
            if (i > 0 && i < PROBPOINTS - 1)
                prior[i] += dDriftVal * (prior[i - 1] + prior[i + 1]) / 100.0;

        //constant shift   NECESSARY FOR JUMPS
        for (i = 0; i < PROBPOINTS; i++)
            prior[i] += dSwitchVal * 1.0E-12;

        // Do estimation step, get sum	
        for (i = 0; i < PROBPOINTS; i++)
        {
            prior[i] *= Math.Pow(((double)i) / PROBPOINTS, v) * expx[i];  //poisson
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
        dCurrEst = ((double)max_pdf_index) / PROBPOINTS;
        //dCurrEst = samp;
        return dCurrEst;
    }


}



