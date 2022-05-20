using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Function_Plotter
{
    public partial class Form1 : Form
    {
        public bool ifexistpoint = false;
        public bool usedonce = false;
        public bool ifexistx = false;
        public bool ifexistnum = false;
        public Form1()
        {
            InitializeComponent();
        }

        private bool Calc(ref string term, char op, ref double result, int value)
        {
            while (term.Contains('x'))
            {
                term = term.Substring(0, term.IndexOf('x')) + value.ToString() + term.Substring(term.IndexOf('x') + 1);
            }
            while (term.Contains(op))
            {
                int operatorIndex = term.IndexOf(op);

                int leftindex = operatorIndex - 1;
                double left = 0;
                while (leftindex >= 0 && (double.TryParse(term[leftindex].ToString(), out left) || term[leftindex].ToString() == "."))
                {
                    leftindex--;
                }

                if (leftindex >= 0 && leftindex != operatorIndex - 1 && term[leftindex] == '-')
                {
                    leftindex--;
                }
                if (leftindex == operatorIndex - 1)
                {

                    label4.Text = "Error : No Left Operant";
                    return false;
                }
                else
                {
                    if (leftindex == -1)
                    {
                        leftindex = 0;
                        left = double.Parse(term.Substring(leftindex, operatorIndex - leftindex));
                    }
                    else
                        left = double.Parse(term.Substring(leftindex + 1, operatorIndex - leftindex - 1));
                }
                int rightindex = operatorIndex + 1;
                double right = 0;
                if(term[rightindex] == '-')
                {
                    rightindex++;
                }
                while (rightindex < term.Length && (double.TryParse(term[rightindex].ToString(), out right) || term[rightindex].ToString() == "."))
                {
                    rightindex++;
                }
                if (rightindex == operatorIndex + 1)
                {
                    label4.Text = "Error : No Right Operant";
                    return false;
                }
                else
                {
                    if (rightindex == term.Length)
                    {
                        rightindex = term.Length - 1;
                        right = double.Parse(term.Substring(operatorIndex + 1, rightindex - operatorIndex));
                    }
                    else
                        right = double.Parse(term.Substring(operatorIndex + 1, rightindex - operatorIndex - 1));
                }
                if (op == '^')
                {

                    result = Math.Pow(left, right);
                }
                else if (op == '/')
                {
                    if (right == 0)
                    {
                        result = int.MaxValue;
                    }
                    else
                        result = left / right;
                }
                else if (op == '*')
                {
                    result = left * right;
                }
                else if(op == '-')
                {
                    result = left - right;
                }
                if (leftindex == 0 && rightindex == term.Length - 1)
                    term = result.ToString();
                else if (leftindex == 0)
                {
                    term = result.ToString() + term.Substring(rightindex, term.Length - rightindex);
                }
                else if (rightindex == term.Length - 1)
                {
                    term = term.Substring(0, leftindex + 1) + result.ToString();
                }
                else
                    term = term.Substring(0, leftindex + 1) + result.ToString() + term.Substring(rightindex, term.Length - rightindex);
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //chart1.Series.Clear();
            label4.Text = "Error : No Errors";
            if (textBox1.Text.Length == 0)
            {
                label4.Text = "Error : Enter Equation";
                return;
            }
            chart1.Series["line"].Points.Clear();
            ////chart1.Series.Add("line");
            //dataGridView1.Rows.Clear();
            //dataGridView1.Refresh();
            string eq = textBox1.Text.ToString();
            string[] terms = eq.Split('+');
            double[] var = new double[terms.Length];
            bool[] constTerm = new bool[terms.Length];
            int min;
            if(!int.TryParse(textBox2.Text.ToString(),out min))
            {
                label4.Text = "Error : Enter minimum value";
                return;
            }
            int max ;
            if (!int.TryParse(textBox3.Text.ToString(), out max))
            {
                label4.Text = "Error : Enter maximum value";
                return;
            }
            for (int i = 0; i < terms.Length; i++)
            {
                if (!terms[i].Contains("x"))
                {
                    constTerm[i] = true;
                }
            }
            for (int i = 0; i < terms.Length; i++)
            {
                if (constTerm[i] == true)
                {
                    if (terms[i] == "")
                    {
                        label4.Text = "Error in term";
                        return;
                    }

                    double temp;
                    if (double.TryParse(terms[i], out temp))
                    {
                        var[i] = temp;
                    }
                    else
                    {
                        if (Calc(ref terms[i], '^', ref var[i], 0)) ;
                        else
                            return;

                        if (Calc(ref terms[i], '*', ref var[i], 0)) ;
                        else
                            return;
                        if (Calc(ref terms[i], '/', ref var[i], 0)) ;
                        else
                            return;

                        if(!double.TryParse(terms[i],out temp))
                        {
                            if (Calc(ref terms[i], '-', ref var[i], 0)) ;
                            else
                                return;
                        }
                        
                    }

                }

            }
            List<string> X = new List<string>();
            List<double> Y = new List<double>();
            for (int j = min; j <= max; j++)
            {
                for (int i = 0; i < terms.Length; i++)
                {
                    if (constTerm[i] == false)
                    {
                        if (terms[i] == "")
                        {
                            label4.Text = "Error in term";
                            return;
                        }

                        double temp;
                        if (double.TryParse(terms[i], out temp))
                        {
                            var[i] = temp;
                        }
                        else
                        {
                            string originaleq = terms[i];
                            if (Calc(ref terms[i], '^', ref var[i], j)) ;
                            else
                                return;
                            if (Calc(ref terms[i], '*', ref var[i], j)) ;
                            else
                                return;
                            if (Calc(ref terms[i], '/', ref var[i], j)) ;
                            else
                                return;
                            if(!double.TryParse(terms[i], out temp))
                            {
                                if (Calc(ref terms[i], '-', ref var[i], j)) ;
                                else
                                    return;
                            }
                            
                            terms[i] = originaleq;
                        }

                    }

                }
                double result = 0;
                for (int i = 0; i < terms.Length; i++)
                {
                    result += var[i];
                }
                //textBox4.Text = result.ToString();
                string[] row = new string[] { j.ToString(), result.ToString() };
                //dataGridView1.Rows.Add(row);
                chart1.Series["line"].Points.AddXY(j, result);

            }
            
            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{


            //    double x, y;
            //    if (row.Cells["Y"].Value != null)
            //    {

            //        Y.Add(double.Parse(row.Cells["Y"].Value.ToString()));
            //        if (row.Cells["X"].Value != null)
            //        {
            //            X.Add((row.Cells["X"].Value.ToString()));
            //            y = double.Parse(row.Cells["Y"].Value.ToString());
            //            x = double.Parse(row.Cells["X"].Value.ToString());
            //            chart1.Series["line"].Points.AddXY(x, y);
                        
            //        }
            //    }
                
            //    //More code here
            //}
            chart1.Series["line"].BorderWidth = 4;
            //cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            //{
            //    Labels = new List<string>(X)
            //}
                //);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (int.TryParse(e.KeyChar.ToString(), out int o))
            {
                return;
            }
            else if (e.KeyChar == '.')
            {
                int left = textBox1.SelectionStart;
                int right = textBox1.SelectionStart;
                int numofdot = 0;
                int index = textBox1.SelectionStart;

                //if(index == 0)
                //{
                //    left = -1;
                //    right = 0;
                //}
                //else if(index == textBox1.Text.Length - 1)
                //{
                //    right = textBox1.Text.Length;
                //    left = index;
                //}
                 if (index == 0 && textBox1.Text.Length == 0)
                {
                    e.Handled = true;
                    return;
                }
                else if (index == textBox1.Text.Length && textBox1.Text.Length > 0)
                {
                    left--;
                    right--;
                }
                char c = textBox1.Text.ToString()[left];
                while (!(c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == 'x') && left >= 0)
                {
                    if (c == '.')
                    {
                        numofdot++;
                    }
                    left--;
                    if (left == -1)
                        break;                    
                    c = textBox1.Text.ToString()[left];
                }
                if (numofdot != 0)
                {
                    e.Handled = true;
                    return;
                }
                c = textBox1.Text.ToString()[right];
                while (!(c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == 'x') && right < textBox1.Text.Length)
                {
                    if (c == '.')
                    {
                        numofdot++;
                    }
                    right++;
                    if (right == textBox1.Text.Length)
                        break;
                    c = textBox1.Text.ToString()[right];
                }
                if (numofdot != 0)
                {
                    e.Handled = true;
                    return;
                }
                if (left == right)
                {
                    e.Handled = true;
                    return;
                }


            }
            //if (ifexistpoint)
            //{
            //    if (int.TryParse(e.KeyChar.ToString(), out int temp))
            //    {
            //        usedonce = true;
            //        ifexistpoint = false;
            //        //e.Handled = true;
            //    }
            //    else
            //        e.Handled = true;
            //}
            //else if (e.KeyChar == '.' && !usedonce)
            //{
            //    if (ifexistnum)
            //    {
            //        usedonce = true;
            //        //e.Handled = true;
            //    }
            //    else
            //    {
            //        ifexistpoint = true;
            //        //e.Handled=true;
            //    }
            //}
            //else if (int.TryParse(e.KeyChar.ToString(), out int value))
            //{
            //    if (!usedonce)
            //    {
            //        ifexistnum = true;
            //        //e.Handled = true;
            //    }
            //}

            else if (e.KeyChar == 'x')
            {
                //if (!ifexistx)
                
                if (textBox1.Text.Length == 0)
                    ifexistx = true;
                else if (textBox1.SelectionStart == 0)
                {
                    char l = textBox1.Text.ToString()[textBox1.SelectionStart];
                    if (l == '+' || l == '-' || l == '*' || l == '/' || l == '^')
                    {
                        ifexistx = true;
                    }
                    else
                        e.Handled = true;
                }
                else if (textBox1.Text.Length > 0)
                {
                    char l = textBox1.Text.ToString()[textBox1.SelectionStart - 1];
                    if (l == '+' || l == '-' || l == '*' || l == '/' || l == '^')
                    {
                        ifexistx = true;
                    }
                    else
                        e.Handled = true;
                    if (textBox1.SelectionStart + 1 < textBox1.Text.Length)
                    {
                        l = textBox1.Text.ToString()[textBox1.SelectionStart];
                        if (l == '+' || l == '-' || l == '*' || l == '/' || l == '^')
                        {
                            ifexistx = true;
                        }
                        else
                            e.Handled = true;
                    }
                }
                else
                    e.Handled = true;
                
                //else
                //{
                //    e.Handled = true;
                //}
            }
            else if (e.KeyChar == '+' || e.KeyChar == '-' || e.KeyChar == '*' || e.KeyChar == '/' || e.KeyChar == '^')
            {
                return;
            }
            else if (e.KeyChar == (char)8)
                return;
            else
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '-')
            {
                if (textBox2.SelectionStart == 0)
                {
                    if (textBox2.Text.Contains("-"))
                    {
                        e.Handled = true;
                        return;
                    }
                    else
                        return;
                }
                else
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-')
            {
                if (textBox3.SelectionStart == 0)
                {
                    if (textBox3.Text.Contains("-"))
                    {
                        e.Handled = true;
                        return;
                    }
                    else
                        return;
                }
                else
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
