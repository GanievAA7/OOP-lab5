using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Figures;

namespace Lab4
{
    public partial class Form1 : Form
    {
        private Stack<Operator> operators = new Stack<Operator>();
        private Stack<Operand> operands = new Stack<Operand>();
        private List<string> history = new List<string>();

        int ellipse_count = 0;
        int history_index = -1;
        public Form1()
        {
            InitializeComponent();
            Init.bitmap = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            Init.pen = new Pen(Color.Black, 3);
            Init.pictureBox = pictureBox1;
        }

        private void textBoxInputString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                operators.Clear();
                operands.Clear();
                history.Add(textBoxInputString.Text.Replace(" ", ""));
                history_index = history.Count - 1;
                try
                {
                    string sourceExpression = textBoxInputString.Text.Replace(" ", "");
                    for (int i = 0; i < sourceExpression.Length; i++)
                    {
                        char c = sourceExpression[i];
                        if (IsNotOperation(c))
                        {
                            if (!Char.IsDigit(c))
                            {
                                operands.Push(new Operand(c));
                                while (i < sourceExpression.Length - 1 && IsNotOperation(sourceExpression[i + 1]))
                                {
                                    string temp_str = operands.Pop().value.ToString() + sourceExpression[i + 1].ToString();
                                    operands.Push(new Operand(temp_str));
                                    i++;
                                }
                            }
                            else if (Char.IsDigit(c))
                            {
                                operands.Push(new Operand(c.ToString()));
                                while (i < sourceExpression.Length - 1 && Char.IsDigit(sourceExpression[i + 1])
                                    && IsNotOperation(sourceExpression[i + 1]))
                                {
                                    int temp_num = Convert.ToInt32(operands.Pop().value.ToString()) * 10 +
                                        (int)Char.GetNumericValue(sourceExpression[i + 1]);
                                    operands.Push(new Operand(temp_num.ToString()));
                                    i++;
                                }
                            }
                        }

                        else if ((c == 'E') || (c == 'M') || (c == 'D') || (c == 'I') || (c == 'R'))
                        {
                            if (operators.Count == 0)
                            {
                                operators.Push(OperatorContainer.FindOperator(c));
                            }
                        }
                        else if (c == '(')
                        {
                            operators.Push(OperatorContainer.FindOperator(c));
                        }
                        else if (c == ')')
                        {
                            do
                            {
                                if (operators.Peek().symbolOperator == '(')
                                {
                                    operators.Pop();
                                    break;
                                }
                                if (operators.Count == 1)
                                {
                                    break;
                                }
                            }
                            while (operators.Peek().symbolOperator != '(');
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Аргументы введены некорректно.");
                    comboBox1.Items.Add("Аргументы введены некорректно.");
                }
                try
                {
                    SelectingPerformingOperation(operators.Peek());
                }
                catch
                {
                    MessageBox.Show("Аргументы введены некорректно.");
                    comboBox1.Items.Add("Аргументы введены некорректно.");
                }
            }
            if (e.KeyCode == Keys.Up)
            {
                history_index -= 1;
                if (history_index < 0)
                {
                    history_index = history.Count - 1;
                }
                textBoxInputString.Text = history[history_index];
            }
        }
        private void SelectingPerformingOperation(Operator op)
        {
            if (op.symbolOperator == 'E')
            {
                if (operands.Count == 5)
                {
                    int r2 = Convert.ToInt32(operands.Pop().value.ToString());
                    int r1 = Convert.ToInt32(operands.Pop().value.ToString());
                    int y = Convert.ToInt32(operands.Pop().value.ToString());
                    int x = Convert.ToInt32(operands.Pop().value.ToString());
                    string name = operands.Pop().value.ToString();
                    if (Init.Coords_check(x, y, r1 * 2, r2 * 2))
                    {
                        foreach (Ellipse ell in ShapeContainer.figureList)
                        {
                            if (ell.name == name)
                            {
                                comboBox1.Items.Add($"Эллипс с именем {name} уже существует.");
                                MessageBox.Show($"Эллипс с именем {name} уже существует.");
                                return;
                            }
                        }
                        Ellipse ellipse = new Ellipse(name, ellipse_count, x, y, r1, r2);
                        ShapeContainer.figureList.Add(ellipse);
                        ellipse.Draw();
                        comboBox1.Items.Add($"Эллипс {ellipse.name} отрисован.");
                    }
                    else
                    {
                        MessageBox.Show("Фигура не может выйти за границы.");
                        comboBox1.Items.Add("Фигура не может выйти за границы.");
                    }
                }
                else
                {
                    MessageBox.Show("Опертор E принимает 5 аргументов.");
                    comboBox1.Items.Add("Неверное число аргументов для оператора E.");
                }
            }
            else if (op.symbolOperator == 'R')
            {
                if (operands.Count == 4)
                {
                    int b = Convert.ToInt32(operands.Pop().value.ToString());
                    int g = Convert.ToInt32(operands.Pop().value.ToString());
                    int r = Convert.ToInt32(operands.Pop().value.ToString());
                    string name = operands.Pop().value.ToString();
                    Ellipse ellipse = FindEllipse(name);
                    if (ellipse != null)
                    {
                        ellipse.Recolor(Color.FromArgb(r, g, b));
                    }
                    else
                    {
                        MessageBox.Show("Эллипса с указанным именем не существует.");
                        comboBox1.Items.Add("Эллипса с указанным именем не существует.");
                    }
                }
                else
                {
                    MessageBox.Show("Опертор R принимает 4 аргумента.");
                    comboBox1.Items.Add("Неверное число аргументов для оператора R.");
                }
            }
            else if (op.symbolOperator == 'I')
            {
                if (operands.Count == 3)
                {
                    int r2 = Convert.ToInt32(operands.Pop().value.ToString());
                    int r1 = Convert.ToInt32(operands.Pop().value.ToString());
                    string name = operands.Pop().value.ToString();
                    Ellipse ellipse = FindEllipse(name);
                    if (ellipse != null)
                    {
                        if (Init.Coords_check(ellipse.x, ellipse.y, r1 * 2, r2 * 2))
                        {
                            ellipse.w = r1 * 2;
                            ellipse.h = r2 * 2;
                            ellipse.DeleteF(ellipse, false);
                            ellipse.Draw();
                            comboBox1.Items.Add($"Радиусы эллипса {ellipse.name} изменены.");
                        }
                        else
                        {
                            MessageBox.Show("Фигура не может выйти за границы.");
                            comboBox1.Items.Add("Фигура не может выйти за границы.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Эллипса с указанным именем не существует.");
                        comboBox1.Items.Add("Эллипса с указанным именем не существует.");
                    }
                }
                else
                {
                    MessageBox.Show("Опертор I принимает 3 аргумента.");
                    comboBox1.Items.Add("Неверное число аргументов для оператора I.");
                }
            }
            else if (op.symbolOperator == 'M')
            {
                if (operands.Count == 3)
                {
                    int y = Convert.ToInt32(operands.Pop().value.ToString());
                    int x = Convert.ToInt32(operands.Pop().value.ToString());
                    string name = operands.Pop().value.ToString();
                    Ellipse ellipse = FindEllipse(name);
                    if (ellipse != null)
                    {
                        if (Init.Coords_check(ellipse.x + x, ellipse.y + y, ellipse.w, ellipse.h))
                        {
                            ellipse.MoveTo(x, y);
                            comboBox1.Items.Add($"Эллипс {ellipse.name} перемещен.");
                        }
                        else
                        {
                            MessageBox.Show("Фигура не может выйти за границы.");
                            comboBox1.Items.Add("Фигура не может выйти за границы.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Эллипса с указанным именем не существует.");
                        comboBox1.Items.Add("Эллипса с указанным именем не существует.");
                    }
                }
                else
                {
                    MessageBox.Show("Опертор M принимает 3 аргумента.");
                    comboBox1.Items.Add("Неверное число аргументов для оператора M.");
                }
            }
            else if (op.symbolOperator == 'D')
            {
                if (operands.Count == 1)
                {
                    string name = operands.Pop().value.ToString();
                    Ellipse ellipse = FindEllipse(name);
                    if (ellipse != null)
                    {
                        ellipse.DeleteF(ellipse, true);
                        comboBox1.Items.Add($"Эллипс {ellipse.name} удален.");
                    }
                    else
                    {
                        MessageBox.Show("Эллипса с указанным именем не существует.");
                        comboBox1.Items.Add("Эллипса с указанным именем не существует.");
                    }
                }
                else
                {
                    MessageBox.Show("Опертор D принимает 1 аргумент.");
                    comboBox1.Items.Add("Неверное число аргументов для оператора D.");
                }
            }
            else
            {
                MessageBox.Show("Команда введена некорректно.");
                comboBox1.Items.Add("Команда введена некорректно.");
            }   
        }
        private bool IsNotOperation(char item)
        {
            if (!(item == 'R' || item == 'I' || item == 'D' || item == 'M' || item == 'E' || item == ',' || item == '(' || item == ')'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Ellipse FindEllipse(string name)
        {
            foreach (Ellipse el in ShapeContainer.figureList)
            {
                if (el.name == name)
                {
                    return el;
                }
            }
            return null;
        }
    }
}
