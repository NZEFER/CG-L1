using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace funcpanel
{
    public partial class Form1 : Form
    {
        // Определение делегата для функций
        private delegate double Function(double x);

        // Массив функций для выбора
        private Function[] functions;
        private string[] functionNames;
        private Function currentFunction;

        // Панель для отрисовки графика
        private Panel plotPanel;

        public Form1()
        {
            InitializeComponent();

            // Инициализация функций
            functions = new Function[]
            {
                Math.Sin,    // sin(x)
                x => x * x   // x^2
            };

            // Имена функций для ComboBox
            functionNames = new string[]
            {
                "sin(x)",
                "x^2"
            };

            // Установка первой функции по умолчанию
            currentFunction = functions[0];

            // Создание интерфейса
            CreateUI();
        }

        private void CreateUI()
        {
            // ComboBox для выбора функции
            ComboBox functionSelector = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            functionSelector.Items.AddRange(functionNames);
            functionSelector.SelectedIndex = 0;
            functionSelector.SelectedIndexChanged += (sender, args) =>
            {
                currentFunction = functions[functionSelector.SelectedIndex];
                plotPanel.Invalidate();  // Перерисовать график именно на панели
            };
            this.Controls.Add(functionSelector);

            // Панель для отрисовки графика
            plotPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            plotPanel.Paint += PlotPanel_Paint;
            this.Controls.Add(plotPanel);

            // Событие изменения размера
            this.Resize += (sender, args) => plotPanel.Invalidate();
        }

        private void PlotPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawFunctionGraph(e.Graphics, plotPanel.ClientSize, currentFunction);
        }

        // Функция для отрисовки графика
        private void DrawFunctionGraph(Graphics g, Size clientSize, Function func)
        {
            g.Clear(Color.White);

            int width = clientSize.Width;
            int height = clientSize.Height;

            // Параметры для отрисовки осей
            Pen axisPen = new Pen(Color.Black, 2);
            Pen graphPen = new Pen(Color.Blue, 2);

            // Определение границ по X
            double xMin = -10;
            double xMax = 10;

            // Вычисление минимального и максимального значения функции для корректного масштабирования по Y
            double yMin = double.MaxValue;
            double yMax = double.MinValue;
            double step = (xMax - xMin) / width;

            for (double x = xMin; x <= xMax; x += step)
            {
                double y = func(x);
                if (y < yMin) yMin = y;
                if (y > yMax) yMax = y;
            }

            // Добавляем небольшой запас по краям (10%)
            double yRange = yMax - yMin;
            yMin -= yRange * 0.001;
            yMax += yRange * 0.05;

            // Масштабирование
            double xScale = width / (xMax - xMin);
            double yScale = height / (yMax - yMin);

            // Смещение по оси Y (чтобы 0 был по середине)
            double yOffset = yMax * yScale;
            double xOffset = width / 2;

            // Отрисовка осей
            g.DrawLine(axisPen, 0, (float)(yOffset), width, (float)(yOffset)); // X-axis
            g.DrawLine(axisPen, (float)xOffset, 0, (float)xOffset, height); // Y-axis

            // Предыдущая точка для соединения линиями
            PointF? prevPoint = null;

            // Отрисовка графика
            for (double x = xMin; x <= xMax; x += step)
            {
                // Вычисление Y по функции
                double y = func(x);

                // Преобразование координат функции в координаты экрана
                float screenX = (float)((x - xMin) * xScale);
                float screenY = (float)((yMax - y) * yScale);  // Учитываем масштабирование по Y

                // Соединение точек линиями
                if (prevPoint != null)
                {
                    g.DrawLine(graphPen, prevPoint.Value.X, prevPoint.Value.Y, screenX, screenY);
                }

                prevPoint = new PointF(screenX, screenY);
            }
        }
    }
}
