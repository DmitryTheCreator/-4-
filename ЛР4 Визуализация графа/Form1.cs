using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ЛР4_Визуализация_графа
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();        
        }
        // Класс круг, его координаты и радиус
        class Circle 
        {
            private int x, y;
            private int rad = 15;
            public int X { get { return x; } set { x = value; } }
            public int Y { get { return y; } set { y = value; } }
            public int Rad { get { return rad; } }
        }

        LinkedList<Circle> circles = new LinkedList<Circle>();
        // Переменная, необходимая для обработки различных ситуаций при нажатии клавишей мыши
        int versh = -1;
        // Проверка для заполнения первых двух столбцов
        bool check = false;
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            Circle circle = new Circle();
            switch (e.Button)
            {
                // Нажата ПКМ
                case MouseButtons.Right:
                    {
                        // Рисуется круг по координатам и число внутри круша
                        Pen pen_default = new Pen(Color.Red, 2);
                        
                        circle.X = e.X - circle.Rad;
                        circle.Y = e.Y - circle.Rad;
                        Canvas.CreateGraphics().DrawEllipse(pen_default, circle.X, circle.Y,
                            circle.Rad * 2, circle.Rad * 2);
                        String drawString = (circles.Count + 1).ToString();
                        int fontSize = 14;
                        if (circles.Count + 1 >= 10)
                            fontSize = 11;

                        Font font = new Font("Arial", fontSize);
                        SolidBrush brush = new SolidBrush(Color.Black);
                        PointF point = new PointF(circle.X + circle.Rad / 2, circle.Y + circle.Rad / 2);
                        Canvas.CreateGraphics().DrawString(drawString, font, brush, point);
                        circles.AddLast(circle);

                        // Создание матрицы
                        if (check == false)
                        {
                            var column1 = new DataGridViewColumn();
                            column1.HeaderText = "0";
                            column1.ReadOnly = true; 
                            column1.Name = "0"; 
                            column1.CellTemplate = new DataGridViewTextBoxCell();
                            column1.DefaultCellStyle.BackColor = Color.White;
                            column1.DefaultCellStyle.ForeColor = Color.Black;
                            Matrix.Columns.Add(column1);
                            Matrix.Columns.Add("1", "1");
                            Matrix.Rows.Add();
                            Matrix[0, 0].Value = 1;
                            Matrix[1, 0].Value = 0;
                            check = true;
                        }
                        else
                        {
                            for (int i = circles.Count; i < circles.Count + 2; ++i)
                            {
                                Matrix.Columns.Add(i.ToString(), i.ToString());
                                Matrix.Rows.Add();
                            }
                            for (int i = 0; i < circles.Count; ++i)
                                Matrix[0, i].Value = i + 1;
                            Matrix[0, circles.Count - 1].Value = circles.Count;
                            Matrix[circles.Count, circles.Count - 1].Value = 0;
                        }
                        break;
                    }
                     // Нажата ПКМ                    
                case MouseButtons.Left:
                    {
                        // Если точка находится внутри круга
                        Pen pen_selected = new Pen(Color.DeepPink, 2);
                        // Если не запомнили ни одну из вершин
                        if (versh == -1)
                        {
                            int count = -1;
                            // Идет проверка на нахождение точки на одной из вершин
                            foreach (Circle c in circles) 
                            {
                                count++;
                                if (e.X - c.Rad <= c.X + c.Rad &&
                                    e.X - c.Rad >= c.X - c.Rad &&
                                    e.Y - c.Rad <= c.Y + c.Rad &&
                                    e.Y - c.Rad >= c.Y - c.Rad)
                                {
                                    // Выделение точки особым цветом
                                    versh = count;
                                    Canvas.CreateGraphics().DrawEllipse(pen_selected, c.X, c.Y,
                                    c.Rad * 2, c.Rad * 2);
                                    break;
                                }
                            }
                        }
                        // Если по одной из вершин уже раннее щелкнули
                        else
                        {
                            int toversh = -1;
                            int count = -1;
                            // Идет проверка на нахождение точки на одной из вершин
                            foreach (Circle c in circles) 
                            {
                                count++;
                                if (e.X - c.Rad <= c.X + c.Rad &&
                                    e.X - c.Rad >= c.X - c.Rad &&
                                    e.Y - c.Rad <= c.Y + c.Rad &&
                                    e.Y - c.Rad >= c.Y - c.Rad)
                                {
                                    // Выделение точки особым цветом
                                    toversh = count;
                                    Canvas.CreateGraphics().DrawEllipse(pen_selected, c.X, c.Y,
                                    c.Rad * 2, c.Rad * 2);
                                    break;
                                }

                            }
                            // Если выделились две вершины
                            if ((toversh != -1) && (versh != toversh))
                            {
                                int it = -1;
                                Point p1 = new Point(0, 0);
                                Point p2 = new Point(0, 0);
                                // Запоминаем координаты этих вершин
                                foreach (Circle c in circles)
                                {
                                    it++;
                                    if (it == versh)
                                        p1 = new Point(c.X + c.Rad, c.Y + c.Rad);
                                    if (it == toversh)
                                        p2 = new Point(c.X + c.Rad, c.Y + c.Rad);                                 
                                }
                                // Рисуем линию
                                Canvas.CreateGraphics().DrawLine(pen_selected, p1, p2);
                                // Отмечаем на матрице
                                Matrix[versh + 1, toversh].Value = 1;
                                Matrix[toversh + 1, versh].Value = 1;
                                versh = -1;                              
                            }
                        }
                        break;
                    }                  
            } 
            tbStart.Text = versh.ToString();
        }
        // Поиск в глубину
        public void DFS(int start)
        {
            List<int> list = new List<int>(); 
            // Для поиска в глубину используется стэк,
            // а для поиска в ширину - очередь
            Stack<int> s = new Stack<int>(); 
            list.Add(start);
            // Все не пустые значения ячеек таблицы засовываем в стек
            for (int i = circles.Count; i >= 1; i--)
            {
                if (Matrix.Rows[start - 1].Cells[i].Value == null)
                    continue;
                s.Push(i);
            }
            rep:
            // Пока стек не пуст вынимаем из него число.
            // Засовываем его в лист
            while (s.Count != 0)
            {
                int v = s.Peek();
                s.Pop();
                foreach (int i in list)
                {
                    if (i == v)
                        goto rep;
                }
                list.Add(v);
                // Если содержимое ячейки не пусто - засовываем в стек
                for (int i = circles.Count; i >= 1; i--)
                {
                    if (Matrix.Rows[v - 1].Cells[i].Value == null)
                        continue;
                    s.Push(i);
                }
            }
            tbStart.Text = "";
            // Выводим все элементы списка в TextBox
            foreach (int i in list)
                 tbStart.Text += i.ToString() + " ";
        }
        // Запускаем поиск в глубину
        private void btnSearch_Click(object sender, EventArgs e)
        {
            int start = Convert.ToInt32(Numb_TextBox.Text);
            DFS(start);
        }
        // Очищаем экран, список кругов и таблицу
        private void btnClear_Click(object sender, EventArgs e)
        {
            Matrix.Columns.Clear();
            Matrix.Rows.Clear();
            Canvas.Refresh();
            circles.Clear();
            Matrix.Columns.Add(0.ToString(), 0.ToString());
        }
    }
}
