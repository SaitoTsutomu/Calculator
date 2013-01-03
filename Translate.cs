using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculator
{
	/// <summary>
	/// 数式の評価を行う
	/// </summary>
	public class Translate
	{
		public delegate void DelegateTrace(string before, string after);
		public event DelegateTrace Trace;
		public string Eval(string s)
		{
			s = Regex.Replace(s, "pi", Math.PI.ToString("F14"));
			s = Regex.Replace(s, @"[0-9.)]\s*-", "$0 ");
			EvalSub(ref s);
			return s;
		}
		void OnTrace(ref string before, string after)
		{
			if (before == after) return;
			if (Trace != null) Trace(before, after);
			before = after;
		}
		void EvalSub(ref string s)
		{
			string t;
			string rep = "(-?[0-9.]+(?:E[+-][0-9]{1,3}|))";
			for (string pre1 = s; ; pre1 = s)
			{
				for (string pre2 = s; ; pre2 = s)
				{
					for (string pre3 = s; ; pre3 = s)
					{
						for (string pre4 = s; ; pre4 = s)
						{
							for (string pre5 = s; ; pre5 = s)
							{
								for (string pre6 = s; ; pre6 = s)
								{
									t = Regex.Replace(s, @"\((.*)\)", new MatchEvaluator(RepBrackets)); OnTrace(ref s, t);
									if (s == pre6) break;
								}
								t = Regex.Replace(s, @"abs\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepAbs)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"floor\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepFloor)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"sign\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepSign)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"sin\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepSin)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"cos\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepCos)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"tan\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepTan)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"exp\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepExp)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"log\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepLog)); OnTrace(ref s, t);
								t = Regex.Replace(s, @"sqrt\s*\(\s*" + rep + @"\s*\)", new MatchEvaluator(RepSqrt)); OnTrace(ref s, t);
								if (s == pre5) break;
							}
							t = Regex.Replace(s, @"\(\s*" + rep + @"\s*\)", "$1"); OnTrace(ref s, t);
							if (s == pre4) break;
						}
						t = Regex.Replace(s, @"([0-9.]+(?:E[+-][0-9]{1,3}|))\s*\^\s*" + rep, new MatchEvaluator(RepPower)); OnTrace(ref s, t);
						if (s == pre3) break;
					}
					t = Regex.Replace(s, rep + @"\s*([\*/])\s*" + rep, new MatchEvaluator(RepMulDiv)); OnTrace(ref s, t);
					if (s == pre2) break;
				}
				t = Regex.Replace(s, rep + @"\s*([\+-])\s*" + rep, new MatchEvaluator(RepPlusMinus)); OnTrace(ref s, t);
				if (s == pre1) break;
			}
			s = s.Trim();
		}

		string RepBrackets(Match m)
		{
			string s = m.Groups[1].Value;
			EvalSub(ref s);
			return "(" + s + ")";
		}
		string RepPower(Match m)
		{
			double d1 = double.Parse(m.Groups[1].Value);
			double d2 = double.Parse(m.Groups[2].Value);
			return Math.Pow(d1, d2).ToString();
		}
		string RepMulDiv(Match m)
		{
			double d1 = double.Parse(m.Groups[1].Value);
			double d2 = double.Parse(m.Groups[3].Value);
			return (m.Groups[2].Value == "*" ? d1 * d2 : d1 / d2).ToString();
		}
		string RepPlusMinus(Match m)
		{
			double d1 = double.Parse(m.Groups[1].Value);
			double d2 = double.Parse(m.Groups[3].Value);
			return (m.Groups[2].Value == "+" ? d1 + d2 : d1 - d2).ToString();
		}
		string RepAbs(Match m)
		{
			return Math.Abs(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepFloor(Match m)
		{
			return Math.Floor(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepSign(Match m)
		{
			return Math.Sign(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepSin(Match m)
		{
			return Math.Sin(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepCos(Match m)
		{
			return Math.Cos(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepTan(Match m)
		{
			return Math.Tan(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepExp(Match m)
		{
			return Math.Exp(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepLog(Match m)
		{
			return Math.Log(double.Parse(m.Groups[1].Value)).ToString();
		}
		string RepSqrt(Match m)
		{
			return Math.Sqrt(double.Parse(m.Groups[1].Value)).ToString();
		}

		public static void Test()
		{
			string[,] dat = {
				{"abs(0.1^3-0.1^2)","0.009"},
				{"floor(1.9)","1"},
				{"sign(3.3)","1"},
				{" 1.1 + 1.1 * 1.1 ^ -1.1 * 1.1 - 1.1 ","1.08956568403597"},
				{" -1 - 2 + 4 ","1"},
				{" -2 / -4 * 5 ","2.5"},
				{" 1 + 2 * ( 3 + 4 ) ","15"},
				{" sin ( sin ( 0.1 ) ) ","0.0996676641321335"},
				{" sin ( 1 + 2 * ( 3 + cos ( exp ( 1 ) ) ) ) ","-0.894205450594065"},
				{" sqrt ( 1.1E-1 + 1.1E-2 * 1.1E+2 ) ","1.14891252930761"},
				{" log ( -1.1 - exp ( pi * 1.1 ) * 1.1 + 40.1 ) ","1.42303307567651"},
				{" tan ( 1.1 ) - sin ( 1.1 ) / cos ( 1.1 ) ","0"},
			};
			int i, n = dat.GetLength(0);
			string s;
			Translate tr = new Translate();
			for (i = 0; i < n; ++i)
			{
				s = dat[i, 0];
				s = tr.Eval(s);
				if (s != dat[i, 1])
				{
					System.Windows.Forms.MessageBox.Show(dat[i, 0] + " != " + dat[i, 1]);
					return;
				}
			}
			System.Windows.Forms.MessageBox.Show("All OK!");
		}
	}
}
